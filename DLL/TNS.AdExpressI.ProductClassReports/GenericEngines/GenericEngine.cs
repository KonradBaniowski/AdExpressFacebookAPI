#region Information
/*
 * Author : G Ragneau
 * Created on : 12/01/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Classification.Universe;

using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpressI.ProductClassReports.DAL;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Level;
using CstClassif = TNS.AdExpress.Constantes.Classification;



namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{
    /// <summary>
    /// Define default behaviours of engines like global process, lengendaries, properties...
    /// </summary>
    public abstract class GenericEngine
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Type of result
        /// </summary>
        protected int _tableType;
        /// <summary>
        /// Type of output
        /// </summary>
        protected bool _excel = false;

        #region Rules Engine attributes
        /// <summary>
        /// Specify if the table contains advertisers as references or competitors
        /// </summary>
        protected int _isPersonalized = 0;

        #endregion

        /// <summary>
        /// Report vehicle
        /// </summary>
        protected Vehicles.names _vehicle;

        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Type of result
        /// </summary>
        protected int TableType
        {
            get { return _tableType; }
            set { _tableType = value; }
        }
        /// <summary>
        /// Type of output
        /// </summary>
        public bool Excel
        {
            get { return _excel; }
            set { _excel = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Type of table</param>
        public GenericEngine(WebSession session, int result)
        {
            _session = session;
            _tableType = result;
            _vehicle = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
        }
        #endregion

        #region Access Points
        /// <summary>
        /// Define default process to build a result
        /// </summary>
        /// <returns>Data Result</returns>
        public virtual ResultTable GetResult()
        {

            #region DAL Access
            DataSet data = null;
            try
            {
                data = GetData();
            }
            catch (DeliveryFrequencyException)
            {
                return GetUnvalidFrequencyDelivery();
            }
            catch (NoDataException)
            {
                return GetNoData();
            }
            catch (Exception e) { throw new ProductClassReportsException("Error while retrieving data for database.", e); }
            #endregion

            #region Compute data
            try
            {
                return ComputeData(data);
            }
            catch (DeliveryFrequencyException)
            {
                return GetUnvalidFrequencyDelivery();
            }
            catch (System.Exception e)
            {
                throw (new ProductClassReportsException(string.Format("Unable to compute data for product class report {0} ", _tableType), e));
            }
            #endregion

            return null;

        }
        #endregion

        #region Table engine
        /// <summary>
        /// Access to the DAL layer to get data required for the report
        /// </summary>
        /// <returns>DataSet loaded with data required by the report</returns>
        protected virtual DataSet GetData()
        {
            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_session.CurrentModule);
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("DataAccess layer is null for the Product Class Analysis"));
            object[] param = new object[1];
            param[0] = _session;
            IProductClassReportsDAL productClassReportDAL = (IProductClassReportsDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryDataAccessLayer.AssemblyName, module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
            return productClassReportDAL.GetData();
        }
        /// <summary>
        /// Compute data got from the DAL layer
        /// </summary>
        /// <param name="data">DAL data</param>
        /// <returns>Table of computed data</returns>
        protected abstract ResultTable ComputeData(DataSet data);
        #endregion

        #region Common methods about UI

        #region No Data
        /// <summary>
        /// Append message meaning "No data for the selected univers"
        /// </summary>
        protected virtual ResultTable GetNoData()
        {
            return null;
            //str.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\"><br><br>{0}<br><br><br></div>", GestionWeb.GetWebWord(177, _session.SiteLanguage));
        }
        #endregion

        #region Unvalid frequency
        /// <summary>
        /// Append message meaning "Frequency for data delivering is not valid"
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Code HTML</returns>
        protected virtual ResultTable GetUnvalidFrequencyDelivery()
        {
            return null;
            //str.AppendFormat("<div align=\"center\" class=\"txtViolet11Bold\"><br><br>{0}<br><br><br></div>", GestionWeb.GetWebWord(1234, _session.SiteLanguage));
        }
        #endregion

        #endregion

        #region Common Methods about rules
        /// <summary>
        /// Method to clean data from empty lines
        /// </summary>
        /// <param name="dsData">Dataset from database access</param>
        /// <param name="firstData">First column with data</param>
        /// <returns>DataTable without empty lines</returns>
        protected void CleanDataTable(DataTable dtData, int firstData)
        {

            int maxColumn = dtData.Columns.Count - _isPersonalized;
            bool hasData = false;

            for (int i = dtData.Rows.Count - 1; i >= 0; i--)
            {

                hasData = false;
                for (int j = firstData; j < maxColumn; j++)
                {

                    if (Convert.ToDouble(dtData.Rows[i][j]) != 0)
                    {
                        hasData = true;
                        break;
                    }

                }
                if (!hasData)
                {
                    dtData.Rows.RemoveAt(i);
                }

            }

        }
        #endregion

        #region SetPersoAdvertiser
        protected virtual void SetPersoAdvertiser(ProductClassResultTable tab, Int32 cLine, DataRow row, DetailLevelItemInformation.Levels level)
        {
            ProductClassLineStart ls = (ProductClassLineStart)tab[cLine, 0];

            switch (level)
            {
                case DetailLevelItemInformation.Levels.advertiser:
                case DetailLevelItemInformation.Levels.product:
                case DetailLevelItemInformation.Levels.brand:
                    DisplayPerso(ls, row, level);
                    break;
                default:
                    SetUniversType(ls, row);
                    break;
            }
        }

        protected virtual void SetPersoAdvertiser(ProductClassResultTable tab, Int32 cLine, DataRow row, DetailLevelItemInformation.Levels level, CstClassif.Branch.type branchType)
        {
            if (branchType == CstClassif.Branch.type.product)
            {
                SetPersoAdvertiser(tab, cLine, row, level);
            }
            else
            {
                ProductClassLineStart ls = (ProductClassLineStart)tab[cLine, 0];
                SetUniversType(ls, row);
            }
        }
        #endregion

        #region DisplayPerso
        protected virtual void DisplayPerso(ProductClassLineStart ls, DataRow row, DetailLevelItemInformation.Levels level)
        {
            NomenclatureElementsGroup refElts = null;
            switch (level)
            {
                case DetailLevelItemInformation.Levels.advertiser:

                    if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0))
                    {
                        refElts = _session.SecondaryProductUniverses[0].GetGroup(0);
                        //Advertisers references
                        if (refElts != null && refElts.Count() > 0 && refElts.Contains(TNSClassificationLevels.ADVERTISER))
                        {
                            if (refElts.Get(TNSClassificationLevels.ADVERTISER).Contains(long.Parse(row["id_p" + GetClassifIdFieldIndex().ToString()].ToString())))
                            {
                                if (Convert.ToInt32(row["inref"]) > 0)
                                {
                                    ls.SetUniversType(UniversType.reference);
                                }
                                ls.DisplayPerso = true; break;
                            }
                        }
                    }
                    if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1))
                    {
                        refElts = _session.SecondaryProductUniverses[1].GetGroup(0);
                        //Competing Advertisers 
                        if (refElts != null && refElts.Count() > 0 && refElts.Contains(TNSClassificationLevels.ADVERTISER))
                        {
                            if (refElts.Get(TNSClassificationLevels.ADVERTISER).Contains(long.Parse(row["id_p" + GetClassifIdFieldIndex().ToString()].ToString())))
                            {
                                if (Convert.ToInt32(row["incomp"]) > 0)
                                {
                                    ls.SetUniversType(UniversType.concurrent);
                                }
                                ls.DisplayPerso = true; break;
                            }
                        }

                    }
                    if (Convert.ToInt32(row["inneutral"]) > 0)
                    {
                        ls.SetUniversType(UniversType.neutral);
                    }
                    break;
                default:
                    SetUniversType(ls, row);
                    ls.DisplayPerso = true;
                    break;
            }
        }

        #endregion

        #region SetUniversType
        protected virtual void SetUniversType(ProductClassLineStart ls, DataRow row)
        {
            if (Convert.ToInt32(row["inref"]) > 0)
            {
                ls.SetUniversType(UniversType.reference);
            }
            if (Convert.ToInt32(row["incomp"]) > 0)
            {
                ls.SetUniversType(UniversType.concurrent);
            }
            if (Convert.ToInt32(row["inneutral"]) > 0)
            {
                ls.SetUniversType(UniversType.neutral);
            }
        }

        #endregion

        #region GetClassifIdFieldIndex
        protected virtual int GetClassifIdFieldIndex()
        {
            int index = -1;
            List<DetailLevelItemInformation> levels = DetailLevelItemsInformation.Translate(_session.PreformatedProductDetail);
            for (int i = 0; i < levels.Count; i++)
            {
                if (levels[i].Id == DetailLevelItemInformation.Levels.advertiser) return (i + 1);

            }
            return index;
        }

        #endregion

        /// <summary>
        /// Check if children of current level are all neutral 
        /// </summary>
        /// <param name="tab">result table</param>
        /// <param name="i">index of current line</param>
        /// <returns>True if all children of current level are neutral </returns>
        protected virtual bool ChildrenAreNeutral(ProductClassResultTable tab, int i)
        {
            if (tab != null && tab[i, 1] != null && tab[i, 1] is CellLevel)
            {
                CellLevel cParent = (CellLevel)tab[i, 1];
                for (int j = i + 1; j < tab.LinesNumber; j++)
                {
                    CellLevel cChild = (CellLevel)tab[j, 1];
                    ProductClassLineStart ls = (ProductClassLineStart)tab[j, 0];
                    if (cChild != null && ls != null)
                    {
                        if (!cParent.Equals(cChild.ParentLevel)) return true;
                        if (ls.LineUnivers != UniversType.neutral) return false;
                    }
                }

            }
            return true;
        }

        #region HideNonCustomisedLines

        /// <summary>
        /// Hide Non CustomisedLines
        /// </summary>
        /// <param name="tab">result table</param>
        /// <param name="lTypes">levels type</param>
        /// <param name="lSubTypes">sub level types</param>
        protected virtual void HideNonCustomisedLines(ProductClassResultTable tab, List<LineType> lTypes, List<LineType> lSubTypes)
        {
            List<int> lineIndexRange = null;
            int nextLine = -1;
            ProductClassLineStart ls = null, ls2 = null;
            bool hasCustomisedChildren = false;
            bool showTotalLines = false;

            for (int i = 0; i < tab.LinesNumber; i++)
            {
                ls = (ProductClassLineStart)tab[i, 0];
                if (ls.LineUnivers == UniversType.neutral)
                {
                    if (lTypes.Contains(ls.LineType))
                    {
                        //Get range of lines for current level
                        lineIndexRange = new List<int>();
                        lineIndexRange.Add(i);
                        nextLine = i + 1;
                        for (int j = i + 1; j < tab.LinesNumber; j++)
                        {
                            ls2 = (ProductClassLineStart)tab[j, 0];
                            if (lSubTypes.Contains(ls2.LineType))
                            {
                                lineIndexRange.Add(j);
                            }
                            else
                            {
                                nextLine = j;
                                break;
                            }
                        }
                        //Check if current level has customised children 
                        for (int k = nextLine; k < tab.LinesNumber; k++)
                        {
                            ls2 = (ProductClassLineStart)tab[k, 0];
                            if (lTypes.Contains(ls2.LineType) && lTypes.IndexOf(ls2.LineType) > lTypes.IndexOf(ls.LineType))
                            {
                                if (ls2.LineUnivers != UniversType.neutral)
                                {
                                    hasCustomisedChildren = true;
                                    showTotalLines = true;
                                    break;
                                }
                            }
                            else if (lTypes.Contains(ls2.LineType) && lTypes.IndexOf(ls2.LineType) <= lTypes.IndexOf(ls.LineType))
                            {
                                break;
                            }
                        }
                        //Non hide current level if has customised children then go to next level
                        if (!hasCustomisedChildren)
                        {
                            for (int n = 0; n < lineIndexRange.Count; n++)
                            {
                                ls2 = (ProductClassLineStart)tab[lineIndexRange[n], 0];
                                if (ls2.LineUnivers == UniversType.neutral)
                                    tab.SetLineStart(new LineHide(ls.LineType), lineIndexRange[n]);
                            }
                        }
                        i = (lineIndexRange != null && lineIndexRange.Count > 0) ? lineIndexRange[lineIndexRange.Count - 1] : nextLine;
                    }
                }
                else
                {
                    showTotalLines = true;
                }
                hasCustomisedChildren = false;
            }
            //Hide total lines if required
            if (!showTotalLines)
            {     List<LineType> totalTypes = new List<LineType> { LineType.total, LineType.subTotal1, LineType.subTotal2, LineType.subTotal3, LineType.subTotal4};

                for (int i = 0; i < tab.LinesNumber; i++)
                {                    
                    if (tab[i, 0] is ProductClassLineStart && totalTypes.Contains(((ProductClassLineStart)tab[i, 0]).LineType))                    
                        tab.SetLineStart(new LineHide(ls.LineType), i);                    
                    else break;
                }
            }
        }
        #endregion
    }
}
