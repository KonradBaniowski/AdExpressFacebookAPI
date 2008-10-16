#region Info
/*
 * Author : Y. Rkaina
 * Creation : 17/08/2006
 * Modification :
 *		Author - Date - description
 * 
 * */
#endregion

using System;

namespace TNS.AdExpress.Web.Common.Results
{
	/// <summary>
	/// Description r�sum�e de DetailVersionItem.
	/// </summary>
	public class _ExportVersionItem:_VersionItem{


		#region Variables
		/// <summary>date de la premi�re insertion dans le plan</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		private string _firstInsertionDate;
		/// <summary>Nombre d'insertions</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		private Int64 _nbInsertion;
		/// <summary>Nombre de supports</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		private Int64 _nbMedia;
		/// <summary>Budget</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		private Double _expenditureEuro;
		/// <summary>Nombre de visuels</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 21 ao�t 2006</since>
		private Int64 _nbVisuel;
		#endregion

		#region Accessors
		///<summary>Get / Set date de la premi�re insertion dans le plan</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		public string FirstInsertionDate {
			get {
				return (_firstInsertionDate);
			}
			set {
				_firstInsertionDate = value;
			}
		}
		///<summary>Get / Set Nombre d'insertions</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		public Int64 NbInsertion {
			get {
				return (_nbInsertion);
			}
			set {
				_nbInsertion = value;
			}
		}
		///<summary>Get / Set Nombre de supports</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		public Int64 NbMedia {
			get {
				return (_nbMedia);
			}
			set {
				_nbMedia = value;
			}
		}
		///<summary>Get / Set Budget</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		public Double ExpenditureEuro {
			get {
				return (_expenditureEuro);
			}
			set {
				_expenditureEuro = value;
			}
		}
		///<summary>Get / Set Nombre de visuels</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 21 ao�t 2006</since>
		public Int64 NbVisuel {
			get {
				return (_nbVisuel);
			}
			set {
				_nbVisuel = value;
			}
		}
		#endregion

		#region Constructors
		///<summary>Constructor</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		public _ExportVersionItem( Int64 id, string cssClass, string parution ):base( id, cssClass, parution ){
		}

		///<summary>Constructor</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 ao�t 2006</since>
		public _ExportVersionItem( Int64 id, string cssClass ):base( id, cssClass ){
		}
		#endregion


	}
}
