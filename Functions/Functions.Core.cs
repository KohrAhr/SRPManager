using Hardcodet.Wpf.TaskbarNotification;
using Lib.Strings;
using Lib.System;
using Microsoft.Win32;
using SRPManagerV2.Core;
using SRPManagerV2.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace SRPManagerV2.Functions
{
    public static class CoreFunctions
    {
        /// <summary>
        ///     Установить параметры командной строки
        /// </summary>
        /// <param name="commandLineParams"></param>
        public static bool ApplyCommandLineParameters(CommandLineParamsType commandLineParams)
        {
            // Специальный режим. Показать Help в консоле и выйти
            if (commandLineParams.ShowHelp)
            {

                ConsoleManager.Show();

                Console.WriteLine("\n\n======================================");
                Console.WriteLine(StringsFunctions.ResourceString("resVersion"));
                Console.WriteLine("======================================\n");
                Console.WriteLine("Usage: SrpManager.exe [[[-master] | [-exit]] [[-enable] | [-disable]] [[-run_force] | [-stop_force]]] | [-?]");
                Console.WriteLine("\nOptions:");
                Console.WriteLine("\t-master    \tclose all other instance of application, and run new instance");
                Console.WriteLine("\t-exit      \tclose all other instance of application");
                Console.WriteLine("\t-enable    \tswitch SRP/AppLocker to the \"Whitelisting\" mode");
                Console.WriteLine("\t-disable   \tswitch SRP/AppLocker to the \"Blacklisting\" mode");
                Console.WriteLine("\t-force \tenable keeping of the selected mode while running");
                Console.WriteLine("\t-?         \tshow this help");
                ConsoleManager.FreeConsole();

                Application.Current.Shutdown();
                return false;
            }

            if (commandLineParams.RequestedStatus != Status.sNotAvailable)
            {
                AppData.MustSwitch = true;
                AppData.SwitchToStatus = commandLineParams.RequestedStatus;
            }

            if (commandLineParams.ForceMode != null)
            {
                AppData.EnforceWhileRun = (bool)commandLineParams.ForceMode;
            }

//            AppData.SilentMode = commandLineParams.SilentMode;

            return true;
        }

        /// <summary>
        ///     Обработать параметры командной строки
        /// </summary>
        /// <param name="e">
        ///     Параметры командной строки
        /// </param>
        /// <returns>
        ///     Извлечённые значения
        /// </returns>
        public static CommandLineParamsType RecognizeCommandLineParams(StartupEventArgs e)
        {
            CommandLineParamsType result = new CommandLineParamsType();

            // proceed command line parameters
            foreach (string param in e.Args)
            {
                if (param.Contains("-?"))
                {
                    result.ShowHelp = true;
                }
                if (param.Contains("-master"))
                {
                    result.MasterInstance = true;
                }
                else
                if (param.Contains("-enable"))
                {
                    result.RequestedStatus = Status.sOff;
                }
                else
                if (param.Contains("-disable"))
                {
                    result.RequestedStatus = Status.sOn;
                }
                else
                if (param.Contains("-force"))
                {
                    result.ForceMode = true;
                }
                else
                //if (param.Contains("-stop_force"))
                //{
                //    result.ForceMode = false;
                //}
                //else
                //if (param.Contains("-silent"))
                //{
                //    result.SilentMode = true;
                //}
                //else
                if (param.Contains("-exit"))
                {
                    result.ExitRequest = true;
                }
                else
                if (param.Contains("-IgnoreMutex"/* + AppConsts.MUTEX_ID*/))
                {
                    result.IgnoreMutex = true;
                }
            }

            return result;
        }

        public static string GetSrpLogFile()
        {
            return new RegistryFunctions().GetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_LOG, RegistryValueKind.String);
        }

        public static bool HasRightToWriteInRegistry(RegistryKey registry = null)
        {
            using (WindowsIdentity.GetCurrent().Impersonate())
            {
                bool result = new RegistryFunctions().CanSetRegKeyValue(AppConsts.KEY_SRP_NODE, AppConsts.KEY_SRP_DEFAULT_LEVEL, registry);
                return result;
            }
        }

        /// <summary>
        ///     Проверка (по mutex-у), что запущена только одна копия программы
        ///     <para>Verify (by using mutex), that running only one instance of application</para>
        ///     <para>Terminate application if such mutex already exists</para>
        /// </summary>
        /// <param name="mutex"></param>
        public static void OneInstanceCheck(Mutex mutex)
        {
            try
            {
                if (!mutex.WaitOne(TimeSpan.Zero, true))
                {
                    // Only one instance can run
                    MoreThanOneInstanceIsRunning();

                    Application.Current.Shutdown();
                    return;
                }
            }
            catch (AbandonedMutexException)
            {
                // Когда программа перезапускается: User -> Admin mode. Считаем этот случай нормальным.
            }
        }

        /// <summary>
        ///     Показать информацию о том что другая копия программы уже запущена
        /// </summary>
        public static void MoreThanOneInstanceIsRunning()
        {
            //if (!AppData.SilentMode)
            //{
            //    MessageBox.Show(
            //        StringsFunctions.ResourceString("resOnlyOneInstanceIsAllowed"),
            //        StringsFunctions.ResourceString("resVersion"),
            //        MessageBoxButton.OK,
            //        MessageBoxImage.Stop
            //    );
            //}
        }
    }
}
