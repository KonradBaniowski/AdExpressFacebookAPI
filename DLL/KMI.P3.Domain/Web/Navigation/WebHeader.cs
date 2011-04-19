using System;
using System.Collections.Generic;
using System.Text;

namespace KMI.P3.Domain.Web.Navigation
{
    /// <summary>
    /// Informations nécessaires au bon fonctionnement de l'entête.
    /// </summary>
    public class WebHeader
    {

        #region Variables
        ///<summary>
        /// Menus de l'entête(List de HeaderMenuItem)
        /// </summary>
        ///  <link>aggregation</link>
        ///  <supplierCardinality>0..*</supplierCardinality>
        ///  <associates>TNS.EasyMusic.Web.Core.Navigation.HeaderMenuItem</associates>
        ///  <label>_menuItems</label>
        protected List<WebHeaderMenuItem> _menuItems;
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public WebHeader()
        {
            _menuItems = new List<WebHeaderMenuItem>();
        }
        #endregion

        #region Accesseurs
        /// <summary>
        /// Get and set menuItems
        /// </summary>
        public List<WebHeaderMenuItem> MenuItems
        {
            get { return _menuItems; }
            set { _menuItems = value; }
        }
        #endregion

    }
}
