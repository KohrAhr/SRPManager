using Hardcodet.Wpf.TaskbarNotification;
using Lib.MVVM;
using Lib.Strings;
using Lib.System;
using Lib.UI;
using SRPManagerV2.Functions;
using SRPManagerV2.Core;
using SRPManagerV2.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Controls;
using System.ServiceProcess;

namespace SRPManagerV2.ViewModels
{
    public partial class NotifyIconVM : PropertyChangedNotification
    {
        #region Commands definition
        public ICommand CmdExit { get; set; }
        public ICommand CmdAbout { get; set; }
        public ICommand CmdRunAs { get; set; }
        public ICommand CmdGPupdate { get; set; }
        public ICommand CmdShowLog { get; set; }
        public ICommand CmdShowAwl { get; set; }
        public ICommand CmdEnableAWL { get; set; }
        public ICommand CmdDisableAWL { get; set; }
        public ICommand CmdEnforceWhileRun { get; set; }
        public ICommand CmdLanguageEN { get; set; }
        public ICommand CmdLanguageRU { get; set; }
        public ICommand CmdStartSrpV2 { get; set; }
        public ICommand CmdStopSrpV2 { get; set; }
        public ICommand CmdRestartSrpV2 { get; set; }
        #endregion Commands definition

        private FileMonitorCore _FileMonitorCore = null;
        private EventLogMonitorCore _EventLogMonitorCore = null;
        private RegistryMonitorCore _RegistryMonitorCore = null;

        private ServicesFunctions servicesFunctions = new ServicesFunctions();

        /// <summary>
        ///     Indicate that command GpUpdate.EXE is running
        /// </summary>
        private Process GPUpdate = null;
        private bool RunAsAdmin = false;

        /// <summary>
        ///     Current status
        /// </summary>
        public bool EnforceWhileRun
        {
            get
            {
                return AppData.EnforceWhileRun;
            }
            set
            {
                AppData.EnforceWhileRun = value;
                NotifyPropertyChanged(nameof(EnforceWhileRunInProgress));

                #region Force Mode
                if (value == true)
                {
                    AppData._HklmRunning = AppData.HklmRunning;
                }
                #endregion Force Mode
            }
        }

        /// <summary>
        ///     
        /// </summary>
        public bool SrpHklmRunning
        {
            get
            {
                return AppData.HklmRunning == Status.sOn;
            }
            set
            {
                SetValue(() => SrpHklmRunning, value);
                NotifyPropertyChanged(nameof(DisableSrp));
            }
        }

        //public bool SrpHkcuRunning
        //{
        //    get
        //    {
        //        return AppData.HkcuRunning == Status.sOn;
        //    }
        //    set
        //    {
        //        SetValue(() => SrpHkcuRunning, value);
        //        NotifyPropertyChanged(nameof(DisableSrp));
        //    }
        //}

        public bool PolicyScore
        {
            get
            {
                return AppData.PolicyScore;
            }
            set
            {
            }
        }

        public bool EnableNotifications
        {
            get => GetValue(() => EnableNotifications);
            set
            {
                SetValue(() => EnableNotifications, value);
                AppData.EnableNotifications = value;
            }
        }

        public bool AuthentiCodeEnabled
        {
            get
            {
                return AppData.AuthentiCodeEnabled;
            }
            set
            {
            }
        }

        public bool TransparentEnabled
        {
            get
            {
                return AppData.TransparentEnabled;
            }
            set
            {
            }
        }

        /// <summary>
        ///     HKLM DefaultLevel can write to registry -- Yes/No
        /// </summary>
        public bool HklmKeyExist
        {
            get => GetValue(() => HklmKeyExist);
            set
            {
                SetValue(() => HklmKeyExist, value);
            }
        }
        ///// <summary>
        /////     HKCU DefaultLevel can write to registry -- Yes/No
        ///// </summary>
        //public bool HkcuKeyExist
        //{
        //    get => GetValue(() => HkcuKeyExist);
        //    set
        //    {
        //        SetValue(() => HkcuKeyExist, value);
        //    }
        //}

