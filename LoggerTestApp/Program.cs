using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoggerAssembly;

namespace LoggerTestApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var _logger = new CustomLogger(ErrorMode: true, DebugMode: true, InfoMode: true, CustomMode: true, ApplicationName: "LoggerTestApp");
            _logger.Initialize();

            try
            {
                Console.WriteLine("Console App Running...");

                _logger.Debug("---------------- Debug Log ----------------");
                //Thread.Sleep(1);
                _logger.Info("---------------- Info Log ----------------");
                //Thread.Sleep(1);

                for (int i = 0; i < 10; i++)
                {
                    _logger.Debug($"{i}");

                    if(i == 9)
                    {
                        i = i / 0;
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            Console.WriteLine("Console App Closing...");
            Console.Read();
        }
    }
}
