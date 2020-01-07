using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.MVVM;
using SRPManagerV2.Types;

namespace SRPManagerV2.Models
{
    public class AwlRulesWindowModel : PropertyChangedNotification
    {
        public ObservableCollection<AwlRuleType> rules
        {
            get => GetValue(() => rules);
            set => SetValue(() => rules, value);
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public AwlRulesWindowModel()
        {
            rules = new ObservableCollection<AwlRuleType>();
        }

        ~AwlRulesWindowModel()
        {
        }
    }
}
