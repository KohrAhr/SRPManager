using SRPManagerV2.Core;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SRPManagerV2.Functions
{
    public static class MMFFunctions
    {
        public delegate void OnRequest();
        public static event OnRequest OnRequestEvent = null;

        public static string GetSessionMmfName(int id)
        {
            return "Session\\" + id.ToString() + "\\" + AppConsts.MEMORY_MANAGED_FILE;
        }

        public static void Write(MemoryMappedFile memoryMappedFile, int lenght, Int16 command, byte subCommand, byte param1, byte param2)
        {
            using (MemoryMappedViewAccessor accessor = memoryMappedFile.CreateViewAccessor(0, lenght))
            {
                // Command available
                accessor.Write(0, command);
                // Sub-Command available
                accessor.Write(2, subCommand);
                // Disabled or Enabled
                accessor.Write(3, param1);
                // Force mode on or off
                accessor.Write(4, param2);    
            }
        }

        /// <summary>
        ///     Запустить мониторинг изменений в MemoryMappedFile
        /// </summary>
        /// <param name="memoryMappedFile"></param>
        /// <param name="memoryMappedViewAccessor"></param>
        /// <param name="task"></param>
        public static void RunMonitor(MemoryMappedFile memoryMappedFile, MemoryMappedViewAccessor memoryMappedViewAccessor, Task task)
        {
            task = new Task(() =>
            {
                memoryMappedViewAccessor = memoryMappedFile.CreateViewAccessor(0, 8);

                while (AppData.runWatchThread)
                {
                    memoryMappedViewAccessor.Read(0, out Int16 commandIndicator);
                    if (commandIndicator == AppData.COMMAND)
                    {
                        memoryMappedViewAccessor.Read(2, out byte s0);

                        // Disabled or Enabled
                        memoryMappedViewAccessor.Read(3, out byte s1);
                        // Force mode on or off
                        memoryMappedViewAccessor.Read(4, out byte s2);    

                        // This is how we reset command
                        memoryMappedViewAccessor.Write(0, (Int32)0);

                        if (s0 == AppData.SUB_COMMAND_EXIT)
                        {
                            App.Current.Dispatcher.Invoke(() => { App.Current.Shutdown(); });
                            break;
                        }
                        else
                        if (s0 == AppData.SUB_COMMAND_CHANGE)
                        {
                            if (s1 == 0)
                            {
                                AppData.SwitchToStatus = Status.sOn;
                            }
                            else
                            if (s1 == 1)
                            {
                                AppData.SwitchToStatus = Status.sOff;
                            }

                            AppData.EnforceWhileRun = s2 == 1;

                            AppData.MustSwitch = true;
                        }

                        OnRequestEvent?.Invoke();
                    }
                    Thread.Sleep(100);
                }
            });

            task.Start();
        }
    }
}
