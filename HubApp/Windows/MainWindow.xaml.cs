using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace HubApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<Page, int> _pageMappings;
        private int _numberOfPagesSpawned;
        private WindowState _lastWindowState;
        private WindowStyle _lastWindowStyle;
        // private TwitterStream _twitterStream;
        // private Trivia _trivia;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundVideo();
            InitializeWindowOptions();
            InitializeFrameOptions();
            InitializeGUIWindow();
        }

        private void InitializeGUIWindow()
        {
            GUIWindow window = new GUIWindow(this);
            window.Show();
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
            // this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
        }

        private void InitializeFrameOptions()
        {
            this._pageMappings = new Dictionary<Page, int>();
            this._numberOfPagesSpawned = 0;
        }

        public void SetWindow(Page page)
        {
            if (!this._pageMappings.ContainsKey(page))
            {
                this.mainFrame.NavigationService.Navigate(page);
                this._pageMappings.Add(page, this._numberOfPagesSpawned++);
                return;
            }
            else
            {
                Page currentPage = (Page)this.mainFrame.Content;
                int currentPageNumber = this._pageMappings[currentPage];
                int newPageNumber = this._pageMappings[page];
                bool isInFront = newPageNumber > currentPageNumber;
                for (int i = 0; i < Math.Abs(newPageNumber - currentPageNumber); i++)
                {
                    if (isInFront)
                    {
                        this.mainFrame.GoForward();
                    }
                    else
                    {
                        this.mainFrame.GoBack();
                    }
                }
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

        private void mediaElement_OnMediaEnded(object sender, RoutedEventArgs e)
        {
            // start video over
            this.bgvideo.Position = new TimeSpan(0, 0, 1);
            this.bgvideo.Play();
        }

        //public void SetTrack(String track)
        //{
        //    if (this._twitterStream != null)
        //    {
        //        this._twitterStream.SetTrack(track);
        //    }
        //}

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }
    }
}