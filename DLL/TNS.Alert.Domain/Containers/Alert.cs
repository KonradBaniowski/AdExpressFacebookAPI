using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Exceptions;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using TNS.AdExpress.Domain.Translation;

namespace TNS.Alert.Domain
{
    public class Alert
    {
        #region Variables

        private DataRow _row;

        #endregion

        #region Properties

        public string Title
        {
            get { return (this._row["alert"].ToString()); }
        }

        public int AlertId
        {
            get { return (int.Parse(this._row["id_alert"].ToString())); }
        }

        public int CustomerId
        {
            get { return (int.Parse(this._row["id_login"].ToString())); }
        }

        public string Recipients
        {
            get { return (this._row["email_list"].ToString()); }
        }

        public DateTime CreationDate
        {
            get { return (DateTime.Parse(this._row["date_creation"].ToString())); }
        }

        public DateTime ExpirationDate
        {
            get { return (DateTime.Parse(this._row["date_end"].ToString())); }
        }

        public DateTime ValidationDate
        {
            get { return (DateTime.Parse(this._row["date_validation"].ToString())); }
        }

        public DateTime NextAlertDate
        {
            get { return (DateTime.Parse(this._row["date_next_alert"].ToString())); }
        }

        public object Session
        {
            get
            {
                byte[] session = (byte[])this._row["session_"];
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                ms.Write(session, 0, session.Length);
                ms.Position = 0;
                object o = bf.Deserialize(ms);
                return (o);
            }
        }

        public Alerts.AlertStatuses Status
        {
            get { return ((Alerts.AlertStatuses)Enum.Parse(
                            typeof(Alerts.AlertStatuses),
                            this._row["activation"].ToString())); }
        }

        public Alerts.AlertPeriodicity Periodicity
        {
            get { return ((Alerts.AlertPeriodicity)Enum.Parse(
                            typeof(Alerts.AlertPeriodicity),
                            this._row["id_type_periodicity"].ToString()));
            }
        }

        public int PeriodicityValue
        {
            get { return (int.Parse(this._row["expiry"].ToString())); }
        }

        #endregion

        #region Methods

        public string ToHtml(int lang, long idWebText, string dayName)
        {
            StringBuilder html = new StringBuilder(5000);            
            html.Append(GestionWeb.GetWebWord(1293, lang) + " : ");

            switch (this.Periodicity)
            {
                case TNS.AdExpress.Constantes.DB.Alerts.AlertPeriodicity.Daily:
                    html.Append(GestionWeb.GetWebWord(2579, lang) + "<br />");
                    break;
                case TNS.AdExpress.Constantes.DB.Alerts.AlertPeriodicity.Weekly:
                    html.Append(GestionWeb.GetWebWord(2580, lang));
                    html.Append("<br />");
                    html.Append(GestionWeb.GetWebWord(2603, lang) + " : ");
                    html.Append(GestionWeb.GetWebWord(2604, lang) + " ");
                    html.Append(dayName);
                    html.Append("<br />");
                    break;
                case TNS.AdExpress.Constantes.DB.Alerts.AlertPeriodicity.Monthly:
                    html.Append(GestionWeb.GetWebWord(1294, lang));
                    html.Append("<br />");
                    html.Append(GestionWeb.GetWebWord(2603, lang) + " : ");
                    html.Append(GestionWeb.GetWebWord(2605, lang) + " ");
                    html.Append(this.PeriodicityValue.ToString());
                    html.Append("<br />");
                    break;
            }
            html.Append(GestionWeb.GetWebWord(2606, lang) + " : ");
            html.Append(this.ExpirationDate.ToString("dd/MM/yyyy") + "<br />");

            html.Append(GestionWeb.GetWebWord(2607, lang) + " : ");
            html.Append(GestionWeb.GetWebWord(idWebText, lang) + "<br />");
            return (html.ToString());
        }

        #endregion

        public Alert(DataRow row)
        {
            if (row == null)
                throw new NoDataException("No data in alert row");
            this._row = row;
        }
    }
}