        public long TotalAppLockBlocks
        {
            get => GetValue(() => TotalAppLockBlocks);
            set
            {
                SetValue(() => TotalAppLockBlocks, value);
                NotifyPropertyChanged(nameof(TotalSummary));
            }
        }
        public long TotalSrpBlocks
        {
            get => GetValue(() => TotalSrpBlocks);
            set
            {
                SetValue(() => TotalSrpBlocks, value);
                NotifyPropertyChanged(nameof(TotalSummary));
            }
        }
        public string TotalSummary
        {
            get
            {
                return String.Format(
                    StringsFunctions.ResourceString("resSummaryValue"),
                    TotalSrpBlocks, TotalAppLockBlocks
                );
            }
            set => SetValue(() => TotalSummary, value);
        }

        private ServiceControllerStatus SrpV2Status;

        public string SrpV2Summary
        {
            get
            {
                SrpV2Status = new ServicesFunctions().GetServiceStatus(AppConsts.SERVICE_SRPV2);
                // Collect information from Registry. The same data we displaying in different form :/
                // Here we should show summary about SrpV2 rules.
                string status = new ServicesFunctions().ServiceStatusToString(SrpV2Status);

                return String.Format(
                    StringsFunctions.ResourceString("resSrpV2SummaryValue"),
                    status
                );
            }
            set => SetValue(() => SrpV2Summary, value);
        }

        public string DisableSrp
        {
            get
            {
                if (DisableSrpInProgress)
                {
                    //string result = String.Format(
                    //    "{0}{1}{2}",
                    //    SrpHklmRunning ? "(Computer only)" : "",
                    //    SrpHklmRunning && SrpHkcuRunning ? " & " : "",
                    //    SrpHkcuRunning ? "(Computer and User)" : ""
                    //);


                    string result = String.Format(
                        "{0}",
                        SrpHklmRunning ? StringsFunctions.ResourceString("resComputerOnly") : ""
                    );

                    return StringsFunctions.ResourceString("resAwlDisabled") + " " + result;
                }
                else
                {
                    return StringsFunctions.ResourceString("resDisableAwl");
                }
            }
            set => SetValue(() => DisableSrp, value);
        }

