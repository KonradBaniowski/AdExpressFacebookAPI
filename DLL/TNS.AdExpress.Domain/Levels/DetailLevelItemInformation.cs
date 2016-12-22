#region Informations
// Auteur: G. Facon
// Création: 27/03/2006
// Modification:
#endregion

using System;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.Level{


	///<summary>
	/// Description d'un niveau de détail d'un tableau
	/// </summary>
	///  <author>G.Facon</author>
	public class DetailLevelItemInformation {

		#region Enum
		/// <summary>
		/// Eléments compsant les Niveaux de détailles
		/// </summary>
        public enum Levels {
            /// <summary>
            /// Media
            /// </summary>
            vehicle = 1,
            /// <summary>
            /// Catégorie
            /// </summary>
            category = 2,
            /// <summary>
            /// Support
            /// </summary>
            media = 3,
            /// <summary>
            /// Centre d'interet
            /// </summary>
            interestCenter = 4,
            /// <summary>
            /// Régie
            /// </summary>
            mediaSeller = 5,
            /// <summary>
            /// Version
            /// </summary>
            slogan = 6,
            /// <summary>
            /// Groupe de société
            /// </summary>
            holdingCompany = 7,
            /// <summary>
            /// Annonceur
            /// </summary>
            advertiser = 8,
            /// <summary>
            /// Marque
            /// </summary>
            brand = 9,
            /// <summary>
            /// Produit
            /// </summary>
            product = 10,
            /// <summary>
            /// Famille
            /// </summary>
            sector = 11,
            /// <summary>
            /// Classe
            /// </summary>
            subSector = 12,
            /// <summary>
            /// Groupe
            /// </summary>
            group = 13,
            /// <summary>
            /// Variété
            /// </summary>
            segment = 14,
            /// <summary>
            /// Groupe d'agences
            /// </summary>
            groupMediaAgency = 15,
            /// <summary>
            /// Agence
            /// </summary>
            agency = 16,
            /// <summary>
            /// Titre
            /// </summary>
            title = 17,
            /// <summary>
            /// Date
            /// </summary>
            date = 18,
            /// <summary>
            /// Durée
            /// </summary>
            duration = 19,
            /// <summary>
            /// Format
            /// </summary>
            format = 20,
            /// <summary>
            /// Code écran
            /// </summary>
            commecialBreak = 21,
            /// <summary>
            /// Format affiche
            /// </summary>
            typeBoard = 22,
            /// <summary>
            /// Genre d'émission
            /// </summary>
            programType = 23,
            /// <summary>
            /// Emission
            /// </summary>
            program = 24,
            /// <summary>
            /// Forme de parrainage
            /// </summary>
            sponsorshipForm = 25,
            /// <summary>
            /// Pays
            /// </summary>
            country = 26,
            /// <summary>
            /// Basic media
            /// </summary>
            basicMedia = 27,
            /// <summary>
            /// Sub brand
            /// </summary>
            subBrand = 28,
            /// <summary>
            /// Network
            /// </summary>
            network = 29,
            /// <summary>
            /// Region
            /// </summary>
            region = 30,
            /// <summary>
            /// TV Company
            /// </summary>
            tvCompany = 31,
            /// <summary>
            /// Holding
            /// </summary>
            holding = 32,
            /// <summary>
            /// Station
            /// </summary>
            station = 33,
            /// <summary>
            /// Publishing House
            /// </summary>
            publishingHouse = 34,
            /// <summary>
            /// Syndicate
            /// </summary>
            syndicate = 35,
            /// <summary>
            /// Periodic
            /// </summary>
            periodic = 36,
            /// <summary>
            /// Edition Type
            /// </summary>
            editionType = 37,
            /// <summary>
            ///Advertisement Type
            /// </summary>
            advertisementType = 38,
            /// <summary>
            /// National Channel
            /// </summary>
            nationalChannel = 39,
            /// <summary>
            /// TV Channel
            /// </summary>
            tvChannel = 40,
            /// <summary>
            /// Advertisment
            /// </summary>
            advertisment = 41,
            /// <summary>
            /// Break distribution
            /// </summary>
            breakDistribution = 42,
            /// <summary>
            /// Radio holding
            /// </summary>
            radioHolding = 43,
            /// <summary>
            /// Edition
            /// </summary>
            edition = 44,
            /// <summary>
            /// Position
            /// </summary>
            position = 45,
            /// <summary>
            /// St Format
            /// </summary>
            stFormat = 46,
            /// <summary>
            ///St Design
            /// </summary>
            stDesign = 47,
            /// <summary>
            /// Carrier type
            /// </summary>
            carrierType = 48,
            /// <summary>
            ///Outdoor agency
            /// </summary>
            outdoorAgency = 49,
            /// <summary>
            /// Outdoor Network
            /// </summary>
            outdoorNetwork = 50,
            /// <summary>
            /// District
            /// </summary>
            district = 51,
            /// <summary>
            /// Address
            /// </summary>
            address = 52,
            /// <summary>
            /// Site
            /// </summary>
            site = 53,
            /// <summary>
            /// Site section
            /// </summary>
            siteSection = 54,
            /// <summary>
            /// Site subsection
            /// </summary>
            siteSubsection = 55,
            /// <summary>
            /// Advertisment file type
            /// </summary>
            advertismentFileType = 56,
            /// <summary>
            /// Advertisment display type
            /// </summary>
            advertismentDisplayType = 57,
            /// <summary>
            /// Age
            /// </summary>
            age = 58,
            /// <summary>
            /// Gender
            /// </summary>
            gender = 59,
            /// <summary>
            /// Distribution type 
            /// </summary>
            distributionType = 60,
            /// <summary>
            /// Advertisment St format
            /// </summary>
            advertismentStFormat = 61,
            /// <summary>
            /// Type of publication 
            /// </summary>
            publicationType = 62,
            /// <summary>
            /// Circuit VP
            /// </summary>
            vpCircuit = 63,
            /// <summary>
            /// Segmennt VP
            /// </summary>
            vpSegment = 64,
            /// <summary>
            /// Sub Segment VP
            /// </summary>
            vpSubSegment = 65,
            /// <summary>
            /// Brand VP
            /// </summary>
            vpBrand = 66,
            /// <summary>
            /// Product VP
            /// </summary>
            vpProduct = 67,
            /// <summary>
            /// Profession
            /// </summary>
            profession = 68,
            /// <summary>
            /// Name
            /// </summary>
            name = 69,
            /// <summary>
            /// Programme
            /// </summary>
            programme = 70,
            /// <summary>
            /// Programme genre
            /// </summary>
            programmeGenre = 71,
             /// <summary>
             /// Type of presence
             /// </summary>
            presenceType =72,
            /// <summary>
            /// Rubric
            /// </summary>
            rubric = 73,
              /// <summary>
            /// Target
            /// </summary>
            target = 74,
            /// <summary>
            /// wAVE
            /// </summary>
            wave = 75,
            /// <summary>
            /// Location
            /// </summary>
            location = 76,
            /// <summary>
            /// Media Group
            /// </summary>
            mediaGroup = 77,
            /// <summary>
            /// Health Canal
            /// </summary>
            canal = 78

        }
		#endregion

		#region Variables
		/// <summary>
		/// Identitifant du niveau de détail
		/// </summary>
		private DetailLevelItemInformation.Levels _id;
		/// <summary>
		/// Nom du niveau de détail
		/// </summary>
		private string _name;
		/// <summary>
		/// Identifiant du texte du niveau de détail
		/// </summary>
		private Int64 _webTextId;
		/// <summary>
		/// Champ identifiant pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseIdField;
		/// <summary>
		/// Champ libellé logique pour l'identifiant de niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseAliasIdField=null;
		/// <summary>
		/// Champ libellé pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseField;
		/// <summary>
		/// Champ libellé logique pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseAliasField=null;
		/// <summary>
		/// Nom de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseTableName=null;
		/// <summary>
		/// Préfixe de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		private string _dataBaseTableNamePrefix=null;
		/// <summary>
		/// Le niveau doit être utilisé dans le tracking
		/// </summary>
		private bool _toTrack=false;
		/// <summary>
		/// Doit convertir la valeur null d'un identifiant en 0
		/// </summary>
		private bool _convertNullDbId=false;
		/// <summary>
		/// Doit convertir la valeur null d'un champ en 0
		/// </summary>
		private bool _convertNullDbField=false;
		/// <summary>
		/// Relation de données de type jointure externe
		/// </summary>
		private bool _dataBaseExternalJoin=false;
        /// <summary>
        /// Indique si les données peuvent contenir un separateur
        /// </summary>
        private bool _isContainsSeparator = false;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">Identitifant du niveau de détail</param>
		/// <param name="name">Nom du niveau de détail</param>
		/// <param name="webTextId">Identifiant du texte du niveau de détail</param>
		/// <param name="dataBaseIdField">Champ identifiant pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseField">Champ libellé pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseAliasField">Champ libellé logique pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseTableName">Nom de la table pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="dataBaseTableNamePrefix">Préfixe de la table pour le niveau de détail de la base de données AdExpress 3</param>
		/// <param name="toTrack">Le niveau doit être utilisé dans le tracking</param>
		/// <param name="convertNullDbId">Doit convertir la valeur null d'un identifiant en 0</param>
		/// <param name="convertNullDbField">Doit convertir la valeur null d'un champ en 0</param>
		/// <param name="dataBaseAliasIdField">Champ libellé logique pour l'identifiant de niveau de détail de la base de données AdExpress 3</param>
		///<param name="dataBaseExternalJoin">Doit être relié à un autre champ de données par une jointure externe</param>
		public DetailLevelItemInformation(int id,string name,Int64 webTextId,string dataBaseIdField,string dataBaseAliasIdField,bool convertNullDbId,string dataBaseField,string dataBaseAliasField,bool convertNullDbField,string dataBaseTableName,string dataBaseTableNamePrefix,bool toTrack,bool dataBaseExternalJoin){
			if(id<0)throw(new ArgumentException("Invalid argument id"));
			if(name==null ||name.Length<1)throw(new ArgumentException("Invalid argument name"));
			if(webTextId<0)throw(new ArgumentException("Invalid argument webTextId"));
			if(dataBaseIdField==null ||dataBaseIdField.Length<1)throw(new ArgumentException("Invalid argument dataBaseIdField"));
			if(dataBaseField==null ||dataBaseField.Length<1)throw(new ArgumentException("Invalid argument dataBaseField"));  
			try{
				_id=(DetailLevelItemInformation.Levels)id;
			}
			catch(System.Exception err){
				throw(new ArgumentException("Incorrect level Id","id",err));
			}
			_name=name;
			_webTextId=webTextId;
			_dataBaseIdField=dataBaseIdField;
			_dataBaseField=dataBaseField;
			_toTrack=toTrack;
			_convertNullDbId=convertNullDbId;
			_convertNullDbField=convertNullDbField;
			_dataBaseExternalJoin=dataBaseExternalJoin;
			if(dataBaseAliasField!=null && dataBaseAliasField.Length>0)_dataBaseAliasField=dataBaseAliasField;
			if(dataBaseAliasIdField!=null && dataBaseAliasIdField.Length>0)_dataBaseAliasIdField=dataBaseAliasIdField;
			if(dataBaseTableName!=null && dataBaseTableName.Length>0)_dataBaseTableName=dataBaseTableName;
			if(dataBaseTableNamePrefix!=null && dataBaseTableNamePrefix.Length>0)_dataBaseTableNamePrefix=dataBaseTableNamePrefix;
			if(_convertNullDbId && dataBaseAliasIdField==null)throw(new GenericDetailLevelException("Id alias have to be defined if convertNullDbId is true"));
			if(_convertNullDbField && dataBaseAliasField==null)throw(new GenericDetailLevelException("Field alias have to be defined if convertNullDbId is true"));
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient l'identitifant du niveau de détail
		/// </summary>
		public DetailLevelItemInformation.Levels Id{
			get{return(_id);}
		}
		/// <summary>
		/// Obtient le nom du niveau de détail
		/// </summary>
		public string Name{
			get{return(_name);}
		}
		/// <summary>
		/// Obtient l'identifiant du texte du niveau de détail
		/// </summary>
		public Int64 WebTextId{
			get{return(_webTextId);}
		}
		/// <summary>
		/// Obtient le champ identifiant pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseIdField{
			get{return(_dataBaseIdField);}
		}
		/// <summary>
		/// Obtient le champ libellé pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseField{
			get{return(_dataBaseField);}
		}
		/// <summary>
		/// Obtient le champ libellé logique pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseAliasField{
			get{return(_dataBaseAliasField);}
		}
		/// <summary>
		/// Obtient le champ libellé logique pour l'identifiant du niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseAliasIdField{
			get{return(_dataBaseAliasIdField);}
		}
		/// <summary>
		/// Obtient le nom de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseTableName{
			get{return(_dataBaseTableName);}
		}
		/// <summary>
		/// Obtient le préfixe de la table pour le niveau de détail de la base de données AdExpress 3
		/// </summary>
		public string DataBaseTableNamePrefix{
			get{return(_dataBaseTableNamePrefix);}
		}
		/// <summary>
		/// Indique si le niveau doit être utilisé dans le tracking
		/// </summary>
		public bool ToTrack{
			get{return(_toTrack);}
		}
		/// <summary>
		/// Indique si on doit convertir la valeur null d'un identifiant en 0
		/// </summary>
		public bool ConvertNullDbId{
			get{return(_convertNullDbId);}
		}
		/// <summary>
		/// Indique si on doit convertir la valeur null d'un champ en 0
		/// </summary>
		public bool ConvertNullDbField{
			get{return(_convertNullDbField);}
		}

		/// <summary>
		///Relation de données de type jointure externe
		/// </summary>
		public bool DataBaseExternalJoin{
			get{return(_dataBaseExternalJoin);}
		}
        /// <summary>
        ///Obtient \ Définit si les données peuvent contenir un separateur
        /// </summary>
        public bool IsContainsSeparator
        {
            get { return _isContainsSeparator; }
            set { _isContainsSeparator = value; }
        }
		#endregion

		#region Méthode publiques

		#region SQL
		/// <summary>
		/// Obtient le code SQL pour le champ
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlField(){
			string sql="";
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			if(_convertNullDbField)sql+="nvl("+prefix+_dataBaseField+",0)";
			else sql+=prefix+_dataBaseField;
			if(_dataBaseAliasField!=null)sql+=" as "+_dataBaseAliasField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL pour le champ sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldWithoutTablePrefix(){
			string sql="";
			if(_convertNullDbField)sql+="nvl("+_dataBaseField+",0)";
			else sql+=_dataBaseField;
			if(_dataBaseAliasField!=null)sql+=" as "+_dataBaseAliasField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL pour l'identifiant du champ
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldId(){
			string sql="";
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			if(_convertNullDbId)sql+="nvl("+prefix+_dataBaseIdField+",0)";
			else sql+=prefix+_dataBaseIdField;
			if(_dataBaseAliasIdField!=null)sql+=" as "+_dataBaseAliasIdField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL pour l'identifiant du champ sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldIdWithoutTablePrefix(){
			string sql="";
			if(_convertNullDbField)sql+="nvl("+_dataBaseIdField+",0)";
			else sql+=_dataBaseIdField;
			if(_dataBaseAliasIdField!=null)sql+=" as "+_dataBaseAliasIdField;
			return(sql);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForOrder(){
			if(_dataBaseAliasField!=null)return(_dataBaseAliasField);
			return(_dataBaseTableNamePrefix+"."+_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande order sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForOrderWithoutTablePrefix(){
			if(_dataBaseAliasField!=null)return(_dataBaseAliasField);
			return(_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForOrder(){
			if(_dataBaseAliasIdField!=null)return(_dataBaseAliasIdField);
			return(_dataBaseTableNamePrefix+"."+_dataBaseIdField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande order sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForOrderWithoutTablePrefix(){
			if(_dataBaseAliasIdField!=null)return(_dataBaseAliasIdField);
			return(_dataBaseIdField);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande Group By sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForGroupBy(){
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			return(prefix+_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL du le champ pour la commande Group By
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlFieldForGroupByWithoutTablePrefix(){
			return(_dataBaseField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande Group By
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForGroupBy(){
			string prefix="";
			if(_dataBaseTableNamePrefix!=null && _dataBaseTableNamePrefix.Length>0)prefix=_dataBaseTableNamePrefix+".";
			return(prefix+_dataBaseIdField);
		}
		/// <summary>
		/// Obtient le code SQL de l'identifiant de champ pour la commande Group By sans le préfixe de la table
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetSqlIdFieldForGroupByWithoutTablePrefix(){
			return(_dataBaseIdField);
		}

		#endregion

		#region Rules
		/// <summary>
		/// Obtient le libélle du le champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetFieldForRules(){
			if(_dataBaseAliasField!=null)return(_dataBaseAliasField);
			return(_dataBaseField);
		}
		/// <summary>
		/// Obtient le libélle de l'identifiant du le champ pour la commande order
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Code SQL</returns>
		public string GetIdFieldForRules(){
			if(_dataBaseAliasIdField!=null)return(_dataBaseAliasIdField);
			return(_dataBaseIdField);
		}
		#endregion

		/// <summary>
		/// Obtient le nom du champ avec le nom logique (s'il existe)
		/// </summary>
		/// <remarks>La virgule n'est pas ajoutée</remarks>
		/// <returns>Nom du champ avec le nom logique</returns>
		public string GetTableNameWithPrefix(){
			string tmp="";
			if(_dataBaseTableName==null)return(null);
			tmp+=_dataBaseTableName+" ";
			if(_dataBaseTableNamePrefix!=null)tmp+=_dataBaseTableNamePrefix+" ";
			return(tmp);
		}

		#endregion

	}
}
