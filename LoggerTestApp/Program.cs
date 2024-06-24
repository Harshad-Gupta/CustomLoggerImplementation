using LoggerAssembly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerTestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Console App Running...");

            double value = TimeSpan.FromSeconds(1).TotalMilliseconds;

            var _logger = new CustomLogger(ErrorMode: true, DebugMode: true, InfoMode: true, CustomMode: true, ApplicationName: "LoggerTestApp");
            _logger.Initialize();
            _logger.InitializeCustom(CustomFilename: "CustomLog");

            //Server server = new Server(_logger);

            _logger.Debug("---------------- Debug Log ----------------");

            for (int i = 0; i < 10; i++)
            {
                _logger.Debug($"{i}");
            }

            //Thread.Sleep(1);
            _logger.Info("---------------- Info Log ----------------");
            //Thread.Sleep(1);
            _logger.Error(null, "---------------- Error Log ----------------");
            //Thread.Sleep(1);
            _logger.Custom("---------------- Custom Log ----------------");

            Console.WriteLine("Console App Closing...");
            Console.Read();
        }
    }
}
