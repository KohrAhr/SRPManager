using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SRPManagerV2.Core
{
    public class FileMonitorCore
    {
        public delegate void OnMatch();
        public event OnMatch OnMatchEvent = null;

        /// <summary>
        ///     SRPv2 log file. Last position/offset in file we proceed.
        /// </summary>
        private long LastPosition = 0;

        FileSystemWatcher srpFileWatcher = null;

        public FileMonitorCore(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                return;
            }

            srpFileWatcher = new FileSystemWatcher();
            srpFileWatcher.Path = Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar;
            srpFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
            srpFileWatcher.Filter = Path.GetFileName(fileName);

            srpFileWatcher.Changed += new FileSystemEventHandler(OnSrpRecordAdded);
            srpFileWatcher.Created += new FileSystemEventHandler(OnSrpFileCreated);
            srpFileWatcher.Deleted += new FileSystemEventHandler(OnSrpFileDeleted);
            srpFileWatcher.Renamed += new RenamedEventHandler(OnSrpFileRenamed);

            srpFileWatcher.EnableRaisingEvents = true;

            LastPosition = new System.IO.FileInfo(fileName).Length;
        }

        ~FileMonitorCore()
        {
            if (srpFileWatcher != null)
            {
                srpFileWatcher.EnableRaisingEvents = false;
            }
        }

        public void OnSrpFileCreated(object source, FileSystemEventArgs e)
        {
            LastPosition = 0;
            AppData._nLog.Debug("<<<<< SRPv2 file created >>>>>");
        }

        public void OnSrpFileDeleted(object source, FileSystemEventArgs e)
        {
            LastPosition = 0;
            AppData._nLog.Debug("<<<<< SRPv2 file deleted >>>>>");
        }

        public void OnSrpFileRenamed(object source, FileSystemEventArgs e)
        {
            LastPosition = 0;
            AppData._nLog.Debug("<<<<< SRPv2 file renamed >>>>>");
        }

        public void OnSrpRecordAdded(object source, FileSystemEventArgs e)
        {
            OnMatchEvent?.Invoke();

            const int CONST_ATTEMPTS = 16;

            // read last line
            String last = "";

            bool isLocked = true;
            int i = 0;

            while (isLocked && i < CONST_ATTEMPTS)
            {
                try
                {
                    using (FileStream srpFileStream = File.Open(AppData.SrpFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        srpFileStream.Position = LastPosition;
                        LastPosition = srpFileStream.Length;

                        using (StreamReader srpStreamReader = new StreamReader(srpFileStream, Encoding.Unicode))
                        {
                            last = srpStreamReader.ReadToEnd();

                            if (last.EndsWith(Environment.NewLine))
                            {
                                last = last.Remove(last.Length - AppData.NewLineLength, AppData.NewLineLength);
                            }

                            foreach (string line in last.Split(Environment.NewLine.ToCharArray()))
                            {
                                if (!String.IsNullOrEmpty(line))
                                {
                                    AppData._nLog.Debug(line);
                                }
                            }
                        }

                        break;
                    }
                }
                catch (IOException ex)
                {
                    int errorCode = Marshal.GetHRForException(ex) & ((1 << 16) - 1);
                    isLocked = errorCode == 32 || errorCode == 33;
                    i++;

                    if (!isLocked)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
    }
}
