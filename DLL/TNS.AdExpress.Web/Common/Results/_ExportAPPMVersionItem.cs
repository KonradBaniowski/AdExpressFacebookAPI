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
	/// Description résumée de ExportAPPMVersionItem.
	/// </summary>
	public class _ExportAPPMVersionItem:_VersionItem {

		#region Variables
		/// <summary>Nom d'Agence Média</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _agency;
		/// <summary>Date de début</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _dateBegin;
		/// <summary>Date de fin</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _dateEnd;
		/// <summary>budget brut (euros)</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _budget;
		/// <summary>Version Weight</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _versionWeight;
		/// <summary>Marque</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _brand;
		/// <summary>nombre d'insertions</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _insertions;
		/// <summary>nombre de pages utilisés</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _pages;
		/// <summary>nombre de supports utilisés</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _supports;
		/// <summary>Part de voix de la campagne</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _pdv;
		/// <summary>cible selectionnée</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _targetSelected;
		/// <summary>cible de base</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _baseTarget;
		/// <summary>nombre de GRP(cible selectionnée</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _grpNumber;
		/// <summary>nombre de GRP(cible 15 ans et +)</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _grpNumberBase;
		/// <summary>Indice GRP vs cible 15 ans à +</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _indiceGRP;
		/// <summary>Indice GRP vs cible 15 ans à +</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _grpCost;
		/// <summary>Coût GRP(cible 15 et +)</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _grpCostBase;
		/// <summary>Indice coût GRP vs cible 15 ans à +</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private string _indiceGRPCost;
		/// <summary>Nombre de visuels</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		private Int64 _nbVisuel;
		#endregion

		#region Accessors
		///<summary>Get / Set Nom d'Agence Média</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string Agency {
			get {
				return (_agency);
			}
			set {
				_agency = value;
			}
		}
		///<summary>Get / Set Date de début</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string DateBegin {
			get {
				return (_dateBegin);
			}
			set {
				_dateBegin = value;
			}
		}
		///<summary>Get / Set Date de fin</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string DateEnd {
			get {
				return (_dateEnd);
			}
			set {
				_dateEnd = value;
			}
		}
		///<summary>Get / Set budget brut (euros)</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string Budget {
			get {
				return (_budget);
			}
			set {
				_budget = value;
			}
		}
		///<summary>Get / Set Version Weight</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string VersionWeight {
			get {
				return (_versionWeight);
			}
			set {
				_versionWeight = value;
			}
		}
		///<summary>Get / Set Marque</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string Brand {
			get {
				return (_brand);
			}
			set {
				_brand = value;
			}
		}
		///<summary>Get / Set nombre d'insertions</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string Insertions {
			get {
				return (_insertions);
			}
			set {
				_insertions = value;
			}
		}
		///<summary>Get / Set nombre de pages utilisés</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string Pages {
			get {
				return (_pages);
			}
			set {
				_pages = value;
			}
		}
		///<summary>Get / Set nombre de supports utilisés</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string Supports {
			get {
				return (_supports);
			}
			set {
				_supports = value;
			}
		}
		///<summary>Get / Set Part de voix de la campagne</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string PDV {
			get {
				return (_pdv);
			}
			set {
				_pdv = value;
			}
		}
		///<summary>Get / Set cible selectionnée</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string TargetSelected {
			get {
				return (_targetSelected);
			}
			set {
				_targetSelected = value;
			}
		}
		///<summary>Get / Set cible de base</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string BaseTarget {
			get {
				return (_baseTarget);
			}
			set {
				_baseTarget = value;
			}
		}
		///<summary>Get / Set nombre de GRP(cible selectionnée)</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string GRPNumber {
			get {
				return (_grpNumber);
			}
			set {
				_grpNumber = value;
			}
		}
		///<summary>Get / Set nombre de GRP(cible 15 ans et +)</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string GRPNumberBase {
			get {
				return (_grpNumberBase);
			}
			set {
				_grpNumberBase = value;
			}
		}
		///<summary>Get / Set Indice GRP vs cible 15 ans à +</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string IndiceGRP {
			get {
				return (_indiceGRP);
			}
			set {
				_indiceGRP = value;
			}
		}
		///<summary>Get / Set Indice GRP vs cible 15 ans à +</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string GRPCost {
			get {
				return (_grpCost);
			}
			set {
				_grpCost = value;
			}
		}
		///<summary>Get / Set Coût GRP(cible 15 et +)</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string GRPCostBase {
			get {
				return (_grpCostBase);
			}
			set {
				_grpCostBase = value;
			}
		}
		///<summary>Get / Set Indice coût GRP vs cible 15 ans à +</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
		public string IndiceGRPCost {
			get {
				return (_indiceGRPCost);
			}
			set	{
				_indiceGRPCost = value;
			}
		}
		///<summary>Get / Set Nombre de visuels</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 29 août 2006</since>
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
		/// <since>jeudi 17 août 2006</since>
		public _ExportAPPMVersionItem( Int64 id, string cssClass, string parution ):base( id, cssClass, parution ) 
		{
		}

		///<summary>Constructor</summary>
		/// <author>rkaina</author>
		/// <since>jeudi 17 août 2006</since>
		public _ExportAPPMVersionItem( Int64 id, string cssClass ):base( id, cssClass ) {
		}
		#endregion

	}
}
