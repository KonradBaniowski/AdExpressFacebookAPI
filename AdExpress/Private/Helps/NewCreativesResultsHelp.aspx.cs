﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Web.UI;

public partial class Private_Helps_NewCreativesResultsHelp : WebPage
{
    #region Evènements

    #region Chargement
    /// <summary>
    /// Chargement de la page
    /// </summary>
    /// <param name="sender">Objet qui lance l'évènement</param>
    /// <param name="e">Aguments</param>	
    protected void Page_Load(object sender, System.EventArgs e)
    {
        //Modification de la langue pour les Textes AdExpress
        //TNS.AdExpress.Web.Translation.Functions.Translate.SetTextLanguage(this.Controls[3].Controls,_siteLanguage);
    }
    #endregion

    #region Code généré par le Concepteur Web Form
    /// <summary>
    /// Evènement d'initialisation
    /// </summary>
    /// <param name="e">Arguments</param>
    override protected void OnInit(EventArgs e)
    {
        //
        // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
        //
        InitializeComponent();
        base.OnInit(e);
    }

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {

    }
    #endregion

    #endregion
}