using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace SRPManagerV2.Core
{
    /// <summary>
    ///     Responsible for monitor in real-time mode changes in (a) Event Logs and (b) Channels in Windows Event Log
    ///         "Event \ Application" -- is an Event Log
    ///         "Microsoft-Windows-AppLocker/EXE and DLL" is a Channel in Windows Event Log
    /// </summary>
    public class EventLogMonitorCore
    {
        public delegate void OnMatch();
        public delegate void OnPopupNotify(string message);

        /// <summary>
        ///     Callback function when raised matched event for "Event \ Application"
        /// </summary>
        public event OnMatch OnSrpMatchEvent = null;

        /// <summary>
        ///     Callback function when raised matched event for "Microsoft-Windows-AppLocker/EXE and DLL"
        /// </summary>
        public event OnMatch OnAppLockMatchEvent = null;

        /// <summary>
        ///     Callback function for display notification popup window with message about blocked file
        /// </summary>
        public event OnPopupNotify OnNotifyEvent = null;

        private EventLog eventLogSRP = null;
        private EventLogWatcher eventLogAppLocker = null;

        /// <summary>
        ///     Windows Events \ Applications
        /// </summary>
        public const string EVENT_APPLICATION = "Application";

        /// <summary>
        ///     
        /// </summary>
        public const string EVENT_APPLOCKER = "Microsoft-Windows-AppLocker/EXE and DLL";

        /// <summary>
        ///     Events ID for "Event \ Application" we are looking for
        /// </summary>
        private readonly long[] idSrp =
        {
            /// <summary>
            ///     Block by Path
            /// </summary>
            865,
            /// <summary>
            ///     Block by Hash
            /// </summary>
            868
        };

        /// <summary>
        ///     Events ID for "Microsoft-Windows-AppLocker/EXE and DLL" we are looking for
        ///     https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/applocker/using-event-viewer-with-applocker
        /// </summary>
        private readonly long[] idAppLock =
        {
            /// <summary>
            ///     *File name* was allowed to run but would have been prevented from running if the AppLocker policy were enforced. 
            ///     Applied only when the **Audit only ** enforcement mode is enabled. Specifies that the .exe or .dll file would be blocked if the **Enforce rules ** enforcement mode were enabled. 
            /// </summary>
            8003,
            /// <summary>
            ///     *File name* was not allowed to run.
            ///     Access to *file name* is restricted by the administrator. Applied only when the **Enforce rules ** enforcement mode is set either directly or indirectly through Group Policy inheritance. The .exe or .dll file cannot run.
            /// </summary>
            8004,

            /// <summary>
            ///     *File name* was allowed to run but would have been prevented from running if the AppLocker policy were enforced. 
            ///     Applied only when the **Audit only ** enforcement mode is enabled. Specifies that the script or .msi file would be blocked if the **Enforce rules ** enforcement mode were enabled.
            /// </summary>
            8006,
            /// <summary>
            ///     *File name* was not allowed to run.
            ///     Access to *file name* is restricted by the administrator. Applied only when the **Enforce rules ** enforcement mode is set either directly or indirectly through Group Policy inheritance. The script or .msi file cannot run.
            /// </summary>
            8007
        };

        /// <summary>
        ///     Constructor. Set Hook
        /// </summary>
        public EventLogMonitorCore()
        {
            // Application - SRP
            eventLogSRP = new EventLog(EVENT_APPLICATION);
            eventLogSRP.EntryWritten += new EntryWrittenEventHandler(OnApplicationEventWritten);
            eventLogSRP.EnableRaisingEvents = true;

            // AppLocker
            eventLogAppLocker = new EventLogWatcher(EVENT_APPLOCKER);
            eventLogAppLocker.EventRecordWritten += OnApplockEventWritten;
            eventLogAppLocker.Enabled = true;
        }

        /// <summary>
        ///     Destructor
        /// </summary>
        ~EventLogMonitorCore()
        {
            eventLogSRP.EnableRaisingEvents = false;
            eventLogSRP.Dispose();

            eventLogAppLocker.Enabled = false;
            eventLogAppLocker.Dispose();
        }

        /// <summary>
        ///     Application events
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void OnApplicationEventWritten(object source, EntryWrittenEventArgs e)
        {
            if (e.Entry != null && idSrp.Contains(e.Entry.InstanceId))
            {
                OnSrpMatchEvent?.Invoke();

                ShowBlockPopup(e.Entry.ReplacementStrings[0]);
            }
        }

        /// <summary>
        ///     AppLocker event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnApplockEventWritten(object sender, EventRecordWrittenEventArgs e)
        {
            if (e.EventRecord != null && idAppLock.Contains(e.EventRecord.Id))
            {
                OnAppLockMatchEvent?.Invoke();

                ShowBlockPopup(e.EventRecord.FormatDescription());
            }
        }

        private void ShowBlockPopup(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                OnNotifyEvent?.Invoke(message);
            }
        }
    }
}
