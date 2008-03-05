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
        /// cl� : type de liste dans la nomenclature en acc�s ou en exception 
        /// valeur : liste correspondant � la cl� 
        /// </summary>
        protected Dictionary<CustomerCst.Right.type,string> htRight;
        /// <summary>
        /// Modules list
        /// </summary>
        protected Hashtable htModulesList=new Hashtable();
        /// <summary>
        /// hashtable : cl� idFlag
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
        /// Nbre de lignes dans la base de donn�es que l'on compare
        /// avec les listes des droits clients
        /// </summary>
        protected int nbLineBD;
        /// <summary>
        /// V�rifie si les droits ont �t� d�termin�s
        /// </summary>
        protected bool rightDetermined;
        /// <summary>
        /// Indique si l'utilisateur a le droit de se connecter
        /// </summary>
        protected bool rightValidated;
        /// <summary>
        /// Cha�ne de connection � la base de donn�es
        /// </summary>
        protected string connectionString;
        /// <summary>
        /// bool indiquant si c'est la premi�re connection au site
        /// true si premi�re connection
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
