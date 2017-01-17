using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HubApp.Utilities
{
    interface Tabable
    {
        /// <summary>
        /// Called when tab is switched off
        /// </summary>
        void OnPause();

        /// <summary>
        /// Called when tab is switch onto after already being open
        /// </summary>
        void OnResume();
    }
}
