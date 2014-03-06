using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
namespace TNS.AdExpress.PigeLuxottica.Rules
{
    public class PigeLuxotticaConfiguration
    {


        protected string _idAdvertisers = "";

        protected string _idProducts = "";

        protected Int64 _idVehicle = -1;

        protected int _dataLanguage = -1;

        protected string _beginningDate = "";

        protected string _endDate = "";

        protected string _password = "";

        protected string _login = "";

        protected IDataSource _source = null;

        protected string _connectionString = "";

        public PigeLuxotticaConfiguration()
        {
            //TODO : A mettre dans fichier XML
            _login = "gfacon";
            _password = "sandie5";
            _dataLanguage = 33;
            _idAdvertisers = "10919,6761,49811";
            _idProducts = "170768,31695,284565,64488,58050,183809,69324,423781,110189,41781,45930,463887,58960,166239";
            _idVehicle = 1;
            _beginningDate = "20130101";
            _endDate = "20131231";
            _connectionString = "User Id=gfacon; Password=sandie5; Data Source=adexpr03.pige;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120";
            _source = new OracleDataSource(_connectionString);
            ExcelFileName = "PigeLuxottica.xls";
            DestinationDirectory = @"F:\";
        }
        public PigeLuxotticaConfiguration(string login, string password, int dataLanguage, string idAdvertisers, string idProducts, Int64 idVehicle, string beginningDate, string endDate)
        {
            _login = login;
            _password = password;
            _dataLanguage = dataLanguage;
            _idAdvertisers = idAdvertisers;
            _idProducts = idProducts;
            _idVehicle = idVehicle;
            _beginningDate = beginningDate;
            _endDate = endDate;
            //TODO : créer un USER dans ISIS
            _connectionString = "User Id=gfacon; Password=sandie5; Data Source=adexpr03.pige;Pooling=true; Max Pool Size=150; Decr Pool Size=20; Connection Timeout=120"; 
            _source = new OracleDataSource(_connectionString);
            ExcelFileName = "PigeLuxottica.xls";
            DestinationDirectory = "DestinationDirectory";
        }


        public string IdAdvertisers
        {
            get { return _idAdvertisers; }
            set { _idAdvertisers = value; }
        }
        public string IdProducts
        {
            get { return _idProducts; }
            set { _idProducts = value; }
        }
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }
        public string BeginningDate
        {
            get { return _beginningDate; }
            set { _beginningDate = value; }
        }
        public string EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        public string Login
        {
            get { return _login; }
            set { _login = value; }
        }
        public Int64 IdVehicle
        {
            get { return _idVehicle; }
            set { IdVehicle = value; }
        }
        public int DataLanguage
        {
            get { return _dataLanguage; }
            set { _dataLanguage = value; }
        }

        public IDataSource Source{
            get { return _source; }
        }

        public string DestinationDirectory { get; set; }

        public string ExcelFileName { get; set; }
    }
}
