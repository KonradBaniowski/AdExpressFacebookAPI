using System;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace OracleDataToJson
{
    class NonRelationalFactory
    {
        public void ImportJsonFileToMongoDb(string jsonFile, string collectionName, bool dropCollectionBeforeImport = true)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ConfigurationManager.AppSettings.Get("mongoDbImport_Path");
                startInfo.Arguments = $" --db {ConfigurationManager.AppSettings.Get("mongoDbName")} --collection {collectionName} --type json {(dropCollectionBeforeImport ? "--drop" : "")} --file \"{jsonFile}\" --jsonArray";
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


        public void DeleteDataMongoDb(string collectionName, string whereField, Int64 whereValue)
        {
            try
            {
                IMongoClient client = new MongoClient(ConfigurationManager.AppSettings.Get("mongoDbConnString"));
                IMongoDatabase  database = client.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDbName"));
                IMongoCollection<BsonDocument> collection =  database.GetCollection<BsonDocument>(collectionName);

                //Delete
                collection.DeleteMany(Builders<BsonDocument>.Filter.Eq(whereField, whereValue));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        private void startMongoDbServer()
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


    }
}
