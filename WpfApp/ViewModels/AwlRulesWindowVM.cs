using Lib.MVVM;
using Lib.Strings;
using Lib.System;
using Microsoft.Win32;
using SRPManagerV2.Core;
using SRPManagerV2.Models;
using SRPManagerV2.Types;
using SRPManagerV2.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace SRPManagerV2.ViewModels
{
    public class AwlRulesWindowVM : BaseVM
    {
        public AwlRulesWindowModel Model { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        public AwlRulesWindowVM()
        {
            InitData();

            InitCommands();
        }

        private void InitData()
        {
            Model = new AwlRulesWindowModel();

            // Get SRP rules from registry
            new SrpRulesCore().ProceedSrp(Model.rules);

            // Get AppLocker rules from registry
        }

        /// <summary>
        /// </summary>
        private void InitCommands()
        {
            OkCommand = new RelayCommand(OkCommandProc);
        }

        private void OkCommandProc(object o)
        {
            // Raise Close event
            CloseAsOkEvent();
        }
    }
}
