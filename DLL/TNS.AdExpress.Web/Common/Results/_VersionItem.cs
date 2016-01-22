#region Info
/*
 * Author : G Facon / G Ragneau
 * Creation : 13/07/2006
 * Modification :
 *		Author - Date - description
 *		Y. Rkaina - 17/08/2006 - Ajout de nouveaux attributs
 * */
#endregion

using System;

namespace TNS.AdExpress.Web.Common.Results
{
	/// <summary>
	/// VersionItem provides information for the display of a version
	/// </summary>
	public class _VersionItem
	{

		#region Variables
		///<summary>Version Id</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		private Int64 _id;
		///<summary>Component CssClass</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		private string _cssClass;
		///<summary>Parution date</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		private string _parution;
		///<summary>Version path</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		private string _path = string.Empty; 
		/// <summary>Annonceur</summary>
		/// <author>rkaina</author>
		/// 		/// <since>jeudi 08 août 2006</since>
		private string _advertiser;
		/// <summary>Id produit</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		private string _productId;
		/// <summary>Produit</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		private string _product;
		/// <summary>groupe de produit</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		private string _group;
		/// <summary>Famille</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		private string _sector;
		/// <summary>Classe</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		private string _subsector;
		#endregion

		#region Accessors
		///<summary>Get / Set Id Version</summary>
		///  <author>gragneau</author>
		///  <since>jeudi 13 juillet 2006</since>
		public Int64 Id {
    		get {
        		return (_id);
    		}
		}
 		///<summary>Get / Set CssClass</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public string CssClass {
    		get {
        		return (_cssClass);
    		}
		}
		///<summary>Get / Set Parution Date</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public string Parution {
    		get {
        		return (_parution);
    		}
			set {
				_parution = value;
			}
		}
		///<summary>Get / Set Version Path</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public string Path {
			get {
				return (_path);
			}
			set {
				_path = value;
			}
		}
		///<summary>Get / Set Annonceur</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		public string Advertiser {
			get {
				return (_advertiser);
			}
			set {
				_advertiser = value;
			}
		}
		///<summary>Get / Set Id produit</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		public string ProductId {
			get {
				return (_productId);
			}
			set {
				_productId = value;
			}
		}
		///<summary>Get / Set Produit</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		public string Product {
			get {
				return (_product);
			}
			set {
				_product = value;
			}
		}
		///<summary>Get / Set groupe de produit</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		public string Group {
			get {
				return (_group);
			}
			set {
				_group = value;
			}
		}
		///<summary>Get / Set Famille</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		public string Sector {
			get {
				return (_sector);
			}
			set {
				_sector = value;
			}
		}
		///<summary>Get / Set Classe</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 08 août 2006</since>
		public string Subsector {
			get {
				return (_subsector);
			}
			set {
				_subsector = value;
			}
		}
		#endregion
		
		#region Constructors
		///<summary>Constructor</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public _VersionItem( Int64 id, string cssClass, string parution ) {
			_id = id;
			_cssClass = cssClass;
			_parution = parution;
		}

		///<summary>Constructor</summary>
		/// <author>gragneau</author>
		/// <since>jeudi 13 juillet 2006</since>
		public _VersionItem( Int64 id, string cssClass ) {
			_id = id; 
			_cssClass = cssClass;
		}
		#endregion


	}
}
