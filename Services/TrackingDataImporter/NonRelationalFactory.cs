using System;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;

namespace OracleDataToJson
{
    class NonRelationalFactory
    {

        public void startMongoDbServer()
        {
            try
            {
                //starting the mongod server
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = @"C:\Program Files\MongoDB\Server\3.4\bin\mongod.exe";
                start.WindowStyle = ProcessWindowStyle.Hidden;
                start.Arguments = @"--dbpath F:\mongodb\data\db";
                Process mongod = Process.Start(start);

                //stopping the mongod server
                mongod.Kill();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        public void ImportJsonFileToMongoDb(string jsonFile, string collectionName)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ConfigurationManager.AppSettings.Get("mongoDbImport_Path");
                startInfo.Arguments = @" --db " + ConfigurationManager.AppSettings.Get("mongoDbName") + " --collection " + collectionName + " --type json --drop --file \"" + jsonFile + "\" --jsonArray";
                Process proc = new Process();
                proc.StartInfo = startInfo;
                startInfo.UseShellExecute = false;
                proc.Start();
                proc.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
    }
}
