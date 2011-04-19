using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.P3.Web.UI;
using KMI.P3.Domain.Translation;

public partial class Contact : WebPage  //Public_Contact
{
    /// <summary>
    /// Constructor
    /// </summary>
    public Contact() : base() { PageTypeInfo = PageType.contact; }
   
    #region On PreInit
    /// <summary>
    /// On preinit event
    /// </summary>
    /// <param name="e">Arguments</param>
    protected override void OnPreInit(EventArgs e) {
        base.OnPreInit(e);
    }
    #endregion

    #region On Load
    /// <summary>
    /// OnLoad
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event</param>
    protected void Page_Load(object sender, EventArgs e){

        SetAllTextLanguage(this.Controls, _siteLanguage);

        link1.HRef = "mailto:" + GestionWeb.GetWebWord(179, _siteLanguage);
        link2.HRef = "mailto:" + GestionWeb.GetWebWord(183, _siteLanguage);
        link3.HRef = "mailto:" + GestionWeb.GetWebWord(187, _siteLanguage);
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="page"></param>
    /// <param name="language"></param>
    public void SetAllTextLanguage(System.Web.UI.ControlCollection controlCollection, int language)
    {
        //foreach (System.Web.UI.Control currentControl in controlCollection)
        //{
        //    if (currentControl is ITranslation)
        //        ((ITranslation)currentControl).Language = language;
        //    if (currentControl.Controls.Count > 0)
        //    {
        //        SetAllTextLanguage(currentControl.Controls, language);
        //    }
        //}
    }
}