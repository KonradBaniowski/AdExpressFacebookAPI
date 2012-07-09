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

            _updateColumnHeader1.Text = (_webSession.DataLanguage == 7) ? "Тип медиа" : "Medium";
            _updateColumnHeader2.Text = (_webSession.DataLanguage == 7) ? "Дата поставки" : "Date";

            _mediaTvNationalColumn.Text = (_webSession.DataLanguage == 7) ? "ТВ Национальная реклама и Москва" : "TV National advertising & Moscow";
            _mediaTvRegionColumn.Text = (_webSession.DataLanguage == 7) ? "ТВ Регионы" : "TV Regions";
            _mediaTvSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "ТВ Спонсорство" : "TV Sponsorship";
            _mediaTvNicheChannelsColumn.Text = (_webSession.DataLanguage == 7) ? "ТВ Нишевые каналы" : "TV Niche channels";
            _mediaPressColumn.Text = (_webSession.DataLanguage == 7) ? "Пресса" : "Press";
            _mediaEditorialColumn.Text = (_webSession.DataLanguage == 7) ? "Редакционная поддержка" : "Editorial";
            _mediaRadioMoscowColumn.Text = (_webSession.DataLanguage == 7) ? "Радио Москва" : "Radio Moscow";
            _mediaRadioRegionColumn.Text = (_webSession.DataLanguage == 7) ? "Радио Регионы" : "Radio Regions";
            _mediaRadioSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "Радио Спонсорство" : "Radio Sponsorship";
            _mediaRadioMusicColumn.Text = (_webSession.DataLanguage == 7) ? "Радио Музыка" : "Radio Music";
            _mediaOutdoorColumn.Text = (_webSession.DataLanguage == 7) ? "Наружная реклама" : "Outdoor";
            _mediaInternetColumn.Text = (_webSession.DataLanguage == 7) ? "Интернет" : "Internet";

            DataTable table = GetUpdateInfo();

            _dateTvNationalColumn.Text = GetSubMediaLastDate(table, 10);
            _dateTvRegionColumn.Text = GetSubMediaLastDate(table, 12);
            _dateTvSponsorshipColumn.Text = GetSubMediaLastDate(table, 13);
            _dateTvNicheChannelsColumn.Text = GetSubMediaLastDate(table, 15);
            _datePressColumn.Text = GetSubMediaLastDate(table, 30);
            _dateEditorialColumn.Text = GetSubMediaLastDate(table, 80);
            _dateRadioMoscowColumn.Text = GetSubMediaLastDate(table, 20);
            _dateRadioRegionColumn.Text = GetSubMediaLastDate(table, 21);
            _dateRadioSponsorshipColumn.Text = GetSubMediaLastDate(table, 22);
            _dateRadioMusicColumn.Text = GetSubMediaLastDate(table, 23);
            _dateOutdoorColumn.Text = GetSubMediaLastDate(table, 40);
            _dateInternetColumn.Text = GetSubMediaLastDate(table, 60);

            #endregion

            _tableShedule.Text = (_webSession.DataLanguage == 7) ? "График прогрузки данных:" : "Schedule of update:";

            #region Schedule of update

            _scheduleColumnHeader1.Text = (_webSession.DataLanguage == 7) ? "Тип медиа" : "Medium";
            _scheduleColumnHeader2.Text = (_webSession.DataLanguage == 7) ? "География" : "Geography";
            _scheduleColumnHeader3.Text = (_webSession.DataLanguage == 7) ? "Ежедневно" : "Daily";
            _scheduleColumnHeader4.Text = (_webSession.DataLanguage == 7) ? "Еженедельно" : "Weekly";
            _scheduleColumnHeader5.Text = (_webSession.DataLanguage == 7) ? "Ежемесячно" : "Monthly";

            _mediumTvColumn.Text = (_webSession.DataLanguage == 7) ? "ТВ" : "TV";
            _marketTvNationalColumn.Text = (_webSession.DataLanguage == 7) ? "Национальная реклама+Москва" : "National advertising+Moscow";
            _dailyTvNationalColumn.Text = (_webSession.DataLanguage == 7) ? "минус 1 день" : "1 day minus";
            _marketTvRegionColumn.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _dailyTvRegionColumn.Text = (_webSession.DataLanguage == 7) ? "минус 4 дня" : "4 day minus";

            _mediumTvSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "ТВ Спонсорство" : "TV Sponsorship";
            _marketTvSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _weeklyTvSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "по средам за предыдущую неделю" : "on Wednesday for the last week";

            _mediumTvNicheChannelsColumn.Text = (_webSession.DataLanguage == 7) ? "ТВ Нишевые каналы" : "TV Niche channels";
            _marketTvNicheChannelsColumn.Text = (_webSession.DataLanguage == 7) ? "Национальная реклама" : "National advertising";
            _dailyTvNicheChannelsColumn.Text = (_webSession.DataLanguage == 7) ? "минус 1 день" : "1 day minus";

            _mediumPressColumn.Text = (_webSession.DataLanguage == 7) ? "Пресса" : "Press";
            _marketPressColumn.Text = (_webSession.DataLanguage == 7) ? "Национальная и локальная реклама" : "National & Local advertising";
            _monthlyPressColumn.Text = (_webSession.DataLanguage == 7) ? "до 20 числа за предыдущий месяц" : "before 20 date for the last month";

            _mediumEditorialColumn.Text = (_webSession.DataLanguage == 7) ? "Редакционная поддержка" : "Editorial";
            _marketEditorialColumn.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _monthlyEditorialColumn.Text = (_webSession.DataLanguage == 7) ? "до 10 числа за предыдущий месяц" : "before 10 date for the last month";

            _mediumRadioColumn.Text = (_webSession.DataLanguage == 7) ? "Радио" : "Radio";
            _marketRadioMoscowColumn.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _dailyRadioMoscowColumn.Text = (_webSession.DataLanguage == 7) ? "минус 2 дня" : "2 day minus";
            _marketRadioRegionColumn.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _dailyRadioRegionColumn.Text = (_webSession.DataLanguage == 7) ? "минус 3 дня" : "3 day minus";

            _mediumRadioSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "Радио Спонсорство" : "Radio Sponsorship";
            _marketRadioSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _weeklyRadioSponsorshipColumn.Text = (_webSession.DataLanguage == 7) ? "по средам за предыдущую неделю" : "on Wednesday for the last week";

            _mediumRadioMusicColumn.Text = (_webSession.DataLanguage == 7) ? "Радио Музыка" : "Radio Music";
            _marketRadioMusicColumn.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _weeklyRadioMusicColumn.Text = (_webSession.DataLanguage == 7) ? "по средам за предыдущую неделю" : "on Wednesday for the last week";

            _mediumOutdoorColumn.Text = (_webSession.DataLanguage == 7) ? "Наружная реклама" : "Outdoor";
            _marketOutdoorMoscowColumn.Text = (_webSession.DataLanguage == 7) ? "Москва" : "Moscow";
            _monthlyOutdoorMoscowColumn.Text = (_webSession.DataLanguage == 7) ? "до 20 числа за предыдущий месяц" : "before 20 date for the last month";
            _marketOutdoorRegionColumn.Text = (_webSession.DataLanguage == 7) ? "Регионы" : "Regions";
            _monthlyOutdoorRegionColumn.Text = (_webSession.DataLanguage == 7) ? "до 25 числа за предыдущий месяц" : "before 25 date for the last month";

            _mediumInternetColumn.Text = (_webSession.DataLanguage == 7) ? "Интернет" : "Internet";
            _marketInternetColumn.Text = (_webSession.DataLanguage == 7) ? "Национальная и локальная реклама" : "National & Local advertising";
            _weeklyInternetColumn.Text = (_webSession.DataLanguage == 7) ? "по пятницам за предыдущую неделю" : "on Friday for the last week";

            #endregion
        }

        protected virtual TNS.FrameWork.DB.Common.IDataSource GetDataSource()
        {
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            object[] param = new object[1];
            param[0] = _webSession;
            if (cl == null) throw (new NullReferenceException("Core layer is null for the source provider layer"));
            TNS.AdExpress.Web.Core.ISourceProvider sourceProvider = (TNS.AdExpress.Web.Core.SourceProvider)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
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