using SRPManagerV2.Controls;
using SRPManagerV2.ViewModels;
using SRPManagerV2.Views.Base;
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

namespace SRPManagerV2.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : BaseWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            DataContext = new AboutWindowVM();
            ((AboutWindowVM)DataContext).CloseAsOK += CloseWindowAsOK;

            ucTop.SetButtons(
                new ActionBar.AvailableButtons[]
                {
                    ActionBar.AvailableButtons.abOk
                }
            );
        }

        private void ucTop_UserControlOkButtonClicked(object sender, EventArgs e)
        {
            ((AboutWindowVM)DataContext).OkCommand.Execute(null);
        }
    }
}
