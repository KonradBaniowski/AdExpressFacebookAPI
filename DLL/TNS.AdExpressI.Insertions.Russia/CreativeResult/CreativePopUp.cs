
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.IO;
using System.Web.UI;
using TNS.AdExpressI.Visual;
using CstClassificationVehicle = TNS.AdExpress.Constantes.Classification.DB.Vehicles;
using CstWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Domain.Translation;
using System.Data;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Insertions.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.Insertions.Russia.CreativeResult{
    /// <summary>
    /// Class used to display tv and radio creatives in reading and streamin mode
    /// </summary>
    public class CreativePopUp : TNS.AdExpressI.Insertions.CreativeResult.CreativePopUp{
        /// <summary>
        ///File name decrypted
        /// </summary>
        protected string _fileDecrypted = "";

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idSlogan">Slogan identifier</param>
        /// <param name="file">File</param>
        /// <param name="webSession">WebSession</param>
        /// <param name="title">Title</param>
        public CreativePopUp(Page popUp, CstClassificationVehicle.names vehicle, string idSlogan, string file, WebSession webSession, string title, bool hasCreationReadRights, bool hasCreationDownloadRights)
            : base(popUp, vehicle, idSlogan, file, webSession, title, hasCreationReadRights, hasCreationDownloadRights) { }
        #endregion

        #region Get Creative PopUp
        /// <summary>
        /// Get creative popup Html code
        /// </summary>
        /// <returns>Html code</returns>
        override public string CreativePopUpRender()
        {


            #region Construction et vérification des chemins d'accès aux fichiers real ou wm
            //Vérification de l'existence des fichiers et construction des chemins d'accès suivant la volonté de 
            //lire ou de télécharger le fichier

            _fileDecrypted = TNS.AdExpress.Web.Functions.QueryStringEncryption.DecryptQueryString(_file);

            var parameters = new object[5];
            parameters[0] = VehiclesInformation.Get(_vehicle).DatabaseId;
            parameters[1] = _fileDecrypted;
            parameters[2] = _webSession.IdSession;
            parameters[3] = false;
            parameters[4] = false;


            var visual = (IVisual)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].AssemblyName, WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.visual].Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

            bool windowsFormatFound = visual.IsExist();      

            #endregion

            #region Design Tableau d'images real ou wm suivant la disponibilité des fichiers

            StringBuilder res = new StringBuilder(2000);


            if ((windowsFormatFound) && (_hasCreationReadRights || _hasCreationDownloadRights)){

                #region scripts
                //Gestion de l'ouverture du fichier windows media
                if (!_popUp.ClientScript.IsClientScriptBlockRegistered("GetObjectWindowsMediaPlayerRender"))
                {
                    _popUp.ClientScript.RegisterClientScriptBlock(_popUp.GetType(), "GetObjectWindowsMediaPlayerRender", GetObjectWindowsMediaPlayerRender(_webSession.SiteLanguage));
                }
                #endregion

                //Encrypt creative path
                string encryptedParams = "";
                if (!string.IsNullOrEmpty(_file))
                {
                    _pathReadingWindowsFile = _pathDownloadingWindowsFile =TNS.AdExpress.Constantes.Web.Links.CREATIVE_VIEW_PAGE + "?path=" + _file + "&id_vehicle=" + VehiclesInformation.Get(_vehicle).DatabaseId.ToString() + "&is_blur=false&crypt=1&idSession=" + _webSession.IdSession;
                }               
                res.Append("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"770\" border=\"0\" align=\"center\" height=\"100%\" ><TR>");
                //Rendu créations en lecture
                if (_hasCreationReadRights) res.Append(ManageCreationsDownloadWithCookies(_pathReadingRealFile, _pathReadingWindowsFile, true, false, windowsFormatFound));

                //Tableau des options
                res.Append(GetCreationsOptionsRender(false, windowsFormatFound, _pathDownloadingRealFile, _pathDownloadingWindowsFile, false, 2079));
            }
            else{
                //Aucun fichier n'est dispo	
                res.Append("<TABLE height=\"40%\"><TR><TD>&nbsp;</TD></TR></TABLE>");
                res.Append("<TABLE cellSpacing=\"0\" cellPadding=\"10\" width=\"440\" border=\"0\" align=\"center\" height=\"10%\" ><TR valign=\"middle\">");
                res.Append("<TD  align=\"center\" class=\"txtViolet11Bold backGroundWhite\" >");
                res.Append(GestionWeb.GetWebWord(892, _webSession.SiteLanguage));
                res.Append("</TD>");
            }

            res.Append("</TR></TABLE>");

            return res.ToString();
            #endregion

        }
        #endregion

        #region Get creative DS
        /// <summary>
        /// Get creative description
        /// </summary>
        /// <returns>DataSet</returns>
        protected override  DataSet GetCreativeDS() {

            DataSet ds = null;

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertionsDAL];
            if (cl == null) throw (new NullReferenceException("Core layer is null for insertions DAL"));
            object[] param = new object[2];
            param[0] = _webSession;
            param[1] = _webSession.CurrentModule;
            IInsertionsDAL dalLayer = (IInsertionsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, param, null, null, null);
            ds = dalLayer.GetVersion(_fileDecrypted, VehiclesInformation.Get(_vehicle).DatabaseId);

            return ds;
        }
        #endregion
       

        #region Windows Media Player Object
        /// <summary>
        /// Script d'ouveture d'un fichier Media Payer
        /// </summary>
        /// <returns>Script</returns>
        protected override string GetObjectWindowsMediaPlayerRender(int siteLanguage)
        {

            StringBuilder res = new StringBuilder(2000);
            res.Append("<script language=\"JavaScript\" type=\"text/javascript\">");
            res.Append(" function GetObjectWindowsMediaPlayerRender(filepath){");
            res.Append(" document.write('<TD><TABLE height=\"326\" cellPadding=\"5\" width=\"368\" align=\"center\" class=\"backGroundWhite\"><TBODY><TR><TD>');");
            //Lecture par Media player
            res.Append(" document.write('<object id=\"video1\"  classid=\"CLSID:22D6F312-B0F6-11D0-94AB-0080C74C7E95\" height=\"288\" width=\"352\" align=\"middle\"  codebase=\"http://activex.microsoft.com/activex/controls/mplayer/en/nsmp2inf.cab#Version=6,4,5,715\"  standby=\"" + GestionWeb.GetWebWord(1911, siteLanguage) + "\" type=\"application/x-oleobject\">');");
            res.Append(" document.write('<param name=\"FileName\" value='+filepath+' >');");
            res.Append(" document.write('<param name=\"AutoStart\" value=\"true\">');");
            res.Append(" document.write('<embed type=\"application/x-mplayer2\" pluginspage=\"http://www.microsoft.com/Windows/MediaPlayer/\"  src='+filepath+' name=\"video1\" height=\"288\" width=\"352\" AutoStart=true>'); ");
            res.Append(" document.write('</embed>');");
            res.Append(" document.write('</object>');");
            res.Append(" document.write('</TD></TR></TBODY></TABLE></TD>');");
            res.Append(" }");
            res.Append("</script>");
            return res.ToString();
        }
        #endregion

    }
}
