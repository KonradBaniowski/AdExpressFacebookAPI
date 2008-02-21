using System;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using TNS.FrameWork.DB.Common;


namespace TNS.AdExpress.Rules.Customer {
	/// <summary>
	/// Classe qui sauvegarde les droits des clients dans une hashtables
	/// et qui possède des méthodes pour vérifier la cohérence des droits
	/// </summary>
	
	[System.Serializable]
	public class Right:TNS.AdExpress.DataAccess.Customer.RightDataAccess{
		
		#region Constructeurs

		/// <summary>
		/// Constructeur qui vérifie si l'accès au site est possible
		/// (couple login-password ok, accès au projet adExpress, inscription à au moins un module)
		/// </summary>
		/// <param name="login">login</param>
		/// <param name="password">mot de passe</param>
		/// <param name="source">Source de données</param>
		public Right(string login, string password,IDataSource source):base(login,password){
			
		//	this.oracle="User Id="+login+"; Password="+password+"; Data Source="+TNS.AdExpress.Constantes.DB.TNSName.CUSTOMER_TNS_NAME+" ; validate connection=true ;Pooling=false;Connection Timeout=120";
			
		//	this.oracle="User Id=adexpress_dev2; Password=fachion98; Data Source="+TNS.AdExpress.Constantes.DB.TNSName.CUSTOMER_TNS_NAME+" ";
			this.dateConnexion=DateTime.Now; 
			
			try{
				if(!CheckLogin(source)) {
					throw(new TNS.AdExpress.Exceptions.AdExpressCustomerException("Accès au site impossible")); 
				}
			}
			catch(TNS.AdExpress.Exceptions.AdExpressCustomerDBException ey){
				throw(new TNS.AdExpress.Exceptions.AdExpressCustomerException("Accès au site impossible (Oracle)",ey)); 
			}

		}

