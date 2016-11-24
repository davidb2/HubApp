using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
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
        private const string TRACK = "#MannequinChallenge";
        private const int SLEEP_TIME = 5000;
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

        private void InitializeTwitterStream()
        {
            Thread thread = new Thread(TwitterView_StartStreaming);
            thread.Start();
        }

        private void InitializeTwitterQueue()
        {
            _tweetQueue = new Queue<ITweet>();
            Thread thread = new Thread(TwitterView_Queue);
            thread.Start();
        }

        private void InitializeTwitterCard()
        {
            //this.twitterMedia.Source = new BitmapImage(new Uri(DEFAULT_TWITTER_PIC));
        }

        private void InitializeBackgroundVideo()
        {
            this.bgvideo.MediaEnded += new RoutedEventHandler(mediaElement_OnMediaEnded);
            this.bgvideo.LoadedBehavior = MediaState.Manual;
            this.bgvideo.Play();
        }

        private void InitializeWindowOptions()
        {
            _lastWindowState = this.WindowState;
            _lastWindowStyle = this.WindowStyle;
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
        }

        private void TwitterView_StartStreaming()
        {

            TwitterView twitter = new TwitterView();
            var tweetStream = Stream.CreateFilteredStream();

            // add filter
            tweetStream.AddTrack(TRACK);
            // add action event
            tweetStream.MatchingTweetReceived +=
                  new EventHandler<Tweetinvi.Events.MatchedTweetReceivedEventArgs>(TwitterView_ReceivedPost);
            tweetStream.StartStreamMatchingAllConditions();
        }

        private void TwitterView_Queue()
        {
            if (_tweetQueue.Count != 0)
            {
                // get first tweet out of queue
                ITweet tweet = _tweetQueue.Dequeue();
                var link = tweet.Url;
                var user = tweet.CreatedBy;
                var text = tweet.FullText;
                var media = tweet.Media.Count == 0 ? DEFAULT_TWITTER_PIC : tweet.Media[0].MediaURL;
                var userPic = user.ProfileImageUrlHttps;
                var screenName = user.ScreenName;


                // go on Window thread and change the contents of the View 
                Dispatcher.BeginInvoke(new Action(delegate ()
                {
                    this.profilePic.Source = new BitmapImage(new Uri(userPic));
                    this.screenName.Text = string.Format("@{0}", screenName);
                    this.realName.Text = user.ToString();
                    this.tweetText.Text = text;
                    
                    this.tweetMediaBg.Background =
                    new BrushConverter().ConvertFromString(
                        string.Format("#{0}", user.ProfileBackgroundColor)
                    ) as SolidColorBrush;

                    
                    if (tweet.Media[0].MediaType == "video" && tweet.Media[0].VideoDetails.Variants[0].URL.Last().Equals('4'))
                    {
                        this.twitterMedia.Visibility = Visibility.Hidden;
                        this.twitterVideo.Visibility = Visibility.Visible;

                        this.twitterMedia.Source = null;

                        // Convert https to http if need be
                        this.twitterVideo.Source = new Uri(tweet.Media[0].VideoDetails.Variants[0].URL.Remove(4, 1));
                    }
                    else
                    {
                        this.twitterVideo.Visibility = Visibility.Hidden;
                        this.twitterMedia.Visibility = Visibility.Visible;

                        this.twitterVideo.Source = null;
                        this.twitterMedia.Source = new BitmapImage(new Uri(media));
                    }

                    
                }));
            }
            Thread.Sleep(SLEEP_TIME);
            TwitterView_Queue();
        }

        private void TwitterView_ReceivedPost(object sender, Tweetinvi.Events.MatchedTweetReceivedEventArgs args)
        {
            // add to the queue only if there is a picture in the tweet
            if (args.Tweet.Media.Count > 0)
            {
                _tweetQueue.Enqueue(args.Tweet);

                // debugging
                Trace.WriteLine(string.Format("Tweets in Queue: {0}", _tweetQueue.Count));
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // toggle fullscreen
            if (e.Key == Key.F11)
            {
                if (WindowStyle == WindowStyle.None)
                {
                    WindowState = _lastWindowState;
                    WindowStyle = _lastWindowStyle;
                    ResizeMode = ResizeMode.CanResize;

                }
                else
                {
                    _lastWindowState = WindowState;
                    _lastWindowStyle = WindowStyle;
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                }
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size newSize = e.NewSize;
            double height = newSize.Height, width = newSize.Width;
            this.tweetText.FontSize = (height + width) / 125;
        }

        private void mediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            // start video over
            this.bgvideo.Position = new TimeSpan(0, 0, 1);
            this.bgvideo.Play();
        }
    }
}