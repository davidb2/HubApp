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
using System.Windows.Shapes;
using System.Diagnostics;

namespace HubApp
{
    /// <summary>
    /// Interaction logic for GUIWindow.xaml
    /// </summary>
    public partial class GUIWindow : Window
    {
        private MainWindow mainWindow;
        public GUIWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            InitializeWindowOptions();
            InitializeFormActions();
            this.mainWindow = mainWindow;
        }

        private void InitializeFormActions()
        {
            this.submitButton.Click += SubmitButton_OnClick;
        }

        private void InitializeWindowOptions()
        {
            this.SizeChanged += GUIWindow_SizeChanged;
        }

        private void SubmitButton_OnClick(object sender, EventArgs e)
        {
            string newTrack = this.track.Text.ToString();
            Trace.WriteLine(string.Format("Now tracking {0}", newTrack));
            mainWindow.SetTrack(newTrack);
        }

        private void GUIWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size newSize = e.NewSize;
            double height = newSize.Height, width = newSize.Width;
            this.track.FontSize = (height + width) / 250;
        }

    }
}
