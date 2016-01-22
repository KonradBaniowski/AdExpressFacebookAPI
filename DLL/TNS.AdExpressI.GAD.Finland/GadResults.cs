using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;
using TNS.AdExpressI.GAD.DAL;
using System.Reflection;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpressI.GAD.Finland
{
    public class GadResults : TNS.AdExpressI.GAD.GadResults
    {
        private string _vatCode;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idAddress">id adresss</param>
        /// <param name="advertiser"> advertiser</param>
        public GadResults(WebSession session, string idAddress, string advertiser) : base(session, idAddress, advertiser)
        {
        }

        /// <summary>
        /// Get Gad html result
        /// </summary>
        /// <returns>Gad html result</returns>
        public override string GetHtml()
        {

            var html = new StringBuilder();
            var param = new object[2];
            param[0] = _session;
            param[1] = _idAddress;
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.gadDAL];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the gad DAL"));
            var gadDal = (IGadDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            DataSet data = gadDal.GetData();

            foreach (DataRow myRow in data.Tables[0].Rows)
            {
                _company = myRow["company"].ToString();
                _street = myRow["street"].ToString();
                _street2 = myRow["street2"].ToString();
                _codePostal = myRow["code_postal"].ToString();
                _town = myRow["town"].ToString();
                _phone = myRow["telephone"].ToString();
                _fax = myRow["fax"].ToString();
                _email = myRow["email"].ToString();
                if (_session.CustomerLogin.CustormerFlagAccess((long)AdExpress.Constantes.Customer.DB.Flag.id.gad.GetHashCode()))
                {
                    _docMarketingId = myRow["id_gad"].ToString();                 
                }
                _vatCode = myRow["vat_code"].ToString();
            }         

            html.Append(" <table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" height=\"100%\" border=\"0\">");
            //Header
            html.Append(" <!-- Header --> ");
            html.Append(" <tr> ");
            html.AppendFormat(" <td class=\"popUpHeaderBackground popUpTextHeader\">&nbsp;{0}&nbsp;:&nbsp;<span id=\"advertiserLabel\">{1}</span></td>", GestionWeb.GetWebWord(857, _session.SiteLanguage), _advertiser);
            html.Append(" </tr>");

            //Content
            html.AppendLine(" <!-- Content --> ");

            html.AppendLine("  <tr>  <td style=\"height:100%;background-color:#FFF;padding:10;\" valign=\"top\">");

            html.AppendLine(" <table id=\"SaveTable\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");

            //Company
            html.AppendLine(" <!-- Company --> ");
            html.AppendLine(" <tr valign=\"top\">");
            html.AppendLine("<td align=\"center\" rowspan=\"6\" width=\"180\" nowrap>");
            html.AppendFormat("<img width=\"135\" height=\"135\"src=\"/App_Themes/{0}/Images/Common/TNS_logowebsafe.jpg\"><br>", _theme);
            html.AppendFormat("<span  class=\"txtViolet11Bold\" id=\"sourceAdress\">{0}</span><br/>", GestionWeb.GetWebWord(2973, _session.SiteLanguage));
            html.AppendFormat("<span class=\"txtViolet11\" id=\"fonectaAdress\">{0}</span><br/>", GestionWeb.GetWebWord(2974, _session.SiteLanguage));
            html.AppendLine(" </td>");
            html.AppendFormat("<td class=\"txtViolet11Bold\" valign=\"top\" width=\"1%\" noWrap>&nbsp;{0}</td>", GestionWeb.GetWebWord(1132, _session.SiteLanguage));
            html.AppendLine(" <td class=\"txtViolet11Bold\" valign=\"top\" width=\"1%\">&nbsp;:&nbsp;</td>");
            html.AppendFormat("<td class=\"txtViolet11\" valign=\"top\"><span id=\"companyLabel\">{0}</span></td>", _company);
            html.AppendLine(" </tr>");

            //Address
            html.AppendLine(" <!-- Address --> ");
            html.AppendLine(" <tr valign=\"top\">");
            html.AppendFormat("    <td class=\"txtViolet11Bold\" nowrap>&nbsp;{0}</td>", GestionWeb.GetWebWord(1133, _session.SiteLanguage));
            html.AppendLine(" <td class=\"txtViolet11Bold\" valign=\"top\">&nbsp;:&nbsp;</td>");
            html.AppendFormat("<td class=\"txtViolet11\" valign=\"top\"><span id=\"streetLabel\">{0}</span><br><span id=\"street2Label\">{1}</span><br><span id=\"codePostalLabel\">{2}</span>&nbsp;&nbsp;<span id=\"townLabel\">{3}</span></td>", _street, _street2, _codePostal, _town);
            html.AppendLine(" </tr>");

            //Telephone
            html.AppendLine(" <!-- Telephone --> ");
            html.AppendLine(" <tr valign=\"top\">");
            html.AppendFormat("    <td class=\"txtViolet11Bold\" nowrap>&nbsp;{0}</td>", GestionWeb.GetWebWord(1134, _session.SiteLanguage));
            html.AppendLine(" <td class=\"txtViolet11Bold\" valign=\"top\">&nbsp;:&nbsp;</td>");
            html.AppendFormat("<td class=\"txtViolet11\" valign=\"top\"><span id=\"phoneLabel\">{0}</span></td>", _phone);
            html.AppendLine(" </tr>");

            //Fax
            html.AppendLine(" <!-- Fax --> ");
            html.AppendLine(" <tr valign=\"top\">");
            html.AppendFormat("    <td class=\"txtViolet11Bold\" nowrap>&nbsp;{0}</td>", GestionWeb.GetWebWord(1135, _session.SiteLanguage));
            html.AppendLine(" <td class=\"txtViolet11Bold\" valign=\"top\">&nbsp;:&nbsp;</td>");
            html.AppendFormat("<td class=\"txtViolet11\" valign=\"top\"><span id=\"faxLabel\">{0}</span></td>", _fax);
            html.AppendLine(" </tr>");

            //Email
            html.AppendLine(" <!-- Email --> ");
            html.AppendLine(" <tr valign=\"top\">");
            html.AppendFormat("    <td class=\"txtViolet11Bold\" nowrap>&nbsp;{0}</td>", GestionWeb.GetWebWord(1136, _session.SiteLanguage));
            html.AppendLine(" <td class=\"txtViolet11Bold\" valign=\"top\">&nbsp;:&nbsp;</td>");
            html.AppendFormat("<td class=\"txtViolet11\" valign=\"top\"><span id=\"emailLabel\">{0}</span></td>", _email);
            html.AppendLine(" </tr>");

            //VAT code
            html.AppendLine(" <!-- VAT CODE --> ");
            html.AppendLine(" <tr valign=\"top\">");
            html.AppendFormat("    <td class=\"txtViolet11Bold\" nowrap>&nbsp;{0}</td>", GestionWeb.GetWebWord(2972, _session.SiteLanguage));
            html.AppendLine(" <td class=\"txtViolet11Bold\" valign=\"top\">&nbsp;:&nbsp;</td>");
            html.AppendFormat("<td class=\"txtViolet11\" valign=\"top\"><span id=\"emailLabel\">{0}</span></td>", _vatCode);
            html.AppendLine(" </tr>");

            html.AppendLine(" <!-- Empty --> ");
            html.Append(" <tr><td colspan=\"3\">&nbsp;</td> </tr>");

            html.AppendLine("  </table>");

            html.AppendLine(" </td> </tr>");

            html.Append(" <!-- Footer  --> ");
            html.AppendLine(" <tr >  <td class=\"popUpFooterBackground\" align=\"right\">");
            //html.Append(_docMarketingTarget);
            html.AppendLine(" &nbsp;&nbsp;</td> </tr>");

            html.AppendLine("  </table>");



            return html.ToString();
        }
    }
}
