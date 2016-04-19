using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Classification.DAL;
using TNS.Classification.Universe;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class DetailSelectionService : IDetailSelectionService
    {
        public DetailSelectionResponse GetDetailSelection(string idWebSession)
        {
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            var domain = new DetailSelectionResponse();

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = _webSession.Source;
            param[1] = _webSession.SiteLanguage;

            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL universeItems = null;

            #region Choix de l'étude :
            domain.ModuleLabel = GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_webSession.CurrentModule), _webSession.SiteLanguage);
            #endregion

            #region Niveau détaillés par :
            ArrayList detailSelections = null;
            TNS.AdExpress.Domain.Web.Navigation.Module currentModule = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
            try
            {
                detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
            }
            catch (System.Exception)
            {
                if (currentModule.Id == WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
                    detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
            }
            foreach (int currentType in detailSelections)
            {
                switch ((WebConstantes.DetailSelection.Type)currentType)
                {
                    //case WebConstantes.DetailSelection.Type.genericMediaLevelDetail:
                    //    domain.NiveauDetailLabel = (excel) ? Convertion.ToHtmlString(_webSession.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage))
                    //        : _webSession.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage);
                    //    break;
                    //case WebConstantes.DetailSelection.Type.genericProductLevelDetail:
                    //    domain.NiveauDetailLabel = (excel) ? Convertion.ToHtmlString(_webSession.GenericProductDetailLevel.GetLabel(_webSession.SiteLanguage))
                    //        : _webSession.GenericProductDetailLevel.GetLabel(_webSession.SiteLanguage);
                    //    break;
                    case WebConstantes.DetailSelection.Type.genericMediaLevelDetail:
                        domain.NiveauDetailLabel = _webSession.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage);
                        break;
                    case WebConstantes.DetailSelection.Type.genericProductLevelDetail:
                        domain.NiveauDetailLabel = _webSession.GenericProductDetailLevel.GetLabel(_webSession.SiteLanguage);
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Unité :
            domain.UniteLabel = GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage);
            #endregion

            #region Univers Produit :
            domain.UniversMarche = new List<Core.Domain.Tree>();
            List<long> itemIdList = null;
            for (int k = 0; k < _webSession.PrincipalProductUniverses.Count; k++)
            {
                #region IF ? 
                //if (_webSession.PrincipalProductUniverses.Count > 1)
                //{
                //    if (_webSession.PrincipalProductUniverses.ContainsKey(k))
                //    {
                //        if (k > 0)
                //        {
                //            nameProduct = GestionWeb.GetWebWord(2301, _webSession.SiteLanguage);
                //        }
                //        else {
                //            nameProduct = GestionWeb.GetWebWord(2302, _webSession.SiteLanguage);
                //        }

                //        t.Append("<TR>");
                //        t.Append("<TD class=\"txtViolet11Bold\" >&nbsp;");
                //        t.Append("<label>" + nameProduct + "</label></TD>");
                //        t.Append("</TR>");

                //        // Universe Label
                //        if (_webSession.PrincipalProductUniverses[k].Label != null && _webSession.PrincipalProductUniverses[k].Label.Length > 0)
                //        {
                //            t.Append("<TR>");
                //            t.Append("<TD class=\"txtViolet11Bold\" >&nbsp;");
                //            t.Append("<Label>" + _webSession.PrincipalProductUniverses[k].Label + "</Label>");
                //            t.Append("</TD></TR>");
                //        }

                //        // Render universe html code
                //        t.Append("<TR height=\"20\">");
                //        t.Append("<TD vAlign=\"top\">" + selectItemsInClassificationWebControl.ShowUniverse(_webSession.PrincipalProductUniverses[k], _webSession.DataLanguage, DBFunctions.GetDataSource(_webSession)) + "</TD>");
                //        t.Append("</TR>");
                //        t.Append("<TR height=\"10\"><TD></TD></TR>");
                //    }
                //}
                #endregion
                //else {
                if (_webSession.PrincipalProductUniverses.ContainsKey(k))
                {
                    var result = _webSession.PrincipalProductUniverses[k].ElementsGroupDictionary;
                    if (result != null && result.Count > 0)
                    {
                        for (int i = 0; i < result.Count; i++)
                        {
                            var model = new Core.Domain.Tree();
                            List<long> levelIdsList = result[i].GetLevelIdsList();
                            model.AccessType = result[i].AccessType == AccessType.excludes ? AccessType.excludes : AccessType.includes;
                            if (model.AccessType == AccessType.excludes)
                            {
                                model.LabelId = 2270;
                            }
                            else
                            {
                                model.LabelId = 2269;
                            }
                            model.UniversLevels = new List<Core.Domain.UniversLevel>();
                            if (levelIdsList != null)
                            {
                                for (int j = 0; j < levelIdsList.Count; j++)
                                {
                                    //MAPPING
                                    var universLevel = new UniversLevel();
                                    universLevel.LabelId = UniverseLevels.Get(levelIdsList[j]).LabelId;
                                    universLevel.UniversItems = new List<UniversItem>();
                                    universeItems = factoryLevels.CreateDefaultClassificationLevelListDAL(UniverseLevels.Get(levelIdsList[j]), result[i].GetAsString(levelIdsList[j]));
                                    if (universeItems != null)
                                    {
                                        itemIdList = universeItems.IdListOrderByClassificationItem;
                                        if (itemIdList != null && itemIdList.Count > 0)
                                        {
                                            for (int z = 0; z < itemIdList.Count; z++)
                                            {
                                                var universItem = new UniversItem();
                                                universItem.Label = universeItems[itemIdList[z]];
                                                universLevel.UniversItems.Add(universItem);
                                            }
                                        }
                                    }
                                    model.UniversLevels.Add(universLevel);
                                }
                            }
                            domain.UniversMarche.Add(model);
                        }
                    }
                }
                //}
            }
            #endregion

            #region Media : 
            domain.MediasSelected = GetLabels(_webSession.SelectionUniversMedia, _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
            #endregion

            #region Période sélectionnée :
            domain.DateBegin = !string.IsNullOrEmpty(_webSession.PeriodBeginningDate) ? Dates.YYYYMMDDToDD_MM_YYYY(_webSession.PeriodBeginningDate) : null;
            domain.DateEnd = !string.IsNullOrEmpty(_webSession.PeriodEndDate) ? Dates.YYYYMMDDToDD_MM_YYYY(_webSession.PeriodEndDate) : null;
            if (Dates.isPeriodSet(domain.DateBegin, domain.DateEnd))
            {
                domain.Dates = Dates.GetPeriodDetail(_webSession);
            }
            #endregion

            return domain;
        }

        private List<TextData> GetLabels(System.Windows.Forms.TreeNode elem, int idLanguage, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source)
        {
            var levelInformations = elem.Nodes.Cast<System.Windows.Forms.TreeNode>().Select(_ => _.Tag).Cast<LevelInformation>().ToList();
            if (levelInformations.Count == 0)
                return new List<TextData>();

            var searchTodo = levelInformations.GroupBy(_ => _.Type, _ => _.ID).ToDictionary(e => e.Key, e => e.Distinct().ToList());
            var key = searchTodo.Keys.SingleOrDefault();

            var cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            object[] param = new object[2];
            param[0] = source;
            param[1] = dataLanguage;
            var factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            var dal = factoryLevels.CreateClassificationLevelListDAL(key, string.Join(",", searchTodo[key]));

            return searchTodo[key].Select(_ => new TextData { Id = _, Label = dal[_] }).ToList();
        }
    }
}
