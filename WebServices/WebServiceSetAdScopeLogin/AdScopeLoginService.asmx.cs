#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TNS.FrameWork.DB;
using TNS.FrameWork.DB.Common;
using KMI.Isis.AdScopeLogins.Domain;
using KMI.Isis.AdScopeLogins.DAL.Isis;
using WebServiceSetAdScopeLogin.Parameters;

namespace WebServiceSetAdScopeLogin
{
    /// <summary>
    /// Description résumée de Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante. 
    // [System.Web.Script.Services.ScriptService]
    public class AdScopeLoginService : System.Web.Services.WebService
    {

        [WebMethod]
        public bool SetAdScopeLogin(long loginId, string login, string password, string name, string firstName, string account, string creationDate, string action, int crypteIdent)
        {
            string body = "";
            bool res = false;
            AdScopeLogin adScopeLogin = null;
            List<long> allAdScopeLogin = new List<long>();

          
               try
            {
                
                //Get source
                IDataSource dataSource = WebServiceSetAdScopeLogin.Parameters.ServiceParams.Source;
              
                #region Check parameters validity
               // Check parameters validity
                if (string.IsNullOrEmpty(login)) throw new ArgumentNullException("Parameter login cannot be null");
                if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("Parameter password cannot be null");
                if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("Parameter name cannot be null");
                if (string.IsNullOrEmpty(firstName)) throw new ArgumentNullException("Parameter firstName cannot be null");
                if (string.IsNullOrEmpty(account)) throw new ArgumentNullException("Parameter password account be null");
                if (string.IsNullOrEmpty(creationDate)) throw new ArgumentNullException("Parameter creationDate cannot be null");
                if (string.IsNullOrEmpty(action)) throw new ArgumentNullException("Parameter action cannot be null");

              
                #endregion

                #region Decrypt parameters  

              
                //Convert HEXA to CHAR
                login = KMI.P3.Web.Functions.QueryStringEncryption.HexAsciiConvert(login);
                password = KMI.P3.Web.Functions.QueryStringEncryption.HexAsciiConvert(password);
                name = KMI.P3.Web.Functions.QueryStringEncryption.HexAsciiConvert(name);
                firstName = KMI.P3.Web.Functions.QueryStringEncryption.HexAsciiConvert(firstName);
                account = KMI.P3.Web.Functions.QueryStringEncryption.HexAsciiConvert(account);
                creationDate = KMI.P3.Web.Functions.QueryStringEncryption.HexAsciiConvert(creationDate);
                action = KMI.P3.Web.Functions.QueryStringEncryption.HexAsciiConvert(action);


                 //Decrypt parameters               
                 string decryptedLogin = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(login);
                 string decryptedpassword = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(password);
                 string decryptedName = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(name);
                 string decryptedFirstName = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(firstName);
                 string decryptedAccount = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(account);
                 string decryptedCreationDate = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(creationDate);
                 string decryptedAction = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(action);

                
                #endregion

              
                try
                {
                    //Load all AdScope logins
                    allAdScopeLogin = AdScopeLoginsDAL.GetIds(dataSource);
                } catch(System.Exception err) {
                    throw new WebServiceSetAdScopeLogin.Exceptions.AdScopeLoginServiceException(" Impossible to load all adscope login from DataBase ", err);
                }

                #region Select  Operation
                //select operation 
                switch (decryptedAction.ToUpper().Trim())
                {
                    //delete login
                    case "D":
                        if (allAdScopeLogin != null && allAdScopeLogin.Count > 0 && allAdScopeLogin.Contains(loginId))
                        {
                            AdScopeLoginsDAL.Remove(loginId, dataSource);
                            res = true;
                        }
                        break;
                    //create login
                    case "C":
                    //update login
                    case "U":
                        try
                        {
                            adScopeLogin = new AdScopeLogin(loginId, decryptedName, decryptedFirstName, decryptedAccount, decryptedLogin, decryptedpassword);
                            if (allAdScopeLogin != null && allAdScopeLogin.Count > 0 && allAdScopeLogin.Contains(loginId))
                            {
                                dataSource.Open();
                                dataSource.Update(adScopeLogin.GetSQLUpdate());
                                dataSource.Close();
                                res = true;
                            }
                            else
                            {
                                dataSource.Open();
                                dataSource.Insert(adScopeLogin.GetSQLInsert());
                                dataSource.Close();
                                res = true;
                            }
                        }
                        catch (System.Exception err2)
                        {
                            throw new WebServiceSetAdScopeLogin.Exceptions.AdScopeLoginServiceException(" impossible to to insert or update adscope login ", err2);
                        }
                        break;
                    default:
                        throw new WebServiceSetAdScopeLogin.Exceptions.AdScopeLoginServiceException(" impossible to identify the action to execute ");
                }

                #endregion
            }
            catch (Exception error)
            {

                try
                {
                    TNS.FrameWork.Exceptions.BaseException err = (TNS.FrameWork.Exceptions.BaseException)error;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>Une erreur est survenue dans le service web de modification de login Adscope;.</font><br>Erreur" + err.GetHtmlDetail() + "</font></html>";
                }
                catch (System.Exception)
                {
                    try
                    {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>Une erreur est survenue dansle service web de changement de login Adscope;.</font><br>Erreur(" + error.GetType().FullName + "):" + error.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + error.StackTrace.Replace("at ", "<br>at ") + "</font></html>";
                    }
                    catch (System.Exception es)
                    {
                        throw (es);
                    }
                }
                TNS.FrameWork.Net.Mail.SmtpUtilities errorMail = new TNS.FrameWork.Net.Mail.SmtpUtilities(ServiceParams.ConfigurationDirectoryRoot + WebServiceSetAdScopeLogin.Constantes.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.SendWithoutThread("Erreur Web service Changeùment login AdScope " + (Server.MachineName), body, true, false);
                res = false;
            }

            return res;
        }

   

        

    }
}