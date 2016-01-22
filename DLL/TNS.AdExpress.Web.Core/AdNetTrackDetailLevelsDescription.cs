#region Informations
// Auteur: G. Facon
// Création: 21/03/2007
// Modification:

#endregion

using System;
using System.Collections;
using TNS.AdExpress.Web.Core.DataAccess;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core{
	/// <summary>
	/// AdNetTrack detail levels Description
	/// </summary>
	public class AdNetTrackDetailLevelsDescription{

		#region Variables
		///<summary>
		/// Default AdNetTrack Levels
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_defaultMediaDetailLevels</label>
		protected static ArrayList _defaultAdNetTrackDetailLevels=new ArrayList();
		///<summary>
		/// Allowed AdNetTrack Level items
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_allowedProductDetailLevelItems</label>
		protected static ArrayList _allowedAdNetTrackLevelItems=new ArrayList();
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		static AdNetTrackDetailLevelsDescription(){
		}
		#endregion

		#region Méthodes
		#region Initialisation de la classe
		/// <summary>
		/// Initialization
		/// </summary>
		/// <param name="source">Data Source</param>
		public static void Init(IDataSource source){
			try{
				AdNetTrackDetailLevelsDescriptionDataAccess.Load(source);
			}
			catch(System.Exception err){
				throw(new TNS.AdExpress.Web.Core.Exceptions.AdNetTrackDetailLevelsDescriptionException("impossible to get AdNetTrack levels informations",err));
			}
						
		}
		#endregion

		///<author>G. Facon</author>
		///  <since>21/03/2007</since>
		///<summary>Get default AdNetTrack levels</summary>
		public static ArrayList DefaultAdNetTrackDetailLevels {
			get{return (_defaultAdNetTrackDetailLevels);}
		}

		///<author>G. Facon</author>
		///  <since>21/03/2007</since>
		///<summary>Get Allowed AdNetTrack Level items</summary>
		public static ArrayList AllowedAdNetTrackLevelItems {
			get{return (_allowedAdNetTrackLevelItems);}
		}
		#endregion


	}
}
