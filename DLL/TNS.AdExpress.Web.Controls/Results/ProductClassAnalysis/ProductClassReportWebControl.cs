#region Info
/*
 * Author : G Ragneau
 * Created on : 03/02/2009
 * Modification:
 *      date - author - description
 * 
 * 
 * 
 * 
 * */
#endregion

using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;


namespace TNS.AdExpress.Web.Controls.Results.ProductClassAnalysis
{

    [ToolboxData("<{0}:ProductClassReportWebControl runat=server></{0}:ProductClassReportWebControl>")]
    public class ProductClassReportWebControl : ResultWebControl
    {

        protected override ResultTable GetResultTable(WebSession customerWebSession)
        {
            ResultTable r = base.GetResultTable(customerWebSession);

            #region Styles
            LineStart lS = null;
            for (int i = 0; i < r.LinesNumber; i++)
            {
                lS = r.GetLineStart(i);
                if (!(lS is LineHide))
                {
                    switch (lS.LineType)
                    {
                        case LineType.total:
                            lS.CssClass = this.CssLTotal;
                            break;
                        case LineType.level1:
                            lS.BackgroundColor = this.BackgroudColorL1;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL1;
                            lS.CssClass = this.CssL1;
                            break;
                        case LineType.level2:
                            lS.BackgroundColor = this.BackgroudColorL2;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL2;
                            lS.CssClass = this.CssL2;
                            break;
                        case LineType.level3:
                            lS.BackgroundColor = this.BackgroudColorL3;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL3;
                            lS.CssClass = this.CssL3;
                            break;
                        case LineType.level4:
                            lS.BackgroundColor = this.BackgroudColorL4;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL4;
                            lS.CssClass = this.CssL4;
                            break;
                        case LineType.level5:
                            lS.BackgroundColor = this.BackgroudColorL5;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL5;
                            lS.CssClass = this.CssL5;
                            break;
                        case LineType.level6:
                            lS.BackgroundColor = this.BackgroudColorL6;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL6;
                            lS.CssClass = this.CssL6;
                            break;
                        case LineType.level7:
                            lS.BackgroundColor = this.BackgroudColorL7;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL7;
                            lS.CssClass = this.CssL7;
                            break;
                        case LineType.level8:
                            lS.BackgroundColor = this.BackgroudColorL8;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL8;
                            lS.CssClass = this.CssL8;
                            break;
                        case LineType.level9:
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorL9;
                            lS.BackgroundColor = this.BackgroudColorL9;
                            lS.CssClass = this.CssL9;
                            break;
                        case LineType.subTotal1:
                            lS.BackgroundColor = this.BackgroudColorSubTotal1;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorSubTotal1;
                            lS.CssClass = this.CssSubTotal1;
                            break;
                        case LineType.subTotal2:
                            lS.BackgroundColor = this.BackgroudColorSubTotal2;
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorSubTotal2;
                            lS.CssClass = this.CssSubTotal2;
                            break;
                        case LineType.subTotal3:
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorSubTotal2;
                            lS.BackgroundColor = this.BackgroudColorSubTotal3;
                            lS.CssClass = this.CssSubTotal3;
                            break;
                        case LineType.subTotal4:
                            lS.HighlightBackgroundColor = this.HighlightBackgroundColorSubTotal3;
                            lS.BackgroundColor = this.BackgroudColorSubTotal4;
                            lS.CssClass = this.CssSubTotal4;
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion

            #region Legend
            if (customerWebSession.SecondaryProductUniverses.Count > 0 && (customerWebSession.SecondaryProductUniverses.ContainsKey(0) || customerWebSession.SecondaryProductUniverses.ContainsKey(1)))
            {

                StringBuilder str = new StringBuilder();
                str.Append("<div width=\"100%\" align=\"center\"><table class=\"asLegend\">");
                str.Append("<tr>");
                str.AppendFormat("<td class=\"asLegend asLegendRef\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td><td class=\"asLegendText\">: {0}</td>", GestionWeb.GetWebWord(2562, _customerWebSession.SiteLanguage));
                str.AppendFormat("<td class=\"asLegend asLegendComp\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td><td class=\"asLegendText\">: {0}</td>", GestionWeb.GetWebWord(2563, _customerWebSession.SiteLanguage));
                str.AppendFormat("<td class=\"asLegend asLegendBoth\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td><td class=\"asLegendText\">: {0}</td>", GestionWeb.GetWebWord(2564, _customerWebSession.SiteLanguage));
                str.AppendFormat("<td>{0}</td><td class=\"asLegendText\">: {1}</td>", GestionWeb.GetWebWord(2566, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(2565, _customerWebSession.SiteLanguage));
                str.Append("</tr>");
                str.Append("</table></div>");
                _optionHtml = str.ToString();
            }
            #endregion

            return r;
        }

        protected override void Render(HtmlTextWriter output)
        {
            base.Render(output);
        }

        #region Construit  tableau index pour répétition du niveau parent
        /// <summary>
        /// Obtient le tableau d'index
        /// </summary>
        /// <param name="data">Tableau de données à traiter</param>
        /// <param name="nbLineToSchow">Nombre de ligne affichée</param>
        /// <returns>tableau d'index</returns>
        protected override int[] GetTableIndex(ResultTable data, long nbLineToSchow)
        {

            switch (_customerWebSession.PreformatedTable)
            {
                case CstFormat.PreformatedTables.productMedia_X_Year:
                case CstFormat.PreformatedTables.mediaProduct_X_Year:
                case CstFormat.PreformatedTables.mediaProduct_X_YearMensual:
                case CstFormat.PreformatedTables.productMedia_X_YearMensual:
                    return GetTableIndexClassif1_Classif2XYear(data, nbLineToSchow);
                    break;
                case CstFormat.PreformatedTables.mediaYear_X_Cumul:
                case CstFormat.PreformatedTables.mediaYear_X_Mensual:
                case CstFormat.PreformatedTables.productYear_X_Cumul:
                case CstFormat.PreformatedTables.productYear_X_Media:
                case CstFormat.PreformatedTables.productYear_X_Mensual:
                    return GetTableIndexClassif1_Year(data, nbLineToSchow);
                    break;
                default:
                    return base.GetTableIndex(data, nbLineToSchow);
                    break;
            }

        }

        /// <summary>
        /// Get the indexes table for table like GetTableIndexClassif1_Classif2XYear
        /// </summary>
        /// <param name="data">Data to treat</param>
        /// <param name="nbLineToSchow">Number of line to show</param>
        /// <returns>Indexes table</returns>
        protected int[] GetTableIndexClassif1_Classif2XYear(ResultTable data, long nbLineToSchow)
        {


            int[] tab;
            int nbLinesType = data.LinesStart.Count;

            tab = new int[nbLineToSchow + 1];

            List<LineType> mainTypes = new List<LineType>();
            List<int> mainTypeIndex = new List<int>();
            List<LineType> subTypes = new List<LineType>();
            List<int> subTypeIndex = new List<int>();

            #region SUB_TYPES
            List<LineType> SUB_TYPES = new List<LineType>();
            SUB_TYPES.Add(LineType.subTotal1);
            SUB_TYPES.Add(LineType.subTotal2);
            SUB_TYPES.Add(LineType.subTotal3);
            SUB_TYPES.Add(LineType.subTotal4);
            SUB_TYPES.Add(LineType.level5);
            SUB_TYPES.Add(LineType.level6);
            SUB_TYPES.Add(LineType.level7);
            SUB_TYPES.Add(LineType.level8);
            SUB_TYPES.Add(LineType.level9);
            #endregion

            LineType cLineType = LineType.nbParution;
            LineStart cLineStart = null;
            mainTypes.Add(LineType.header);
            mainTypeIndex.Add(0);
            int cLine = 1;

            for (int i = 0; i < data.LinesNumber; i++)
            {
                cLineStart = data.GetLineStart(i);
                cLineType = cLineStart.LineType;
                if (!(cLineStart is LineHide))
                {
                    if (SUB_TYPES.Contains(cLineType))
                    {                        
                        while (subTypes.Contains(cLineType))
                        {
                            subTypes.RemoveAt(subTypes.Count - 1);
                            subTypeIndex.RemoveAt(subTypeIndex.Count - 1);
                        }
                        subTypes.Add(cLineType);
                        subTypeIndex.Add(cLine);
                        if (subTypes.Count > 1)
                        {
                            tab[cLine] = subTypeIndex[subTypeIndex.Count-2];
                        }
                        else
                        {
                            tab[cLine] = mainTypeIndex[mainTypeIndex.Count - 1];
                        }
                    }
                    else
                    {
                        subTypes.Clear();
                        subTypeIndex.Clear();
                        while (mainTypes.Contains(cLineType))
                        {
                            mainTypes.RemoveAt(mainTypes.Count - 1);
                            mainTypeIndex.RemoveAt(mainTypeIndex.Count - 1);
                        }
                        mainTypes.Add(cLineType);
                        mainTypeIndex.Add(cLine);
                        if (mainTypes.Count > 1)
                        {
                            tab[cLine] = mainTypeIndex[mainTypeIndex.Count - 2];
                        }
                        else
                        {
                            tab[cLine] = 0;
                        }

                    }
                    cLine++;
                }
            }


            return tab;
        }

        /// <summary>
        /// Get the indexes table for table like Classif_Year
        /// </summary>
        /// <param name="data">Data to treat</param>
        /// <param name="nbLineToSchow">Number of line to show</param>
        /// <returns>Indexes table</returns>
        protected int[] GetTableIndexClassif1_Year(ResultTable data, long nbLineToSchow)
        {


            int[] tab;
            int nbLinesType = data.LinesStart.Count;

            tab = new int[nbLineToSchow + 1];

            List<LineType> mainTypes = new List<LineType>();
            List<int> mainTypeIndex = new List<int>();
            int parentIndex = -1;

            #region SUB_TYPES
            List<LineType> SUB_TYPES = new List<LineType>();
            SUB_TYPES.Add(LineType.subTotal1);
            SUB_TYPES.Add(LineType.subTotal2);
            SUB_TYPES.Add(LineType.subTotal3);
            SUB_TYPES.Add(LineType.subTotal4);
            SUB_TYPES.Add(LineType.level5);
            SUB_TYPES.Add(LineType.level6);
            SUB_TYPES.Add(LineType.level7);
            SUB_TYPES.Add(LineType.level8);
            SUB_TYPES.Add(LineType.level9);
            #endregion

            LineType cLineType = LineType.nbParution;
            LineStart cLineStart = null;
            mainTypes.Add(LineType.header);
            mainTypeIndex.Add(0);
            int cLine = 1;

            for (int i = 0; i < data.LinesNumber; i++)
            {
                cLineStart = data.GetLineStart(i);
                cLineType = cLineStart.LineType;
                if (!(cLineStart is LineHide))
                {
                    if (SUB_TYPES.Contains(cLineType))
                    {
                        if (parentIndex > -1)
                        {
                            tab[cLine] = parentIndex;
                        }
                        else
                        {
                            tab[cLine] = mainTypeIndex[mainTypeIndex.Count - 1];
                        }
                        mainTypeIndex[mainTypeIndex.Count - 1] = cLine;
                        parentIndex = cLine;
                    }
                    else
                    {
                        parentIndex = -1;
                        while (mainTypes.Contains(cLineType))
                        {
                            mainTypes.RemoveAt(mainTypes.Count - 1);
                            mainTypeIndex.RemoveAt(mainTypeIndex.Count - 1);
                        }
                        mainTypes.Add(cLineType);
                        mainTypeIndex.Add(cLine);
                        if (mainTypes.Count > 1)
                        {
                            tab[cLine] = mainTypeIndex[mainTypeIndex.Count - 2];
                        }
                        else
                        {
                            tab[cLine] = 0;
                        }

                    }
                    cLine++;
                }
            }


            return tab;
        }
        #endregion

    }
}
