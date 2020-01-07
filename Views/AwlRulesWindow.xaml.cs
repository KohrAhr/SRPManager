using Lib.Data;
using SRPManagerV2.Controls;
using SRPManagerV2.Types;
using SRPManagerV2.ViewModels;
using SRPManagerV2.Views.Base;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
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
    /// Interaction logic for AwlRulesWindow.xaml
    /// </summary>
    public partial class AwlRulesWindow : BaseWindow
    {
        public string TextSearch
        {
            get
            {
                return txtSearch.Text.ToUpper();
            }
            set
            {
                txtSearch.Text = value;
            }
        }

        public string TextHighlight
        {
            get
            {
                return txtHighlight.Text.ToUpper();
            }
            set
            {
                txtHighlight.Text = value;
            }
        }

        public AwlRulesWindow()
        {
            InitializeComponent();

            DataContext = new AwlRulesWindowVM();
            ((AwlRulesWindowVM)DataContext).CloseAsOK += CloseWindowAsOK;

            ucTop.SetButtons(
                new ActionBar.AvailableButtons[]
                {
                    ActionBar.AvailableButtons.abOk
                }
            );

            ListCollectionView collection = new ListCollectionView(((AwlRulesWindowVM)DataContext).Model.rules);
            collection.GroupDescriptions.Add(new PropertyGroupDescription("SubSubType"));
            collection.SortDescriptions.Add(new SortDescription("SubSubType", ListSortDirection.Ascending));
            dgMain.ItemsSource = collection;


        }

        private void ucTop_UserControlOkButtonClicked(object sender, EventArgs e)
        {
            ((AwlRulesWindowVM)DataContext).OkCommand.Execute(null);
        }

        /// <summary>
        ///     Reset filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetSearch_Click(object sender, RoutedEventArgs e)
        {
            ResetSearch();
        }

        /// <summary>
        ///     Reset Highlight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetHighlight_Click(object sender, RoutedEventArgs e)
        {
            ResetHighlight();
        }

        private void ResetSearch()
        {
            TextSearch = "";
        }

        private void ResetHighlight()
        {
            TextHighlight = "";
        }

        /// <summary>
        ///     Data grid filter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBoxName = (TextBox)sender;
            string filterText = textBoxName.Text.ToUpper();

            FilterGridView(dgMain.ItemsSource, filterText);
        }

        private void FilterGridView(IEnumerable itemsControls, string filterText)
        {
            ICollectionView cv = CollectionViewSource.GetDefaultView(itemsControls);

            cv.Filter = o => {
                AwlRuleType p = o as AwlRuleType;

                if (String.IsNullOrEmpty(filterText))
                {
                    return true;
                }
                else
                {
                    return ClassFunctions.IsTextMatchInValues(p, filterText, true);
                }
            };
        }
    }
}
