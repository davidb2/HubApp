using System.Windows.Controls;
using HubApp.TriviaHelper;
using System.Net;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading;
using System;
using System.Windows.Threading;

namespace HubApp.Windows.Pages
{
    /// <summary>
    /// Interaction logic for Trivia.xaml
    /// </summary>
    public partial class Trivia : Page
    {
        private const int DISPLAY_TIME = 1000;
        private const int ONE_CENTISECOND = 10;
        private const int WAIT_TIME = 3000;
        private const int DIVISOR = 10;
        private const string TRIVIA_URL = "https://opentdb.com/api.php?amount=1&type=multiple";
        private BackgroundWorker _backgroundWorker;
        private object _sleep;

        public Trivia()
        {
            InitializeComponent();
            InitializeQuestionDisplay();
            DisplayQuestion();
        }

        private void InitializeQuestionDisplay()
        {
            this._sleep = new object();
            this._backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            this._backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(ProgressChanged);
            this._backgroundWorker.DoWork += new DoWorkEventHandler(Countdown);
            this._backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnComplete);
        }

        private void StreamQuestions()
        {
            while (true)
            {
                this._backgroundWorker.RunWorkerAsync();
            }
        }

        public Question GetQuestion()
        {
            string json = new WebClient().DownloadString(TRIVIA_URL);
            Question question = JsonConvert.DeserializeObject<Question>(json);
            question.Normalize();
            return question;   
        }

        private void DisplayQuestion()
        {
            WebClient detail = new WebClient();
            detail.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadComplete);
            detail.DownloadStringAsync(new Uri(TRIVIA_URL));
        }

        private void OnDownloadComplete(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                DisplayQuestion();
            }
            else
            {
                string json = e.Result;
                Question question = JsonConvert.DeserializeObject<Question>(json);
                question.Normalize();
                var triviaQuesiton = question;

                Dispatcher.BeginInvoke(
                    DispatcherPriority.Background, 
                    (Action)(() => 
                        this.question.Text = triviaQuesiton.Results[0].Question
                    )
                );
                this._backgroundWorker.RunWorkerAsync();
            }
        }

        private void Countdown(object sender, DoWorkEventArgs e)
        {
            for (int centisecond = DISPLAY_TIME; 0 <= centisecond; centisecond--)
            {
                if (this._backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
                else
                {
                    this._backgroundWorker.ReportProgress(centisecond / 10);
                    Thread.Sleep(ONE_CENTISECOND);
                }
            }
            for (int i = 0; i < WAIT_TIME; i++)
            {
                if (this._backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
                Thread.Sleep(1);
            }
        }

        private void OnComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            DisplayQuestion();
        }


        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
            // percentageLabel.Text = e.ProgressPercentage.ToString() + " %";
        }
    }
}
