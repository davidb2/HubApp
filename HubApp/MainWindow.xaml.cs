using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using HubApp.Twitter;
using Tweetinvi.Models;
using Tweetinvi;


namespace HubApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string TRACK = "#MAGA";
        private const int SLEEP_TIME = 2500;
        private const string DEFAULT_TWITTER_PIC = "https://camo.githubusercontent.com/93c261622c63780cb29c6423b0787c8de460aff6/687474703a2f2f636c6f75642e73636f74742e65652f696d616765732f6e756c6c2e706e67";
        private WindowState _lastWindowState;
        private WindowStyle _lastWindowStyle;
        private Queue<ITweet> _tweetQueue;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTwitterStream();
            InitializeTwitterCard();
            InitializeTwitterQueue();
            InitializeBackgroundVideo();
            InitializeWindowOptions();
        }

        public void InitializeTwitterStream()
        {
            Thread thread = new Thread(TwitterView_StartStreaming);
            thread.Start();
        }

        public void InitializeTwitterQueue()
        {
            _tweetQueue = new Queue<ITweet>();
            Thread thread = new Thread(TwitterView_Queue);
            thread.Start();
        }

        public void InitializeTwitterCard()
        {
            //this.twitterMedia.Source = new BitmapImage(new Uri(DEFAULT_TWITTER_PIC));
        }

        public void InitializeBackgroundVideo()
        {
            this.bgvideo.MediaEnded += new RoutedEventHandler(mediaElement_OnMediaEnded);
            this.bgvideo.LoadedBehavior = MediaState.Manual;
            this.bgvideo.Play();
        }

        public void InitializeWindowOptions()
        {
            _lastWindowState = this.WindowState;
            _lastWindowStyle = this.WindowStyle;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        public void TwitterView_StartStreaming()
        {
            TwitterView twitter = new TwitterView();
            var tweetStream = Stream.CreateFilteredStream();
            tweetStream.AddTrack(TRACK);
            tweetStream.MatchingTweetReceived +=
                  new EventHandler<Tweetinvi.Events.MatchedTweetReceivedEventArgs>(TwitterView_ReceivedPost);
            tweetStream.StartStreamMatchingAllConditions();
        }

        public void TwitterView_Queue()
        {
            if (_tweetQueue.Count != 0)
            {
                ITweet tweet = _tweetQueue.Dequeue();
                var link = tweet.Url;
                var user = tweet.CreatedBy;
                var text = tweet.FullText;
                var media = tweet.Media.Count == 0 ? DEFAULT_TWITTER_PIC : tweet.Media[0].MediaURLHttps;
                var userPic = user.ProfileImageUrlHttps;
                var screenName = user.ScreenName;

                Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    this.profilePic.Source = new BitmapImage(new Uri(userPic));
                    this.screenName.Text = string.Format("@{0}", screenName);
                    this.tweetText.Text = text;
                    this.twitterMedia.Source = new BitmapImage(new Uri(media));
                    this.tweetMediaBg.Background =
                    new BrushConverter().ConvertFromString(
                        string.Format("#{0}", user.ProfileBackgroundColor)
                    ) as SolidColorBrush;
                }));
            }
            Thread.Sleep(SLEEP_TIME);
            TwitterView_Queue();
        }

        public void TwitterView_ReceivedPost(object sender, Tweetinvi.Events.MatchedTweetReceivedEventArgs args)
        {
            if (args.Tweet.Media.Count > 0)
            {
                _tweetQueue.Enqueue(args.Tweet);
            }
        }

        public void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (WindowStyle == WindowStyle.None)
                {
                    WindowState = _lastWindowState;
                    WindowStyle = _lastWindowStyle;

                }
                else
                {
                    _lastWindowState = WindowState;
                    _lastWindowStyle = WindowStyle;
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                }
            }
        }

        private void mediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            this.bgvideo.Position = new TimeSpan(0, 0, 1);
            this.bgvideo.Play();
        }
    }
}