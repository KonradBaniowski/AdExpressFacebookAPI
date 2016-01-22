#region Information
//Author : G. RAGNEAU
//Creation : 05/08/2005
#endregion

using System;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.DataAccess.Configuration;

namespace TNS.AdExpress.Anubis.Common.Configuration{
	/// <summary>
	/// Static class containing every cifiguration elements for anubis
	/// </summary>
	public class AnubisConfig {

		#region Attributes
		/// <summary>
		/// Interval de temps pour le refresh de la liste des demandes
		/// </summary>
		private int _jobInterval = 60000;
		/// <summary>
		/// Nombre d'ouvrier pour traiter les demandes
		/// </summary>
		private int _jobsNumber=1;
		/// <summary>
		/// Interval de temps d'attente avant l'analyse des demandes à traiter
		/// </summary>
		private int _distributionInterval=30000;
		/// <summary>
		///	Path for the log files
		/// </summary>
		private string _tracePath="";
		/// <summary>
		/// Interval de temps avant de relancer une requête de mis à jour des jobs suite à une erreur du serveur.
		/// </summary>
		private int _updateErrorDelay = 600000;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		public AnubisConfig(IDataSource dataSource){
			AnubisConfigDataAccess.Load(dataSource,this);
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient l'interval de temps pour le refrsh de la liste des demandes
		/// </summary>
		public int JobInterval{
			get{return _jobInterval;}
			set{_jobInterval = value;}
		}

		/// <summary>
		/// Get/Set l'interval de temps avant de relancer le serveur de mis à jour des jobs suite à une erreur
		/// </summary>
		public int UpdateErrorDelay{
			get{return _updateErrorDelay;}
			set{_updateErrorDelay = value;}
		}

		/// <summary>
		/// Obtient le nombre d'ouvrier pour traiter les demandes
		/// </summary>
		public int JobsNumber{
			get{return _jobsNumber;}
			set{_jobsNumber = value;}
		}

		/// <summary>
		/// Obtient l'interval de temps d'attente avant l'analyse des demandes à traiter
		/// </summary>
		public int DistributionInterval{
			get{return _distributionInterval;}
			set{_distributionInterval = value;}
		}

		/// <summary>
		/// Get the file to acces the log file
		/// </summary>
		public string TracePath{
			get{return _tracePath;}
			set{_tracePath = value;}
		}
		#endregion

	}
}
