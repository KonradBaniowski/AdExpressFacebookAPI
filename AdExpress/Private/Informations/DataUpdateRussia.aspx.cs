using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Web;

namespace AdExpress.Private.Informations
{
    public partial class DataUpdateRussia : TNS.AdExpress.Web.UI.PrivateWebPage
    {
        public DataUpdateRussia()
        {
            // Chargement de la Session
            try
            {
                string idSession = HttpContext.Current.Request.QueryString.Get("idSession");
            }
            catch (System.Exception)
            {
                Response.Write(TNS.AdExpress.Web.Functions.Script.ErrorCloseScript("Your session is unavailable. Please reconnect via the Homepage"));
                Response.Flush();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _tableData.Text = (_webSession.DataLanguage == 7) ? "Данные доступны по:" : "Updated on:";

            #region Media date

            _columnHeader13.Text = (_webSession.DataLanguage == 7) ? "Тип медиа" : "Medium";
            _columnHeader14.Text = (_webSession.DataLanguage == 7) ? "Дата поставки" : "Date";

            _mediaColumn1.Text = (_webSession.DataLanguage == 7) ? "ТВ Национальная реклама и Москва" : "TV National advertising & Moscow";
            _mediaColumn2.Text = (_webSession.DataLanguage == 7) ? "ТВ Регионы" : "TV Regions";
            _mediaColumn3.Text = (_webSession.DataLanguage == 7) ? "ТВ Спонсорство" : "TV Sponsorship";
            _mediaColumn4.Text = (_webSession.DataLanguage == 7) ? "ТВ Анонсы" : "TV Announces";
            _mediaColumn5.Text = (_webSession.DataLanguage == 7) ? "ТВ Нишевые каналы" : "TV Niche channels";
            _mediaColumn6.Text = (_webSession.DataLanguage == 7) ? "Радио Москва" : "Radio Moscow";
            _mediaColumn7.Text = (_webSession.DataLanguage == 7) ? "Радио Регионы" : "Radio Regions";
            _mediaColumn8.Text = (_webSession.DataLanguage == 7) ? "Радио Спонсорство" : "Radio Sponsorship";
            _mediaColumn9.Text = (_webSession.DataLanguage == 7) ? "Радио Музыка" : "Radio Music";
            _mediaColumn10.Text = (_webSession.DataLanguage == 7) ? "Пресса" : "Press";
            _mediaColumn11.Text = (_webSession.DataLanguage == 7) ? "Наружная реклама" : "Outdoor";
            _mediaColumn12.Text = (_webSession.DataLanguage == 7) ? "Кинотеатры" : "Cinema";
            _mediaColumn13.Text = (_webSession.DataLanguage == 7) ? "Интернет" : "Internet";
            _mediaColumn14.Text = (_webSession.DataLanguage == 7) ? "Магазины" : "Indoor";
            _mediaColumn15.Text = (_webSession.DataLanguage == 7) ? "Редакционная поддержка" : "Editorial";

            #endregion

            DataTable table = GetUpdateInfo();

            _dateColumn1.Text = GetSubMediaLastDate(table, 10);
            _dateColumn2.Text = GetSubMediaLastDate(table, 12);
            _dateColumn3.Text = GetSubMediaLastDate(table, 13);
            _dateColumn4.Text = GetSubMediaLastDate(table, 14);
            _dateColumn5.Text = GetSubMediaLastDate(table, 15);
            _dateColumn6.Text = GetSubMediaLastDate(table, 20);
            _dateColumn7.Text = GetSubMediaLastDate(table, 21);
            _dateColumn8.Text = GetSubMediaLastDate(table, 22);
            _dateColumn9.Text = GetSubMediaLastDate(table, 23);
            _dateColumn10.Text = GetSubMediaLastDate(table, 30);
            _dateColumn11.Text = GetSubMediaLastDate(table, 40);
            _dateColumn12.Text = GetSubMediaLastDate(table, 50);
            _dateColumn13.Text = GetSubMediaLastDate(table, 60);
            _dateColumn14.Text = GetSubMediaLastDate(table, 70);
            _dateColumn15.Text = GetSubMediaLastDate(table, 80);

            _tableShedule.Text = (_webSession.DataLanguage == 7) ? "График прогрузки данных:" : "Shedule of update:";

            #region Competative analysis & audit report

            _columnHeader1.Text = (_webSession.DataLanguage == 7) ? "Модуль" : "Report";
            _columnHeader2.Text = (_webSession.DataLanguage == 7) ? "Тип медиа" : "Medium";
            _columnHeader3.Text = (_webSession.DataLanguage == 7) ? "География" : "Geography";
            _columnHeader4.Text = (_webSession.DataLanguage == 7) ? "Ежедневно" : "Daily";
            _columnHeader5.Text = (_webSession.DataLanguage == 7) ? "Еженедельно" : "Weekly";
            _columnHeader6.Text = (_webSession.DataLanguage == 7) ? "Ежемесячно" : "Monthly";

            _reportColumn1.Text = (_webSession.DataLanguage == 7) ? "Конкурентный анализ и аудит" : "Competative analysis & audit report";

            _mediumColumn1.Text = (_webSession.DataLanguage == 7) ? "ТВ" : "TV";
            _marketColumn1.Text = (_webSession.DataLanguage == 7) ? "Национальная реклама+Москва" : "National advertising+Moscow";
            _dailyColumn1.Text = (_webSession.DataLanguage == 7) ? "минус 1 день" : "1 day minus";
            _marketColumn2.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _dailyColumn2.Text = (_webSession.DataLanguage == 7) ? "минус 4 дня" : "4 day minus";

            _mediumColumn3.Text = (_webSession.DataLanguage == 7) ? "ТВ Спонсорство" : "TV Sponsorship";
            _marketColumn3.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _weeklyColumn3.Text = (_webSession.DataLanguage == 7) ? "по средам за предыдущую неделю" : "on Wednesday for the last week";

            _mediumColumn4.Text = (_webSession.DataLanguage == 7) ? "ТВ Нишевые каналы" : "TV Niche channels";
            _marketColumn4.Text = (_webSession.DataLanguage == 7) ? "Национальная реклама" : "National advertising";
            _dailyColumn4.Text = (_webSession.DataLanguage == 7) ? "минус 1 день" : "1 day minus";

            _mediumColumn5.Text = (_webSession.DataLanguage == 7) ? "Радио" : "Radio";
            _marketColumn5.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _dailyColumn5.Text = (_webSession.DataLanguage == 7) ? "минус 2 дня" : "2 day minus";
            _marketColumn6.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _dailyColumn6.Text = (_webSession.DataLanguage == 7) ? "минус 3 дня" : "3 day minus";

            _mediumColumn7.Text = (_webSession.DataLanguage == 7) ? "Радио Спонсорство" : "Radio Sponsorship";
            _marketColumn7.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _weeklyColumn7.Text = (_webSession.DataLanguage == 7) ? "по средам за предыдущую неделю" : "on Wednesday for the last week";

            _mediumColumn8.Text = (_webSession.DataLanguage == 7) ? "Радио Музыка" : "Radio Music";
            _marketColumn8.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _weeklyColumn8.Text = (_webSession.DataLanguage == 7) ? "по понедельникаи - по пт предыдущей недели, по средам - по вс предыдущей недели" : "On Monday - till Friday of the last week, on Wednesday - till Sunday of the last week";

            _mediumColumn9.Text = (_webSession.DataLanguage == 7) ? "Пресса" : "Press";
            _marketColumn9.Text = (_webSession.DataLanguage == 7) ? "Национальная и локальная реклама" : "National & Local advertising";
            _monthlyColumn9.Text = (_webSession.DataLanguage == 7) ? "до 20 числа за предыдущий месяц" : "before 20 date for the last month";

            _mediumColumn10.Text = (_webSession.DataLanguage == 7) ? "ТВ анонсы" : "TV Announces";
            _marketColumn10.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _weeklyColumn10.Text = (_webSession.DataLanguage == 7) ? "по вторникам по пнд" : "On Tuesday  - till Monday";

            _mediumColumn11.Text = (_webSession.DataLanguage == 7) ? "Интернет" : "Internet";
            _marketColumn11.Text = (_webSession.DataLanguage == 7) ? "Москва+Санкт-Петербург" : "Moscow+Saint-Petersburg";
            _weeklyColumn11.Text = (_webSession.DataLanguage == 7) ? "по пятницам за предыдущую неделю" : "on Friday for the last week";

            _mediumColumn22.Text = (_webSession.DataLanguage == 7) ? "Редакционная поддержка" : "Editorial";
            _marketColumn22.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _monthlyColumn22.Text = (_webSession.DataLanguage == 7) ? "до 10 числа за предыдущий месяц" : "before 10 date for the last month";

            #endregion

            #region Advertising reports

            _columnHeader7.Text = (_webSession.DataLanguage == 7) ? "Модуль" : "Report";
            _columnHeader8.Text = (_webSession.DataLanguage == 7) ? "Тип медиа" : "Medium";
            _columnHeader9.Text = (_webSession.DataLanguage == 7) ? "География" : "Geography";
            _columnHeader10.Text = (_webSession.DataLanguage == 7) ? "Ежедневно" : "Daily";
            _columnHeader11.Text = (_webSession.DataLanguage == 7) ? "Еженедельно" : "Weekly";
            _columnHeader12.Text = (_webSession.DataLanguage == 7) ? "Ежемесячно" : "Monthly";

            _reportColumn2.Text = (_webSession.DataLanguage == 7) ? "Отчеты для рекламодателей" : "Advertising reports";

            _mediumColumn12.Text = (_webSession.DataLanguage == 7) ? "ТВ" : "TV";
            _marketColumn12.Text = (_webSession.DataLanguage == 7) ? "Национальная реклама+Москва" : "National advertising+Moscow";
            _dailyColumn12.Text = (_webSession.DataLanguage == 7) ? "минус 1 день" : "1 day minus";
            _marketColumn13.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _dailyColumn13.Text = (_webSession.DataLanguage == 7) ? "минус 4 дня" : "4 day minus";

            _mediumColumn14.Text = (_webSession.DataLanguage == 7) ? "ТВ Нишевые каналы" : "TV Niche channels";
            _marketColumn14.Text = (_webSession.DataLanguage == 7) ? "Национальная реклама" : "National advertising";
            _dailyColumn14.Text = (_webSession.DataLanguage == 7) ? "минус 1 день" : "1 day minus";

            _mediumColumn15.Text = (_webSession.DataLanguage == 7) ? "Радио" : "Radio";
            _marketColumn15.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _dailyColumn15.Text = (_webSession.DataLanguage == 7) ? "минус 2 дня" : "2 day minus";
            _marketColumn16.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _dailyColumn16.Text = (_webSession.DataLanguage == 7) ? "минус 3 дня" : "3 day minus";

            _mediumColumn17.Text = (_webSession.DataLanguage == 7) ? "Пресса" : "Press";
            _marketColumn17.Text = (_webSession.DataLanguage == 7) ? "Национальная и локальная реклама" : "National & Local advertising";
            _monthlyColumn17.Text = (_webSession.DataLanguage == 7) ? "до 20 числа за предыдущий месяц" : "before 20 date for the last month";

            _mediumColumn18.Text = (_webSession.DataLanguage == 7) ? "Интернет" : "Internet";
            _marketColumn18.Text = (_webSession.DataLanguage == 7) ? "Москва+Санкт-Петербург" : "Moscow+Saint-Petersburg";
            _weeklyColumn18.Text = (_webSession.DataLanguage == 7) ? "по пятницам за предыдущую неделю" : "on Friday for the last week";

            _mediumColumn19.Text = (_webSession.DataLanguage == 7) ? "Наружная реклама" : "Outdoor";
            _marketColumn19.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _monthlyColumn19.Text = (_webSession.DataLanguage == 7) ? "до 20 числа за предыдущий месяц" : "before 20 date for the last month";
            _marketColumn20.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _monthlyColumn20.Text = (_webSession.DataLanguage == 7) ? "до 25 числа за предыдущий месяц" : "before 25 date for the last month";

            _mediumColumn21.Text = (_webSession.DataLanguage == 7) ? "Кинотеатры" : "Cinema";
            _marketColumn21.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _monthlyColumn21.Text = (_webSession.DataLanguage == 7) ? "до 20 числа за предыдущий месяц" : "before 20 date for the last month";

            #endregion
        }

        protected virtual TNS.FrameWork.DB.Common.IDataSource GetDataSource()
        {
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            object[] param = new object[1];
            param[0] = _webSession;
            if (cl == null) throw (new NullReferenceException("Core layer is null for the source provider layer"));
            TNS.AdExpress.Web.Core.ISourceProvider sourceProvider = (TNS.AdExpress.Web.Core.SourceProvider)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            return sourceProvider.GetSource();
        }

        private DataTable GetUpdateInfo()
        {
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    #region Create Procedure Description

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[update_info_get]";
                    cmd.Parameters.Add("@id_language", SqlDbType.SmallInt);


                    // SP set Parameters
                    cmd.Parameters["@id_language"].Value = Convert.ToInt16(_webSession.DataLanguage);

                    #endregion

                    // SP Execute
                    DataTable result = new DataTable();
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        result.Load(rs);
                    }

                    conn.Close();

                    return result;
                }
            }
            catch (System.Exception err)
            {
                throw new Exception("Unable to get update info:", err);
            }
        }

        private string GetSubMediaLastDate(DataTable table, int idSubmedia)
        {
            string result = "-";

            if (table.Rows.Count > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (Convert.ToInt32(row["id_submedia"]) == idSubmedia)
                    {
                        if (!Convert.IsDBNull(row["date"]))
                        {
                            result = Convert.ToDateTime(row["date"]).ToString("dd/MM/yyyy");
                        }
                        break;
                    }
                }
            }

            return result;
        }

        protected void closeRollOverWebControl_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterClientScriptBlock(GetType(), "closeScript", TNS.AdExpress.Web.Functions.Script.CloseScript());
        }
    }
}