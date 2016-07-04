using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.GAD.DAL;

namespace TNS.AdExpressI.GAD
{
    public abstract class GadResults : IGadResults
    {
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;

        /// <summary>
        /// ID adresse
        /// </summary>
        protected string _idAddress;

        /// <summary>
        /// Advertiser
        /// </summary>
        protected readonly string _advertiser;

        /// <summary>
        /// Theme
        /// </summary>
        protected string _theme;
        /// <summary>
        /// link GAD
        /// </summary>
        protected string _linkGad;

        /// <summary>
        /// Email Gad
        /// </summary>
        protected string _emailGad;

        protected string _company;
        protected string _street;
        protected string _street2;
        protected string _codePostal;
        protected string _town;
        protected string _phone;
        protected string _fax;
        protected string _email;
        protected string _docMarketingId;
        protected string _docMarketingKey;
        protected string _docMarketingTarget;

        #region Construtor
        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idAddress">id adresss</param>
        /// <param name="advertiser"> advertiser</param>
        public GadResults(WebSession session, string idAddress, string advertiser)
        {
            _session = session;
            _idAddress = idAddress;
            _advertiser = advertiser;
        }
        #endregion

        /// <summary>
        /// Get /Set Theme
        /// </summary>
        public string Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        /// <summary>
        /// Get Gad html result
        /// </summary>
        /// <returns>Gad html result</returns>
        public virtual string GetHtml()
        {

            var html = new StringBuilder();

            var param = new object[2];
            param[0] = _session;
            param[1] = _idAddress;
            TNS.AdExpress.Domain.Layers.CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.gadDAL];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the gad DAL"));
            dynamic gadDal = (IGadDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
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
                    _docMarketingKey = myRow["docKey"].ToString();
                }
            }
            if (!string.IsNullOrEmpty(_company) && !string.IsNullOrEmpty(_docMarketingId))
            {
                _docMarketingTarget = string.Format("<a href=\"javascript:OpenWindow('{0}');\" onMouseOver=\"advertiserFile.src=ficheDown.src\" onMouseOut=\"advertiserFile.src=ficheUp.src\"><img title=\"{1}\" border=0 name=\"advertiserFile\" src=\"/App_Themes/{2}/Images/Culture/Button/bt_fiche_up.gif\"/></a>",
                    //lien
                    string.Format(GestionWeb.GetWebWord(2092, _session.SiteLanguage), _company.Replace("/","-"), _docMarketingId),
                    GestionWeb.GetWebWord(2098, _session.SiteLanguage),
                    _theme
                    );
            }
            else
            {
                _docMarketingTarget = string.Format("<img title=\"{0}\" border=0 name=\"advertiserFile\" src=\"/App_Themes/{1}/Images/Culture/Button/bt_fiche_off.gif\"/>",
                    GestionWeb.GetWebWord(2098, _session.SiteLanguage),_theme);
            }

            //Header
            html.Append(" <!-- Header --> ");
            html.Append("  <div class=\"popUpHead popUpHeaderBackground popUpTextHeader\"> ");
            html.AppendFormat(" &nbsp;{0}&nbsp;:&nbsp;<span id=\"advertiserLabel\">{1}</span>", GestionWeb.GetWebWord(857, _session.SiteLanguage), _advertiser);
            html.Append(" </div>");

            //Content
            html.AppendLine(" <!-- Content --> ");

            html.AppendLine("  <div class=\"popUpContent\">  <div class=\"popUpPad2\"></div> ");

            html.AppendLine(" <table id=\"SaveTable\" cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");

            //Company
            html.AppendLine(" <!-- Company --> ");
            html.AppendLine(" <tr valign=\"top\">");
            html.AppendLine("<td align=\"center\" rowspan=\"6\" width=\"170\">");
            html.AppendFormat("<img src=\"/App_Themes/{0}/Images/Common/Gad.jpg\"><br><br>", _theme);
            _linkGad = "http://" + GestionWeb.GetWebWord(1137, _session.SiteLanguage);
            html.AppendFormat("<a class=\"roll02\" href=\"{0}\" target=\"_blank\"><span id=\"linkGadLabel\">{0}</span></a><br><br>", _linkGad);
            _emailGad = GestionWeb.GetWebWord(1138, _session.SiteLanguage);
            html.AppendFormat("<a class=\"roll02\" href=\"mailto:{0}\" target=\"_blank\"><span id=\"emailGadLabel\">{0}</span></a><br>", _emailGad);
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
            html.AppendFormat("<td class=\"txtViolet11\" valign=\"top\"><span id=\"streetLabel\">{0}</span><br><span id=\"street2Label\">{1}</span><br><span id=\"codePostalLabel\">{2}</span>&nbsp;&nbsp;<span id=\"townLabel\">{3}</span></td>",_street,_street2,_codePostal,_town) ;
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

            html.AppendLine(" <!-- Empty --> ");
            html.Append(" <tr><td colspan=\"3\">&nbsp;</td> </tr>");

            html.AppendLine("  </table>");

            html.AppendLine(" <div class=\"popUpPad2\"></div>");
            html.AppendLine(" </div>");

            html.Append(" <!-- Footer  --> ");
            html.AppendLine(" <div class=\"popUpFoot popUpFooterBackground\">");
            html.AppendLine(" <div style=\"padding-top:12px\"> ");
            html.Append(_docMarketingTarget);
            html.AppendLine(" &nbsp;&nbsp;");
            html.AppendLine(" </div> ");
            html.AppendLine(" </div> ");


            //html.Append(TNS.AdExpress.Web.Functions.Script.OpenWindow());

            return html.ToString();
        }
    }
}
