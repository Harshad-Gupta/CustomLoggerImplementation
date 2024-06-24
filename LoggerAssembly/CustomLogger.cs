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
        bool DebugMode = false;
        bool InfoMode = false;
        bool ErrorMode = false;
        bool CustomMode = false;

        bool TrimmedFormat = false;

        string ApplicationName = string.Empty;

        StringBuilder sb_ErrorMessage = new StringBuilder();
        StringBuilder sb_DebugMessage = new StringBuilder();
        StringBuilder sb_InfoMessage = new StringBuilder();
        StringBuilder sb_CustomMessage = new StringBuilder();

        string ErrorFilePath = string.Empty;
        string DebugFilePath = string.Empty;
        string InfoFilePath = string.Empty;
        string CustomFilePath = string.Empty;

        Timer log_Timer = null;

        public CustomLogger(bool ErrorMode = false, bool DebugMode = false, bool CustomMode = false, bool InfoMode = false, string ApplicationName = "", bool TrimmedFormat = false)
        {
            this.ErrorMode = ErrorMode;
            this.DebugMode = DebugMode;
            this.InfoMode = InfoMode;
            this.CustomMode = CustomMode;
            this.ApplicationName = ApplicationName;
            this.TrimmedFormat = TrimmedFormat;
        }

        public bool[] Initialize(string path = "auto", string ExceptionFilename = "ExceptionLOG", string DebugFilename = "DebugLOG", string InfoFilename = "InfoLOG")
        {
            bool[] fileExists = new bool[3] { false, false, false };
            try
            {
                if (ErrorMode)
                {
                    ExceptionFilename = CreateRequiredFiles(path, ExceptionFilename);
                    ErrorFilePath = ExceptionFilename;
                    fileExists[0] = CreateLogFile(ExceptionFilename);
                }

                if (DebugMode)
                {
                    DebugFilename = CreateRequiredFiles(path, DebugFilename);
                    DebugFilePath = DebugFilename;
                    fileExists[1] = CreateLogFile(DebugFilename);
                }

                if (InfoMode)
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

        public bool InitializeCustom(string path = "auto", string CustomFilename = "CustomLOG")
        {
            bool fileExists = false;
            try
            {
                if (CustomMode)
                {
                    CustomFilename = CreateRequiredFiles(path, CustomFilename);
                    CustomFilePath = CustomFilename;
                    fileExists = CreateLogFile(CustomFilename);
                }
            }
            catch (Exception ee) { Console.WriteLine(DateTime.Now + " => InitializeCustom | " + ee.Message); }

            return fileExists;
        }

        string CreateRequiredFiles(string path, string Filename)
        {
            try
            {
                string AssemblyPath = Assembly.GetExecutingAssembly().Location;

                if (path == "auto")
                    path = Path.GetDirectoryName(AssemblyPath);
                    //path = AssemblyPath.Substring(0, AssemblyPath.Length - 12);

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
            if (DebugMode)
            {
                lock (sb_DebugMessage)
                {
                    if (TrimmedFormat)
                        sb_DebugMessage.AppendLine($"{DateTime.Now.ToString("ddMMMyyyy HH:mm:ss:fff").ToUpper()} > {Message}");
                    else
                        sb_DebugMessage.AppendLine($"[DEBUG] {DateTime.Now} => {Message}");
                }
            }
        }

        public void Info(string Message = "")
        {
            if (InfoMode)
            {
                lock (sb_InfoMessage)
                {
                    if (TrimmedFormat)
                        sb_InfoMessage.AppendLine($"{DateTime.Now.ToString("ddMMMyyyy HH:mm:ss:fff").ToUpper()} > {Message}");
                    else
                        sb_InfoMessage.AppendLine($"[INFO] {DateTime.Now} => {Message}");
                }
            }
        }

        public void Error(Exception obj, [CallerMemberName] string ErrorMessage = "")
        {
            if (ErrorMode)
            {
                lock (sb_ErrorMessage)
                {
                    if (TrimmedFormat)
                        sb_ErrorMessage.AppendLine($"{DateTime.Now.ToString("ddMMMyyyy HH:mm:ss:fff").ToUpper()} > {ErrorMessage} {Environment.NewLine} {((obj is null) ? string.Empty : obj.ToString())} {Environment.NewLine}");
                    else
                        sb_ErrorMessage.AppendLine($"[EXCEPTION] => {ErrorMessage} at {DateTime.Now}\n{((obj is null) ? string.Empty : obj.ToString())}\n");
                }
            }
        }
        public void Custom(string Message)
        {
            if (CustomMode)
            {
                lock (sb_CustomMessage)
                {
                    sb_CustomMessage.AppendLine(Message);
                }
            }
        }

        void WriteToLogs(object sender, ElapsedEventArgs e)
        {
            if (ErrorMode)
            {
                lock (sb_ErrorMessage)
                {
                    if ((sb_ErrorMessage.Length > 0) && (ErrorFilePath != string.Empty))
                    {
                        File.AppendAllText(ErrorFilePath, sb_ErrorMessage.ToString());
                        sb_ErrorMessage.Clear();
                    }
                }
            }

            if (DebugMode)
            {
                lock (sb_DebugMessage)
                {
                    if ((sb_DebugMessage.Length > 0) && (DebugFilePath != string.Empty))
                    {
                        File.AppendAllText(DebugFilePath, sb_DebugMessage.ToString());
                        sb_DebugMessage.Clear();
                    }
                }
            }

            if (InfoMode)
            {
                lock (sb_InfoMessage)
                {
                    if ((sb_InfoMessage.Length > 0) && (InfoFilePath != string.Empty))
                    {
                        File.AppendAllText(InfoFilePath, sb_InfoMessage.ToString());
                        sb_InfoMessage.Clear();
                    }
                }
            }

            if (CustomMode)
            {
                lock (sb_CustomMessage)
                {
                    if ((sb_CustomMessage.Length > 0) && (CustomFilePath != string.Empty))
                    {
                        File.AppendAllText(CustomFilePath, sb_CustomMessage.ToString());
                        sb_CustomMessage.Clear();
                    }
                }
            }
        }
    }
}
