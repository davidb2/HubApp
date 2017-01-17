using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HubApp.Utilities
{
    /// <summary>
    /// Interaction logic for TabablePage.xaml
    /// </summary>
    public partial class TabablePage : Page, Tabable
    {
        public virtual void OnResume()
        {

        }

        public virtual void OnPause()
        {

        }
    }
}