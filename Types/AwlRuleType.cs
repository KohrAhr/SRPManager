using Lib.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPManagerV2.Types
{
    public class AwlRuleType : PropertyChangedNotification
    {
        public string AwlType
        {
            get => GetValue(() => AwlType);
            set => SetValue(() => AwlType, value);
        }

        public string SubType
        {
            get => GetValue(() => SubType);
            set => SetValue(() => SubType, value);
        }

        /// <summary>
        ///     PC/User
        /// </summary>
        public string Scope
        {
            get => GetValue(() => Scope);
            set => SetValue(() => Scope, value);
        }

        /// <summary>
        ///     On/Off/Basic
        /// </summary>
        public string SubSubType
        {
            get => GetValue(() => SubSubType);
            set => SetValue(() => SubSubType, value);
        }

        /// <summary>
        ///     ItemData
        /// </summary>
        public string Value
        {
            get => GetValue(() => Value);
            set => SetValue(() => Value, value);
        }

        /// <summary>
        ///     SaferFlag
        /// </summary>
        public string Value2
        {
            get => GetValue(() => Value2);
            set => SetValue(() => Value2, value);
        }

        /// <summary>
        ///     Description
        /// </summary>
        public string Description
        {
            get => GetValue(() => Description);
            set => SetValue(() => Description, value);
        }

        /// <summary>
        ///     Last modified
        /// </summary>
        public DateTime LastModified
        {
            get => GetValue(() => LastModified);
            set => SetValue(() => LastModified, value);
        }
    }
}
