using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Core {
    /// <summary>
    /// This class provides all properties to filter AdExpress queries :
    /// - Filter with classification rights items (products and media classification brand)
    /// - Filter with classification selected items (products and media classification brand)
    /// - Filter with customer period selected
    /// - Filter with inset types
    /// - Filter with unit selected
    /// </summary>
	public class CustomerDataFilters {
		
		#region Variables
		/// <summary>
		/// Customer session
		/// </summary>
		WebSession _customerSession = null;
		/// <summary>
		/// Current module Descritpion
		/// </summary>
		Module _currentModule = null;
        /// <summary>
        /// Customer's Media rights
        /// </summary>
       protected  Dictionary<CstCustomer.Right.type, string> _mediaRights = null;
       /// <summary>
       ///  Customer's Products rights
       /// </summary>
       protected Dictionary<CstCustomer.Right.type, string> _productsRights = null;
       /// <summary>
       /// Get Identifier list inset type
       /// </summary>
       /// <returns>ID list of insets</returns>
        protected string _insets = "";
        /// <summary>
        /// Universe products selected by the customer
        /// </summary>
        TNS.AdExpress.Classification.AdExpressUniverse _selectedProducts = null;
        /// <summary>
        /// selected media category
        /// </summary>
        protected string _selectedMediaCategory = "";
        /// <summary>
        /// Vehicles selected by the customer
        /// </summary>
        public Dictionary<CstCustomer.Right.type, string> _selectedVehicles = null;
          
		#endregion

        #region Properties
       /// <summary>
       /// Get customer vehicle classification rights. It provides all the identifiers of vehicle 
       /// classiffication's items allowed for the customer.
       /// Return a dictionary which key corresponds to classification level type and value the identifiers list
       /// of classification level.
       /// <example>
       /// To apply the rights in Sql query :
       /// <code>
       /// ...
       /// Dictionary[TNS.AdExpress.Constantes.Customer.Right.type,string] dic = customerSession.CustomerDataFilters.MediaRights;
       /// bool fisrt = true, beginByAnd=true;
       /// 
       /// // Get the medias authorized for the current customer
       ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.vehicleAccess) && dic[CustomerRightConstante.type.vehicleAccess].Length > 0)
       ///   {
       ///		if (beginByAnd) sql += " and";
       ///		sql += " (( id_vehicle in (" + dic[CustomerRightConstante.type.vehicleAccess] + ") ";
       ///       fisrt = false;
       ///	}   
       ///	 // Get the sub medias authorized for the current customer            
       ///     if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.categoryAccess) && dic[CustomerRightConstante.type.categoryAccess].Length > 0)
       ///   {
       ///       if (!fisrt) sql += " or";
       ///		else {
       ///			if (beginByAnd) sql += " and";
       ///			sql += " ((";
       ///		}
       ///        sql += " id_category in (" + dic[CustomerRightConstante.type.categoryAccess] + ") ";
       ///        fisrt = false;
       ///	}
       ///	// Get the medias not authorized for the current customer        
       ///      if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.vehicleException) && dic[CustomerRightConstante.type.vehicleException].Length > 0)
       ///    {
       ///         if (!fisrt) sql += " and";
       /// 		else {
       /// 			if (beginByAnd) sql += " and";
       /// 			sql += " (";
       /// 		}
       ///         sql += " id_vehicle not in (" + dic[CustomerRightConstante.type.vehicleException] + ") ";
       ///         fisrt = false;
       /// 	}
       /// 
       /// 
       ///  // Get the sub medias not authorized for the current customer
       ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.categoryException) && dic[CustomerRightConstante.type.categoryException].Length > 0)
       ///    {
       ///       if (!fisrt) sql += " and";
       ///		else {
       ///			if (beginByAnd) sql += " and";
       ///			sql += " (";
       ///		}
       ///        sql += " " + categoryTable.Prefix + ".id_category not in (" + _session.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ";
       ///       fisrt = false;
       ///	}
       /// ...
       ///  if (!fisrt) sql += " )";
       /// </code>
       /// </example>
       /// </summary>       
        public Dictionary<CstCustomer.Right.type, string> MediaRights
        {
            get
            {
                if (_mediaRights == null) _mediaRights = GetMediaRights();
                return _mediaRights;
            }

        }

        /// <summary>
        /// Get the product classification items rights of the customer from current module.
        /// Return a dictionary which key corresponds to classification level type and value the identifiers list
        /// of classification level.
        /// <example>
        /// To apply the product selected  in Sql query :
        /// <code>
        /// ...
        /// Dictionary[TNS.AdExpress.Constantes.Customer.Right.type,string] dic = customerSession.CustomerDataFilters.ProductsRights;
        /// bool fisrt = true, beginByAnd=true;
        /// 
        /// // Get the categories in access for the current customer
        ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.sectorAccess) && dic[CustomerRightConstante.type.sectorAccess].Length > 0)
        ///   {
        ///		if (beginByAnd) sql += " and";
        ///		sql += " (( id_sector in (" + dic[CustomerRightConstante.type.sectorAccess] + ") ";
        ///       fisrt = false;
        ///	}   
        ///	  // Get the sub catergories in access for the current customer            
        ///     if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.subSectorAccess) && dic[CustomerRightConstante.type.subSectorAccess].Length > 0)
        ///   {
        ///       if (!fisrt) sql += " or";
        ///		else {
        ///			if (beginByAnd) sql += " and";
        ///			sql += " ((";
        ///		}
        ///        sql += " id_subsector in (" + dic[CustomerRightConstante.type.subSectorAccess] + ") ";
        ///        fisrt = false;
        ///	}
        ///	// Get the categories in exceptionfor the current customer        
        ///      if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.sectorAccess) && dic[CustomerRightConstante.type.sectorAccess].Length > 0)
        ///    {
        ///         if (!fisrt) sql += " and";
        /// 		else {
        /// 			if (beginByAnd) sql += " and";
        /// 			sql += " (";
        /// 		}
        ///         sql += " id_sector not in (" + dic[CustomerRightConstante.type.sectorAccess] + ") ";
        ///         fisrt = false;
        /// 	}
        /// 
        /// 
        ///  // Get the sub categories not authorized for the current customer
        ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.subSectorAccess) && dic[CustomerRightConstante.type.subSectorAccess].Length > 0)
        ///    {
        ///       if (!fisrt) sql += " and";
        ///		else {
        ///			if (beginByAnd) sql += " and";
        ///			sql += " (";
        ///		}
        ///        sql += " id_subsector not in (" + _session.CustomerLogin[CustomerRightConstante.type.subSectorAccess] + ") ";
        ///       fisrt = false;
        ///	}
        /// ...
        ///  if (!fisrt) sql += " )";
        /// </code>
        /// </example>
        /// </summary>
        public Dictionary<CstCustomer.Right.type, string> ProductsRights
        {
            get
            {
                if (_productsRights == null) _productsRights = GetProductsRights();
                return _productsRights;
            }

        }

        /// <summary>
        /// Get allowed media universe according to the Module restrictions. The property return then object
        /// "MediaItemsList" which contains the following properties :
        /// - VehicleList :  Get the list of medias type (ex. 1,2,3 =>PRESS,RADIO,TV)
        /// - CategoryList :  Get the list of media category (ex. 45,562,32) 
        /// - MediaList : Get the list of vehicles (ex. 489,8,563) 
        /// </summary>
        /// <returns>Media items List</returns>
        public MediaItemsList AllowedMediaUniverse
        {  get
            {
                return _currentModule.AllowedMediaUniverse;
            }
        }

        /// <summary>
        /// Get beginning date selected by the customer 
        /// </summary>
        /// <returns>Date string</returns>
        public string BeginningDate
        {
            get
            {
                return _customerSession.CustomerPeriodSelected.StartDate;
            }
        }
        /// <summary>
        /// Get end date selected by the customer 
        /// </summary>
        /// <returns>Date string</returns>
        public string EndDate
        {
            get
            {
                return _customerSession.CustomerPeriodSelected.EndDate;
            }
        }

        /// <summary>
        /// Get Identifier list inset type
        /// </summary>
        /// <returns>ID list of insets</returns>
        public string InsetTypesAsString
        {
            get
            {
                if (string.IsNullOrEmpty(_insets))
                {
                    Dictionary<CstWeb.CustomerSessions.InsertType, long> col = WebApplicationParameters.InsetTypeCollection;
                    if (col != null && col.Count > 0)
                    {
                        foreach (KeyValuePair<CstWeb.CustomerSessions.InsertType, long> kpv in col)
                        {
                            _insets += kpv.Value + ",";
                        }
                        if (!string.IsNullOrEmpty(_insets)) _insets = _insets.Substring(0, _insets.Length - 1);
                    }
                }
                return _insets;
            }
        }

        /// <summary>
        /// Get Identifier  list of inset type
        /// </summary>
        /// <returns>Dictionary  of insets</returns>
        public Dictionary<CstWeb.CustomerSessions.InsertType, long> InsetTypes
        {
            get
            {
                return WebApplicationParameters.InsetTypeCollection;
            }
        }

        /// <summary>
        /// Get  Media type selected by the customer from current module. 
        /// </summary>
        /// <returns>MEDIA type identifier (ex. 3 for TV in France)</returns>
        public string SelectedMediaType
        {
            get
            {
                /*Obtains the identifier of the selected current media. Example if the user as selected the media PRESS,
                        the joins could be like this : id_vehicle = 3" */
                return ((LevelInformation)_customerSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
            }
        }

        /// <summary>
        /// Get media category  selected by the customer from current module. 
        /// </summary>
        /// <returns>media category identifier </returns>
        public string SelectedMediaCategory
        {
              get
            {
                if (string.IsNullOrEmpty(_selectedMediaCategory))
                {
                    /*Obtains the identifier of the selected current media. Example if the user as selected the media PRESS,
                            the joins could be like this : id_vehicle = 3" */
                    if (_customerSession.SelectionUniversMedia != null && _customerSession.SelectionUniversMedia.FirstNode != null &&
                        _customerSession.SelectionUniversMedia.FirstNode.Nodes.Count > 0)
                        _selectedMediaCategory = ((LevelInformation)_customerSession.SelectionUniversMedia.FirstNode.FirstNode.Tag).ID.ToString();
                }
            return _selectedMediaCategory;
            }
        }
          /// <summary>
        /// Get the vehicles selected by the customer from current module.
        /// Return a dictionary which key corresponds to classification level type and value the identifiers list
        /// of classification level.
        /// <example>
        /// To ge the list (separated by comma)of vehicles in access , selected by the customer :
        /// <code>
        /// Dictionary[TNS.AdExpress.Constantes.Customer.Right.type,string] dic = GetSelectedVehicles(); 
        /// string idVehicles Selected = dic[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess];    
        /// </code>
        /// 
        /// So in a SQL query it can be use as follows :
        /// <code>
        /// string sql="";
        /// sql +=" select id_media,media "
        /// sql +=" from media "
        /// sql +=" where id_media in ("+idVehicles+") ";
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Dictionary of  vehicle classification items selected by the customer</returns>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.CustomerDataFiltersException">Impossible to identify the current module</exception>
        public Dictionary<CstCustomer.Right.type, string> SelectedVehicles
        {
            get{
                if(_selectedVehicles == null)
                _selectedVehicles = GetSelectedVehicles();
                return _selectedVehicles;
            }
        }
        /// <summary>
        /// Get the product classification items selected by the customer from current module.            
        /// </summary>
        /// <returns>TNS.AdExpress.Classification.AdExpressUniverse of  product classification items selected by the customer</returns>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.CustomerDataFiltersException">Impossible to identify the current module</exception>
        protected TNS.AdExpress.Classification.AdExpressUniverse SelectedProducts
        {
            get
            {
                if (_selectedProducts == null)
                {
                    _selectedProducts = GetSelectedProducts();
                }
                return _selectedProducts;
            }
        }

      
        /// <summary>
        /// Get informations about selected unit
        /// </summary>
        /// <returns>informations about selected unit</returns>
        public UnitInformation SelectedUnit
        {
            get
            {
                return _customerSession.GetSelectedUnit();
            }
        }
      

        #endregion

        #region Constructors
        /// <summary>
	    /// Default constructor
	    /// </summary>
	    /// <param name="customerSession">Customer web session</param>
		public CustomerDataFilters(WebSession customerSession) {
			if (customerSession == null) throw new ArgumentNullException(" customerSession parameter is null");
			_customerSession = customerSession;
            //Get current module
			_currentModule = ModulesList.GetModule(_customerSession.CurrentModule);
		}
		#endregion
		
        #region Protected methods

        #region Selected vehicles
       

        /// <summary>
        /// Get the vehicles selected by the customer from current module.
        /// Return a dictionary which key corresponds to classification level type and value the identifiers list
        /// of classification level.
        /// <example>
        /// To ge the list (separated by comma)of vehicles in access , selected by the customer :
        /// <code>
        /// Dictionary[TNS.AdExpress.Constantes.Customer.Right.type,string] dic = GetSelectedVehicles(); 
        /// string idVehicles Selected = dic[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess];    
        /// </code>
        /// 
        /// So in a SQL query it can be use as follows :
        /// <code>
        /// string sql="";
        /// sql +=" select id_media,media "
        /// sql +=" from media "
        /// sql +=" where id_media in ("+idVehicles+") ";
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Dictionary of  vehicle classification items selected by the customer</returns>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.CustomerDataFiltersException">Impossible to identify the current module</exception>
		protected Dictionary<CstCustomer.Right.type,string> GetSelectedVehicles() {

			int positionUnivers = 1;
			Dictionary<CstCustomer.Right.type, string> selection = new Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string>();
			string mediaList = "";

			switch (_currentModule.Id) {

                //Get selected vehicles from the module " Present /Absent report"
				case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
				//Get selected vehicles from the module " Lost /Won report "
				case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:

					//Get competing vehicles selection					
					while (_customerSession.CompetitorUniversMedia[positionUnivers] != null) {
						mediaList += _customerSession.GetSelection((TreeNode)_customerSession.CompetitorUniversMedia[positionUnivers], CstCustomer.Right.type.mediaAccess) + ",";
						positionUnivers++;
					}
					if (mediaList.Length > 0) selection.Add(CstCustomer.Right.type.mediaAccess, mediaList.Substring(0, mediaList.Length - 1));					
					break;
                //Get selected vehicles from the module "Vehicle Portofolio"
				case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE :
					mediaList = _customerSession.GetSelection((TreeNode)_customerSession.ReferenceUniversMedia, CstCustomer.Right.type.mediaAccess);
					if (mediaList.Length > 0) selection.Add(CstCustomer.Right.type.mediaAccess, mediaList.Substring(0, mediaList.Length - 1));					
					break;
				default:
					throw (new CustomerDataFiltersException("Impossible to identify the current module "));
			}
			return selection;
		}

				
		#endregion

		#region Media Rights
        /// <summary>
        /// Get customer vehicle classification rights. It provides all the identifiers of vehicle 
        /// classiffication's items allowed for the customer.
        /// Return a dictionary which key corresponds to classification level type and value the identifiers list
        /// of classification level.
        /// <example>
        /// To apply the rights in Sql query :
        /// <code>
        /// ...
        /// Dictionary[TNS.AdExpress.Constantes.Customer.Right.type,string] dic = GetMediaRights();
        /// bool fisrt = true, beginByAnd=true;
        /// 
        /// // Get the medias authorized for the current customer
        ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.vehicleAccess) && dic[CustomerRightConstante.type.vehicleAccess].Length > 0)
        ///   {
		///		if (beginByAnd) sql += " and";
        ///		sql += " (( id_vehicle in (" + dic[CustomerRightConstante.type.vehicleAccess] + ") ";
        ///       fisrt = false;
        ///	}   
        ///	 // Get the sub medias authorized for the current customer            
        ///     if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.categoryAccess) && dic[CustomerRightConstante.type.categoryAccess].Length > 0)
        ///   {
        ///       if (!fisrt) sql += " or";
		///		else {
		///			if (beginByAnd) sql += " and";
		///			sql += " ((";
		///		}
        ///        sql += " id_category in (" + dic[CustomerRightConstante.type.categoryAccess] + ") ";
        ///        fisrt = false;
        ///	}
        ///	// Get the medias not authorized for the current customer        
        ///      if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.vehicleException) && dic[CustomerRightConstante.type.vehicleException].Length > 0)
        ///    {
        ///         if (!fisrt) sql += " and";
		/// 		else {
		/// 			if (beginByAnd) sql += " and";
		/// 			sql += " (";
		/// 		}
        ///         sql += " id_vehicle not in (" + dic[CustomerRightConstante.type.vehicleException] + ") ";
         ///         fisrt = false;
        /// 	}
        /// 
        /// 
        ///  // Get the sub medias not authorized for the current customer
        ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.categoryException) && dic[CustomerRightConstante.type.categoryException].Length > 0)
        ///    {
         ///       if (!fisrt) sql += " and";
		///		else {
		///			if (beginByAnd) sql += " and";
		///			sql += " (";
		///		}
        ///        sql += " " + categoryTable.Prefix + ".id_category not in (" + _session.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ";
        ///       fisrt = false;
        ///	}
        /// ...
        ///  if (!fisrt) sql += " )";
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Dictionary of vehicle classification items rights</returns>
		protected Dictionary<CstCustomer.Right.type, string> GetMediaRights() {

			Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string> rights = new Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string>();
            
			/*Rights in acccess*/

            //Get the medias not authorized for the current customer   
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.vehicleAccess].Length > 0) {
				rights.Add(CstCustomer.Right.type.vehicleAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.vehicleAccess]);
			}
            // Get the sub medias authorized for the current customer   
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.categoryAccess].Length > 0) {
				rights.Add(CstCustomer.Right.type.categoryAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.categoryAccess]);			
			}
            // Get the vehicles authorized for the current customer   
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.mediaAccess].Length > 0) {
				rights.Add(CstCustomer.Right.type.mediaAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.mediaAccess]);
			}			

			/* Rights in exception*/

            // Get the medias not authorized for the current customer   
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.vehicleException].Length > 0) {
				rights.Add(CstCustomer.Right.type.vehicleAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.vehicleException]);
			}
            // Get the sub medias not authorized for the current customer   
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.categoryException].Length > 0) {
				rights.Add(CstCustomer.Right.type.categoryException, _customerSession.CustomerLogin[CstCustomer.Right.type.categoryException]);
			}
            // Get the vehicles not authorized for the current customer   
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.mediaException].Length > 0) {
				rights.Add(CstCustomer.Right.type.mediaException, _customerSession.CustomerLogin[CstCustomer.Right.type.mediaException]);
			}
			return rights;
		}
		#endregion

		#region Selected Products
        /// <summary>
        /// Get the product classification items selected by the customer from current module.            
        /// </summary>
        /// <returns>TNS.AdExpress.Classification.AdExpressUniverse of  product classification items selected by the customer</returns>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.CustomerDataFiltersException">Impossible to identify the current module</exception>
        protected TNS.AdExpress.Classification.AdExpressUniverse GetSelectedProducts()
        {
            TNS.AdExpress.Classification.AdExpressUniverse selection = null;
			switch (_currentModule.Id) {
				//Get selected products the module "Vehicle Portofolio"
				case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
				//Get selected products the module " Present /Absent report"
				case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
				//Get selected products the module " Lost /Won report "
				case CstWeb.Module.Name.ANALYSE_POTENTIELS :
					if (_customerSession.PrincipalProductUniverses != null && _customerSession.PrincipalProductUniverses.Count > 0) {
                        return _customerSession.PrincipalProductUniverses[0];
					}
					break;								
				default:
					throw (new CustomerDataFiltersException("Impossible to identify the current module "));
			}
			return selection;
		}

		
		#endregion

		#region Products
        /// <summary>
        /// Get the product classification items rights of the customer from current module.
        /// Return a dictionary which key corresponds to classification level type and value the identifiers list
        /// of classification level.
        /// <example>
        /// To apply the product selected  in Sql query :
        /// <code>
        /// ...
        /// Dictionary[TNS.AdExpress.Constantes.Customer.Right.type,string] dic = GetProductsRights();
        /// bool fisrt = true, beginByAnd=true;
        /// 
        /// // Get the categories in access for the current customer
        ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.sectorAccess) && dic[CustomerRightConstante.type.sectorAccess].Length > 0)
        ///   {
        ///		if (beginByAnd) sql += " and";
        ///		sql += " (( id_sector in (" + dic[CustomerRightConstante.type.sectorAccess] + ") ";
        ///       fisrt = false;
        ///	}   
        ///	  // Get the sub catergories in access for the current customer            
        ///     if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.subSectorAccess) && dic[CustomerRightConstante.type.subSectorAccess].Length > 0)
        ///   {
        ///       if (!fisrt) sql += " or";
        ///		else {
        ///			if (beginByAnd) sql += " and";
        ///			sql += " ((";
        ///		}
        ///        sql += " id_subsector in (" + dic[CustomerRightConstante.type.subSectorAccess] + ") ";
        ///        fisrt = false;
        ///	}
        ///	// Get the categories in exceptionfor the current customer        
        ///      if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.sectorAccess) && dic[CustomerRightConstante.type.sectorAccess].Length > 0)
        ///    {
        ///         if (!fisrt) sql += " and";
        /// 		else {
        /// 			if (beginByAnd) sql += " and";
        /// 			sql += " (";
        /// 		}
        ///         sql += " id_sector not in (" + dic[CustomerRightConstante.type.sectorAccess] + ") ";
        ///         fisrt = false;
        /// 	}
        /// 
        /// 
        ///  // Get the sub categories not authorized for the current customer
        ///   if (dic !=null && dic.ContainsKey(CustomerRightConstante.type.subSectorAccess) && dic[CustomerRightConstante.type.subSectorAccess].Length > 0)
        ///    {
        ///       if (!fisrt) sql += " and";
        ///		else {
        ///			if (beginByAnd) sql += " and";
        ///			sql += " (";
        ///		}
        ///        sql += " id_subsector not in (" + _session.CustomerLogin[CustomerRightConstante.type.subSectorAccess] + ") ";
        ///       fisrt = false;
        ///	}
        /// ...
        ///  if (!fisrt) sql += " )";
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Dictionary of  product classification items selected by the customer</returns>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.CustomerDataFiltersException">Impossible to identify the current module</exception>
		protected Dictionary<CstCustomer.Right.type, string> GetProductsRights() {
			Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string> rights = new Dictionary<TNS.AdExpress.Constantes.Customer.Right.type, string>();

			//Get products rights in access

			// Sector in access
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.sectorAccess].Length > 0) {
				rights.Add(CstCustomer.Right.type.sectorAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.sectorAccess]);
			}
			// SubSector in access
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.subSectorAccess].Length > 0) {
				rights.Add(CstCustomer.Right.type.subSectorAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.subSectorAccess]);
			}
			// Group in access
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.groupAccess].Length > 0) {
				rights.Add(CstCustomer.Right.type.groupAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.groupAccess]);
			}
			// Segment in access		
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.segmentAccess].Length > 0) {
				rights.Add(CstCustomer.Right.type.segmentAccess, _customerSession.CustomerLogin[CstCustomer.Right.type.segmentAccess]);
			}	
			
			//Get products rights in exception			
			// Sector in exception
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.sectorException].Length > 0) {
				rights.Add(CstCustomer.Right.type.sectorException, _customerSession.CustomerLogin[CstCustomer.Right.type.sectorException]);
			}
			// SubSector in exception
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.subSectorException].Length > 0) {
				rights.Add(CstCustomer.Right.type.subSectorException, _customerSession.CustomerLogin[CstCustomer.Right.type.subSectorException]);
			}
			// Group in exception
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.groupException].Length > 0) {
				rights.Add(CstCustomer.Right.type.groupException, _customerSession.CustomerLogin[CstCustomer.Right.type.groupException]);
			}
			// Segment in exception
			if (_customerSession.CustomerLogin[CstCustomer.Right.type.segmentException].Length > 0) {
				rights.Add(CstCustomer.Right.type.segmentException, _customerSession.CustomerLogin[CstCustomer.Right.type.segmentException]);
			}

			return rights;
		}
		#endregion

        #region GetAllowedMediaUniverse
        /// <summary>
        /// Get allowed media universe via properties :
        /// - VehicleList :  Get the list of medias type (ex. 1,2,3 =>PRESS,RADIO,TV)
        /// - CategoryList :  Get the list of sum medias (ex. 45,562,32) 
        /// - MediaList : Get the list of vehicles (ex. 489,8,563) 
        /// </summary>
        /// <returns>Media items List</returns>
        protected MediaItemsList GetAllowedMediaUniverse()
        {
            return _currentModule.AllowedMediaUniverse;
        }
        #endregion

        #region GetClassificationSelection
        /// <summary>
        /// Get selected Classification items 
        /// </summary>
        /// <param name="listGroup">List of classifications items groups</param>
        /// <param name="selection">Customer selection</param>
        /// <param name="accessType">Items Acces type (includes or  excludes)</param>
        /// <returns>Dictionary of selected Classification items </returns>
        /// <exception cref="TNS.AdExpress.Web.Core.Exceptions.CustomerDataFiltersException">Impossible to identify the level of the universe.</exception>
		protected Dictionary<CstCustomer.Right.type, string> GetClassificationSelection(List<NomenclatureElementsGroup> listGroup,Dictionary<CstCustomer.Right.type, string> selection,AccessType accessType) {
			
			#region Variables
			List<long> levelList = null;
			string listIds = "";
			long currentLevelId = -1;
			#endregion

			for (int i = 0; i < listGroup.Count; i++) {
				if (listGroup[i] != null && listGroup[i].Count() > 0) {
					levelList = listGroup[i].GetLevelIdsList();
					
					for (int j = 0; j < levelList.Count; j++) {
						currentLevelId = levelList[j];
						listIds = listGroup[i].GetAsString(currentLevelId);
						if (listIds != null && listIds.Length > 0) {							
							
							switch (currentLevelId) {
								case  TNS.Classification.Universe.TNSClassificationLevels.ADVERTISER :
									if (accessType == AccessType.includes) 
										AddLevelListId(selection, CstCustomer.Right.type.advertiserAccess, listIds);									
									else AddLevelListId(selection, CstCustomer.Right.type.advertiserException, listIds);									
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.BASIC_MEDIA :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.basicMediaAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.basicMediaException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.BRAND :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.brandAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.brandException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.CATEGORY :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.categoryAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.categoryException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.GROUP_ :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.groupAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.groupException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.HOLDING_COMPANY :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.holdingCompanyAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.holdingCompanyException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.INTEREST_CENTER :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.interestCenterAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.interestCenterException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.MEDIA :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.mediaAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.mediaException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.PRODUCT :
									if (accessType == AccessType.includes) 
										AddLevelListId(selection, CstCustomer.Right.type.productAccess, listIds);									
									else {
										AddLevelListId(selection, CstCustomer.Right.type.productException, listIds);
									}
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.SECTOR :
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.sectorAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.sectorException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.SEGMENT:
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.segmentAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.segmentException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.SUB_SECTOR:
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.subSectorAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.subSectorException, listIds);
									break;
								case TNS.Classification.Universe.TNSClassificationLevels.VEHICLE:
									if (accessType == AccessType.includes)
										AddLevelListId(selection, CstCustomer.Right.type.vehicleAccess, listIds);
									else AddLevelListId(selection, CstCustomer.Right.type.vehicleException, listIds);
									break;
								default:
									throw (new CustomerDataFiltersException("Impossible to identify the level of the universe."));


							}
						}
					}
				}
				
			}
			return selection;
        }
        #endregion

        #region AddLevelListId
        /// <summary>
        /// Add level list
        /// </summary>
        /// <param name="selection">Customer selection</param>
        /// <param name="levelType">classification level type</param>
        /// <param name="listIds">Identifier list of classification itmes</param>
		protected void AddLevelListId(Dictionary<CstCustomer.Right.type, string> selection, CstCustomer.Right.type levelType, string listIds) {
			
				if (selection.ContainsKey(levelType)) {
					selection[levelType] +="," + listIds;
				}
				else selection.Add(levelType, listIds);
        }
        #endregion

        #endregion

    }
}
