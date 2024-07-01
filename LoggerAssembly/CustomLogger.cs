using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;

namespace LoggerAssembly
{
    public class CustomLogger
    {
        string ApplicationName = string.Empty;

        bool DebugFile = false;
        bool InfoFile = false;
        bool ErrorFile = false;
        bool CustomFile = false;

        StringBuilder ErrorMessageString = new StringBuilder();
        StringBuilder DebugMessageString = new StringBuilder();
        StringBuilder InfoMessageString = new StringBuilder();

        string ErrorFilePath = string.Empty;
        string DebugFilePath = string.Empty;
        string InfoFilePath = string.Empty;

        Timer log_Timer = null;

        public CustomLogger(bool ErrorMode = false, bool DebugMode = false, bool CustomMode = false, bool InfoMode = false, string ApplicationName = "")
        {
            this.ErrorFile = ErrorMode;
            this.DebugFile = DebugMode;
            this.InfoFile = InfoMode;
            this.ApplicationName = ApplicationName;
        }

        public bool[] Initialize(string path = "auto", string ExceptionFilename = "ExceptionLOG", string DebugFilename = "DebugLOG", string InfoFilename = "InfoLOG")
        {
            bool[] fileExists = new bool[3] { false, false, false };
            try
            {
                if (ErrorFile)
                {
                    ExceptionFilename = CreateRequiredFiles(path, ExceptionFilename);
                    ErrorFilePath = ExceptionFilename;
                    fileExists[0] = CreateLogFile(ExceptionFilename);
                }

                if (DebugFile)
                {
                    DebugFilename = CreateRequiredFiles(path, DebugFilename);
                    DebugFilePath = DebugFilename;
                    fileExists[1] = CreateLogFile(DebugFilename);
                }

                if (InfoFile)
                {
                    InfoFilename = CreateRequiredFiles(path, InfoFilename);
                    InfoFilePath = InfoFilename;
                    fileExists[2] = CreateLogFile(InfoFilename);
                }

                if (log_Timer is null)
                {
                    log_Timer = new Timer(TimeSpan.FromSeconds(1).TotalMilliseconds);
                    log_Timer.AutoReset = true;
                    log_Timer.Elapsed += new System.Timers.ElapsedEventHandler(WriteToLogs);
                    log_Timer.Start();
                }
            }
            catch (Exception ee) { Console.WriteLine(DateTime.Now + " => Initialize | " + ee.Message); }

            return fileExists;
        }

        string CreateRequiredFiles(string path, string Filename)
        {
            try
            {
                string AssemblyPath = Assembly.GetExecutingAssembly().Location;

                if (path == "auto")
                    path = Path.GetDirectoryName(AssemblyPath);

                if (!path.EndsWith("\\"))
                    path += "\\";

                string DirectoryName = string.IsNullOrEmpty(ApplicationName) ? (path + "LOG") : (path + ApplicationName + "-LOG");
                string DateFolderName = DateTime.Now.ToString("ddMMMyyyy").ToUpper();

                CreateDirectoryForLogs(DirectoryName);
                CreateDirectoryForLogs(DirectoryName + @"\" + DateFolderName);

                Filename = DirectoryName + @"\" + DateFolderName + @"\" + Filename + ".txt";
            }
            catch (Exception ee) { Console.WriteLine(DateTime.Now + " => CreateRequiredFiles | " + ee.Message); }

            return Filename;
        }

        void CreateDirectoryForLogs(string DirectoryPath)
        {
            if (!Directory.Exists(DirectoryPath))
                Directory.CreateDirectory(DirectoryPath);
        }

        bool CreateLogFile(string filename)
        {
            if (!File.Exists(filename))
            {
                using (StreamWriter w = File.CreateText(filename))
                    w.Close();
                return false;
            }
            else
                return true;
        }

        public void Debug(string Message)
        {
            if (DebugFile)
            {
                lock (DebugMessageString)
                {
                    DebugMessageString.AppendLine($"[DEBUG] {DateTime.Now} => {Message}");
                }
            }
        }

        public void Info(string Message = "")
        {
            if (InfoFile)
            {
                lock (InfoMessageString)
                {
                    InfoMessageString.AppendLine($"[INFO] {DateTime.Now} => {Message}");
                }
            }
        }

        public void Error(Exception obj, [CallerMemberName] string ErrorMessage = "")
        {
            if (ErrorFile)
            {
                lock (ErrorMessageString)
                {
                    ErrorMessageString.AppendLine($"[EXCEPTION] => {ErrorMessage} at {DateTime.Now}\n{((obj is null) ? string.Empty : obj.ToString())}\n");
                }
            }
        }

        void WriteToLogs(object sender, ElapsedEventArgs e)
        {
            if (ErrorFile)
            {
                lock (ErrorMessageString)
                {
                    if ((ErrorMessageString.Length > 0) && (ErrorFilePath != string.Empty))
                    {
                        File.AppendAllText(ErrorFilePath, ErrorMessageString.ToString());
                        ErrorMessageString.Clear();
                    }
                }
            }

            if (DebugFile)
            {
                lock (DebugMessageString)
                {
                    if ((DebugMessageString.Length > 0) && (DebugFilePath != string.Empty))
                    {
                        File.AppendAllText(DebugFilePath, DebugMessageString.ToString());
                        DebugMessageString.Clear();
                    }
                }
            }

            if (InfoFile)
            {
                lock (InfoMessageString)
                {
                    if ((InfoMessageString.Length > 0) && (InfoFilePath != string.Empty))
                    {
                        File.AppendAllText(InfoFilePath, InfoMessageString.ToString());
                        InfoMessageString.Clear();
                    }
                }
            }
        }
    }
}
