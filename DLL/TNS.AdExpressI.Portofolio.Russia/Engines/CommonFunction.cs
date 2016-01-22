using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using System.Data;
using TNS.AdExpressI.Portofolio.DAL;
using System.Reflection;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using FrameWorkResultsConstantes = TNS.AdExpress.Constantes.FrameWork.Results;
using System.Globalization;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;

namespace TNS.AdExpressI.Portofolio.Russia.Engines{
    /// <summary>
    /// Common functions between the diffrent results
    /// Specification for Russia
    /// </summary>
    public static class CommonFunction{

        #region HTML for vehicle view
        /// <summary>
        /// Get view of the vehicle (HTML)
        /// </summary>
        /// <param name="excel">True for excel result</param>
        /// <param name="resultType">Result Type (Synthesis, MediaDetail)</param>
        /// <returns>HTML code</returns>
        public static string GetVehicleViewHtml(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool excel, int resultType)
        {

            #region Variables
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
            StringBuilder t = new StringBuilder(5000);
            DataSet ds = null;
            string pathWeb = "";
            string media = "";
            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(webSession.CurrentModule); ;
            #endregion

            #region Accès aux tables
            if (module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
            object[] parameters = new object[5];
            parameters[0] = webSession;
            parameters[1] = vehicleInformation;
            parameters[2] = idMedia;
            parameters[3] = periodBeginning;
            parameters[4] = periodEnd;
            IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            ds = portofolioDAL.TableOfIssue();

            if(ds == null || ds.Tables.Count==0 || ds.Tables[0] == null)
                return "";

            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
                media = dt.Rows[0]["media"].ToString();
            #endregion

            // Vérifie si le client a le droit aux créations
            if (webSession.CustomerLogin.ShowCreatives(vehicleInformation.Id))
            {
                if (!excel)
                {
                    if (vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
                        || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress
                        || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.newspaper
                        || vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.magazine
                        )
                    {

                        int compteur = 0;
                        string endBalise = "";
                        string day = "";
                        t.Append("<table  border=1 cellpadding=0 cellspacing=0 width=600 align=center class=\"paleVioletBackGroundV2 violetBorder\">");
                        //Vehicle view
                        switch (resultType)
                        {
                            case FrameWorkResultsConstantes.Portofolio.SYNTHESIS:
                                t.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc12Bold violetBackGround portofolioSynthesisBorder\" align=\"center\">" + GestionWeb.GetWebWord(2763, webSession.SiteLanguage) + "</td></tr>");
                                break;
                            case FrameWorkResultsConstantes.Portofolio.DETAIL_MEDIA:
                                t.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc14Bold violetBackGround portofolioSynthesisBorder\" align=\"center\">" + media + "</td></tr>");
                                break;
                        }

                        CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //date_media_num

                            if (dt.Rows[i]["visual"] != System.DBNull.Value)
                                pathWeb = string.Empty;
                            else
                                pathWeb = "/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif";
                            
                            DateTime dayDT = new DateTime(int.Parse(dt.Rows[i]["date_media_num"].ToString().Substring(0, 4)), int.Parse(dt.Rows[i]["date_media_num"].ToString().Substring(4, 2)), int.Parse(dt.Rows[i]["date_media_num"].ToString().Substring(6, 2)));
                            day = DayString.GetCharacters(dayDT, cultureInfo) + " " + Dates.DateToString(dayDT, webSession.SiteLanguage);

                            if (compteur == 0)
                            {
                                t.Append("<tr>");
                                compteur = 1;
                                endBalise = "";
                            }
                            else if (compteur == 1)
                            {
                                compteur = 2;
                                endBalise = "";
                            }
                            else
                            {
                                compteur = 0;
                                endBalise = "</td></tr>";

                            }
                            t.Append("<td class=\"portofolioSynthesisBorder\"><table  border=0 cellpadding=0 cellspacing=0 width=100% >");
                            t.Append("<tr><td class=\"portofolioSynthesis\" align=center >" + day + "</td><tr>");
                            t.Append("<tr><td align=\"center\" class=\"portofolioSynthesis\" >");
                            if (resultType == FrameWorkResultsConstantes.Portofolio.SYNTHESIS)
                            {
                                if (dt.Rows[i]["visual"] != System.DBNull.Value)
                                    t.Append("<a href=\"javascript:portofolioCreation('" + webSession.IdSession + "','" + idMedia + "','" + dt.Rows[i]["date_media_num"].ToString() + "','" + dt.Rows[i]["date_media_num"].ToString() + "','" + dt.Rows[i]["media"] + "','" + dt.Rows[i]["number_page_media"].ToString() + "');\" >");
                                t.Append(" <img alt=\"" + GestionWeb.GetWebWord(1409, webSession.SiteLanguage) + "\" src='" + pathWeb + "' border=\"0\" width=180 height=220>");
                            }
                            else if (resultType == FrameWorkResultsConstantes.Portofolio.DETAIL_MEDIA)
                            {
                                t.Append("<a href=\"javascript:portofolioDetailMedia('" + webSession.IdSession + "','" + idMedia + "','" + dt.Rows[i]["date_media_num"].ToString() + "','');\" >");
                                t.Append(" <img alt=\"\" src='" + pathWeb + "' border=\"0\" width=180 height=220>");
                            }
                            if (dt.Rows[i]["visual"] != System.DBNull.Value)
                            {
                                t.Append("</a>");
                            }
                            t.Append("</td></tr>");
                            if (dt.Rows.Count > 0)
                            {
                                if (dt.Rows[i]["RUBLES"] != System.DBNull.Value)
                                {
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + " : " + dt.Rows[i]["insertions"].ToString() + "</td><tr>");
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\" style=\"white-space: nowrap;\">" + GestionWeb.GetWebWord(2748, webSession.SiteLanguage) + " :" + int.Parse(dt.Rows[i]["RUBLES"].ToString()).ToString("### ### ### ###") + "</td><tr>");
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\" style=\"white-space: nowrap;\">" + GestionWeb.GetWebWord(2749, webSession.SiteLanguage) + " :" + int.Parse(dt.Rows[i]["USD"].ToString()).ToString("### ### ### ###") + "</td><tr>");
                                }
                                else
                                {
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + " : 0</td><tr>");
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(2748, webSession.SiteLanguage) + " : 0</td><tr>");
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(2749, webSession.SiteLanguage) + " : 0</td><tr>");
                                }
                            }
                            t.Append("</table></td>");
                            t.Append(endBalise);
                        }
                        if (compteur != 0)
                            t.Append("</tr>");

                        t.Append("</table>");
                    }
                }
            }

            return t.ToString();
        }
        #endregion
       
    }
}
