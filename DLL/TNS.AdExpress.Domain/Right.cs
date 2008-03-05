using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.DB;
using CustomerCst=TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain {
    /// <summary>
    /// AdExpress customer rights
    /// </summary>
    [System.Serializable]
    public class Right {

        #region Variables
        /// <summary>
        /// clé : type de liste dans la nomenclature en accès ou en exception 
        /// valeur : liste correspondant à la clé 
        /// </summary>
        protected Dictionary<CustomerCst.Right.type,string> htRight;
        /// <summary>
        /// Modules list
        /// </summary>
        protected Hashtable htModulesList=new Hashtable();
        /// <summary>
        /// hashtable : clé idFlag
        /// valeur : Flag
        /// </summary>
        protected Hashtable listFlags;
        /// <summary>
        /// identifiant login
        /// </summary>		
        protected Int64 idLogin;
        /// <summary>
        /// login
        /// </summary>
        protected string login;
        /// <summary>
        /// mot de passe
        /// </summary>
        protected string password;
        /// <summary>
        /// Nbre de lignes dans la base de données que l'on compare
        /// avec les listes des droits clients
        /// </summary>
        protected int nbLineBD;
        /// <summary>
        /// Vérifie si les droits ont été déterminés
        /// </summary>
        protected bool rightDetermined;
        /// <summary>
        /// Indique si l'utilisateur a le droit de se connecter
        /// </summary>
        protected bool rightValidated;
        /// <summary>
        /// Chaîne de connection à la base de données
        /// </summary>
        protected string connectionString;
        /// <summary>
        /// bool indiquant si c'est la première connection au site
        /// true si première connection
        /// </summary>
        protected bool firstRequest=true;
        /// <summary>
        /// DB Connection
        /// </summary>
        [System.NonSerialized]
        protected IDataSource source=null;
        /// <summary>
        /// date de connection
        /// </summary>		
        protected DateTime dateConnexion;
        /// <summary>
        /// Date de modification des droits utilisateur
        /// </summary>
        protected DateTime dateModif;
        #endregion
    }
}
