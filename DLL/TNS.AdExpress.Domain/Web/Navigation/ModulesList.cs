using System;
using System.Collections;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Classe utilisée pour connaître les données relatives à un module où un groupe de module
	/// Hérite de ModuleHeadingDataAccess 
	/// </summary>
	public class ModulesList{

		#region Variables
		///<summary>
		/// HashTable avec l'objet ModuleGroup comme valeur
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Navigation.ModuleGroup</associates>
		///  <label>_htModuleGroup</label>
		protected static System.Collections.Hashtable _htModuleGroup;
		///<summary>
		/// HashTable avec l'objet Module comme valeur
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Navigation.ModuleItem</associates>
		///  <label>_htModule</label>
		protected static System.Collections.Hashtable _htModule;
		/// <summary>
		/// HashTable avec l'objet ModuleCategory comme valeur
		/// </summary>
		protected static System.Collections.Hashtable _htModuleCategory;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		static ModulesList(){
			_htModuleGroup=new Hashtable();
			_htModule=new Hashtable();
			_htModuleCategory=new Hashtable();
            ModulesListXL.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot+ConfigurationFile.MODULE_CONFIGURATION_FILENAME),_htModuleGroup,_htModule);
            ModuleCategoryListXL.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot+ConfigurationFile.MODULE_CATEGORY_CONFIGURATION_FILENAME),_htModuleCategory);

        }
		#endregion

		#region Initialisation de la classe
		///<summary>
		/// Initialise la classe
		/// </summary>
		///  <param name="pathXMLFile">chemin du fichier xml</param>
		///  <param name="pathXMLModuleCategoryFile">Chemin du fichier xml de la configuration des catégories</param>
		///  <url>element://model:project::TNS.AdExpress.Web.Core/design:view:::f0mlmfqhp2ke24l_v</url>
		public static void Init(){
			
		}
		#endregion

		#region Récupération des données
		/// <summary>
		/// Retourne l'intitulé du module
		/// </summary>
		/// <param name="idModule">Identifiant du module</param>
		/// <returns>Code de l'intitulé du module</returns>
		public static Int64 GetModuleWebTxt(Int64 idModule){
			try{
				return(((Module)_htModule[idModule]).IdWebText);
			}
			catch(System.Exception){
				// le code langue n'est pas correcte
				return(-1);
			}
		}

		/// <summary>
		/// Retourne l'intitulé de la catégorie de module
		/// </summary>
		/// <param name="idModuleCategory">Identifiant de la catégorie de module</param>
		/// <returns>Code de l'intitulé de la catégorie de module</returns>
		public static Int64 GetModuleCategoryWebTxt(Int64 idModuleCategory){
			try{
				return(((ModuleCategory)_htModuleCategory[idModuleCategory]).IdWebText);
			}
			catch(System.Exception){
				// le code langue n'est pas correcte
				return(-1);
			}
		}

		/// <summary>
		/// Retourne l'intitulé du groupe de module
		/// </summary>
		/// <param name="idModuleGroup">identifiant du groupe de module</param>
		/// <returns>Code de l'intitulé du groupe de module</returns>
		public static Int64 GetModuleGroupIdWebTxt(Int64 idModuleGroup){
			try{
				return(((ModuleGroup)_htModuleGroup[idModuleGroup]).IdWebText);
			}
			catch(System.Exception){
				// le code langue n'est pas correcte
				return(-1);
			}
		}

		/// <summary>
		/// Retourne l'adresse du fichier flash
		/// </summary>
		/// <param name="idModuleGroup">identifiant du groupe de module</param>
		/// <returns>adresse du fichier flash</returns>
		public static string GetModuleGroupFlash(Int64 idModuleGroup){
			try{
				string tmp=((ModuleGroup)_htModuleGroup[idModuleGroup]).FlashPath;
				if(tmp.Length<5)throw(new ModuleException("L'url du fichier flash n'est pas valide"));
				return(tmp);
			}catch(System.Exception){
				return ("Erreur");
			}
		}

		/// <summary>
		/// Retourne l'adresse de l'image remplaçant le flash
		/// </summary>
		/// <param name="idModuleGroup">identifiant du groupe de module</param>
		/// <returns>adresse de l'image remplaçant le flash</returns>
		public static string GetMissingModuleGroupFlash(Int64 idModuleGroup){
			try{
				string tmp=((ModuleGroup)_htModuleGroup[idModuleGroup]).MissingFlashUrl;
				if(tmp.Length<5)throw(new ModuleException("L'url du fichier flash n'est pas valide"));
				return(tmp);
			}catch(System.Exception){
				return ("Erreur");
			}
		}

		/// <summary>
		/// Retourne le commentaire lié au groupe de module
		/// </summary>
		/// <param name="idModuleGroup">identifiant du groupe de module</param>
		/// <returns>Code du commentaire lié au groupe de module</returns>
		public static Int64 GetModuleGroupDescriptionWebTextId(Int64 idModuleGroup){
			try{
				return(((ModuleGroup)_htModuleGroup[idModuleGroup]).DescriptionWebTextId);
			}
			catch(System.Exception){
				// le code langue n'est pas correcte
				return(-1);
			}
		}

		/// <summary>
		/// Retourne le commentaire lié au module
		/// </summary>
		/// <param name="idModule">Identifiant du module</param>
		/// <returns>Code du commentaire lié au module</returns>
		public static Int64 GetModuleDescriptionWebTextId(Int64 idModule){
			try{
				return(((Module)_htModule[idModule]).DescriptionWebTextId);
			}
			catch(System.Exception){
				// le code langue n'est pas correcte
				return(-1);
			}
		}

        /// <summary>
        /// Retourne le chemin de l'image qui apparait dans l'info bulle
        /// </summary>
        /// <param name="idModule">Identifiant du module</param>
        /// <returns>Chemin de l'image qui apparait dans l'info bulle</returns>
        public static string GetModuleDescriptionImageName(Int64 idModule) {
            try {
                return (((Module)_htModule[idModule]).DescriptionImageName);
            }
            catch(System.Exception) {
                return ("");
            }
        }

		/// <summary>
		/// Retourne l'adresse de la page url suivante
		/// </summary>
		/// <param name="idModule">identifiant module</param>
		/// <returns>adresse de la page url suivante</returns>
		public static string GetModuleNextUrl(Int64 idModule){
			try{
				string tmp=((Module)_htModule[idModule]).UrlNextPage;
				if(tmp.Length<5)throw(new ModuleException("L'url n'est pas valide"));
				return(tmp);
			}
			catch(System.Exception){
				return("Erreur");
			}
		}	
	
		/// <summary>
		/// Retourne l'objet Module
		/// </summary>
		/// <param name="idModule">Identifiant du module</param>
		/// <returns>Module</returns>
		public static Module GetModule(Int64 idModule){
			try{
				return (Module)_htModule[idModule]; 
			}
			catch(System.Exception){
				return(null);
			}
		}

        /// <summary>
		/// Retourne l'objet Module
		/// </summary>
		/// <param name="idModule">Identifiant du module</param>
		/// <returns>Module</returns>
		public static Dictionary<long,Module> GetModules()
        {
            try
            {
                           
                return    _htModule
         .Cast<DictionaryEntry>()
         .ToDictionary(kvp => (Int64)kvp.Key, kvp => (Module)kvp.Value);
             
               
            }
            catch (System.Exception)
            {
                return (null);
            }
        }
        #endregion
    }
}
