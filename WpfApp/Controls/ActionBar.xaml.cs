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
using Lib.MVVM;
using Lib.UI;

namespace SRPManagerV2.Controls
{
    /// <summary>
    /// Interaction logic for ItemFormActionBar.xaml
    /// </summary>
    public partial class ActionBar : UserControl
    {
        public enum AvailableButtons
        {
            abOk = 0,
            abContinue = 1,
            abCancel = 2
        };

        #region Events definition
        public event EventHandler UserControlOkButtonClicked;
        public event EventHandler UserControlContinueButtonClicked;
        #endregion Events definition

        #region Commands definition
        //public ICommand NewCommand { get; set; }
        //public ICommand UpdateCommand { get; set; }
        #endregion Commands definition

        public ActionBar()
        {
            InitializeComponent();

//            InitCommands();
        }

        //private void InitCommands()
        //{
        //    NewCommand = new RelayCommand(NewCommandProc);
        //    UpdateCommand = new RelayCommand(UpdateCommandProc);
        //}

        //public void NewCommandProc(object o)
        //{
        //    UserControlNewButtonClicked?.Invoke(this, EventArgs.Empty);
        //}

        //private void UpdateCommandProc(object o)
        //{
        //    UserControlUpdateButtonClicked?.Invoke(this, EventArgs.Empty);
        //}

        public void SetButtons(AvailableButtons[] availableButtons)
        {
            // Hide all
            foreach(AvailableButtons i in Enum.GetValues(typeof(AvailableButtons)))
            {
                HideButtonByTag((int)i);
            }

            // Set only what we need
            foreach (AvailableButtons i in availableButtons)
            {
                ShowButtonByTag((int)i);
            }
        }

        private void HideButtonByTag(int buttonTag)
        {
            // TODO: gdTop should be replaced on this
            var z = UIFunctions.FindControl<WrapPanel>(this.gdTop);
            Button element = UIFunctions.FindControlByTag<Button>(z, buttonTag.ToString());
            if (element != null)
            {
                element.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowButtonByTag(int buttonTag)
        {
            // TODO: gdTop should be replaced on this
            Button element = UIFunctions.FindControlByTag<Button>(gdTop, buttonTag.ToString());
            if (element != null)
            {
                element.Visibility = Visibility.Visible;
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            UserControlOkButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            UserControlContinueButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
