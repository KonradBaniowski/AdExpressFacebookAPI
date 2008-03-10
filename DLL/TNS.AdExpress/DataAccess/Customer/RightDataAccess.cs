using System;
using Oracle.DataAccess.Client;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using alias=TNS.AdExpress.Constantes.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;




/*
namespace TNS.AdExpress.DataAccess.Customer{
	/// <summary>
	/// Classe qui vérifie dans la base de données si les droits clients sont valides
	/// </summary>
	[System.Serializable]
	public class RightDataAccess{

		#region Variables

		/// <summary>
		/// clé : type de liste dans la nomenclature en accès ou en exception 
		/// valeur : liste correspondant à la clé 
		/// </summary>
		protected Hashtable htRight;
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
		/// OracleDataReader
		/// </summary>
		[System.NonSerialized]protected OracleDataReader sqlOracleDataReader1=null;
		/// <summary>
		/// OracleCommand
		/// </summary>
		[System.NonSerialized]protected OracleCommand sqlOracleCommand1=null;
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
		/// Chaîne de connection oracle
		/// </summary>
		protected string oracle;
		/// <summary>
		/// bool indiquant si c'est la première connection au site
		/// true si première connection
		/// </summary>
		protected bool firstRequest=true;
		/// <summary>
		/// Oracle connection
		/// </summary>
		[System.NonSerialized]protected OracleConnection connection=null;
		/// <summary>
		/// date de connection
		/// </summary>		
		protected DateTime dateConnexion;
		/// <summary>
		/// Date de modification des droits utilisateur
		/// </summary>
		protected DateTime dateModif;

		#endregion

		#region Constructeur
		
		/// <summary>
		/// Constructeur
		/// </summary>		
		protected RightDataAccess(string login, string password){
			this.login=login;
			this.password=password;
			this.oracle="User Id="+login+"; Password="+password+""+TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING;
		}
		#endregion

		#region Accesseur
		/// <summary>
		/// Obtient et modifie la connection oracle
		/// </summary>
		public OracleConnection Connection{
			get{return this.connection;}
			set{this.connection=value;}
		}
		#endregion

		#region méthodes
		
		
		/// <summary>
		/// Vérifie l'existence du projet adExpress 
		/// avec au moins un module.
		/// Si true assigne idLogin
		/// </summary>
		/// <returns></returns>
		protected bool CanAccessToAdExpressDB(IDataSource source){
			
			bool moduleExist=false;

			#region Construction de la requête
			string  sql="select  distinct ma.id_module";					
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.LOGIN_SCHEMA+".login lo, "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".right_assignment ra, "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".module_assignment ma";
			//	sql+=" from PRIVILEGE01.LOGIN lo, PRIVILEGE01.RIGHT_ASSIGNMENT ra, PRIVILEGE01.MODULE_ASSIGNMENT ma";
			sql+=" where lo.id_login=ra.id_login";
			sql+=" and lo.id_login=ma.id_login";
			sql+=" and lo.id_login="+idLogin+"";					
			sql+=" and ra.id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
			sql+=" and lo.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and ra.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and lo.date_expired>=sysdate";
			sql+=" and ma.date_beginning_module<=sysdate";
			sql+=" and ma.date_end_module>=sysdate";
			sql+=" and ma.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			#endregion

			#region Execution de la requête
			try{
				DataSet ds=source.Fill(sql);
				if(ds.Tables[0].Rows.Count>0)moduleExist=true;
				else moduleExist=false;
			}
			catch(System.Exception){
				moduleExist=false;
			}
			#endregion
			
			return (moduleExist);
		}

		/// <summary>
		/// vérifie le Login-mot de passe
		/// </summary>
		/// <returns>true si login-mot de passe correct, false sinon</returns>
		protected bool CheckLoginDB(IDataSource source){
			bool loginPwdOk=false;

			#region Construction de la requête
			string	sql=" select id_login";
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.LOGIN_SCHEMA+".my_login";
			//	sql+=" from PRIVILEGE01.LOGIN";
			sql+=" where login=upper('"+login+"')";
			sql+=" and password=upper('"+password+"')";
			sql+=" and date_expired>=sysdate";
			sql+=" and activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			#endregion

			#region Execution de la requête
			try{
				DataSet ds=source.Fill(sql);
				foreach(DataRow currentRow in ds.Tables[0].Rows){
					this.idLogin=Int64.Parse(currentRow[0].ToString());
					loginPwdOk=true;
				}
			}
			catch(System.Exception){
				loginPwdOk=false;
			}
			#endregion
			return(loginPwdOk);
		
		}





		/// <summary>
		/// vérifie que les droits n'ont pas été changés
		/// </summary>
		/// <returns>true si les droits n'ont pas été changés</returns>
		protected bool isRightModifiedDB(){

			if(connection==null)
				connection =new OracleConnection(oracle);

			#region ouverture base de données
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}			
				catch(System.Exception e){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de se connecter à la base de données avec le login "+idLogin,e));
				}
			}			
			#endregion

			string	sql=" select date_modification_right";
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".right_assignment";
			sql+=" where id_login="+idLogin+"";
			sql+=" and id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";

			try{
				sqlOracleCommand1=new OracleCommand(sql,connection);
				sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
				if (sqlOracleDataReader1.Read()) {
					dateModif=(DateTime)sqlOracleDataReader1[0];
				}
				#region Fermeture de la base de données
				// Fermeture de la base de données

				if (sqlOracleDataReader1!=null){
					sqlOracleDataReader1.Close();
					sqlOracleDataReader1.Dispose();
				}

				if(sqlOracleCommand1!=null)sqlOracleCommand1.Dispose();
				if (DBToClosed) connection.Close();		
				#endregion 
				
				if(dateModif<dateConnexion){
					return false;
				}
				else{return true;}

			}
				#region	Traitement d'erreur du chargement des données
			catch(System.Exception e){
				try{
					// Fermeture de la base de données

					if (sqlOracleDataReader1!=null){
						sqlOracleDataReader1.Close();
						sqlOracleDataReader1.Dispose();
					}
					if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
				}
					
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
			}			
			#endregion
		}



		/// <summary>
		/// Requête produit
		/// </summary>
		/// <returns></returns>
		protected string htRightProductUserBD(){
		
			//Récupère list_product, l'exception et le type product pour un id_login
			string  sql="select "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".listnum_to_char(list_product)list,exception,tp.id_type_product";
			sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".order_client_product ocp, "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".type_product tp";
			sql+=" where ocp.id_type_product=tp.id_type_product";
			sql+=" and id_login="+idLogin+"";
			sql+=" and id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
			sql+=" and ocp.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql+=" and tp.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			
			return sql;
		
		}

		/// <summary>
		/// Requête media 
		/// </summary>
		/// <returns></returns>
		protected string htRightMediaUserBD(){
		
			string  sql1="select "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".listnum_to_char(list_media)list,exception,tm.id_type_media";
			sql1+=" from "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".order_client_media ocm, "+TNS.AdExpress.Constantes.DB.Schema.RIGHT_SCHEMA+".type_media tm";
			sql1+=" where ocm.id_type_media=tm.id_type_media";
			sql1+=" and id_login="+idLogin+"";
			sql1+=" and id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"";
			sql1+=" and ocm.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
			sql1+=" and tm.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"";
		
			return sql1;
		}

	
		/// <summary>
		/// Remplit les droits d'un utilisateur dans htRight
		/// </summary>
		/// <returns>htRight</returns>
		public Hashtable htRightUserBD(){
			if(connection==null)
				connection =new OracleConnection(oracle);
			
			#region ouverture base de données
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}			
				catch(System.Exception e){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de se connecter à la base de données avec le login "+idLogin,e));
				}
			}			
			#endregion

			#region Variables
			string vh="";
			string vehicleAccess="";
			string vehicleException="";
			string categoryAccess="";
			string categoryException="";
			string mediaAccess="";
			string mediaException="";
			string listVehicleForRecap="";
			string sql="";
			string sql1="";
			#endregion

			htRight=new Hashtable();

			if(this.isProductTemplateExist()){
				sql=htRightProductTemplateUserBD();
			
			
				try{
					sqlOracleCommand1=new OracleCommand(sql,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();

					#region Rempli ds htRight les droits de la classification Product
					while(sqlOracleDataReader1.Read()){
						//sector en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						//sector en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//subsector en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//subsector en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//Group en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//Group en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//Segment en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//Segment en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//Holding_company en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//Holding_company en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//advertiser en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						//advertiser en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}									
					}	
					#endregion
				}
					#region	Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
					
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
				}			
				#endregion			
			
			}
		
			sql=htRightProductUserBD();

			try{
				sqlOracleCommand1=new OracleCommand(sql,connection);
				sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();

				#region Rempli ds htRight les droits de la classification Product
				while(sqlOracleDataReader1.Read()){
					//sector en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
							
						}
						
					}
					//sector en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.sectorException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.sectorException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.sectorException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
						
					}
					//subsector en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess]==null){	
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
						
					}
					//subsector en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SUBSECTOR_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}
					//Group en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.groupAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.groupAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.groupAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}						
					}
					//Group en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_GROUP_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.groupException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.groupException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.groupException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}
					//Segment en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}
					//Segment en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_SEGMENT_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.segmentException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.segmentException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.segmentException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}
					//Holding_company en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}
					//Holding_company en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_HOLDING_COMPANY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}
					//advertiser en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}
					//advertiser en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_ADVERTISER_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.advertiserException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException,sqlOracleDataReader1[0].ToString().Split(',')); 
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.advertiserException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.advertiserException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
					}									
				}	
				#endregion
			}
				#region	Traitement d'erreur du chargement des données
			catch(System.Exception e){
				try{
					// Fermeture de la base de données

					if (sqlOracleDataReader1!=null){
						sqlOracleDataReader1.Close();
						sqlOracleDataReader1.Dispose();
					}
					if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
				}
					
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
			}			
			#endregion			

			

		

			if(this.isMediaTemplateExist()){
				sql1=htRightMediaTemplateUserBD();

				try{
					sqlOracleCommand1=new OracleCommand(sql1,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();

					#region Rempli ds htRight les droits de la classification Media
					while(sqlOracleDataReader1.Read()){
						//Vehicle en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							vh=sqlOracleDataReader1[0].ToString();
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,vh.Split(',')) ;
							vehicleAccess=vh;
							// On ajoute AdNetTrack si Internet est présent
						}
						//Vehicle en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleException,sqlOracleDataReader1[0].ToString().Split(',')) ;
							vehicleException=sqlOracleDataReader1[0].ToString();
						}
						//Category en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess,sqlOracleDataReader1[0].ToString().Split(',')) ;
							categoryAccess=sqlOracleDataReader1[0].ToString();
						}
						//Category en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryException,sqlOracleDataReader1[0].ToString().Split(',')) ;
							categoryException=sqlOracleDataReader1[0].ToString();					
						}
						//média en accès
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,sqlOracleDataReader1[0].ToString().Split(',')) ;
							mediaAccess=sqlOracleDataReader1[0].ToString();
						}
						//média en exception
						if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaException,sqlOracleDataReader1[0].ToString().Split(',')) ;
							mediaException=sqlOracleDataReader1[0].ToString();
						}

					}
					#endregion
				}
					#region	Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
					
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
				}			
				#endregion

			}
			
			sql1=htRightMediaUserBD();

			try{
				sqlOracleCommand1=new OracleCommand(sql1,connection);
				sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();

				#region Rempli ds htRight les droits de la classification Media
				while(sqlOracleDataReader1.Read()){
					//Vehicle en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
						
						if( htRight[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
						if(vehicleAccess.Length>0)
						vehicleAccess+=","+sqlOracleDataReader1[0].ToString();
						else
						vehicleAccess+=sqlOracleDataReader1[0].ToString();

					}
					//Vehicle en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleException,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}	
						if(vehicleException.Length>0)
							vehicleException+=","+sqlOracleDataReader1[0].ToString();
						else
							vehicleException=sqlOracleDataReader1[0].ToString();

					}
					//Category en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
						if(categoryAccess.Length>0)
							categoryAccess+=","+sqlOracleDataReader1[0].ToString();
						else
							categoryAccess=sqlOracleDataReader1[0].ToString();
					}
					//Category en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.categoryException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryException,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.categoryException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.categoryException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
						if(categoryException.Length>0)
							categoryException+=","+sqlOracleDataReader1[0].ToString();
						else
							categoryException=sqlOracleDataReader1[0].ToString();
					}
					//média en accès
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_NOT_EXCEPTION) {
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						else{							
						htRight[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
						if(mediaAccess.Length>0)
							mediaAccess+=","+sqlOracleDataReader1[0].ToString();
						else
							mediaAccess=sqlOracleDataReader1[0].ToString();

					}
					//média en exception
					if((Int64)sqlOracleDataReader1[2]==TNS.AdExpress.Constantes.Customer.DB.MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(sqlOracleDataReader1[1])==TNS.AdExpress.Constantes.DB.ExceptionValues.IS_EXCEPTION) {
						
						if(htRight[TNS.AdExpress.Constantes.Customer.Right.type.mediaException]==null){
							htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaException,sqlOracleDataReader1[0].ToString().Split(',')) ;
						}
						else{
							htRight[TNS.AdExpress.Constantes.Customer.Right.type.mediaException]=listValue((string[])htRight[TNS.AdExpress.Constantes.Customer.Right.type.mediaException],sqlOracleDataReader1[0].ToString().Split(',')).Split(',');
						}
						if(mediaException.Length>0)
							mediaException+=","+sqlOracleDataReader1[0].ToString();
						else
							mediaException=sqlOracleDataReader1[0].ToString();

					}

				}
				#endregion
			}
				#region	Traitement d'erreur du chargement des données
			catch(System.Exception e){
				try{
					// Fermeture de la base de données

					if (sqlOracleDataReader1!=null){
						sqlOracleDataReader1.Close();
						sqlOracleDataReader1.Dispose();
					}
					if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
				}
					
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
			}			
			#endregion
						
		
			#region Recap

			#region Construction de la requete pour recupérer la liste des vehicles (utilisée dans les recap)
			bool premier=true;
			//Requête SQL
			sql="Select distinct vh.id_vehicle from "+alias.Schema.ADEXPRESS_SCHEMA+".vehicle vh,"+alias.Schema.ADEXPRESS_SCHEMA+".category ct,"+alias.Schema.ADEXPRESS_SCHEMA+".basic_media bm,"+alias.Schema.ADEXPRESS_SCHEMA+".media md";
			sql+=" where";
			// Langue
			sql+=" vh.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and ct.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and bm.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and md.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			// Activation
			sql+=" and vh.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;	
			sql+=" and ct.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;	
			sql+=" and bm.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;	
			sql+=" and md.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;	

			// Jointure
			sql+=" and vh.id_vehicle=ct.id_vehicle";
			sql+=" and ct.id_category=bm.id_category";
			sql+=" and bm.id_basic_media=md.id_basic_media";
			

			// Ordre
			
			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(vehicleAccess.Length>0){
				if(beginByAnd) sql+=" and";
				sql+=" ((vh.id_vehicle in ("+vehicleAccess+") ";
				premier=false;
			}
			// Category
			if(categoryAccess.Length>0){
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" ct.id_category in ("+categoryAccess+") ";
				premier=false;
			}
			// Media
			if(mediaAccess.Length>0) {
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" md.id_media in ("+mediaAccess+") ";
				premier=false;
			}
			if(!premier) sql+=" )";

			// Droits en exclusion
			// Vehicle
			if(vehicleException.Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" vh.id_vehicle not in ("+vehicleException+") ";
				premier=false;
			}
			// Category
			if(categoryException.Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" ct.id_category not in ("+categoryException+") ";
				premier=false;
			}
			// Media
			if(mediaException.Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" md.id_media not in ("+mediaException+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			
			#endregion

			try{
				sqlOracleCommand1=new OracleCommand(sql,connection);
				sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();

				#region Rempli ds htRight la liste des vehicles pour les recaps
				while(sqlOracleDataReader1.Read()){
					listVehicleForRecap+=sqlOracleDataReader1[0]+",";
				}
				if(listVehicleForRecap.Length>0){
					htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap,listVehicleForRecap.Substring(0,listVehicleForRecap.Length-1));
				}

				#endregion
			}
				#region	Traitement d'erreur du chargement des données
			catch(System.Exception e){
				try{
					// Fermeture de la base de données

					if (sqlOracleDataReader1!=null){
						sqlOracleDataReader1.Close();
						sqlOracleDataReader1.Dispose();
					}
					if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
				}
					
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
			}			
			#endregion

			#endregion

			#region Fréquence

			string sql2=" select id_module, ma.id_frequency";
			sql2+=" from  "+alias.Schema.RIGHT_SCHEMA+".MODULE_ASSIGNMENT ma, "+alias.Schema.RIGHT_SCHEMA+".FREQUENCY fr";
			sql2+=" where ma.id_login="+idLogin+" ";
			sql2+=" and fr.ID_FREQUENCY=ma.ID_FREQUENCY ";
			sql2+=" and ma.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql2+=" and fr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
						
			Hashtable htFrequency=new Hashtable();


			try{
				sqlOracleCommand1=new OracleCommand(sql2,connection);
				sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();

				#region Rempli ds htRight les droits fréquence
				while(sqlOracleDataReader1.Read()){
					htFrequency.Add(sqlOracleDataReader1[0],sqlOracleDataReader1[1]);
				}
				htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.frequency,htFrequency);	

				#endregion
			}
				#region	Traitement d'erreur du chargement des données
			catch(System.Exception e){
				try{
					// Fermeture de la base de données

					if (sqlOracleDataReader1!=null){
						sqlOracleDataReader1.Close();
						sqlOracleDataReader1.Dispose();
					}
					if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
				}
					
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
			}			
			#endregion
			#endregion

			#region Fermeture de la base de données
			
			try{
				// Fermeture de la base de données

				if (sqlOracleDataReader1!=null){
					sqlOracleDataReader1.Close();
					sqlOracleDataReader1.Dispose();
				}

				if(sqlOracleCommand1!=null)sqlOracleCommand1.Dispose();
				if (DBToClosed) connection.Close();

			}
			catch(System.Exception e){
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de fermer la base de données",e));
			}			
			#endregion 

			return htRight;
		}


		#region Template
		
		/// <summary>
		/// Requête produit
		/// </summary>
		/// <returns></returns>
		public string htRightProductTemplateUserBD(){
			
			string sql="select "+alias.Schema.RIGHT_SCHEMA+".listnum_to_char(list_product)list,exception,"+alias.Tables.TYPE_PRODUCT_PREFIXE+".id_type_product";
			sql+=" from "+alias.Schema.RIGHT_SCHEMA+".order_template_product "+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+", "+alias.Schema.RIGHT_SCHEMA+".type_product "+alias.Tables.TYPE_PRODUCT_PREFIXE+", "+alias.Schema.RIGHT_SCHEMA+".TEMPLATE "+alias.Tables.TEMPLATE_PREFIXE+",";
			sql+=" "+alias.Schema.RIGHT_SCHEMA+".TEMPLATE_ASSIGNMENT "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+"";
			sql+=" where "+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+".id_type_product="+alias.Tables.TYPE_PRODUCT_PREFIXE+".id_type_product";
			sql+=" and "+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+".id_template="+alias.Tables.TEMPLATE_PREFIXE+".id_template";
			//sql+=" and "+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+".id_user_="+alias.Tables.TEMPLATE_PREFIXE+".id_user_ ";
			sql+=" and "+alias.Tables.TEMPLATE_PREFIXE+".id_template="+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_template ";
			//sql+=" and "+alias.Tables.TEMPLATE_PREFIXE+".id_user_="+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_user_ ";
			sql+=" and id_login="+idLogin+" ";
			sql+= "and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+" ";
			sql+=" and "+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and "+alias.Tables.TYPE_PRODUCT_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and "+alias.Tables.TEMPLATE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
				
			return sql;

		}
		
		/// <summary>
		/// Requête Media
		/// </summary>
		/// <returns></returns>
		public string htRightMediaTemplateUserBD(){
		
			string sql1="select "+alias.Schema.RIGHT_SCHEMA+".listnum_to_char(list_media)list,exception,"+alias.Tables.TYPE_MEDIA_PREFIXE+".id_type_media";
			sql1+=" from "+alias.Schema.RIGHT_SCHEMA+".order_template_media "+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+", "+alias.Schema.RIGHT_SCHEMA+".type_media "+alias.Tables.TYPE_MEDIA_PREFIXE+", "+alias.Schema.RIGHT_SCHEMA+".TEMPLATE "+alias.Tables.TEMPLATE_PREFIXE+",";
			sql1+=" "+alias.Schema.RIGHT_SCHEMA+".TEMPLATE_ASSIGNMENT "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+"";
			sql1+=" where "+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+".id_type_media="+alias.Tables.TYPE_MEDIA_PREFIXE+".id_type_media";
			sql1+=" and "+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+".id_template="+alias.Tables.TEMPLATE_PREFIXE+".id_template";
			//sql1+=" and "+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+".id_user_="+alias.Tables.TEMPLATE_PREFIXE+".id_user_ ";
			sql1+=" and "+alias.Tables.TEMPLATE_PREFIXE+".id_template="+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_template ";
			//sql1+=" and "+alias.Tables.TEMPLATE_PREFIXE+".id_user_="+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_user_ ";
			sql1+=" and id_login="+idLogin+" ";
			sql1+= "and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+" ";
			sql1+=" and "+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql1+=" and "+alias.Tables.TYPE_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql1+=" and "+alias.Tables.TEMPLATE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql1+=" and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";

			return sql1;

		}

		#region Vérifie l'existence de template
		/// <summary>
		/// Vérifie l'existance d'un template pour un login dans la branche produit
		/// </summary>
		/// <returns>Retourne true si un template existe</returns>
		public bool isProductTemplateExist(){
			IDataSource source=new OracleDataSource(oracle);
			
			bool isTemplateExist=false;
			int nbreTemplate=0;

			#region requête
			string	sql=" select "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_template ";
			sql+=" from "+alias.Schema.RIGHT_SCHEMA+".TEMPLATE_ASSIGNMENT "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+",";
			sql+=" "+alias.Schema.RIGHT_SCHEMA+".ORDER_TEMPLATE_PRODUCT "+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+" ";
			sql+=" where id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"  ";
			sql+=" and id_login="+idLogin+" ";
			sql+=" and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and "+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_template="+alias.Tables.ORDER_TEMPLATE_PRODUCT_PREFIXE+".id_template ";
			#endregion
					
					

			try{
				DataSet ds=source.Fill(sql);
				if(ds!=null && ds.Tables!=null && ds.Tables.Count>0 && ds.Tables[0]!=null && ds.Tables[0].Rows!=null &&ds.Tables[0].Rows.Count>0){
					nbreTemplate=Convert.ToInt32(ds.Tables[0].Rows[0][0].ToString());
				}
				
			}
			#region	Traitement d'erreur du chargement des données
			catch(System.Exception err){
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",err));				
			}			
			#endregion

			if(nbreTemplate>0){
				isTemplateExist=true;
			}
			
			return isTemplateExist;

		}
	

		
		/// <summary>
		/// Vérifie l'existance d'un template pour un login dans la branche media
		/// </summary>
		/// <returns>Retourne true si un template existe</returns>
		public bool isMediaTemplateExist(){
			if(connection==null)
				connection =new OracleConnection(oracle);
			
			#region ouverture base de données
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}			
				catch(System.Exception e){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de se connecter à la base de données avec le login "+idLogin+" :",e));
				}
			}			
			#endregion

			bool isTemplateExist=false;
			int nbreTemplate=0;

			string	sql=" select "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_template ";
			sql+=" from "+alias.Schema.RIGHT_SCHEMA+".TEMPLATE_ASSIGNMENT "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+",";
			sql+=" "+alias.Schema.RIGHT_SCHEMA+".ORDER_TEMPLATE_MEDIA "+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+" ";
			sql+=" where id_project="+TNS.AdExpress.Constantes.Project.ADEXPRESS_ID+"  ";
			sql+=" and id_login="+idLogin+" ";
			sql+=" and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and "+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+".activation <"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ";
			sql+=" and "+alias.Tables.TEMPLATE_ASSIGNMENT_PREFIXE+".id_template="+alias.Tables.ORDER_TEMPLATE_MEDIA_PREFIXE+".id_template ";
					
					

			try{
				sqlOracleCommand1=new OracleCommand(sql,connection);
				sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
				

				#region regarde si il y a un template produit
				if(sqlOracleDataReader1.Read()){
					nbreTemplate=Convert.ToInt32(sqlOracleDataReader1[0]);
				}	
				#endregion
			}
				#region	Traitement d'erreur du chargement des données
			catch(System.Exception e){
				try{
					// Fermeture de la base de données

					if (sqlOracleDataReader1!=null){
						sqlOracleDataReader1.Close();
						sqlOracleDataReader1.Dispose();
					}
					if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
				}
					
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
			}			
			#endregion


			#region Fermeture de la base de données
			
			try{
				// Fermeture de la base de données

				if (sqlOracleDataReader1!=null){
					sqlOracleDataReader1.Close();
					sqlOracleDataReader1.Dispose();
				}

				if(sqlOracleCommand1!=null)sqlOracleCommand1.Dispose();
				if (DBToClosed) connection.Close();

			}
			catch(System.Exception e){
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de fermer la base de données",e));
			}			
			#endregion 

			if(nbreTemplate>0){
				isTemplateExist=true;
			}
			
			return isTemplateExist;

		}
		#endregion

		#endregion

		#region Cohérence des droits
		/// <summary>
		///Méthode vérifiant la cohérence des droits client dans la branche media
		/// </summary>
		/// <returns>retourne false si il y a une incohérence</returns>
		public bool checkRightCohesionMedia(){
		
			if(connection==null)
				connection =new OracleConnection(oracle);

			#region variables
			//true=cohérence dans les droits false=incohérence dans les droits
			bool mediaBool=true;		
			string vehicleKo= this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException];		
			string categoryOk=this[TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess];
			string categoryKo=this[TNS.AdExpress.Constantes.Customer.Right.type.categoryException];
			string mediaOk=this[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess];
			#endregion

			#region ouverture base de données
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}			
				catch(System.Exception e){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de se connecter à la base de données avec le login "+idLogin,e));
				}
			}			
			#endregion

			//1) cas vehicle-category
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess]!=null) {	 
				string sql="select count(distinct id_category) as nb from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".category";
				sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql+=" and id_vehicle  not in ("+vehicleKo+")";
				sql+=" and id_category in("+categoryOk+")";
				
				try{
					sqlOracleCommand1=new OracleCommand(sql,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();

					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region	Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
					
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Problème lors de la lecture des données",e));				
				}			
				#endregion
				
				//vérifie la cohérence des droits entre la base de données 
				//et la liste
				if(nbLineBD!=categoryOk.Split(',').Length) {
					mediaBool=false; 
				}
			}
			
			//2) cas vehicle-media
			
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess]!=null){
				
				#region requête
				string  sql1=" select count(distinct id_media) as nb ";
				sql1+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".category ct,";	
				sql1+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".basic_media bm,";
				sql1+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".media md";
				sql1+=" where ct.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql1+=" AND bm.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql1+=" AND ct.id_category=bm.id_category";
				sql1+=" AND md.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql1+=" AND bm.id_basic_media=md.id_basic_media";
				sql1+=" and id_vehicle  not in ("+vehicleKo+")";
				sql1+=" and id_media in("+mediaOk+")";
				#endregion							

				try{
					sqlOracleCommand1=new OracleCommand(sql1,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}				
					
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
				
				if(nbLineBD!=mediaOk.Split(',').Length) {
					mediaBool=false; 
				}
			}

			//3) cas category-media
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.categoryException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess]!=null){
				
				#region requête
				string sql2=" select count(distinct id_media) as nb";
				sql2+=" from  "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".basic_media bm,";					
				sql2+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".media md ";
				sql2+=" where bm.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql2+=" AND md.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql2+=" AND bm.id_basic_media=md.id_basic_media";
				sql2+=" and bm.id_category  not in ("+categoryKo+")";
				sql2+=" and id_media in("+mediaOk+")";								
				#endregion

				try{
					sqlOracleCommand1=new OracleCommand(sql2,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
				
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
				
				if(nbLineBD!=mediaOk.Split(',').Length) {
					mediaBool=false; 
				}
			}

			#region Fermeture de la base de données
			
			try{
				// Fermeture de la base de données

				if (sqlOracleDataReader1!=null){
					sqlOracleDataReader1.Close();
					sqlOracleDataReader1.Dispose();
				}

				if(sqlOracleCommand1!=null)sqlOracleCommand1.Dispose();
				if (DBToClosed) connection.Close();

			}
			catch(System.Exception e){
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de fermer la base de données :",e));
			}			
			#endregion

			return mediaBool;	

		}

		/// <summary>
		/// Méthode vérifiant la cohérence des droits client dans la branche produit
		/// </summary>
		/// <returns>retourne false si il y a une incohérence</returns>
		protected bool checkRightCohesionProduct(){
		
			if(connection==null)
				connection =new OracleConnection(oracle);

			#region variables
			//true=cohérence dans les droits false=incohérence dans les droits
			bool productBool=true;
			string sectorKo= this[TNS.AdExpress.Constantes.Customer.Right.type.sectorException];		
			string subsectorOk=this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess];
			string subsectorKo=this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException];
			string groupOk=this[TNS.AdExpress.Constantes.Customer.Right.type.groupAccess];
			string groupKo=this[TNS.AdExpress.Constantes.Customer.Right.type.groupException];
			string segmentOk=this[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess];
			#endregion
		
			#region ouverture base de données
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}			
				catch(System.Exception e){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de se connecter à la base de données avec le login "+idLogin,e));
				}
			}			
			#endregion

			//cas sector-subsector
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.sectorException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess]!=null){
				#region requête	
				string sql="select count(distinct id_subsector) as nb from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".subsector";
				sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql+=" and id_sector not in ("+sectorKo+")";
				sql+=" and id_subsector in("+subsectorOk+")";				
				#endregion

				try{
					sqlOracleCommand1=new OracleCommand(sql,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}		
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
				
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion							

				if(nbLineBD!=subsectorOk.Split(',').Length) {
					productBool=false; 
				}
			}
			//cas sector-group
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.sectorException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.groupAccess]!=null){
		
				#region requête
				string  sql1="select count(distinct ID_GROUP_) as nb";
				sql1+=" from  "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".group_  gp,";
				sql1+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".subsector ss";
				sql1+=" where gp.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql1+=" and ss.id_subsector=gp.id_subsector";
				sql1+=" and ss.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql1+=" and id_sector not in("+sectorKo+")";
				sql1+=" and id_group_ in("+groupOk+")";				
				#endregion

				try{
					sqlOracleCommand1=new OracleCommand(sql1,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
				
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
			
				if(nbLineBD!=groupOk.Split(',').Length) {
					productBool=false; 
				}
			}
			//cas sector-segment
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.sectorException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess]!=null){
				#region requête
				string	sql2="select count(distinct id_segment)";
				sql2+=" from  "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".subsector ss,";
				sql2+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".group_ gp,";
				sql2+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".segment sg";
				sql2+=" where ss.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql2+=" and ss.id_subsector=gp.id_subsector";
				sql2+=" and gp.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql2+=" and gp.id_group_=sg.id_group_";
				sql2+=" and sg.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql2+=" and id_sector not in("+sectorKo+")";
				sql2+=" and id_segment in("+segmentOk+")";
				#endregion
				
				try{
					sqlOracleCommand1=new OracleCommand(sql2,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
				
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
			
				if(nbLineBD!=segmentOk.Split(',').Length) {
					productBool=false; 
				}
			}
			//cas subsector-group
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.groupAccess]!=null){
				#region requête
				string	sql3="select count(distinct id_group_)";
				sql3+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".group_";
				sql3+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql3+=" and id_subsector not in ("+subsectorKo+")";
				sql3+=" and id_group_  in ("+groupOk+")";
				#endregion
					
				try{
					sqlOracleCommand1=new OracleCommand(sql3,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
					
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
							
				if(nbLineBD!=groupOk.Split(',').Length) {
					productBool=false; 
				}
			}

			//cas subsector-segment
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess]!=null){
			
				#region requête
				string  sql4="select count(distinct id_segment)";
				sql4+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".group_ gp,";
				sql4+=" "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".segment sg";
				sql4+=" where gp.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql4+=" and gp.id_group_=sg.id_group_";
				sql4+=" and sg.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql4+=" and id_subsector not in ("+subsectorKo+")";
				sql4+=" and id_segment in ("+segmentOk+")";
				#endregion
								
				try{
					sqlOracleCommand1=new OracleCommand(sql4,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
					
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
							
				if(nbLineBD!=segmentOk.Split(',').Length) {
					productBool=false; 
				}
			}
			//cas group-segment
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.groupException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess]!=null){
				
				#region requête
				string  sql5=" select count(distinct id_segment)";
				sql5+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".segment";
				sql5+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql5+=" and id_group_  not in ("+groupKo+")";
				sql5+=" and  id_segment in ("+segmentOk+")";		
				#endregion

				try{
					sqlOracleCommand1=new OracleCommand(sql5,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
					
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
							
				if(nbLineBD!=segmentOk.Split(',').Length) {
					productBool=false; 
				}
			}
			#region Fermeture de la base de données
			
			try{
				// Fermeture de la base de données

				if (sqlOracleDataReader1!=null){
					sqlOracleDataReader1.Close();
					sqlOracleDataReader1.Dispose();
				}

				if(sqlOracleCommand1!=null)sqlOracleCommand1.Dispose();
				if (DBToClosed) connection.Close();

			}
			catch(System.Exception e){
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de fermer la base de données",e));
			}			
			#endregion       		
			
			return productBool;
		}

		/// <summary>
		/// Méthode vérifiant la cohérence des droits client dans la branche annonceur
		/// </summary>
		/// <returns>retourne false s'il y a une incohérence</returns>
		protected bool checkRightCohesionCompany(){
			
			if(connection==null)
				connection =new OracleConnection(oracle);
			bool companyBool=true;
			string holdingCompanyKo=this[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException];		
			string advertiserOk=this[TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess];

			#region ouverture base de données
			bool DBToClosed=false;
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}			
				catch(System.Exception e){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de se connecter à la base de données avec le login "+idLogin,e));
				}
			}			
			#endregion

			//cas holdingCompany-advertiser
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException]!=null && this[TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess]!=null){
			
				#region requête
				string  sql="select count(distinct id_advertiser)";
				sql+=" from "+TNS.AdExpress.Constantes.DB.Schema.ADEXPRESS_SCHEMA+".advertiser";
				sql+=" where id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH+"";
				sql+=" and id_holding_company not in ("+holdingCompanyKo+")";
				sql+=" and id_advertiser in ("+advertiserOk+")";				
				#endregion

				try{
					sqlOracleCommand1=new OracleCommand(sql,connection);
					sqlOracleDataReader1=sqlOracleCommand1.ExecuteReader();
					if(sqlOracleDataReader1.Read()){
						nbLineBD=Convert.ToInt32(sqlOracleDataReader1[0]);
					}
				}
					#region Traitement d'erreur du chargement des données
				catch(System.Exception e){
					try{
						// Fermeture de la base de données

						if (sqlOracleDataReader1!=null){
							sqlOracleDataReader1.Close();
							sqlOracleDataReader1.Dispose();
						}
						if(sqlOracleCommand1!=null) sqlOracleCommand1.Dispose();
						if (DBToClosed) connection.Close();
					}
					catch(System.Exception et){
						throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("impossible de fermer la base de données",et));
					}
				
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException(e.Message));}
				#endregion
			
				if(nbLineBD!=advertiserOk.Split(',').Length) {
					companyBool=false; 
				}
			}
			#region Fermeture de la base de données
			
			try{
				// Fermeture de la base de données

				if (sqlOracleDataReader1!=null){
					sqlOracleDataReader1.Close();
					sqlOracleDataReader1.Dispose();
				}

				if(sqlOracleCommand1!=null)sqlOracleCommand1.Dispose();
				if (DBToClosed) connection.Close();

			}
			catch(System.Exception e){
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerDBException("Impossible de fermer la base de données",e));
			}			
			#endregion      				

			return companyBool;
		}

		#endregion

		#endregion

		#region accesseur
		/// <summary>
		/// Retourne un  string correspondant aux éléments d'une liste représentant
		/// un type de droit (famille,media,annonceur...). Ces listes sont soient en accès soient en exception.
		/// </summary>
		/// <param name="typeRight">Choix d'une liste en accès ou en exception</param>
		/// <returns>string d'une liste</returns>
		public string this [TNS.AdExpress.Constantes.Customer.Right.type typeRight]{		
			get{
				try{
					//on test si le tableau est vide
					if ((string [])htRight[typeRight]==null)return(null);
					string list="";
					//on créée la liste
					foreach(string currentItem in (string [])htRight[typeRight]){
						list+=currentItem+",";
					}
					return(list.Substring(0,list.Length-1));
				}
				catch(System.Exception e){
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerException("Impossible de créer la liste",e));
				}
			}
		}
		/// <summary>
		/// Obtient la chaîne de connexion oracle
		/// </summary>
		public string OracleConnectionString{
			get{
				if(oracle!=null)return(oracle);
				else return("OracleConnection is null");
			}
		}

		#endregion

		#region Méthodes internes
		/// <summary>
		/// Retourne la liste des droits
		/// </summary>
		/// <param name="tab1"></param>
		/// <param name="tab2"></param>
		/// <returns></returns>
		protected string listValue(string [] tab1,string[] tab2){

			string res="";
			int i=0;
			int j=0;
			bool notExist=true;
			for(i=0;i<tab1.Length;i++){
				res+=tab1[i]+",";			
			}
			for(i=0;i<tab2.Length;i++){
				notExist=true;
					for(j=0;j<tab1.Length;j++){
						if(tab1[j]==tab2[i]){
							notExist=false;
						}
					}
				if(notExist){
					res+=tab2[i]+",";
				}			
			}			
			return (res.Substring(0,res.Length-1));	
		}
		#endregion

	}
}
*/