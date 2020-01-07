using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using SRPManagerV2.Functions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using Lib.Strings;
using Lib.System;
using SRPManagerV2.Core;
using System.Threading;
using SRPManagerV2.Types;
using System.Windows.Controls;
using System.Linq;
using System.IO.MemoryMappedFiles;

namespace SRPManagerV2
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static Mutex mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool shutDown = false;
            CommandLineParamsType commandLineParams = CoreFunctions.RecognizeCommandLineParams(e);

            if (!CoreFunctions.ApplyCommandLineParameters(commandLineParams))
            {
                return;
            }

            //////////////

            string currentSessionUsername = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            int currentSessionId = 0;
            bool alreadyExistInCurrent = false;

            // Get all sessions with username
            List<SessionsFunctions.SessionInfo> sessionInfos = new SessionsFunctions().ListUsers(Environment.MachineName).Where(z => !String.IsNullOrEmpty(z.DomainName) && !String.IsNullOrEmpty(z.UserName)).ToList();
            List<SessionsFunctions.SessionInfo> sessionWithMmf = new List<SessionsFunctions.SessionInfo>();

            
            currentSessionId = sessionInfos.Where(z => z.DomainName + "\\" + z.UserName == currentSessionUsername).FirstOrDefault().SessionID;

            // Try to get access to MMF in other Sessions
            foreach (SessionsFunctions.SessionInfo sessionInfo in sessionInfos)
            {
                // Session/
                string name = Functions.MMFFunctions.GetSessionMmfName(sessionInfo.SessionID);

                if (Lib.System.MMFFunctions.Exist(name))
                {
                    sessionWithMmf.Add(sessionInfo);

                    if (currentSessionId == sessionInfo.SessionID)
                    {
                        alreadyExistInCurrent = true;
                    }
                }
            }
