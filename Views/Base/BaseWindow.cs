using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SRPManagerV2.Views.Base
{
    public class BaseWindow : Window
    {
        /// <summary>
        ///     Close current window as OK
        /// </summary>
        public void CloseWindowAsOK()
        {
            if (Parent != null)
            {
                DialogResult = true;
            }
            Close();
        }

        public void CloseWindowAsCancel()
        {
            DialogResult = false;
            Close();
        }
    }
}
