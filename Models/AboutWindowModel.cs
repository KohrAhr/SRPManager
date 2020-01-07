using Lib.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPManagerV2.Models
{
    public class AboutWindowModel : PropertyChangedNotification
    {
        public string Username
        {
            get => GetValue(() => Username);
            set => SetValue(() => Username, value);
        }

        public string AppBuild
        {
            get => GetValue(() => AppBuild);
            set => SetValue(() => AppBuild, value);
        }
    }
}
