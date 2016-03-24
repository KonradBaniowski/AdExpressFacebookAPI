using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public partial class OptionService
    {
        protected GenericDetailLevel _genericColumnDetailLevel = null;

        private GenericColumnDetailLevelOption GetGenericColumnLevelDetailOptions()
        {
            GenericColumnDetailLevelOption genericColumnDetailLevelOption = new GenericColumnDetailLevelOption();
            _genericColumnDetailLevel = _customerWebSession.GenericColumnDetailLevel;

            #region on vérifie que le niveau sélectionné à le droit d'être utilisé
            bool canAddDetail = false;
            try
            {
                canAddDetail = CanAddColumnDetailLevel(_customerWebSession.GenericColumnDetailLevel, _customerWebSession.CurrentModule);
            }
            catch { }
            if (!canAddDetail)
            {
                // Niveau de détail par défaut
                ArrayList levelsIds = new ArrayList();
                levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
                _customerWebSession.GenericColumnDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
            }
            #endregion

            #region Initialisation Niveau de détaille colonne
            if (_nbColumnDetailLevelItemList == 1)
            {
                ColumnDetailLevelItemInit(genericColumnDetailLevelOption);
            }
            #endregion

            if (_nbColumnDetailLevelItemList == 1 && _genericColumnDetailLevel.GetNbLevels == 1 && _genericColumnDetailLevel.LevelIds[0] != null)
            {
                genericColumnDetailLevelOption.L1Detail.SelectedId = ((DetailLevelItemInformation.Levels)_genericColumnDetailLevel.LevelIds[0]).GetHashCode().ToString();
            }

            return genericColumnDetailLevelOption;
        }

        private void SetGenericColumnLevelDetailOptions( UserFilter userFilter)
        {
            ArrayList levels = new ArrayList();

            if (_nbColumnDetailLevelItemList == 1  )
            {
                levels.Add(userFilter.GenericColumnDetailLevelFilter.L1DetailValue);
            }
            if (levels.Count > 0)
            {
                _genericColumnDetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
            }

            _customerWebSession.GenericColumnDetailLevel = _genericColumnDetailLevel;
        }

        protected void ColumnDetailLevelItemInit(GenericColumnDetailLevelOption genericColumnDetailLevelOption)
        {
            genericColumnDetailLevelOption = new GenericColumnDetailLevelOption();
            genericColumnDetailLevelOption.L1Detail.Id = "columnDetail";
            genericColumnDetailLevelOption.L1Detail.Items = new List<SelectItem>();          
            ArrayList AllowedColumnDetailLevelItems = GetAllowedColumnDetailLevelItems();
        }

        /// <summary>
        /// Retourne les éléments des niveaux de détail colonne autorisés
        /// </summary>
        /// <returns>Niveaux de détail colonne</returns>
        private ArrayList GetAllowedColumnDetailLevelItems()
        {

            List<DetailLevelItemInformation.Levels> vehicleAllowedDetailLevelList = GetVehicleAllowedDetailLevelItems();
            ArrayList allowedColumnDetailLevelList = _currentModule.AllowedColumnDetailLevelItems;
            ArrayList list = new ArrayList();

            List<DetailLevelItemInformation.Levels> vehicleAllowedColumnLevelList = GetVehicleAllowedColumnsLevelItems();

            foreach (DetailLevelItemInformation currentLevel in allowedColumnDetailLevelList)
                if (vehicleAllowedDetailLevelList.Contains(currentLevel.Id)
                    && vehicleAllowedColumnLevelList.Contains(currentLevel.Id))
                    list.Add(currentLevel);

            return list;

        }

        /// <summary>
        /// Return allowed column detail level list for vehicle list seleceted
        /// </summary>
        /// <returns>Detail level list</returns>
        private List<DetailLevelItemInformation.Levels> GetVehicleAllowedColumnsLevelItems()
        {

            List<Int64> vehicleList = new List<Int64>();
            string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (listStr != null && listStr.Length > 0)
            {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else
            {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }
            return VehiclesInformation.GetColumnsDetailLevelList(vehicleList);
        }

        /// <summary>
        /// Test si l'élément de niveau de détail peut être montré
        /// </summary>
        /// <param name="currentColumnDetailLevelItem">Elément de niveau de détail</param>
        /// <param name="module">Module</param>
        /// <returns>True si oui false sinon</returns>
        private bool CanAddDetailLevelItem(DetailLevelItemInformation currentColumnDetailLevelItem, Int64 module)
        {

            switch (currentColumnDetailLevelItem.Id)
            {
                case DetailLevelItemInformation.Levels.mediaSeller:
                    return (!_customerWebSession.isCompetitorAdvertiserSelected());
                default:
                    return (true);
            }

        }


        /// <summary>
        /// Test si un niveau de détail peut être montré
        /// </summary>
        /// <param name="currentColumnDetailLevel">Niveau de détail</param>
        /// <param name="module">Module courrant</param>
        /// <returns>True s'il peut être ajouté</returns>
        protected virtual bool CanAddColumnDetailLevel(GenericDetailLevel currentColumnDetailLevel, Int64 module)
        {
            ArrayList AllowedDetailLevelItems = GetAllowedColumnDetailLevelItems();
            foreach (DetailLevelItemInformation currentColumnDetailLevelItem in currentColumnDetailLevel.Levels)
            {
                if (!AllowedDetailLevelItems.Contains(currentColumnDetailLevelItem)) return (false);
                if (!CanAddDetailLevelItem(currentColumnDetailLevelItem, module)) return (false);
            }
            return (true);
        }

        /// <summary>
        /// Vérifie si le client à le droit de voir un détail produit dans les plan media
        /// </summary>
        /// <returns>True si oui false sinon</returns>
        private bool CheckProductDetailLevelAccess()
        {
            return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG));

        }
    }
}
