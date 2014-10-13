#region Informations
// Auteur:
// Création:
// Modification:
//		G. Facon		12/08/2005	Nom des méthodes

#endregion

using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.FrameWork;
using RightConstantes=TNS.AdExpress.Constantes.Customer.Right;
using TNS.Classification.Universe;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Fonctions des niveaux de détail produit
	/// </summary>
    public class ProductDetailLevel : TNS.AdExpress.Web.Core.Utilities.ProductDetailLevel
    {
        #region Identifiant des éléments de la nomenclature produit
        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        public static void SetProductLevel(WebSession webSession,int id, int level)
        {
            System.Windows.Forms.TreeNode tree = new System.Windows.Forms.TreeNode();
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = webSession.CustomerDataFilters.DataSource;
            param[1] = webSession.DataLanguage;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL levels = null;

            switch ((DetailLevelItemInformation.Levels)webSession.GenericProductDetailLevel.GetDetailLevelItemInformation(level))
            {
                case DetailLevelItemInformation.Levels.sector:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.sector, tree);
                    break;
                case DetailLevelItemInformation.Levels.subSector:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.subsector, tree);
                    break;
                case DetailLevelItemInformation.Levels.group:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.group, tree);
                    break;
                case DetailLevelItemInformation.Levels.segment:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.segment, tree);
                    break;
                case DetailLevelItemInformation.Levels.product:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.productAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.product, tree);
                    break;
                case DetailLevelItemInformation.Levels.advertiser:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.advertiser, tree);
                    break;
                case DetailLevelItemInformation.Levels.brand:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.brand, tree);
                    break;
                case DetailLevelItemInformation.Levels.holdingCompany:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.holding_company, tree);
                    break;
                case DetailLevelItemInformation.Levels.subBrand:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.subBrandAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subBrandAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.subBrand, tree);
                    break;
            }
        }
        #endregion
            
	}
}
