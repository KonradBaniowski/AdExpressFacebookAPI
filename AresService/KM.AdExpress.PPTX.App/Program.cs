using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KM.AdExpress.PPTX.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var isConsoleMode = false;
            var isConsoleAuto = false;


            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            var argsOnStart = new string[0];

            if (args.Length > 0)
            {
                if (args[0].ToLower() == "/console")
                {
                    isConsoleMode = true;
                    if (args.Length > 1)
                    {
                        if (args[1].ToLower() == "/auto")
                        {
                            isConsoleAuto = true;
                        }
                        else
                        {
                            argsOnStart = new string[args.Length - 1];
                            for (var i = 0; i < args.Length - 1; i++)
                            {
                                argsOnStart[i] = args[i + 1];
                            }
                        }
                    }
                }
            }

            #region Mode Console

            var servicesToRun = new AdExpressPPTXService();
            if (isConsoleMode) //Console 
            {

                if (!isConsoleAuto) servicesToRun.StartManual(argsOnStart);
                else servicesToRun.StartService();

                Console.WriteLine("AdExpress Powerpoint Convert Service launch");
                Console.WriteLine("Press [ENTER] to quit...");
                Console.ReadLine();

                servicesToRun.StopManual();
            }
            #endregion

            #region Mode Windows Service

            else //Windows Service 
            {
                ServiceBase.Run(servicesToRun);
            }

            #endregion
        }

        static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            EventLog.WriteEntry("KM.AdExpress.PPTX.App", "KM.AdExpress.PPTX.App" + e.ExceptionObject, EventLogEntryType.Error);
        }
    }
}
