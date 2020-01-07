using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Text;
using System.Windows.Media;
using NLog;
using System.IO.MemoryMappedFiles;
using System.Threading.Tasks;
using System.Linq;
using System.Windows;

namespace SRPManagerV2.Core
{
    /// <summary>
    ///     The same list in icons
    /// </summary>
    public enum Status
    {
        sNotAvailable,
        sOff,
        sOn,
        sBasicUser
    }

    public static class AppData
    {
        #region MMF
        public static MemoryMappedFile memoryMappedFile = null;

        /// <summary>
        ///     Используется в мониторинге
        /// </summary>
        public static MemoryMappedViewAccessor memoryMappedViewAccessor = null;

        /// <summary>
        ///     Используется для мониторинга
        /// </summary>
        public static Task watcher = null;

        /// <summary>
        ///     Используется в мониторинге
        /// </summary>
        public static bool runWatchThread = true;

        public static readonly Int16 COMMAND = 666;

        public static readonly byte SUB_COMMAND_EXIT = 13;

        /// <summary>
        ///     New params from 2nd instance is comming
        /// </summary>
        public static readonly byte SUB_COMMAND_CHANGE = 77;

        /// <summary>
        ///     Close 2nd instance and run 1st instanct
        /// </summary>
        public static readonly byte SUB_COMMAND_ACTIVATE_THIS_INSTANCE = 88;
        #endregion MMF

        public static RoutedEventHandler MenuOpen = null;

//        public static bool SilentMode = false;

        // TODO: Выкинуть все три и сделать ф-ции
        public static bool MustSwitch = false;
        public static Status SwitchToStatus = Status.sNotAvailable;
        public static bool EnforceWhileRun = false;

        /// <summary>
        ///     Instance on Tray icon. We create it in XAML, but because we need access to icon we need variable also.
        /// </summary>
        public static TaskbarIcon notifyIcon = null;

        /// <summary>
        ///     All my icons: Init/Off/On/Basic
        /// </summary>
        public static ImageSource[] imageSources = new ImageSource[Enum.GetValues(typeof(Status)).Cast<int>().Max() + 1];

        /// <summary>
        ///     SRPv2 log file. Obtaining from Registry
        /// </summary>
        public static string SrpFileName = "";

        /// <summary>
        ///     NLog instance. Using for save with timestamp splitted data from SRPv2 log file.
        /// </summary>
        public static Logger _nLog = LogManager.GetLogger("MainLogger");

        /// <summary>
        ///     Более одного раза обращаюсь, пусть будет
        /// </summary>
        public static readonly int NewLineLength = Environment.NewLine.Length;

//        public static Status HkcuRunning = Status.sNotAvailable;
        /// <summary>
        ///     Current value/status
        /// </summary>
        public static Status HklmRunning = Status.sNotAvailable;

        #region Force Mode
        /// <summary>
        ///     Previous value
        /// </summary>
        public static Status _HklmRunning = Status.sNotAvailable;
        #endregion Force Mode

        /// <summary>
        ///     Current value/status
        /// </summary>
        public static bool PolicyScore = false;

        /// <summary>
        ///     Popup notification enabled/disabled
        /// </summary>
        public static bool EnableNotifications = false;

        /// <summary>
        ///     Current value/status
        /// </summary>
        public static bool AuthentiCodeEnabled = false;

        /// <summary>
        ///     Current value/status
        /// </summary>
        public static bool TransparentEnabled = false;
    }
}
