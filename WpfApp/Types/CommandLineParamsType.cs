using SRPManagerV2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRPManagerV2.Types
{
    public class CommandLineParamsType
    {
        /// <summary>
        ///     Показать Help
        /// </summary>
        public bool ShowHelp = false;

        /// <summary>
        ///     New status.
        ///     If it's NotAvailable then it's not assigned.
        /// </summary>
        public Status RequestedStatus = Status.sNotAvailable;

        /// <summary>
        ///     Новый instancte является новым и должен стать единственным instance?
        /// </summary>
        public bool MasterInstance = false;

        /// <summary>
        ///     Force mode
        /// </summary>
        public bool? ForceMode = null;

        /// <summary>
        ///     Режим тишины (не показывать диалоговые окна)
        /// </summary>
//        public bool SilentMode = false;

        /// <summary>
        ///     Запрос на завершение работы приложения
        /// </summary>
        public bool ExitRequest = false;

        /// <summary>
        ///     Не проверять на Mutex
        /// </summary>
        public bool IgnoreMutex = false;
    }
}
