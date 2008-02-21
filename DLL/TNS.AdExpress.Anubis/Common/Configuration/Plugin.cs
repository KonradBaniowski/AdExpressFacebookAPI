#region Information
// Auteur G. Facon
// Date de création: 05/08/2005
// Date de modification:

#endregion

using System;
using TNS.AdExpress.Anubis.Exceptions;

namespace TNS.AdExpress.Anubis.Common.Configuration{
	/// <summary>
	/// Description résumée de Plugin.
	/// </summary>
	public class Plugin{

		#region Variables
		/// <summary>
		/// Nom du plugin
		/// </summary>
		protected string _name;
		/// <summary>
		/// Type de résultat que génère le plug-in
		/// </summary>
		protected int _resultType;
		/// <summary>
		/// Nom de l'assembly
		/// </summary>
		protected string _assemblyName;
		/// <summary>
		/// Classe qui doit être utilisée
		/// </summary>
		protected string _class;
		/// <summary>
		/// Chemin du fichier de configuration du plug-in
		/// </summary>
		/// <remarks>
		/// Le chemin doit être relatif par rapport à l'application
		/// </remarks>
		protected string _configurationFilePath;
		/// <summary>
		/// Longévité du résultat généré. Pour une persistance infinie, -1
		/// </summary>
		protected int _resultLongevity = -1;
		/// <summary>
		/// Chemin d'accès aux résultats du plugin
		/// </summary>
		protected string _resultsRoot = String.Empty;


		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du plugin</param>
		/// <param name="resultType">Type de résultat</param>
		/// <param name="assemblyName">Nom de l'assembly</param>
		/// <param name="classe">Classe qui doit être utilisée</param>
		/// <param name="configurationFilePath">Chemin de configuration du plug-in (Le chemin doit être relatif par rapport à l'application)</param>
		public Plugin(string name,int resultType,string assemblyName,string classe,string configurationFilePath){
			if(name==null)throw(new ArgumentNullException("Le nom du plug-in est null"));
			if(assemblyName==null)throw(new ArgumentNullException("Le nom de l'assembly de plug-in est null"));
			if(classe==null)throw(new ArgumentNullException("Le nom de la classe de plug-in est null"));
			if(name.Length<1)throw(new ArgumentException("Le nom du plug-in est vide"));
			if(assemblyName.Length<1)throw(new ArgumentException("Le nom du plug-in est vide"));
			if(classe.Length<1)throw(new ArgumentException("Le nom de la classe est vide"));
			this._assemblyName=assemblyName;
			this._name=name;
			this._class=classe;
			this._resultType=resultType;
			_configurationFilePath=configurationFilePath;
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="name">Nom du plugin</param>
		/// <param name="resultType">Type de résultat</param>
		/// <param name="assemblyName">Nom de l'assembly</param>
		/// <param name="classe">Classe qui doit être utilisée</param>
		/// <param name="configurationFilePath">Chemin de configuration du plug-in (Le chemin doit être relatif par rapport à l'application)</param>
		/// <param name="resultLongevity">Longévité, persistance d'un résultat généré par ce plugin</param>
		/// <param name="resultsRoot">Racine du résultat</param>
		public Plugin(string name,int resultType,string assemblyName,string classe,string configurationFilePath, string resultsRoot ,int resultLongevity){
			if(name==null)throw(new ArgumentNullException("Le nom du plug-in est null"));
			if(assemblyName==null)throw(new ArgumentNullException("Le nom de l'assembly de plug-in est null"));
			if(classe==null)throw(new ArgumentNullException("Le nom de la classe de plug-in est null"));
			if(name.Length<1)throw(new ArgumentException("Le nom du plug-in est vide"));
			if(assemblyName.Length<1)throw(new ArgumentException("Le nom du plug-in est vide"));
			if(classe.Length<1)throw(new ArgumentException("Le nom de la classe est vide"));
			this._assemblyName=assemblyName;
			this._name=name;
			this._class=classe;
			this._resultType=resultType;
			this._configurationFilePath=configurationFilePath;
			this._resultLongevity = resultLongevity;
			this._resultsRoot = resultsRoot;
		}

		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient le nom du Plugin
		/// </summary>
		public string Name{
			get{return(_name);}
		}

		/// <summary>
		/// Obtient le type de résultat du Plugin
		/// </summary>
		public int ResultType{
			get{return(_resultType);}
		}

		/// <summary>
		/// Obtient le nom de l'assembly
		/// </summary>
		public string AssemblyName{
			get{return(_assemblyName);}
		}
		/// <summary>
		/// Obtient le nom de la classe de démarrage
		/// </summary>
		public string Class{
			get{return(_class);}
		}

		/// <summary>
		/// Obtient le chemin du fichier de configuration du plug-in
		/// </summary>
		/// <remarks>
		/// Le chemin doit être relatif par rapport à l'application
		/// </remarks>
		public string ConfigurationFilePath{
			get{return(_configurationFilePath);}
		}

		/// <summary>
		/// Obtient la longévité du résultat du Plugin
		/// </summary>
		public int ResultLongevity{
			get{return(_resultLongevity);}
		}

		/// <summary>
		/// Obtient le chemin des fichiers de résultats du plugin
		/// </summary>
		/// <remarks>
		/// Chemin absolu
		/// </remarks>
		public string ResultsRoot{
			get{return(_resultsRoot);}
		}
		#endregion
	}
}
