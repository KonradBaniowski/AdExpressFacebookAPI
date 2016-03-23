using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public partial class OptionService
    {

        public GenericColumnDetailLevelOption GetGenericColumnLevelDetailOptions( )
        {
            GenericColumnDetailLevelOption genericColumnDetailLevelOption = new GenericColumnDetailLevelOption();

            #region on vérifie que le niveau sélectionné à le droit d'être utilisé
            bool canAddDetail = false;
            try
            {
                canAddDetail = CanAddDetailLevel(_customerWebSession.GenericColumnDetailLevel, _customerWebSession.CurrentModule);
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

            return genericColumnDetailLevelOption;
        }

        protected void ColumnDetailLevelItemInit(GenericColumnDetailLevelOption genericColumnDetailLevelOption)
        {
            genericColumnDetailLevelOption = new GenericColumnDetailLevelOption();
            genericColumnDetailLevelOption.DefaultDetail.Id = "columnDetail_";
            genericColumnDetailLevelOption.DefaultDetail.Items = new List<SelectItem>();
            genericColumnDetailLevelOption.DefaultDetail.Items.Add(new SelectItem { Text = "-------", Value = "-1" });
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
    }
}
