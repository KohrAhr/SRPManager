using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib.MVVM;
using Lib.System;
using SRPManagerV2.Models;
using SRPManagerV2.ViewModels.Base;

namespace SRPManagerV2.ViewModels
{
    public class AboutWindowVM : BaseVM
    {
        public AboutWindowModel Model { get; set; }

        /// <summary>
        ///     Constuctor
        /// </summary>
        public AboutWindowVM()
        {
            InitData();

            InitCommands();
        }

        private void InitData()
        {
            Model = new AboutWindowModel();

            Model.Username = UserFunctions.GetFQDN();
            Model.AppBuild = AppFunctions.GetAppBuild();
        }

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
