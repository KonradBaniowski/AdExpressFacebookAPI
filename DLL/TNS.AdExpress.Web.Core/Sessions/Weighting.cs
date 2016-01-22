using System;

namespace TNS.AdExpress.Web.Core.Sessions{
	/// <summary>
	/// Effectue les pondérations entre unités.
	/// </summary>
	[System.Serializable]
	public class Weighting {

		#region variables
		/// <summary>
		/// vrai si pondération utilisée
		/// </summary>
		private bool _use = false;
		/// <summary>
		/// ratio
		/// </summary>
		private float _value=(float)1.0;
		/// <summary>
		/// libellé de l'unité
		/// </summary>
		private string _displayName="";
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public Weighting(){			
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// vrai si pondération utilisée
		/// </summary>
		public bool Use{
			get{return _use;}
			set{_use=value;}
		}

		/// <summary>
		///  ratio
		/// </summary>
		public float Value{
			get{return _value;}
			set{_value=value;}
		}

		/// <summary>
		///  ratio
		/// </summary>
		public string DisplayName{
			get{return _displayName;}
			set{_displayName=value;}
		}
		#endregion



	}
}
