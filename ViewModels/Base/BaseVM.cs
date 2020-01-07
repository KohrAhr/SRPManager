using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SRPManagerV2.ViewModels.Base
{
    public class BaseVM
    {
        #region Events 
        public event Action CloseAsOK;
        public event Action CloseAsCancel;

        protected void CloseAsOkEvent()
        {
            CloseAsOK?.Invoke();
        }

        protected void CloseAsCancelEvent()
        {
            CloseAsCancel?.Invoke();
        }
        #endregion Events

        #region Commands definition
        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        #endregion Commands definition
    }
}
