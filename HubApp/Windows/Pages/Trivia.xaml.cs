using System.Windows.Controls;
using HubApp.TriviaHelper;
using System.Net;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading;
using System;
using System.Windows.Threading;
using HubApp.Utilities;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media;

namespace HubApp.Windows.Pages
{
    /// <summary>
    /// Interaction logic for Trivia.xaml
    /// </summary>
    public partial class Trivia : TabablePage
    {
        private const int DISPLAY_TIME = 1000;
        private const int ONE_CENTISECOND = 10;
        private const int WAIT_TIME = 3000;
        private const int DIVISOR = 10;
        private const string TRIVIA_URL = "https://opentdb.com/api.php?amount=1&type=multiple";
        private BackgroundWorker _backgroundWorker;
        private object _sleep;
        private WebClient _detail;
        private List<TextBlock> _answers;
        private List<Border> _borders;

        public Trivia()
        {
            InitializeComponent();
            InitializeQuestionDisplay();
            // DisplayQuestion();
        }

        public override void OnPause()
        {
            Trace.WriteLine("Paused Trivia");
            this._backgroundWorker.CancelAsync();
        }

        public override void OnResume()
        {
            Trace.WriteLine("Resumed Trivia");
            DisplayQuestion();
        }

        private void InitializeQuestionDisplay()
        {
            this._answers = (new TextBlock[] {this.answer0, this.answer1, this.answer2, this.answer3}).ToList();
            this._borders = (new Border[] { this.border0, this.border1, this.border2, this.border3 }).ToList();
            this._detail = new WebClient();
            this._detail.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadComplete);
            this._sleep = new object();
            this._backgroundWorker = CreateBackgroundWorker();
        }

        private void DisplayQuestion()
        {
            try
            {
                // this._backgroundWorker = CreateBackgroundWorker();
                this._detail.DownloadStringAsync(new Uri(TRIVIA_URL));
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                // DisplayQuestion();
            }
        }

        private void OnDownloadComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                string json = e.Result;
                Question question = JsonConvert.DeserializeObject<Question>(json);
                question.Normalize();
                var triviaQuesiton = question;
                var answers = triviaQuesiton.Results[0].IncorrectAnswers;
                answers.Add(triviaQuesiton.Results[0].CorrectAnswer);
                answers = answers.OrderBy(a => Guid.NewGuid()).ToList();
                int correctAnswer = answers.IndexOf(triviaQuesiton.Results[0].CorrectAnswer);
                Dispatcher.Invoke(() => 
                {
                    ResetAnswers();
                    this.question.Text = triviaQuesiton.Results[0].Question;
                    for (int i = 0; i < answers.Count; i++)
                    {
                        this._answers[i].Text = answers[i];
                    }
                });
                Trace.WriteLine(string.Format("Busy? {0}", this._backgroundWorker.IsBusy.ToString()));
                this._backgroundWorker.RunWorkerAsync(correctAnswer);
            }
        }

        private BackgroundWorker CreateBackgroundWorker()
        {
            BackgroundWorker backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            backgroundWorker.DoWork += new DoWorkEventHandler(Countdown);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnComplete);
            return backgroundWorker;
        }

        private void ResetAnswers()
        {
            for (int i = 0; i < this._borders.Count; i++)
            {
                this._borders[i].Background = new SolidColorBrush()
                {
                    Color = Colors.White,
                    Opacity = 0.05
                };
            }
        }

        private void ShowCorrectAnswer(int correctAnswerIndex)
        {
            for (int i = 0; i < this._borders.Count; i++)
            {
                // Trace.WriteLine("yeah");
                if (i == correctAnswerIndex)
                {
                    this._borders[i].Background = new SolidColorBrush()
                    {
                        Color = Colors.Green,
                        Opacity = 0.25
                    };
                }
                else
                {
                    this._borders[i].Background = new SolidColorBrush()
                    {
                        Color = Colors.Red,
                        Opacity = 0.25
                    };
                }
            }
        }

        private void Countdown(object sender, DoWorkEventArgs e)
        {
            int correctAnswer = (int) e.Argument;
            for (int centisecond = DISPLAY_TIME; 0 <= centisecond; centisecond--)
            {
                if (this._backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    Trace.WriteLine("cancelled");
                    return;
                }
                else
                {
                    this._backgroundWorker.ReportProgress(centisecond / 10);
                    Thread.Sleep(ONE_CENTISECOND);
                }
            }

            Dispatcher.Invoke(() => 
            {
                ShowCorrectAnswer(correctAnswer);
            });


            for (int i = 0; i < WAIT_TIME; i++)
            {
                if (this._backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    Trace.WriteLine("cancelled");
                    return;
                }
                Thread.Sleep(1);
            }

        }

        private void OnComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            Trace.WriteLine(string.Format("Busy? {0}hi", this._backgroundWorker.IsBusy.ToString()));
            if (!e.Cancelled)
            {
                DisplayQuestion();
            }
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                this.progressBar.Value = e.ProgressPercentage;
                // percentageLabel.Text = e.ProgressPercentage.ToString() + " %";
            });
        }
    }
}
