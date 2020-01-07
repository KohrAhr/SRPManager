using Hardcodet.Wpf.TaskbarNotification;
using Lib.Strings;
using Lib.System;
using Lib.UI;
using SRPManagerV2.Core;
using SRPManagerV2.Views;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SRPManagerV2.ViewModels
{
    /// <summary>
    ///     General (basic) commands implementation
    /// </summary>
    public partial class NotifyIconVM
    {
        /// <summary>
        ///     Shut down the application
        /// </summary>
        private void CmdExitProc(object o)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        ///     Show About window
        /// </summary>
        private void CmdAboutProc(object o)
        {
            WindowsUI.ProceedWindow<AboutWindow>(true);
        }

        /// <summary>
        ///     Run application as Admin
        /// </summary>
        /// <param name="o"></param>
        private void CmdRunAsProc(object o)
        {
            Process process = null;

            process = new ProcessesFunctions().RunAs(Assembly.GetEntryAssembly().CodeBase, "IgnoreMutex{B2A3B94A-26BF-4CC6-AC2B-7DDA34EFD413}");

            if (process != null)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        ///     Run GpUpdate.EXE and wait in new Thread when process will be completed
        /// </summary>
        /// <param name="o"></param>
        private void CmdGPupdateProc(object o)
        {
            GPUpdate = new ProcessesFunctions().Run(AppConsts.CMD_GPUPDATE, AppConsts.CMD_GPUPDATE_PARAMS);

            if (GPUpdate != null)
            {
                new Task(() =>
                {
                    AppData.notifyIcon.CloseBalloon();
                    AppData.notifyIcon.ShowBalloonTip(StringsFunctions.ResourceString("resGPUPdateStart"), StringsFunctions.ResourceString("resVersion"), BalloonIcon.Info);
                }
                ).Start();

                new Task(() =>
                {
                    GPUpdate.WaitForExit();
                    AppData.notifyIcon.CloseBalloon();
                    AppData.notifyIcon.ShowBalloonTip(StringsFunctions.ResourceString("resGPUPdateDone"), StringsFunctions.ResourceString("resVersion"), BalloonIcon.Info);
                    GPUpdate = null;
                }
                ).Start();
            }
            else
            {
                new Task(() =>
                {
                    AppData.notifyIcon.CloseBalloon();
                    AppData.notifyIcon.ShowBalloonTip(StringsFunctions.ResourceString("resGPUPdateFail"), StringsFunctions.ResourceString("resVersion"), BalloonIcon.Error);
                }
                ).Start();
            }
        }

        private bool CmdGPupdateProcEnabled(object o)
        {
            return GPUpdate == null;
        }

        /// <summary>
        ///     Show SRP File Log
        /// </summary>
        /// <param name="o"></param>
        private void CmdShowLogProc(object o)
        {
            Process.Start(AppData.SrpFileName);
        }

        private void CmdShowAwlProc(object o)
        {
            WindowsUI.ProceedWindow<AwlRulesWindow>(true);
        }
    }
}