        #region Visibility
        public Visibility ShowLogVisibility
        {
            get
            {
                if (String.IsNullOrEmpty(AppData.SrpFileName))
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            set
            {

            }
        }

        public Visibility EnforceWhileRunVisibility
        {
            get
            {
                return Visibility.Visible;
            }
            set
            {

            }
        }
        public bool EnforceWhileRunInProgress
        {
            get
            {
                return EnforceWhileRun;
            }
            set
            {

            }
        }

        public bool DisableSrpInProgress
        {
            get
            {
                return !CmdDisableAWLProcEnabled(null);
            }
            set
            {

            }
        }

        public Visibility UserModeDetectedVisibility
        {
            get
            {
                if (!HklmKeyExist)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {

            }
        }

        public Visibility AdminModeDetectedVisibility
        {
            get
            {
                if (HklmKeyExist)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {

            }
        }

        public Visibility EnableAWL
        {
            get
            {
                if (!CmdDisableAWLProcEnabled(null))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {

            }
        }

        public Visibility AwlEnabled
        {
            get
            {
                if (CmdDisableAWLProcEnabled(null))
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            set
            {

            }
        }
        #endregion Visibility

        public void SwitchMode()
        {
            // Set current status
            if (AppData.MustSwitch)
            {
                switch (AppData.SwitchToStatus)
                {
                    case (Status.sOff):
                        {
                            SetMagicNumber(AppConsts.SRP_OFF);
                            break;
                        }
                    case (Status.sOn):
                        {
                            SetMagicNumber(AppConsts.SRP_ON);
                            break;
                        }
                    case (Status.sBasicUser):
                        {
                            SetMagicNumber(AppConsts.SRP_BASIC);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }
            }
            AppData.MustSwitch = false;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        public NotifyIconVM()
        {
            InitCommands();

            InitData();

            SwitchMode();

            Functions.MMFFunctions.OnRequestEvent += MMFFunctions_OnRequestEvent;

            // #1
//            _FileMonitorCore.OnMatchEvent += new FileMonitorCore.OnMatch(OnSrpRecordAdded);

            // #2
            _EventLogMonitorCore.OnSrpMatchEvent += new EventLogMonitorCore.OnMatch(OnSrpEventBlock);
            _EventLogMonitorCore.OnAppLockMatchEvent += new EventLogMonitorCore.OnMatch(OnAppLockEventBlock);
            _EventLogMonitorCore.OnNotifyEvent += new EventLogMonitorCore.OnPopupNotify(OnPopupNotify);

            // #3
            _RegistryMonitorCore.InitRegistryMonitor();
            _RegistryMonitorCore.OnMatchEvent += new RegistryMonitorCore.OnMatch(OnAwlChanged);

            // Load without update, because ViewModel creating before View :/
            // +1h
            _RegistryMonitorCore.LoadDataFromRegistry(true);

            AppData.MenuOpen = NotifyIcon_TrayRightMouseUp1;
        }

        private void NotifyIcon_TrayRightMouseUp1(object sender, RoutedEventArgs e)
        {
            // Notify property change
            object o = AppData.notifyIcon.DataContext;
            AppData.notifyIcon.DataContext = null;
            AppData.notifyIcon.DataContext = o;

            e.Handled = false;
        }

        ~NotifyIconVM()
        {
        }




        private void MMFFunctions_OnRequestEvent()
        {
            SwitchMode();

            NotifyPropertyChanged(nameof(EnforceWhileRunInProgress));
        }

        private void InitCommands()
        {
            CmdExit = new RelayCommand(CmdExitProc);
            CmdAbout = new RelayCommand(CmdAboutProc);
            CmdRunAs = new RelayCommand(CmdRunAsProc);
            CmdGPupdate = new RelayCommand(CmdGPupdateProc, CmdGPupdateProcEnabled);
            CmdShowLog = new RelayCommand(CmdShowLogProc);
            CmdShowAwl = new RelayCommand(CmdShowAwlProc);
            CmdEnableAWL = new RelayCommand(CmdEnableAWLProc, CmdEnableAWLProcEnabled);
            CmdDisableAWL = new RelayCommand(CmdDisableAWLProc, CmdDisableAWLProcEnabled);
            CmdEnforceWhileRun = new RelayCommand(CmdEnforceWhileRunProc, CmdEnforceWhileRunProcEnabled);

            CmdLanguageEN = new RelayCommand(CmdLanguageENProc);
            CmdLanguageRU = new RelayCommand(CmdLanguageRUProc);

            CmdStartSrpV2 = new RelayCommand(CmdStartSrpV2Proc, CmdStartSrpV2ProcEnabled);
            CmdStopSrpV2 = new RelayCommand(CmdStopSrpV2Proc, CmdStopSrpV2ProcEnabled);
            // Enabled -- the same as for Run
            CmdRestartSrpV2 = new RelayCommand(CmdRestartSrpV2Proc, CmdStopSrpV2ProcEnabled);
        }
        
        private void InitData()
        {
            AppData.SrpFileName = CoreFunctions.GetSrpLogFile();
            RunAsAdmin = UserFunctions.IsCurrentUserAdmin();

            HklmKeyExist = CoreFunctions.HasRightToWriteInRegistry();
            //HkcuKeyExist = CoreFunctions.HasRightToWriteInRegistry(Registry.CurrentUser);

            // NLog settings not defined? Do not run SRP Log file monitor.
            if (AppData._nLog.Factory.Configuration != null)
            {
                _FileMonitorCore = new FileMonitorCore(AppData.SrpFileName);
            }
            _EventLogMonitorCore = new EventLogMonitorCore();
            _RegistryMonitorCore = new RegistryMonitorCore();
        }


        private void CmdStartSrpV2Proc(object o)
        {
            new ServicesFunctions().StartService(AppConsts.SERVICE_SRPV2);
        }
        private bool CmdStartSrpV2ProcEnabled(object o)
        {
            return SrpV2Status == ServiceControllerStatus.Stopped || SrpV2Status == ServiceControllerStatus.Paused;
        }

        private void CmdStopSrpV2Proc(object o)
        {
            new ServicesFunctions().StopService(AppConsts.SERVICE_SRPV2);
        }
        private bool CmdStopSrpV2ProcEnabled(object o)
        {
            return SrpV2Status == ServiceControllerStatus.Running || SrpV2Status == ServiceControllerStatus.Paused;
        }

        private void CmdRestartSrpV2Proc(object o)
        {
            new ServicesFunctions().RestartService(AppConsts.SERVICE_SRPV2);
        }

        private void CmdLanguageENProc(object o)
        {
            LocalizationHelper.SelectCulture("en-US");
            ReattachContextMenu();
        }

        private void CmdLanguageRUProc(object o)
        {
            LocalizationHelper.SelectCulture("ru-RU");
            ReattachContextMenu();
        }

        // TODO: Move out! Rewrite!
        /// <summary>
        ///     Move out! Rewrite!
        /// </summary>
        private void ReattachContextMenu()
        {
            // AdHock
            AppData.notifyIcon.ContextMenu = null;
            AppData.notifyIcon.ContextMenu = (ContextMenu)System.Windows.Application.Current?.TryFindResource("SysTrayMenu");
        }

        private void CmdEnforceWhileRunProc(object o)
        {
            EnforceWhileRun = !EnforceWhileRun;
        }
        private bool CmdEnforceWhileRunProcEnabled(object o)
        {
            return HklmKeyExist && RunAsAdmin;
        }

        private void CmdEnableAWLProc(object o)
        {
            SetMagicNumber(AppConsts.SRP_OFF);
        }
        private bool CmdEnableAWLProcEnabled(object o)
        {
            return HklmKeyExist && AppData.HklmRunning == Status.sOn;
        }

        private void CmdDisableAWLProc(object o)
        {
            SetMagicNumber(AppConsts.SRP_ON);
        }
        private bool CmdDisableAWLProcEnabled(object o)
        {
            return HklmKeyExist && AppData.HklmRunning == Status.sOff;
        }

        private bool SetMagicNumber(string magicNumber)
        {
            bool result = false;

            HklmKeyExist = false;
            //HkcuKeyExist = false;

            if (new RegistryFunctions().CanSetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL))
            {
                #region Force Mode
                if (EnforceWhileRun)
                {
                    AppData._HklmRunning = _RegistryMonitorCore.StringToStatus(magicNumber);
                }
                #endregion Force Mode

                HklmKeyExist = true;
                result = new RegistryFunctions().SetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, magicNumber, RegistryValueKind.DWord);
            };

            return result;

            //if (new RegistryFunctions().CanSetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, Registry.CurrentUser))
            //{
            //    HkcuKeyExist = true;
            //    new RegistryFunctions().SetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, magicNumber, RegistryValueKind.DWord, Registry.CurrentUser);
            //}
        }

        private void OnAwlChanged()
        {
            #region Force Mode
            if (EnforceWhileRun)
            {
                switch (AppData._HklmRunning)
                {
                    case (Status.sOff):
                    {
                        if (SetMagicNumber(AppConsts.SRP_OFF))
                        {
                            return;
                        }
                        break;                           
                    }
                    case (Status.sOn):
                    {
                        if (SetMagicNumber(AppConsts.SRP_ON))
                        {
                            return;
                        }
                        break;
                    }
                    case (Status.sBasicUser):
                    {
                        if (SetMagicNumber(AppConsts.SRP_BASIC))
                        {
                            return;
                        }
                        break;
                    }
                }
            }
            #endregion Force Mode

            //  NotifyPropertyChanged(nameof(SrpHkcuRunning));
            NotifyPropertyChanged(nameof(SrpHklmRunning));

            NotifyPropertyChanged(nameof(PolicyScore));
            NotifyPropertyChanged(nameof(AuthentiCodeEnabled));
            NotifyPropertyChanged(nameof(TransparentEnabled));
        }

        #region Event Log alerts
        /// <summary>
        ///     Additional handler -- just counter of events
        /// </summary>
        private void OnSrpEventBlock()
        {
            TotalSrpBlocks++;
        }

        private void OnAppLockEventBlock()
        {
            TotalAppLockBlocks++;
        }
        #endregion Event Log alerts

        private void OnPopupNotify(string message)
        {
            AppData.notifyIcon.CloseBalloon();
            AppData.notifyIcon.ShowBalloonTip(StringsFunctions.ResourceString("resBlocked"), message, BalloonIcon.Error);
        }
    }
}
