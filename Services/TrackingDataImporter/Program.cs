using System;
using System.Collections.Generic;

namespace OracleDataToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = string.Empty;

            RelationalFactory relationalFactory = new RelationalFactory();
            NonRelationalFactory nonRelationalFactory = new NonRelationalFactory();

            Dictionary<string, string> validParams = new Dictionary<string, string>();
            foreach (string arg in args)
            {
                if(arg.Contains("="))
                    validParams.Add(arg.Split('=')[0], arg.Split('=')[1]);
            }

            try
            {
                ////DayConnection
                //filePath = relationalFactory.ExportDayDataToJsonFile();
                //nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "dayconnections");

                ////HourConnection
                //filePath = relationalFactory.ExportHourDataToJsonFile();
                //nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "hourconnections");

                ////MediaConnection
                //filePath = relationalFactory.ExportMediaDataToJsonFile();
                //nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "mediaconnections");

                ////TypologyConnection
                //filePath = relationalFactory.ExportTypologyDataToJsonFile();
                //nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "typologyconnections");

                ////ModuleConnection
                //filePath = relationalFactory.ExportModuleDataToJsonFile();
                //nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "moduleconnections");


                if (validParams.ContainsKey("--load-archive") && validParams["--load-archive"] == "true")
                {
                    //UserSession (archive)
                    filePath = relationalFactory.ExportUserSessionDataToJsonFile();
                    nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "usersessions");
                }

                if (validParams.ContainsKey("--load-logins") && validParams["--load-logins"] == "true")
                {
                    //logins
                    filePath = relationalFactory.ExportLoginsDataToJsonFile();
                    nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "logins");
                }

                //UserSession current day
                filePath = relationalFactory.ExportUserSessionDayDataToJsonFile();
                nonRelationalFactory.DeleteDataMongoDb("usersessions", "day", Convert.ToInt64(DateTime.Now.ToString("yyyyMMdd")));
                nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "usersessions", false);


            }
            catch(Exception e)
            {
                return;
            }
            //test.ExportHour();
        }
    }
}
