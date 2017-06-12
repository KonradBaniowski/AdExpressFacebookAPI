using System;
using System.Collections;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

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
                IMongoDatabase database = client.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDbName"));
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

                //Delete
                collection.DeleteMany(Builders<BsonDocument>.Filter.Eq(whereField, whereValue));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public string AggregateTrackedLoginsDataMongoDb(string collectionName)
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            try
            {
                IMongoClient client = new MongoClient(ConfigurationManager.AppSettings.Get("mongoDbConnString"));
                IMongoDatabase database = client.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDbName"));
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

                //Delete
                var aggregate = collection.Aggregate().Group(new BsonDocument { { "_id", new BsonDocument { { "idLogin", "$idLogin" }, { "login", "$login" } } } });               
                var results = aggregate.ToList();

                ArrayList objs = new ArrayList();

                results.ForEach(p =>
                {
                    objs.Add(new
                    {
                      
                        
                        idLogin = Convert.ToInt64(p["_id"]["idLogin"]),
                        login = Convert.ToString(p["_id"]["login"], CultureInfo.InvariantCulture)

                    }
                           );

                });

                var jsonObj = JsonConvert.SerializeObject(objs);

                File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "trackedLogins.json")), jsonObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Path.Combine(projectDirectory, Path.Combine("output", "trackedLogins.json"));

        }

        public string AggregateTrackedCompaniesDataMongoDb(string collectionName)
        {
            string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            try
            {
                IMongoClient client = new MongoClient(ConfigurationManager.AppSettings.Get("mongoDbConnString"));
                IMongoDatabase database = client.GetDatabase(ConfigurationManager.AppSettings.Get("mongoDbName"));
                IMongoCollection<BsonDocument> collection = database.GetCollection<BsonDocument>(collectionName);

                //Delete
                var aggregate = collection.Aggregate().Group(new BsonDocument { { "_id", new BsonDocument { { "idCompany", "$idCompany" }, { "company", "$company" } } } });
                var results = aggregate.ToList();

                ArrayList objs = new ArrayList();

                results.ForEach(p =>
                {
                    objs.Add(new
                    {


                        idCompany = Convert.ToInt64(p["_id"]["idCompany"]),
                        company = Convert.ToString(p["_id"]["company"], CultureInfo.InvariantCulture)

                    }
                           );

                });

                var jsonObj = JsonConvert.SerializeObject(objs);

                File.WriteAllText(Path.Combine(projectDirectory, Path.Combine("output", "trackedCompanies.json")), jsonObj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Path.Combine(projectDirectory, Path.Combine("output", "trackedCompanies.json"));

        }



        //private void startMongoDbServer()
        //{
        //    try
        //    {
        //        //starting the mongod server
        //        ProcessStartInfo start = new ProcessStartInfo();
        //        start.FileName = @"C:\Program Files\MongoDB\Server\3.4\bin\mongod.exe";
        //        start.WindowStyle = ProcessWindowStyle.Hidden;
        //        start.Arguments = @"--dbpath F:\mongodb\data\db";
        //        Process mongod = Process.Start(start);

        //        //stopping the mongod server
        //        mongod.Kill();

        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //}


    }
}
