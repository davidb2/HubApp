using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HubApp.Options.Pages
{
    /// <summary>
    /// Interaction logic for TwitterStreamOptions.xaml
    /// </summary>
    public partial class TwitterStreamOptions : Page
    {
        private GUIWindow _guiWindow;
        public TwitterStreamOptions(GUIWindow guiWindow)
        {
            InitializeComponent();
            InitializeWindowOptions();
            InitializeFormActions();
            this._guiWindow = guiWindow;
        }

        private void InitializeFormActions()
        {
            this.submitButton.Click += SubmitButton_OnClick;
        }

        private void InitializeWindowOptions()
        {
            this.SizeChanged += TwitterStreamOptionsPage_SizeChanged;
        }

        private void SubmitButton_OnClick(object sender, EventArgs e)
        {
            string newTrack = this.track.Text.ToString();
            Trace.WriteLine(string.Format("Now tracking {0}", newTrack));
            this._guiWindow.SetTrack(newTrack);
        }

        private void TwitterStreamOptionsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Size newSize = e.NewSize;
            double height = newSize.Height, width = newSize.Width;
            this.track.FontSize = (height + width) / 250;
        }
    }
}
