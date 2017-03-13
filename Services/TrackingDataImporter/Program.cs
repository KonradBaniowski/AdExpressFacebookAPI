using Newtonsoft.Json;
using Oracle.DataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleDataToJson
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = string.Empty;

            RelationalFactory relationalFactory = new RelationalFactory();
            NonRelationalFactory nonRelationalFactory = new NonRelationalFactory();

            try
            {
                //DayConnection
                filePath = relationalFactory.ExportDayDataToJsonFile();
                nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "dayconnections");

                //HourConnection
                filePath = relationalFactory.ExportHourDataToJsonFile();
                nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "hourconnections");

                //UserSession
                filePath = relationalFactory.ExportUserSessionDataToJsonFile();
                nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "usersessions");

                //MediaConnection
                filePath = relationalFactory.ExportMediaDataToJsonFile();
                nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "mediaconnections");

                //TypologyConnection
                filePath = relationalFactory.ExportTypologyDataToJsonFile();
                nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "typologyconnections");

                //ModuleConnection
                filePath = relationalFactory.ExportModuleDataToJsonFile();
                nonRelationalFactory.ImportJsonFileToMongoDb(filePath, "moduleconnections");
            }
            catch(Exception e)
            {
                return;
            }
            //test.ExportHour();
        }
    }
}
