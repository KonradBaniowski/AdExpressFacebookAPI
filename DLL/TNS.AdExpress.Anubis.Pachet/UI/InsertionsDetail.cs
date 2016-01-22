using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using TNS.AdExpress.Anubis.Pachet.Common;
using TNS.AdExpress.Anubis.Pachet.Exceptions;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Insertions.DAL;
using TNS.FrameWork.DB.Common;
using Utils = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Anubis.Pachet.UI
{
    public class InsertionsDetail
    {

        /// <summary>
        /// Cretae insertion text file in popcorn format
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        /// <param name="config">configuration</param>	
        /// <param name="webSession">client Session </param>
        /// <param name="textFilePath">File path</param>
        public static bool CreatePachetTextFile(IDataSource dataSource, PachetConfig config, WebSession webSession, string textFilePath)
        {
            StreamWriter writer = null;
            IInsertionsDAL _dalLayer = null;
            bool hasData = false;
            StringBuilder builder = null;

            try
            {
                string sepChar = " ", filters = long.MinValue.ToString() + "," + long.MinValue.ToString() + "," + long.MinValue.ToString() + "," + long.MinValue.ToString();
                const string star = "*", sharp1 = "##", sharp2 = "#";
                long oldIdL1 = long.MinValue, oldIdL2 = long.MinValue;

                #region Get Data
                //Get detail information Levels id
                var gn = DetailLevelsInformation.Get(config.DetailLevel);
                webSession.DetailLevel = gn;

                object[] param = new object[2];
                param[0] = webSession;
                param[1] = webSession.CurrentModule;

                // Sélection du vehicle
                string vehicleSelection = webSession.GetSelection(webSession.SelectionUniversMedia, Right.type.vehicleAccess);
                if (vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw new InsertionsDetailException("The media selection is not valid");
                VehicleInformation vehicleInformation = VehiclesInformation.Get(long.Parse(vehicleSelection));
                if (vehicleInformation == null) throw (new InsertionsDetailException("La sélection de médias est incorrecte"));

                //Periods
                string fromDate = Utils.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd");
                string toDate = Utils.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd");

               

                //Get data
                CoreLayer cl = WebApplicationParameters.CoreLayers[Constantes.Web.Layers.Id.insertionsDAL];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions DAL"));
                _dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);

                DataSet ds =
                    _dalLayer.GetInsertionsData(vehicleInformation, Convert.ToInt32(fromDate),
                                                Convert.ToInt32(toDate), -1, filters);
                #endregion

                //Build text file
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                {
                    // file  Stream Writer
                    writer = new StreamWriter(textFilePath,false, Encoding.ASCII);

                    //Addin creation date
                    writer.WriteLine(star + DateTime.Now.ToString("G", WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo));

                    //Adding Export name
                    writer.WriteLine(star + webSession.ExportedPDFFileName);

                    //Adding source
                    writer.WriteLine(star + GestionWeb.GetWebWord(2943, webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(758, webSession.SiteLanguage));
                    //Adding country
                    writer.WriteLine(star + GestionWeb.GetWebWord(2947, webSession.SiteLanguage));

                    //int nbLevels = gn.GetNbLevels;
                    string dataBaseIdField1 = gn[1].DataBaseIdField;
                    string dataBaseField1 = gn[1].DataBaseField;
                    string dataBaseIdField2 = gn[2].DataBaseIdField;
                    string dataBaseField2 = gn[2].DataBaseField;
                    string reservedCol1_6 = "      ", reservedCol26_29 = "    ";
                    string idMedia = "", dateMediaNum="", dayOfWeek ="",screenCode="", duration="", position="";


                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //Add level 1 (Advertiser)
                        if (oldIdL1 != Convert.ToInt64(dr[dataBaseIdField1]))
                        {
                            builder = new StringBuilder();
                            builder.AppendFormat("{0} {1}", sharp1, dr[dataBaseField1].ToString());
                            writer.WriteLine(builder.ToString());
                        }

                        //Add Level 2
                        if (oldIdL2 != Convert.ToInt64(dr[dataBaseIdField2]))
                        {
                            builder = new StringBuilder();
                            builder.AppendFormat("{0} {1}", sharp2, dr[dataBaseField2].ToString());
                            writer.WriteLine(builder.ToString());
                        }

                        /**********Sart Add spot row*****************************/
                        builder = new StringBuilder();

                        //Add ID Media
                        idMedia = dr["id_media"].ToString();
                        idMedia = AjustStringLength(idMedia, 6, sepChar);

                        //Add date
                        dateMediaNum = dr["date_media_num"].ToString().Trim();

                        //Adding day week
                        DateTime dt = new DateTime(Convert.ToInt32(dr["day_of_week"].ToString().Substring(0, 4)), Convert.ToInt32(dr["day_of_week"].ToString().Substring(4, 2)), Convert.ToInt32(dr["day_of_week"].ToString().Substring(6, 2)));
                         dayOfWeek = (dt.DayOfWeek == DayOfWeek.Sunday) ? "7" : dt.DayOfWeek.GetHashCode().ToString();

                         //Adding Screen_code
                         screenCode = AjustStringLength(dr["id_commercial_break"].ToString(), 4, sepChar);

                         //Adding Duration
                         duration = (dr["duration"] != System.DBNull.Value) ? AjustStringLength(dr["duration"].ToString(), 3, sepChar) : "   ";

                        //Adding Position
                         position = (Convert.ToInt32(dr["id_rank"].ToString()) != 1 && Convert.ToInt32(dr["id_rank"].ToString()) != 2 && Convert.ToInt32(dr["id_rank"].ToString()) != 3) ? "4" : dr["id_rank"].ToString().Trim();
                         position = AjustStringLength(position, 2, sepChar);

                         builder = new StringBuilder();
                         builder.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}", reservedCol1_6, idMedia, dateMediaNum, dayOfWeek, screenCode, reservedCol26_29,duration, position);
                         writer.WriteLine(builder.ToString());
                        /**********End Add spot row*****************************/
                      
                        oldIdL1 = Convert.ToInt64(dr[dataBaseIdField1]);
                        oldIdL2 = Convert.ToInt64(dr[dataBaseIdField2]);
                    }
                    hasData = true;
                }


            }
            catch (System.Exception ex)
            {
                throw new Exceptions.InsertionsDetailException("CreatePachetTextFile: an error occured when creating insertion detail text File ", ex);
            }
            finally
            {
                builder = null;
                if (writer != null)
                    writer.Close();
            }
            return hasData;
        }

        /// <summary>
        /// Ajust string length
        /// </summary>
        /// <param name="input">inpu string </param>
        /// <param name="finalLength">string final Length</param>
        /// <param name="sepChar">separator</param>
        /// <returns>final string </returns>
        protected  static string AjustStringLength(string input, int finalLength, string sepChar)
        {
            if( !string.IsNullOrEmpty(input) &&  input.Length <finalLength)
            {
                string temp = input;
                int dif = finalLength - temp.Length;
                for (int i = 1; i <= dif; i++) input = string.Format("{0}{1}", sepChar, input);
            }
            return input;
        }
    }
}
