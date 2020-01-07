using Hardcodet.Wpf.TaskbarNotification;
using Lib.Strings;
using Lib.System;
using Microsoft.Win32;
using SRPManagerV2.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SRPManagerV2.Core
{
    public class RegistryMonitorCore
    {
        public delegate void OnMatch();
        public event OnMatch OnMatchEvent = null;

        public void InitRegistryMonitor()
        {
            // Start monitor
            RegistryMonitorFunctions monitor = new RegistryMonitorFunctions(RegistryHive.LocalMachine, AppConsts.KEY_SRP_NODE);
            monitor.RegChanged += new EventHandler(OnRegChanged);
            monitor.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRegChanged(object sender, EventArgs e)
        {
            LoadDataFromRegistry(false);

            OnMatchEvent?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceIconUpdate">
        ///     Using only during startup. Because we need to set right initial icon
        /// </param>
        public void LoadDataFromRegistry(bool update = true)
        {
            string result;

            // Load data from Registry
//            string HkcuResult = new RegistryFunctions().GetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, RegistryValueKind.DWord, Registry.CurrentUser);
            string HklmResult = new RegistryFunctions().GetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, RegistryValueKind.DWord);

            result = new RegistryFunctions().GetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_POLICY_SCOPE, RegistryValueKind.DWord);
            AppData.PolicyScore = result == AppConsts.KEY_SRP_POLICY_SCORE_ON;

            result = new RegistryFunctions().GetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_AUTHENTI_CODE_ENABLED, RegistryValueKind.DWord);
            AppData.AuthentiCodeEnabled = result == AppConsts.KEY_SRP_AUTHENTI_CODE_ENABLED_ON;

            result = new RegistryFunctions().GetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_TRANSPARENT_ENABLED, RegistryValueKind.DWord);
            AppData.TransparentEnabled = result != AppConsts.KEY_SRP_TRANSPARENT_ENABLED_OFF;

//            Status _HkcuRunning = StringToStatus(HkcuResult);
            Status _HklmRunning = StringToStatus(HklmResult);

            //Status _HkcuRunning = HkcuResult == AppConsts.SRP_OFF;
            //Status _HklmRunning = HklmResult == AppConsts.SRP_OFF;

            // If necessary -- change Icon status
            if ((/*AppData.HkcuRunning != _HkcuRunning ||*/ AppData.HklmRunning != _HklmRunning) && !update)
            {
                //AppData.HkcuRunning = _HkcuRunning;
                AppData.HklmRunning = _HklmRunning;

                UpdateIconStatus(AppData.HklmRunning);
            }

            // Useless?
            if (AppData._HklmRunning == Status.sNotAvailable && AppData.HklmRunning != Status.sNotAvailable)
            {
                AppData._HklmRunning = AppData.HklmRunning;
            }

        }

        public Status StringToStatus(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return Status.sNotAvailable;
            }

            switch (value)
            {
                case (AppConsts.SRP_OFF):
                {
                    return Status.sOff;
                }
                case (AppConsts.SRP_ON):
                {
                    return Status.sOn;
                }
                case (AppConsts.SRP_BASIC):
                {
                    return Status.sBasicUser;
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }
        }

        public void UpdateIconStatusFirstTime()
        {
            UpdateIconStatus(AppData.HklmRunning, true);
        }

        public void UpdateIconStatus(Status iconStatus, bool withoutDelay = false)
        {
            // Add to list of task
            //lock (AppData.Statuses)
            //{
            //    AppData.Statuses.Add(iconStatus);
            //}

            //if (!withoutDelay)
            //{
            //    // Check for ignoring patterns: Status.sOff -> Status.sNotAvailable -> Status.sOff
            //    int i = statuses.IndexOf(Status.sOff);
            //    if (i > statuses.IndexOf(Status.sNotAvailable) && statuses.IndexOf(Status.sNotAvailable) > statuses.IndexOf(Status.sOff, i))
            //    {
            //        statuses.RemoveRange(i, 3);
            //    }
            //}
            //// Run Timer if needed
            //if (statuses.Count == 0)
            //{
            //    return;
            //}

            Task task = new Task(() =>
            {
                //lock (AppData.Statuses)
                //{
                //    iconStatus = AppData.Statuses[AppData.Statuses.Count - 1];
                //    AppData.Statuses.Clear();
                //};

                BalloonIcon icon;
                string status = "";

                switch (iconStatus)
                {
                    case (Status.sNotAvailable):
                        {
                            status = StringsFunctions.ResourceString("resSrpDisabled");
                            icon = BalloonIcon.Error;
                            break;
                        }
                    case (Status.sOff):
                        {
                            status = StringsFunctions.ResourceString("resSrpInWhiteList");
                            icon = BalloonIcon.Info;
                            break;
                        }
                    case (Status.sOn):
                        {
                            status = StringsFunctions.ResourceString("resSrpInBlackList");
                            icon = BalloonIcon.Warning;
                            break;
                        }
                    case (Status.sBasicUser):
                        {
                            status = StringsFunctions.ResourceString("resSrpInBasicUser");
                            icon = BalloonIcon.Warning;
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException();
                        }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    AppData.notifyIcon.IconSource = AppData.imageSources[(int)iconStatus];
                    AppData.notifyIcon.ToolTipText = status;

                    if (AppData.EnableNotifications)
                    {
                        AppData.notifyIcon.CloseBalloon();
                        AppData.notifyIcon.ShowBalloonTip(StringsFunctions.ResourceString("resVersion"), status, icon);

                        GC.Collect();
                    }
                });
            });
            task.Start();

            //    if (!withoutDelay)
            //    {
            //        Thread.Sleep(5000);
            //    }

            //    int c = 0;
            //    lock (AppData.Statuses)
            //    {
            //        c = AppData.Statuses.Count();
            //    }

            //    if (c == 0)
            //    {
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    AppData.notifyIcon.CloseBalloon();
            //    AppData.notifyIcon.ShowBalloonTip(StringsFunctions.ResourceString("resVersion"), status, icon);
            //});
            //    }

            //    GC.Collect();
            //});
        }
    }
}