		/// <summary>
		/// Constructeur qui sauvegarde les listes de droit des clients 
		/// passées en entrée dans une hashtable
		/// </summary>
		/// <param name="idLogin">Identifiant login</param>
		/// <param name="login">Login</param>
		/// <param name="password">Mot de passe</param>
		/// <param name="sector">Famille en accès</param>
		/// <param name="sectorException">Famille en exception</param>
		/// <param name="subSector">Classe en accès</param>
		/// <param name="subSectorException">Classe en exception</param>
		/// <param name="group">Groupe en accès</param>
		/// <param name="groupException">Groupe en exception</param>
		/// <param name="segment">variété en accès</param>
		/// <param name="segmentException">variété en exception</param>
		/// <param name="holdingCompany">Groupe de sociétés en accès</param>
		/// <param name="holdingCompanyException">Groupe de sociétés en exception</param>
		/// <param name="advertiser">Annonceur en accès</param>
		/// <param name="advertiserException">Annonceur en exception</param>
		/// <param name="vehicle">Média en accès</param>
		/// <param name="VehicleException">Média en exception</param>
		/// <param name="category">Catégorie en accès</param>
		/// <param name="categoryException">Catégorie en exception</param>
		/// <param name="media">support en accès</param>
		/// <param name="mediaException">support en exception</param>
		public Right(Int64 idLogin, string login, string password, string sector, 
			string sectorException,string subSector,string subSectorException,
			string group,string groupException,string segment,string segmentException,
			string holdingCompany,string holdingCompanyException,string advertiser,string advertiserException, string vehicle,
			string VehicleException,string category,string categoryException,string media,string mediaException):base(login,password){
			
			htRight=new Hashtable();
			//On teste si les données sur le login sont correctes
			if(idLogin<0 || login.Length<1 || password.Length<1 ||idLogin.ToString()==null || login==null || password==null )throw(new AdExpress.Exceptions.AdExpressCustomerException("Un des champs suivants est mal renseigné: idLogin,login,password"));
			// On assigne les valeurs du login
			this.idLogin=idLogin;
			//this.login=login;
			//this.password=password;
			//this.oracle="User Id="+login+"; Password="+password+"; Data Source="+TNS.AdExpress.Constantes.DB.TNSName.CUSTOMER_TNS_NAME+" ;Pooling=false;Connection Timeout=120";

			#region Rempli htRight
			
			Regex r=new Regex("^(?<list>([0-9]+([,][0-9]+)*){0,1})$"); 
			
			
			//cas famille en accès			
			if(sector!=null){
				sector=Regex.Replace(sector," ","");
				if(sector.Length>0){			
					if(r.Match(sector).Success){htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess,sector.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne sector en accès"));}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess,null);}

			//cas famille en exception			
			if(sectorException!=null){
			sectorException=Regex.Replace(sectorException," ","");
				if(sectorException.Length>0){			
					if(r.Match(sectorException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorException,sectorException.Split(','));
					}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne sector en Exception"));
					}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorException,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.sectorException,null);}

			
			//cas class en accès			
			if(subSector!=null){
			subSector=Regex.Replace(subSector," ","");
				if(subSector.Length>0){
			
					if(r.Match(subSector).Success){htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,subSector.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne Subsector en accès"));}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess,null);}
			
			//cas class en exception			
			if(subSectorException!=null){
				subSectorException=Regex.Replace(subSectorException," ","");
				if(subSectorException.Length>0){
			
					if(r.Match(subSectorException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException,subSectorException.Split(','));	}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne Subsector en Exception"));}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.subSectorException,null);}
						
			//cas groupe en accès			
			if(group!=null){
				group=Regex.Replace(group," ","");
				if(group.Length>0){			
					if(r.Match(group).Success){htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,group.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne group en accès"));}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess,null);}
			
			
			//cas groupe en exception
			if(groupException!=null){
				groupException=Regex.Replace(groupException," ","");	
				if(groupException.Length>0){			
					if(r.Match(groupException).Success) {
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupException,groupException.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne group en Exception"));
					}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupException,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.groupException,null);}
			
			
			//cas variété en accès
			if(segment!=null){
				segment=Regex.Replace(segment," ","");
				if(segment.Length>0){
		
					if(r.Match(segment).Success){htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess,segment.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne segment en accès"));}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess,null);}

			
			//cas variété en exception
			if(segmentException!=null){
				segmentException=Regex.Replace(segmentException," ","");
				if(segmentException.Length>0){				
					if(r.Match(segmentException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentException,segmentException.Split(','));
					}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne segment en Exception"));}			
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentException,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.segmentException,null);}
			
			
			//cas groupe de sociétés en accès

			if(holdingCompany!=null){
				holdingCompany=Regex.Replace(holdingCompany," ","");
				if(holdingCompany.Length>0) {
					if(r.Match(holdingCompany).Success){htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess,holdingCompany.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne HoldingCompany en accès"));}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess,null);}
						
			//cas groupe de sociétés en exception
			if(holdingCompanyException!=null){
				holdingCompanyException=Regex.Replace(holdingCompanyException," ","");
				if(holdingCompanyException.Length>0){
					if(r.Match(holdingCompanyException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException,holdingCompanyException.Split(','));
					}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne HoldingCompany en exception"));}
				}
				else htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException,null);
			}
			else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException,null);}
			
			//cas annonceur en accès
			if(advertiser!=null){
				advertiser=Regex.Replace(advertiser," ","");
				if(advertiser.Length>0){
					if(r.Match(advertiser).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,advertiser.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne advertiser en accès"));}
				}				
				else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess,null);}
		
			
			//cas annonceur en exception
			if(advertiserException!=null){
				advertiserException=Regex.Replace(advertiserException," ","");
				if(advertiserException.Length>0){
					if(r.Match(advertiserException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException,advertiserException.Split(','));
					}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne advertiser en exception"));}
				}	
				else htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException,null);
			}
			else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.advertiserException,null);}
			
			//cas média en accès
			if(vehicle!=null){
				vehicle=Regex.Replace(vehicle," ","");
				if(vehicle.Length>0){
					if(r.Match(vehicle).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,vehicle.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne vehicle en accès"));}
				}
				else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,null);}
			}
			else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,null);}
			
			//cas média en exception
			if(VehicleException!=null){
				VehicleException=Regex.Replace(VehicleException," ","");
				if(VehicleException.Length>0){
					if(r.Match(VehicleException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleException,VehicleException.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne vehicle en exception"));}
				}
				else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleException,null);}
			}
			else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.vehicleException,null);}
			
			//cas catégorie en accès
			if(category!=null){
				category=Regex.Replace(category," ","");
				if(category.Length>0){
					if(r.Match(category).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess,category.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne category en accès"));}
				}				
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess,null);}
			}
			else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess,null);}
			
			//cas catégorie en exception
			if(categoryException!=null){
				categoryException=Regex.Replace(categoryException," ","");
				if(categoryException.Length>0){
					if(r.Match(categoryException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryException,categoryException.Split(','));
					}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne category en exception"));
					}	
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryException,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.categoryException,null);}
			
			
			//cas support en accès
			if(media!=null){
				media=Regex.Replace(media," ","");
				if(media.Length>0){
					if(r.Match(media).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,media.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne media en accès"));}
				}
				else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,null);}
			}
			else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,null);}
			