//            alreadyExistInCurrent = sessionWithMmf.Where(z => z.SessionID == currentSessionId).Count() > 0;




            ////////////

            if (!alreadyExistInCurrent)
            {
                CreateMMF();

                if (commandLineParams.ForceMode != true)
                {
                    // Just apply 
                    if (commandLineParams.RequestedStatus == Status.sOn)
                    {
                        new RegistryFunctions().SetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, AppConsts.SRP_ON, RegistryValueKind.DWord);
                        shutDown = true;
                    }
                    else
                    if (commandLineParams.RequestedStatus == Status.sOff)
                    {
                        new RegistryFunctions().SetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, AppConsts.SRP_OFF, RegistryValueKind.DWord);
                        shutDown = true;
                    }
                }
            }
            else
            {
                if (UserFunctions.IsCurrentUserAdmin())
                {
                    // Exit & -Enable/Disable могу отправить только если есть права Админа

                    byte subCommand = 0;
                    byte param1 = 0;
                    byte param2 = 0;

                    if (commandLineParams.ExitRequest || commandLineParams.MasterInstance)
                    {
                        subCommand = AppData.SUB_COMMAND_EXIT;
                    }

                    if (subCommand != 0)
                    {
                        foreach (SessionsFunctions.SessionInfo session in sessionWithMmf)
                        {
                            // Skip _FOR_ current one
                            if (session.SessionID == currentSessionId && !alreadyExistInCurrent)
                            {
                                continue;
                            }

                            MemoryMappedFile memoryMappedFile = Lib.System.MMFFunctions.Open(Functions.MMFFunctions.GetSessionMmfName(session.SessionID));

                            Functions.MMFFunctions.Write(
                                memoryMappedFile,
                                AppConsts.MEMORY_MANAGED_FILE_LENGTH,
                                AppData.COMMAND,
                                subCommand,
                                param1,
                                param2
                            );

                            memoryMappedFile.Dispose();
                        }
                    }

                    subCommand = 0;
                    param1 = 0;
                    param2 = 0;

                    // Command to itself for change status
                    if (commandLineParams.RequestedStatus == Status.sOn)
                    {
                        subCommand = AppData.SUB_COMMAND_CHANGE;
                        param1 = 0;
                    }
                    else
                    if (commandLineParams.RequestedStatus == Status.sOff)
                    {
                        subCommand = AppData.SUB_COMMAND_CHANGE;
                        param1 = 1;
                    }

                    if (commandLineParams.ForceMode == true)
                    {
                        subCommand = AppData.SUB_COMMAND_CHANGE;
                        param2 = 1;
                    }

                    if (subCommand != 0)
                    {
                        using (MemoryMappedFile memoryMappedFile = Lib.System.MMFFunctions.Open(Functions.MMFFunctions.GetSessionMmfName(currentSessionId)))
                        {
                            Functions.MMFFunctions.Write(
                                memoryMappedFile,
                                AppConsts.MEMORY_MANAGED_FILE_LENGTH,
                                AppData.COMMAND,
                                subCommand,
                                param1,
                                param2
                            );
                        }
                    }
                }

                //

                if (!commandLineParams.MasterInstance)
                {
                    //if (!commandLineParams.ExitRequest && commandLineParams.RequestedStatus == Status.sNotAvailable)
                    //{
                    //    CoreFunctions.OneInstanceCheck(CreateMutex(AppConsts.MUTEX_ID));
                    //}

                    Application.Current.Shutdown();
                    return;
                }
                ////else
                ////{
                ////    commandLineParams.IgnoreMutex = true;
                ////}

                // Because we need to wait some time, when other instance will proceed command in MMF
                Thread.Sleep(500);
            }

            if (shutDown)
            {
                Application.Current.Shutdown();
                return;
            }

            RunMMFMonitor();

            ////mutex = CreateMutex(AppConsts.MUTEX_ID);
            ////if (!commandLineParams.IgnoreMutex)
            ////{
            ////    CoreFunctions.OneInstanceCheck(mutex);
            ////}

            base.OnStartup(e);

            InitTrayIcon();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            AppData.runWatchThread = false;
            AppData.watcher?.Wait();

            AppData.memoryMappedViewAccessor?.Dispose();
            AppData.memoryMappedFile?.Dispose();

            AppData.notifyIcon?.Dispose();

            base.OnExit(e);
        }

        private void CreateMMF()
        {
            AppData.memoryMappedFile = Lib.System.MMFFunctions.Create(AppConsts.MEMORY_MANAGED_FILE, AppConsts.MEMORY_MANAGED_FILE_LENGTH);
        }

        private void RunMMFMonitor()
        {
            Functions.MMFFunctions.RunMonitor(AppData.memoryMappedFile, AppData.memoryMappedViewAccessor, AppData.watcher);
        }

        private Mutex CreateMutex(string mutex)
        {
            return new Mutex(true, mutex);
        }

        private void InitTrayIcon()
        {
            // Create Tray icon
            //AppData.notifyIcon = new TaskbarIcon();
            AppData.notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
            if (AppData.notifyIcon == null)
            {
                throw new ArgumentNullException("Resource: NotifyIcon");
            }

            AppData.notifyIcon.TrayRightMouseUp += AppData.MenuOpen;
            AppData.notifyIcon.ToolTipText = StringsFunctions.ResourceString("resVersion");

            // Icons.
            AppData.imageSources[0] = ImageFunctions.GetIconFromResource("init");
            AppData.imageSources[1] = ImageFunctions.GetIconFromResource("enabled");
            AppData.imageSources[2] = ImageFunctions.GetIconFromResource("disabled");
            AppData.imageSources[3] = ImageFunctions.GetIconFromResource("notset");

            // Set INIT icon
            this.Dispatcher.Invoke(() =>
            {
                AppData.notifyIcon.IconSource = AppData.imageSources[0];
            });

            // Show Welcome Note
            AppData.notifyIcon.ShowBalloonTip(
                StringsFunctions.ResourceString("resWelcome"),
                StringsFunctions.ResourceString("resVersion"),
                BalloonIcon.Info
            );

            // AdHock
            new RegistryMonitorCore().LoadDataFromRegistry(false);
        }
    }
}
