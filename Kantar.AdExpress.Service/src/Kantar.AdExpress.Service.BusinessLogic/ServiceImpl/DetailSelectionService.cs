using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain;
using KM.AdExpressI.MyAdExpress.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Classification.DAL;
using TNS.Classification.Universe;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using LS = TNS.Ares.Domain.LS;
using TNS.Ares.Alerts.DAL;
using TNSDomain = TNS.Alert.Domain;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using NLog;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class DetailSelectionService : IDetailSelectionService

    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public DetailSelectionResponse GetDetailSelection(string idWebSession)
        {
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            var result = LoadDetailsSelection(_webSession);
            return result;
        }
        public DetailSelectionResponse LoadSessionDetails(string idSession, string idWebSession)
        {
            Int64 idMySession = 0;
            DetailSelectionResponse result = new DetailSelectionResponse();
            if (!String.IsNullOrEmpty(idSession) && !String.IsNullOrWhiteSpace(idWebSession))
            {
                idMySession = Int64.Parse(idSession);
                var webSession = (WebSession)WebSession.Load(idWebSession);
                var webSessionSave = (WebSession)MyResultsDAL.GetResultMySession(idMySession.ToString(), webSession);
                result = LoadDetailsSelection(webSessionSave);
            }
            return result;
        }

        public DetailSelectionResponse LoadUniversDetails(string idUnivers, string idWebSession)
        {
            DetailSelectionResponse result = new DetailSelectionResponse();
            if (!String.IsNullOrEmpty(idUnivers) && !String.IsNullOrWhiteSpace(idWebSession))
            {
                long id = Int64.Parse(idUnivers);
                var webSession = (WebSession)WebSession.Load(idWebSession);
                Dictionary<int, AdExpressUniverse> adExpressUniverse = null;
                if (UniversListDataAccess.IsUniverseBelongToClientDescription(webSession, id, WebConstantes.LoadableUnivers.GENERIC_UNIVERSE))
                {
                    adExpressUniverse = (Dictionary<int, AdExpressUniverse>)UniversListDataAccess.GetObjectUniverses(id, webSession);
                    if (adExpressUniverse != null && adExpressUniverse.Count > 0)
                        result = GetUniversDetails(webSession, adExpressUniverse);
                }
            }

            return result;
        }


        public void LoadUniversAllDetails(string idWebSession)
        {
            DetailSelectionResponse result = new DetailSelectionResponse();
            int j = 0;
            int j2 = 0;

            var webSession = (WebSession)WebSession.Load(idWebSession);
            Dictionary<int, AdExpressUniverse> adExpressUniverse = null;

            List<long> ids = new List<long> { 16376, 16377, 16380, 16381, 16382, 16383, 16391, 16400, 16401, 16402, 16404, 16407, 16420, 16421, 16422, 16423, 16425, 16430, 16431, 16432, 16433, 16437, 16439, 16440, 16441, 16442, 16443, 16444, 16445, 16446, 16448, 16449, 16450, 16452, 16453, 16455, 16456, 16457, 16458, 16459, 16460, 16461, 16463, 16467, 16468, 16469, 16470, 16471, 16472, 16473, 16474, 16475, 16476, 16478, 16479, 16481, 16482, 16483, 16484, 16485, 16486, 16487, 16488, 16489, 16490, 16493, 16494, 16495, 16496, 16497, 16499, 16500, 16502, 16503, 16504, 16505, 16506, 16507, 16508, 16509, 16511, 16512, 16513, 16514, 16515, 16516, 16517, 16518, 16519, 16520, 16521, 16522, 16523, 16524, 16525, 16526, 16527, 16528, 16529, 16530, 16531, 16532, 16533, 16534, 16535, 16536, 16537, 16538, 16539, 16540, 16541, 16542, 16543, 16544, 16547, 16549, 16550, 16551, 16552, 16553, 16554, 16555, 16556, 16557, 16558, 16559, 16560, 16561, 16565, 16566, 16567, 16577, 16578, 16579, 16591, 16592, 16593, 16594, 16595, 16596, 16616, 16623, 16624, 16625, 16626, 16627, 16628, 16629, 16631, 16633, 16634, 16635, 16636, 16637, 16638, 16639, 16640, 16641, 16642, 16643, 16644, 16645, 16646, 16647, 16648, 16649, 16650, 16651, 16654, 16655, 16656, 16657, 16658, 16659, 16660, 16661, 16662, 16663, 16664, 16665, 16666, 16667, 16670, 16672, 16673, 16676, 16678, 16679, 16680, 16682, 16683, 16684, 16685, 16686, 16687, 16688, 16689, 16690, 16691, 16692, 16693, 16695, 16696, 16697, 16698, 16699, 16700, 16701, 16702, 16704, 16705, 16706, 16707, 16708, 16710, 16711, 16712, 16713, 16714, 16715, 16716, 16717, 16718, 16719, 16720, 16721, 16722, 16723, 16724, 16725, 16726, 16727, 16728, 16729, 16730, 16731, 16732, 16733, 16734, 16735, 16736, 16737, 16738, 16739, 16740, 16741, 16742, 16743, 16744, 16745, 16747, 16748, 16754, 16755, 16759, 16760, 16761, 16762, 16763, 16764, 16767, 16768, 16769, 16770, 16772, 16773, 16774, 16775, 16776, 16777, 16778, 16780, 16781, 16782, 16783, 16784, 16785, 16786, 16787, 16788, 16791, 16792, 16793, 16794, 16795, 16796, 16797, 16798, 16799, 16800, 16801, 16803, 16804, 16805, 16806, 16807, 16808, 16809, 16810, 16811, 16812, 16813, 16814, 16815, 16816, 16817, 16818, 16819, 16820, 16821, 16822, 16823, 16824, 16825, 16826, 16827, 16828, 16829, 16830, 16831, 16832, 16835, 16836, 16837, 16839, 16840, 16842, 16844, 16845, 16846, 16848, 16849, 16850, 16851, 16852, 16853, 16859, 16860, 16861, 16863, 16864, 16865, 16866, 16867, 16868, 16869, 16870, 16871, 16872, 16873, 16874, 16875, 16876, 16877, 16878, 16879, 16880, 16881, 16882, 16883, 16884, 16885, 16886, 16887, 16888, 16889, 16890, 16891, 16892, 16893, 16894, 16896, 16897, 16898, 16899, 16900, 16901, 16902, 16903, 16905, 16906, 16907, 16908, 16909, 16910, 16912, 16913, 16914, 16915, 16916, 16917, 16918, 16919, 16920, 16921, 16922, 16923, 16924, 16925, 16926, 16927, 16928, 16929, 16930, 16931, 16932, 16933, 16934, 16935, 16937, 16938, 16939, 16940, 16941, 16942, 16943, 16944, 16945, 16946, 16947, 16952, 16954, 16955, 16961, 16962, 16963, 16964, 16965, 16966, 16967, 16968, 16969, 16970, 16971, 16972, 16973, 16974, 16975, 16976, 16977, 16978, 16980, 16981, 16982, 16984, 16985, 16986, 16988, 16989, 16991, 16996, 16997, 16998, 16999, 17002, 17003, 17004, 17005, 17006, 17007, 17008, 17009, 17010, 17011, 17012, 17015, 17016, 17017, 17018, 17019, 17020, 17021, 17022, 17023, 17024, 17025, 17026, 17027, 17028, 17029, 17030, 17031, 17032, 17034, 17035, 17036, 17037, 17038, 17039, 17040, 17041, 17042, 17043, 17044, 17045, 17046, 17047, 17048, 17049, 17050, 17051, 17052, 17053, 17054, 17055, 17056, 17057, 17058, 17059, 17060, 17061, 17062, 17063, 17064, 17065, 17066, 17067, 17068, 17069, 17070, 17071, 17072, 17073, 17074, 17075, 17076, 17077, 17078, 17079, 17080, 17081, 17082, 17083, 17084, 17085, 17086, 17088, 17089, 17090, 17091, 17092, 17093, 17094, 17095, 17096, 17097, 17098, 17099, 17100, 17101, 17103, 17104, 17105, 17109, 17110, 17111, 17112, 17113, 17114, 17115, 17116, 17117, 17118, 17119, 17120, 17121, 17122, 17123, 17124, 17125, 17126, 17127, 17128, 17129, 17130, 17131, 17132, 17133, 17134, 17135, 17136, 17137, 17138, 17139, 17140, 17141, 17142, 17143, 17144, 17145, 17148, 17149, 17150, 17151, 17154, 17155, 17156, 17161, 17162, 17163, 17164, 17165, 17166, 17167, 17168, 17169, 17170, 17171, 17172, 17173, 17174, 17175, 17176, 17177, 17179, 17180, 17181, 17182, 17183, 17184, 17185, 17187, 17189, 17191, 17193, 17194, 17195, 17196, 17197, 17198, 17199, 17200, 17201, 17202, 17204, 17205, 17206, 17207, 17208, 17209, 17211, 17212, 17213, 17214, 17215, 17216, 17217, 17218, 17219, 17220, 17221, 17223, 17224, 17225, 17226, 17227, 17228, 17229, 17230, 17231, 17232, 17233, 17234, 17235, 17236, 17238, 17240, 17241, 17242, 17243, 17244, 17245, 17246, 17247, 17248, 17249, 17250, 17251, 17252, 17253, 17254, 17255, 17256, 17257, 17258, 17259, 17260, 17261, 17262, 17263, 17264, 17265, 17266, 17267, 17268, 17269, 17270, 17271, 17272, 17273, 17274, 17275, 17276, 17279, 17280, 17281, 17282, 17283, 17284, 17285, 17286, 17287, 17288, 17289, 17290, 17291, 17292, 17293, 17294, 17295, 17296, 17297, 17298, 17299, 17300, 17301, 17302, 17303, 17304, 17305, 17306, 17307, 17308, 17309, 17310, 17311, 17312, 17313, 17314, 17315, 17316, 17317, 17318, 17319, 17320, 17321, 17322, 17323, 17324, 17325, 17326, 17327, 17328, 17329, 17330, 17331, 17332, 17333, 17334, 17335, 17336, 17337, 17338, 17339, 17340, 17341, 17342, 17343, 17344, 17345, 17346, 17347, 17348, 17349, 17350, 17351, 17352, 17353, 17355, 17356, 17357, 17358, 17360, 17361, 17362, 17363, 17364, 17365, 17366, 17367, 17368, 17369, 17370, 17371, 17372, 17373, 17374, 17375, 17376, 17377, 17378, 17379, 17380, 17381, 17382, 17383, 17384, 17385, 17386, 17387, 17388, 17389, 17391, 17393, 17394, 17395, 17396, 17397, 17398, 17399, 17402, 17403, 17404, 17405, 17406, 17407, 17408, 17409, 17410, 17411, 17412 };

            foreach (long id in ids)
            {
                adExpressUniverse = (Dictionary<int, AdExpressUniverse>)UniversListDataAccess.GetObjectUniverses(id, webSession);
                if (adExpressUniverse.Count > 1)
                {
                    //adExpressUniverse[0].ElementsGroupDictionary.OrderBy(t => t).SequenceEqual(adExpressUniverse[1].ElementsGroupDictionary.OrderBy(t => t));

                    try
                    {
                        if (adExpressUniverse[0].ElementsGroupDictionary.Any() && adExpressUniverse[1].ElementsGroupDictionary.Any())
                        {

                            if (adExpressUniverse[1].ElementsGroupDictionary.ContainsKey(0))
                            {
                                if (adExpressUniverse[0].ElementsGroupDictionary[0].Equals(adExpressUniverse[1].ElementsGroupDictionary[0]))
                                    j++;
                            }
                            else
                            {

                                if (adExpressUniverse[0].ElementsGroupDictionary[0].Equals(adExpressUniverse[1].ElementsGroupDictionary[1]))
                                    j2++;

                                Console.WriteLine("Strange");
                            }


                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                }
            }

            Console.WriteLine(j);
        }


        public DetailSelectionResponse LoadAlertDetails(string id, string idWebSession)
        {
            DetailSelectionResponse result = new DetailSelectionResponse();
            if (!String.IsNullOrEmpty(id) && !String.IsNullOrWhiteSpace(idWebSession) && TNS.Alert.Domain.AlertConfiguration.IsActivated)
            {
                int idAlert = Int32.Parse(id);
                TNS.Ares.Domain.Layers.DataAccessLayer layer = LS.PluginConfiguration.GetDataAccessLayer(LS.PluginDataAccessLayerName.Alert);
                TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);
                TNSDomain.Alert alert = alertDAL.GetAlert(idAlert);
                var webSessionSave = (WebSession)alert.Session;
                result = LoadDetailsSelection(webSessionSave);
            }
            return result;
        }

        private DetailSelectionResponse LoadDetailsSelection(WebSession _webSession, bool showUnity = true, bool showStudyType = true)
        {
            var domain = new DetailSelectionResponse
            {
                ShowUnity = showUnity,
                ShowStudyType = showStudyType
            };

            domain.SiteLanguage = _webSession.SiteLanguage;

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = _webSession.Source;
            param[1] = _webSession.SiteLanguage;

            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            //TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL universeItems = null;

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
                    case WebConstantes.DetailSelection.Type.genericMediaLevelDetail:
                        domain.NiveauDetailLabel = _webSession.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage);
                        break;
                    case WebConstantes.DetailSelection.Type.genericProductLevelDetail:
                        domain.NiveauDetailLabel = _webSession.GenericProductDetailLevel.GetLabel(_webSession.SiteLanguage);
                        break;
                    case WebConstantes.DetailSelection.Type.genericColumnLevelDetail:
                        domain.GenericLevelDetailColumn = _webSession.GenericColumnDetailLevel.GetLabel(_webSession.SiteLanguage);
                        break;
                    default:
                        break;
                }
            }
            #endregion

            #region Niveaux de détail colonne générique
            //WebFunctions.MediaDetailLevel.GetGenericLevelDetailColumn(_webSession, ref displayGenericlevelDetailColumnLabel, genericlevelDetailColumnLabel, false);
            #endregion

            #region Unité :
            domain.UniteLabel = GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage);
            #endregion

            #region Univers Market :
            domain.UniversMarket = new List<Core.Domain.Tree>();
            ExtractTreeFromAdExpressUniverse(_webSession.PrincipalProductUniverses, domain.UniversMarket, factoryLevels, _webSession.SiteLanguage, _webSession.CurrentModule);
            #endregion

            #region Media : 
            domain.MediasSelected = GetLabels(_webSession.SelectionUniversMedia, _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
            domain.UniversMedia = new List<Core.Domain.Tree>();
            domain.MediasSelectedLabel = string.Join(",", domain.MediasSelected.Select(_ => _.Label));

            ExtractTreeFromAdExpressUniverse(_webSession.PrincipalMediaUniverses, domain.UniversMedia, factoryLevels, _webSession.SiteLanguage, _webSession.CurrentModule);
            #endregion

            #region Période sélectionnée :
            switch (_webSession.CurrentModule)
            {
                case WebConstantes.Module.Name.INDICATEUR:
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    domain.DateBegin = Dates.GetAnalysisDate(_webSession.PeriodBeginningDate, true);
                    domain.DateEnd = Dates.GetAnalysisDate(_webSession.PeriodBeginningDate, false);
                    break;
                default:
                    domain.DateBegin = !string.IsNullOrEmpty(_webSession.PeriodBeginningDate) ? Dates.YYYYMMDDToDD_MM_YYYY(_webSession.PeriodBeginningDate) : null;
                    domain.DateEnd = !string.IsNullOrEmpty(_webSession.PeriodEndDate) ? Dates.YYYYMMDDToDD_MM_YYYY(_webSession.PeriodEndDate) : null;
                    break;

            }

            if (Dates.isPeriodSet(domain.DateBegin, domain.DateEnd))
            {
                domain.Dates = Dates.GetPeriodDetail(_webSession);
            }
            #endregion

            #region StudyPeriod: 
            domain.StudyPeriod = ManageStudyPeriod(_webSession);
            #endregion

            #region ComparativePeriod:
            domain.ComparativePeriod = ManageComparativePeriod(_webSession);
            #endregion

            #region ComparativePeriodType:
            domain.ComparativePeriodType = ManageComparativePeriodType(_webSession);
            #endregion

            #region PeriodDisponibilityType:
            domain.PeriodDisponibilityType = ManagePeriodDisponibilityType(_webSession);
            #endregion

            #region defined
            domain.ShowDate = Dates.isPeriodSet(domain.DateBegin, domain.DateEnd);
            domain.ShowUnivers = !(domain.MediasSelected.Count == 0);
            domain.ShowUniversDetails = (!(domain.MediasSelected.Count == 0) && !(domain.UniversMedia.Count == 0));
            domain.ShowMarket = (domain.UniversMarket.Count > 0);
            domain.ShowGenericlevelDetail = !string.IsNullOrEmpty(domain.NiveauDetailLabel);
            domain.ShowGenericLevelDetailColumn = !string.IsNullOrEmpty(domain.GenericLevelDetailColumn);
            domain.ShowStudyPeriod = !string.IsNullOrEmpty(domain.StudyPeriod);
            domain.ShowComparativePeriod = !string.IsNullOrEmpty(domain.ComparativePeriod);
            domain.ShowComparativePeriodType = !string.IsNullOrEmpty(domain.ComparativePeriodType);
            domain.ShowPeriodDisponibilityType = !string.IsNullOrEmpty(domain.PeriodDisponibilityType);
            #endregion

            return domain;
        }

        private static void ExtractTreeFromAdExpressUniverse(Dictionary<int, AdExpressUniverse> Principal, List<Tree> treeDefined, ClassificationLevelListDALFactory factoryLevels, int SiteLanguage, long currentModule, bool defaultFcbUniverse = false)
        {
            List<long> itemIdList = null;

            foreach (KeyValuePair<int, AdExpressUniverse> kvp in Principal)
            {
                //if (Principal.Any())
                //{
                    //AdExpressUniverse adExpressUniverse = Principal[0];

                    foreach (KeyValuePair<long, NomenclatureElementsGroup> kvpElements in kvp.Value.ElementsGroupDictionary)
                    {

                        Core.Domain.Tree model = new Core.Domain.Tree();

                        List<long> levelIdsList = kvpElements.Value.GetLevelIdsList();
                        model.AccessType = kvpElements.Value.AccessType == AccessType.excludes ? AccessType.excludes : AccessType.includes;
                        if (model.AccessType == AccessType.excludes)
                        {
                            model.Label = GestionWeb.GetWebWord(2269, SiteLanguage); // 2269 = Exclure
                            model.LabelId = 2269;
                        }
                        else
                        {
                            model.Label = (kvpElements.Key == 0) ? GestionWeb.GetWebWord(3039, SiteLanguage) : GestionWeb.GetWebWord(2869, SiteLanguage); //3039 = Référents; 2869 = Concurrents
                            model.LabelId = (kvpElements.Key == 0) ? 3039 : 2869;
                        }
                        #region populate Universlevels
                        model.UniversLevels = new List<Core.Domain.UniversLevel>();
                        if (levelIdsList != null)
                        {
                            for (int j = 0; j < levelIdsList.Count; j++)
                            {
                                var universLevel = new UniversLevel();
                                universLevel.LabelId = UniverseLevels.Get(levelIdsList[j]).LabelId;
                                universLevel.UniversItems = new List<UniversItem>();
                                universLevel.Label = GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, SiteLanguage);
                                universLevel.Id = levelIdsList[j];
                                TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL universeItems =
                                    factoryLevels.CreateDefaultClassificationLevelListDAL(UniverseLevels.Get(levelIdsList[j]), kvpElements.Value.GetAsString(levelIdsList[j]));
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

                                            universItem.Id = itemIdList[z];
                                            universItem.IdLevelUniverse = levelIdsList[j];
                                        }
                                    }
                                }
                                model.UniversLevels.Add(universLevel);
                            }
                        }
                        #endregion
                        treeDefined.Add(model);
                    }
                //}

            }

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

        private string ManageStudyPeriod(WebSession webSession)
        {
            string StudyPeriod = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    StudyPeriod = Dates.GetStudyPeriodDetail(webSession, webSession.CurrentModule);
            }
            else if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                )
            {
                if (webSession.isStudyPeriodSelected())
                    StudyPeriod = Dates.GetStudyPeriodDetail(webSession, webSession.CurrentModule);
            }
            return StudyPeriod;
        }
        private string ManageComparativePeriod(WebSession webSession)
        {
            string ComparativePeriod = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    ComparativePeriod = Dates.GetComparativePeriodDetail(webSession, webSession.CurrentModule);
            }
            else if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
            {
                if (webSession.isPeriodComparative())
                    ComparativePeriod = Dates.GetComparativePeriodDetail(webSession, webSession.CurrentModule);
            }
            return ComparativePeriod;
        }

        private string ManageComparativePeriodType(WebSession webSession)
        {
            string ComparativePeriodType = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    ComparativePeriodType = Dates.GetComparativePeriodTypeDetail(webSession, webSession.CurrentModule);
            }
            else if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
            {
                if (webSession.isComparativePeriodTypeSelected())
                    ComparativePeriodType = Dates.GetComparativePeriodTypeDetail(webSession, webSession.CurrentModule);
            }
            return ComparativePeriodType;
        }

        private string ManagePeriodDisponibilityType(WebSession webSession)
        {
            string PeriodDisponibilityType = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
            {
                if (webSession.isPeriodDisponibilityTypeSelected())
                {
                    PeriodDisponibilityType = Dates.GetPeriodDisponibilityTypeDetail(webSession);
                }
            }
            return PeriodDisponibilityType;
        }
        private DetailSelectionResponse GetUniversDetails(WebSession webSession, Dictionary<int, AdExpressUniverse> adExpressUniverse)
        {
            var result = new DetailSelectionResponse();
            result.SiteLanguage = webSession.SiteLanguage;
            CoreLayer cl = WebApplicationParameters.CoreLayers[WebConstantes.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = webSession.Source;
            param[1] = webSession.SiteLanguage;
            ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            result.ModuleLabel = adExpressUniverse.FirstOrDefault().Value.UniverseDimension.ToString();
            switch (adExpressUniverse.FirstOrDefault().Value.UniverseDimension)
            {
                case Dimension.product:
                    result.UniversMarket = new List<Tree>();
                    result.ShowMarket = true;
                    result.ShowUniversDetails = true;
                    result.ModuleLabel = GestionWeb.GetWebWord(468, webSession.SiteLanguage);
                    ExtractTreeFromAdExpressUniverse(adExpressUniverse, result.UniversMarket, factoryLevels, webSession.SiteLanguage, webSession.CurrentModule);
                    break;
                case Dimension.media:
                    //TODO //result.MediasSelected = GetLabels(webSession.SelectionUniversMedia, webSession.SiteLanguage, webSession.DataLanguage, webSession.CustomerDataFilters.DataSource);
                    //TODO //result.MediasSelectedLabel = string.Join(",", result.MediasSelected.Select(p => p.Label));
                    result.UniversMedia = new List<Tree>();
                    result.ShowUnivers = true;
                    result.ShowUniversDetails = true;
                    result.ModuleLabel = GestionWeb.GetWebWord(363, webSession.SiteLanguage);
                    ExtractTreeFromAdExpressUniverse(adExpressUniverse, result.UniversMedia, factoryLevels, webSession.SiteLanguage, webSession.CurrentModule);
                    break;
                default:
                    return result;
            }
            return result;
        }

        public List<Tree> GetMarket(string idWebSession)
        {
            var _webSession = (WebSession)WebSession.Load(idWebSession);

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = _webSession.Source;
            param[1] = _webSession.SiteLanguage;

            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            var UniversMarket = new List<Core.Domain.Tree>();

            ExtractTreeFromAdExpressUniverse(_webSession.PrincipalProductUniverses, UniversMarket, factoryLevels, _webSession.SiteLanguage, _webSession.CurrentModule);

            return UniversMarket;
        }
    }
}