			//cas support en exception
			if(mediaException!=null){
				mediaException=Regex.Replace(mediaException," ","");
				if(mediaException.Length>0){
					if(r.Match(mediaException).Success){
						htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaException,mediaException.Split(','));}
					else{throw(new AdExpress.Exceptions.AdExpressCustomerException("Erreur dans la chaîne media en exception"));}
				}
				else {htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaException,null);}
			}
			else{htRight.Add(TNS.AdExpress.Constantes.Customer.Right.type.mediaException,null);}
			
			#endregion		
		}

		#endregion

		#region accesseurs
		/// <summary>
		/// Obtient Identifiant idlogin
		/// </summary>
		public Int64 IdLogin{
			get{return this.idLogin;}			
		}

		/// <summary>
		/// Obtient login
		/// </summary>
		public string Login{
			get{return this.login;}			
		}

		/// <summary>
		/// Obtient mot de passe
		/// </summary>
		public string PassWord{
			get{return this.password;}			
		}

		/// <summary>
		///Obtient et modifie le booléen indiquant si l'utilisateur a le droit de se connecter 
		/// </summary>
		public bool RightValidated{
			get{return this.rightValidated;}
			set{this.rightValidated=value;}
		}
		
		
		

//		public string this [Constantes.Right.type typeRight]{		
//			get{
//				try{
//					if ((string [])htRight[typeRight]==null)return(null);
//					string list="";
//					foreach(string currentItem in (string [])htRight[typeRight]){
//						list+=currentItem+",";
//					}
//					return(list.Substring(0,list.Length-1));
//				}
//				catch(System.Exception e){
//					throw(new AdExpress.Exception.AdExpressCustomerException("il n'y a pas de valeur pour la clé"+typeRight.ToString()+": "+e.Message));
//				}
//			}
//		}


		
		#endregion 



		#region Accès aux données
		
		/// <summary>
		/// Méthode qui retourne un tableau de string correspondant aux éléments d'une liste représentant
		/// un type de droit (famille,media,annonceur...). Ces listes sont soient en accès soient en exception.
		/// </summary>
		/// <param name="typeRight">Choix d'une liste en accès ou en exception</param>
		/// <returns>tableau de string</returns>
		public string[] rightElement(TNS.AdExpress.Constantes.Customer.Right.type typeRight){			
			try{
				return((string [])htRight[typeRight]);
			}
			catch(System.Exception e){
				throw(new AdExpress.Exceptions.AdExpressCustomerException("il n'y a pas de valeur pour la clé"+typeRight.ToString(),e));
			}
		}

		/// <summary>
		/// Retourne une liste de droits séparée par des , .		
		/// </summary>
		/// <param name="typeRight">Choix d'une liste en accès ou en exception</param>
		/// <returns></returns>
		public string rightElementString(TNS.AdExpress.Constantes.Customer.Right.type typeRight){			
			try{
				string tmp="";
				bool premier=true;
				try{
					foreach(string currentItem in (string [])htRight[typeRight]){
						if(premier){
							tmp+=currentItem;
							premier=false;
						}
						else tmp+=","+currentItem;
					}
				}catch(System.Exception){;}

//				if(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess==typeRight){
//					Regex r=new Regex("(^7($|,))|((^|,)7)|(,7,)");
//					if(r.Match(tmp).Success)
//						tmp=tmp.Replace("7","7,6");
//				}

				return(tmp);
			}
			catch(System.Exception e){
				throw(new AdExpress.Exceptions.AdExpressCustomerException("il n'y a pas de valeur pour la clé"+typeRight.ToString(),e));
			}
		}

		/// <summary>
		/// Méthode pour récupérer la fréquence d'un module
		/// </summary>
		/// <param name="idModule">identifiant du module</param>
		/// <returns>Valeur de la fréquence</returns>
		public Int64 getIdFrequency(Int64 idModule){
			try{
				Hashtable htFrequency=(Hashtable)htRight[TNS.AdExpress.Constantes.Customer.Right.type.frequency];
				if(htFrequency[idModule]!=null){
					return (Int64)htFrequency[idModule];
				}
				else return TNS.AdExpress.Constantes.Customer.DB.Frequency.DEFAULT;
			
			}catch(System.Exception e){
				throw(new AdExpress.Exceptions.AdExpressCustomerException("il n'y a pas de valeur pour la fréquence",e));
			}
		}

		
		/// <summary>
		/// Vérifie la cohérence des droits clients dans les 
		/// 3 branches de la nomenclature (produit, média, annonceurs)
		/// </summary>
		/// <returns>retourne false si incohérence</returns>
		public bool checkRightCohesion(){
		
			bool coherence=true;	
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.categoryException]!=null) {
				coherence=this.checkRightCohesionMedia();				
			}
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.sectorException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.groupException]!=null) {
				if(coherence){
					coherence=this.checkRightCohesionProduct();
				}				
			}
			if(this[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException]!=null) {
				if(coherence){
					coherence=this.checkRightCohesionCompany();
				}
			}			
			return coherence;		
		}

		/// <summary>
		/// Vérifie l'existence de droit dans une des trois branches
		/// </summary>
		/// <param name="branch"></param>
		/// <returns></returns>
		public bool branchRightExist(TNS.AdExpress.Constantes.Classification.Branch.type branch){
			if(branch==TNS.AdExpress.Constantes.Classification.Branch.type.media){
				if(this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleException]!=null ||this[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.categoryException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.categoryAccess]!=null) {
					return true;			
				}
				else{return false;}
			}
			else if(branch==TNS.AdExpress.Constantes.Classification.Branch.type.product){
				if(this[TNS.AdExpress.Constantes.Customer.Right.type.sectorException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess]!=null  || this[TNS.AdExpress.Constantes.Customer.Right.type.groupException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.groupAccess]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.segmentException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess]!=null ) {
					return true;
				}
				else{return false;}
			}
			else{
				if(this[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.advertiserException]!=null || this[TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess]!=null) {
					return true;				
				}
				else{return false;}			
			}			
		}

		#endregion
		
		#region méthodes	
		
		/// <summary>
		/// Test l'accès du site
		/// </summary>
		/// <returns>True si le client a accès false sinon</returns>	
		public bool CheckLogin(IDataSource source){
		return CheckLoginDB(source);
		}
	
		/// <summary>
		/// Vérifie que pour un login-password donnée, le project adExpress 
		/// existe avec au moins un module.
		/// </summary>
		/// <returns>Si true assigne idLogin,false sinon</returns>
		public bool CanAccessToAdExpress(IDataSource source){
			return CanAccessToAdExpressDB(source);
		}

		/// <summary>
		/// Vérifie que les droits n'ont pas été modifiés depuis le début
		/// de la connection au site web
		/// </summary>
		/// <returns>true si les droits sont modifiés,false sinon</returns>
		public bool isRightModified(){
		return isRightModifiedDB();
		}

		/// <summary>
		/// Méthode globale 
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <remarks>Méthode n'est pas appelée</remarks>
		public void rightUser(IDataSource source){			
			if(firstRequest) {
				if(CanAccessToAdExpress(source)){
				htRightUserBD();
				firstRequest=false;
					if(checkRightCohesion()){					
						rightValidated=true;
					}
					else{					
						rightValidated=false;
					
					}
				}				
			}
			else{
				if(isRightModified()){
				rightValidated=false;
				
				}
			}
		}

/*		/// <summary>
		/// Récupère le tableau avec les champs suivants
		/// 1ère colonne : idGroupModule 2ème colonne : groupModule
		/// 3ème colonne : idModule 4ème colonne : module
		/// </summary>
		/// <returns>dtModule</returns>
		public DataTable moduleList(){
		return moduleListDB(); 
		}
*/		
		/// <summary>
		/// Remplit les droits d'un utilisateur dans htRight
		/// </summary>
		/// <returns>htRight</returns>
		public Hashtable htRightUser(){
		return htRightUserBD();
		}

		#region Liste des id_vehicle accessibles
		/// <summary>
		/// Retourne la liste des identifiants des vehicles accessibles en fonction des droits clients
		/// (utilisée dans les recap)
		/// </summary>
		/// <remarks>Un vehicle est accessible si au moins un vehicle une categorie au un support est ouvert</remarks>
		/// <returns></returns>
		public string getListIdVehicle(){
			string listVehicle="";
			if(htRightUser()[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap]!=null){
				listVehicle=(string)htRightUser()[TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccessForRecap];
			}
			return (listVehicle);		
		}

		#endregion



		#endregion
	}
}
