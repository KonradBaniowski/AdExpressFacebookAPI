#region Informations
// Auteur: A. Obermeyer
// Date de création: 01/12/2004
// Date de modification: 
//	06/12/2004	D.V. Mussuma				ajout méthode getHTMLPortofolioUI(Page page,object[,] tab,WebSession webSession)
//	03/05/2005	K. Shehzad					changement pour rappel des paramètres pour en tête d'Excel et 3 virgules pour pages 
//	27/06/2005	B. Masson & D.V. Mussuma	Ajout du html pour les Excel des données brutes (detail du portefeuille et calendrier d'action)
//				K.shehzad					Outdoor
//	12/08/2005	G. Facon					Nom de fonction et gestion des exceptions
//	26/10/2005	B.Masson / D.Mussuma		Intégration des unités (centralisée)
#endregion

#region Namespace
using System;
using System.Web.UI;
using System.Windows.Forms;
using System.IO;
using System.Data;
using System.Collections;
using System.Text;
using TNS.FrameWork.Date;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.UI.Selections.Periods;
using TNS.AdExpress.Web.DataAccess.Selections.Periods;
using TNS.AdExpress.Web.Core.Translation;
using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Web.Core.Navigation;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.Portofolio;
using ResultConstantesCalendar=TNS.AdExpress.Constantes.FrameWork.Results.PortofolioCalendar;   
using CstDB = TNS.AdExpress.Constantes.DB;
using WebConstantes=TNS.AdExpress.Constantes.Web;       
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Constantes.FrameWork.Results;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using ResultCstComp=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
using TNS.FrameWork;
using WebDataAccess=TNS.AdExpress.Web.DataAccess;
#endregion

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Génération du code HTML pour les résultats d'un portefeuille
	/// </summary>
	public class PortofolioUI{

		#region Constantes
		/// <summary>
		/// constante pour le style, utiliséé pour les libellés, fond blanc
		/// </summary>
		protected const string portofolioTitle1="portofolio2";
		/// <summary>
		/// constante pour le style, utiliséé pour les valeurs, fond blanc
		/// </summary>
		protected const string portofolioValue1="portofolio22";		
		/// <summary>
		/// constante pour le style, utiliséé pour les libellés, fond violet
		/// </summary>
		protected const string portofolioTitle2="portofolio1";
		/// <summary>
		/// constante pour le style, utiliséé pour les valeurs, fond violet
		/// </summary>
		protected const string portofolioValue2="portofolio11";		
		#endregion		
		
		#region Synthèse
		/// <summary>
		/// Synthèse portefeuille
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		///<param name="excel">booléen à true si excel</param>
		/// <returns></returns>
		public static string Synthesis(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,bool excel){
			
			#region Variables
			string investment="";
			string firstDate="";
			string lastDate="";
			string support="";	
			string periodicity="";
			string category="";
			string classStyleTitle="";	
			string classStyleValue="";	
			string regie="";
			string interestCenter="";
			string pageNumber="";
			string ojd="";
			string pathWeb="";
			string nbrSpot="";
			string nbrEcran="";
			decimal averageDurationEcran=0;
			decimal nbrSpotByEcran=0;
			string totalDuration="";
			string numberBoard="";
			//string typeSale="";
			StringBuilder t=new StringBuilder(5000);
		
			DataSet dsVisuel=null;
			
			#endregion
			//Module currentModuleDescription=TNS.AdExpress.Web.Rules.Selections.Modules.ModulesListRules.GetModule(webSession.CurrentModule);
		
			#region Accès aux tables
			DataSet dsInvestment=PortofolioDataAccess.GetInvestment(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			DataTable dtInvestment=dsInvestment.Tables[0];

			DataSet dsInsertionNumber=PortofolioDataAccess.GetInsertionNumber(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			DataTable dtInsertionNumber=dsInsertionNumber.Tables[0];

			DataSet dsCategory=PortofolioDataAccess.GetCategoryMediaSellerData(webSession,idVehicle,idMedia);
			DataTable dtCategory=dsCategory.Tables[0];

			DataSet dsPage=PortofolioDataAccess.GetPage(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			DataTable dtPage=dsPage.Tables[0];			
			dsVisuel=PortofolioDateDataAccess.GetListDate(webSession,idVehicle,idMedia,dateBegin,dateEnd,true);		


			DataTable dtVisuel=dsVisuel.Tables[0];

			DataTable dtTypeSale=null;
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.outdoor)
			{
				DataSet dsTypeSale=PortofolioDataAccess.GetTypeSale(webSession,idVehicle,idMedia,dateBegin,dateEnd);
				dtTypeSale=dsTypeSale.Tables[0];

			}
			
			object [] tab=PortofolioDataAccess.NumberProductAdvertiser(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			object [] tabEncart=null;
			

			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
				){			
				tabEncart=PortofolioDataAccess.NumberPageEncart(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			}
			#endregion

			#region Parcours des tableaux
			foreach(DataRow row in dtInvestment.Rows){
				investment=row["investment"].ToString();
				firstDate=row["first_date"].ToString();
				lastDate=row["last_date"].ToString();
//				if(dtInvestment.Columns.Contains("insertion")){
//					nbrSpot=row["insertion"].ToString();
//				}
				if(dtInvestment.Columns.Contains("duration")){
					totalDuration=row["duration"].ToString();
				}		
				if(dtInvestment.Columns.Contains("number_board"))
				{
					numberBoard=row["number_board"].ToString();
					
				}
//				if(dtInvestment.Columns.Contains("type_sale"))
//				{
//					typeSale=row["type_sale"].ToString();
//				}
			}
			//nombre d'insertions
			if(!dtInsertionNumber.Equals(System.DBNull.Value) && dtInsertionNumber.Rows.Count>0){
				nbrSpot=dtInsertionNumber.Rows[0]["insertion"].ToString();			
			}
			foreach(DataRow row in dtCategory.Rows){
				support=row["support"].ToString();
				category=row["category"].ToString();
				regie=row["media_seller"].ToString();
				interestCenter=row["interest_center"].ToString();
				if(dtCategory.Columns.Contains("periodicity"))
					periodicity=row["periodicity"].ToString();
				if(dtCategory.Columns.Contains("ojd"))
					ojd=row["ojd"].ToString();
			}

			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
				){
				foreach(DataRow row in dtPage.Rows){
					pageNumber=row["page"].ToString();				
			
				}
			}

			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.radio
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.tv
				|| (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.others
				){
				DataSet dsEcran=PortofolioDataAccess.GetEcranData(webSession,idVehicle,idMedia,dateBegin,dateEnd);
				DataTable dtEcran=dsEcran.Tables[0];

				foreach(DataRow row in dtEcran.Rows){
					nbrEcran=row["nbre_ecran"].ToString();
					if(row["nbre_ecran"]!=System.DBNull.Value){
						averageDurationEcran=decimal.Parse(row["ecran_duration"].ToString())/decimal.Parse(row["nbre_ecran"].ToString());
						nbrSpotByEcran=decimal.Parse(row["nbre_spot"].ToString())/decimal.Parse(row["nbre_ecran"].ToString());
					}			
						
				
				}
			}
			#endregion

			#region Période
			DateTime dtFirstDate=DateTime.Today;
			DateTime dtLastDate=DateTime.Today;
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.outdoor)
			{
				if(firstDate.Length>0)
				{
					dtFirstDate=Convert.ToDateTime(firstDate);
					dtFirstDate=dtFirstDate.Date;
				}
				if(lastDate.Length>0)
				{
					dtLastDate=Convert.ToDateTime(lastDate);
					dtLastDate=dtLastDate.Date;
				}
			}
			else
			{
				if(firstDate.Length>0)
				{
					dtFirstDate=new DateTime(int.Parse(firstDate.Substring(0,4)),int.Parse(firstDate.Substring(4,2)),int.Parse(firstDate.Substring(6,2)));
				}
			
				if(lastDate.Length>0)
				{
					dtLastDate=new DateTime(int.Parse(lastDate.Substring(0,4)),int.Parse(lastDate.Substring(4,2)),int.Parse(lastDate.Substring(6,2)));
				}
			}
			#endregion

			#region HTML
			t.Append("<table  border=0 cellpadding=0 cellspacing=0 width=600 >");			

			//Titre du support
            t.Append("\r\n\t<tr height=\"30px\"><td colspan=2 class=\"portofolioSynthesisBorderHeader\" align=\"center\">" + support + "</td></tr>");	
			// Date de début et fin de vague pour affichage
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.outdoor)
			{
				t.Append("\r\n\t<tr><td class=\""+portofolioTitle1+"\" width=50%>"+GestionWeb.GetWebWord(1607,webSession.SiteLanguage)+" : "+"</td><td class=\""+portofolioValue1+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+dtFirstDate.Date.ToString("dd/MM/yyyy")+"</td></tr>");	
				t.Append("\r\n\t<tr><td class=\""+portofolioTitle2+"\" width=50%>"+GestionWeb.GetWebWord(1608,webSession.SiteLanguage)+" : "+"</td><td class=\""+portofolioValue2+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+dtLastDate.Date.ToString("dd/MM/yyyy")+"</td></tr>");	
		
			}
				// Date de parution ou diffusion
			else
			{
				if(firstDate.Length>0)
				{
					if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
						|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
						)
					{
						//Cas Presse : Date de parution 
						t.Append("\r\n\t<tr  ><td class=\""+portofolioTitle1+"\" width=50%>"+GestionWeb.GetWebWord(1381,webSession.SiteLanguage)+"</td>");
					}
					else
					{
						// Cas TV-Radio : Date de diffusion
						t.Append("\r\n\t<tr ><td class=\""+portofolioTitle1+"\" width=50%>"+GestionWeb.GetWebWord(1380,webSession.SiteLanguage)+"</td>");
					}
					if(firstDate==lastDate)
					{
						t.Append("<td class=\""+portofolioValue1+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+dtFirstDate.Date.ToString("dd/MM/yyyy")+"</td></tr>");
					}
					else
					{
						
						t.Append("<td class=\""+portofolioValue1+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;Du "+dtFirstDate.Date.ToString("dd/MM/yyyy")+" au "+dtLastDate.Date.ToString("dd/MM/yyyy")+"</td></tr>");
					}
				}
			}
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.outdoor)
			{
				classStyleTitle=portofolioTitle1;	
				classStyleValue=portofolioValue1;
			}
			else
			{
				classStyleTitle=portofolioTitle2;	
				classStyleValue=portofolioValue2;
			}
			// Périodicité
			if(dtCategory.Columns.Contains("periodicity")){
				t.Append("<tr><td class=\""+portofolioTitle2+"\" width=50%>"+GestionWeb.GetWebWord(1450,webSession.SiteLanguage)+"</td><td class=\""+portofolioValue2+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+periodicity+"</td></tr>");
				classStyleTitle=portofolioTitle1;	
				classStyleValue=portofolioValue1;	
			}
			// Categorie
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1416,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+category+"</td></tr>");
			
			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);	
			// Régie
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1417,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+regie+"</td></tr>");	

			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);	
			
			// Centre d'intérêt
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1411,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+interestCenter+"</td></tr>");	

			//number board et type de reseau ,cas de l'Affichage
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.outdoor&&dtTypeSale.Rows.Count>0)
			{
				t.Append("<tr><td class=\""+portofolioTitle2+"\" width=50%>"+GestionWeb.GetWebWord(1604,webSession.SiteLanguage)+"</td><td class=\""+portofolioValue2+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+numberBoard+"</td></tr>");	
				int count=0;						
				t.Append("<tr><td VALIGN=top class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1609,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>");	
				if(dtTypeSale.Rows.Count==0)t.Append("&nbsp;");
				else
				{
					foreach(DataRow row in dtTypeSale.Rows)
					{
						if(count>0)
						{
							t.Append("<BR>");
						}
						t.Append("&nbsp;&nbsp;&nbsp;&nbsp;"+WebFunctions.SQLGenerator.SaleTypeOutdoor(row["type_sale"].ToString(),webSession.SiteLanguage));						
						count++;
					}
				}
				t.Append("</td></tr>");
					
			}
			// Cas de la presse
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
				){
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);	
				// Nombre de page
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+pageNumber+"</td></tr>");
				if(((string)tabEncart[0]).Length>0){
					classStyleTitle=InversClassStyleTitle(classStyleTitle);
					classStyleValue=InversClassStyleValue(classStyleValue);
					// Nombre de page pub				
					t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1386,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+WebFunctions.Units.ConvertUnitValueAndPdmToString((string)tabEncart[0],WebConstantes.CustomerSessions.Unit.pages,false)+"</td></tr>");

					classStyleTitle=InversClassStyleTitle(classStyleTitle);
					classStyleValue=InversClassStyleValue(classStyleValue);
					// Ratio
					t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1387,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((decimal)(decimal.Parse(WebFunctions.Units.ConvertUnitValueAndPdmToString((string)tabEncart[0],WebConstantes.CustomerSessions.Unit.pages,false))/decimal.Parse(pageNumber)*100)).ToString("0.###")+" %</td></tr>");	
				}			

				if(((string)tabEncart[1]).Length>0){
					classStyleTitle=InversClassStyleTitle(classStyleTitle);
					classStyleValue=InversClassStyleValue(classStyleValue);
					// Nombre de page de pub hors encarts
					if(((string)tabEncart[1]).Length==0)
						tabEncart[1]="0";
					t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1388,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+WebFunctions.Units.ConvertUnitValueAndPdmToString((string)tabEncart[1],WebConstantes.CustomerSessions.Unit.pages,false)+"</td></tr>");	
				}
				if(((string)tabEncart[2]).Length>0){
					classStyleTitle=InversClassStyleTitle(classStyleTitle);
					classStyleValue=InversClassStyleValue(classStyleValue);
					// Nombre de page de pub encarts
					if(((string)tabEncart[2]).Length==0){
						tabEncart[2]="0";
					}
					t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1389,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+WebFunctions.Units.ConvertUnitValueAndPdmToString((string)tabEncart[2],WebConstantes.CustomerSessions.Unit.pages,false)+"</td></tr>");	
				}				
			}

			// Cas tv, radio
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.radio
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.tv
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.others

				)
			{
				
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);					
				//Nombre de spot
				if(nbrSpot.Length==0)
				{
					nbrSpot="0";
				}
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1404,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+nbrSpot+"</td></tr>");

				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);	
				// Nombre d'ecran
				if(nbrEcran.Length==0)
				{
					nbrEcran="0";
				}
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1412,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+nbrEcran+"</td></tr>");

				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);

				if(totalDuration.Length>0) 
				{
					//totalDuration=GetDurationFormatHH_MM_SS(int.Parse(totalDuration));
					totalDuration=WebFunctions.Units.ConvertUnitValueAndPdmToString(totalDuration,WebConstantes.CustomerSessions.Unit.duration,false);
				}
				else
				{
					totalDuration="0";
				}
				// total durée
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1413,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+totalDuration+"</td></tr>");
			}

			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);
			// Total investissements
			if(investment.Length>0){
//				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1390,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;"+Int64.Parse(investment).ToString("### ### ###")+"</td></tr>");	
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1390,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;"+WebFunctions.Units.ConvertUnitValueAndPdmToString(investment,WebConstantes.CustomerSessions.Unit.euro,false)+"</td></tr>");	
			}			

			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);
			//Nombre de produits
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1393,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+(int)tab[0]+"</td></tr>");
	
			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())!=DBClassificationConstantes.Vehicles.names.outdoor)
			{
				//Nombre de nouveaux produits dans la pige
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1394,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+(int)tab[2]+"</td></tr>");
	
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);
				//Nombre de nouveaux produits dans le support
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50% nowrap>"+GestionWeb.GetWebWord(1395,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50% >&nbsp;&nbsp;&nbsp;&nbsp;"+(int)tab[1]+"</td></tr>");
			}
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())!=DBClassificationConstantes.Vehicles.names.outdoor)
			{
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);
			}
			//Nombre d'annonceurs
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1396,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+(int)tab[3]+"</td></tr>");	
					
		
			// Cas tv, presse
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.radio
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.tv
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.others
				){
				
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);	
				// Durée moyenne d'un écran
				//t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1414,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+GetDurationFormatHH_MM_SS((int)averageDurationEcran)+"</td></tr>");
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1414,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+WebFunctions.Units.ConvertUnitValueAndPdmToString( ((long)averageDurationEcran).ToString() ,WebConstantes.CustomerSessions.Unit.duration,false)+"</td></tr>");

				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);	
				// Nombre moyen de spots par écran
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1415,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+nbrSpotByEcran.ToString("0.00")+"</td></tr>");
				
			}


			#region Chemin de fer
			// Vérifie si le client a le droit aux créations
			//if(webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_CREATION_ACCESS_FLAG)!=null){
            if (webSession.CustomerLogin.ShowCreatives((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()))) {
				if(!excel){
					if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
						|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
						){

						Hashtable htValue=PortofolioDataAccess.GetInvestmentByMedia(webSession,idVehicle,idMedia,dateBegin,dateEnd);

						t.Append("</table>");
						int compteur=0;
						string endBalise="";
						string day="";
						t.Append("<table  border=1 cellpadding=0 cellspacing=0 width=600 class=\"paleVioletBackGroundV2 violetBorder\">");
						//Chemin de fer
                        t.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc12Bold violetBackGround portofolioSynthesisBorder\" align=\"center\">" + GestionWeb.GetWebWord(1397, webSession.SiteLanguage) + "</td></tr>");
						for(int i=0;i<dtVisuel.Rows.Count;i++) {
							//date_media_num

							if(dtVisuel.Rows[i]["disponibility_visual"]!=System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString())>=10){
								pathWeb=CstWeb.CreationServerPathes.IMAGES+"/"+idMedia.ToString()+"/"+dtVisuel.Rows[i]["date_cover_num"].ToString()+"/Imagette/"+CstWeb.CreationServerPathes.COUVERTURE+"";
							}
							else{
								pathWeb="/Images/"+webSession.SiteLanguage+"/Others/no_visuel.gif";
							}
							DateTime dayDT=new DateTime(int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(0,4)),int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(4,2)),int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(6,2)));
							day=PortofolioDateUI.GetDayOfWeek(webSession,dayDT.DayOfWeek.ToString())+" "+dayDT.ToString("dd/MM/yyyy");	

							if(compteur==0){
								t.Append("<tr>");
								compteur=1;
								endBalise="";
							}
							else if(compteur==1){
								compteur=2;	
								endBalise="";
							}
							else{
								compteur=0;
								endBalise="</td></tr>";

							}
                            t.Append("<td class=\"portofolioSynthesisBorder\"><table  border=0 cellpadding=0 cellspacing=0 width=100% >");
                            t.Append("<tr><td class=\"portofolioSynthesis\" align=center >" + day + "</td><tr>");
                            t.Append("<tr><td align=\"center\" class=\"portofolioSynthesis\" >");
                            if (dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10)
                            {	
                                t.Append("<a href=\"javascript:portofolioCreation('" + webSession.IdSession + "','" + idMedia + "','" + dtVisuel.Rows[i]["date_media_num"].ToString() + "','" + dtVisuel.Rows[i]["date_cover_num"].ToString() + "','" + support + "','" + dtVisuel.Rows[i]["number_page_media"].ToString() + "');\" >");
							}
							t.Append(" <img alt=\""+GestionWeb.GetWebWord(1409,webSession.SiteLanguage)+"\" src='"+pathWeb+"' border=\"0\" width=180 height=220>");
                            if (dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10)
                            {			
								t.Append("</a>");
							}
							t.Append("</td></tr>");
							if(htValue.Count>0){
                                if (htValue.ContainsKey(dtVisuel.Rows[i]["date_cover_num"])) {
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + " : " + ((string[])htValue[dtVisuel.Rows[i]["date_cover_num"]])[1] + "</td><tr>");
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1399, webSession.SiteLanguage) + " :" + int.Parse(((string[])htValue[dtVisuel.Rows[i]["date_cover_num"]])[0]).ToString("### ### ### ###") + "</td><tr>");
                                }
                                else {
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + " : 0</td><tr>");
                                    t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1399, webSession.SiteLanguage) + " : 0</td><tr>");

                                }
							}
							t.Append("</table></td>");
							t.Append(endBalise);								
						}
						if(compteur!=0)
							t.Append("</tr>");				
					}
				}
				#endregion
				
			}
				t.Append("</table>");
				
			
			#endregion

			return t.ToString();
		}
		#endregion

		#region Détail du portefeuille
		/// <summary>
		/// Génère le code html d'une alerte portefeuille d'un support
		/// </summary>
		/// <param name="page">Page utilisée pour montrer le résultat</param>
		/// <param name="tab">Tableau de données du résultat</param>
		/// <param name="webSession">Session du client</param>
		/// <param name="Excel">sortie excel</param>
		/// <returns>Code HTML</returns>
		public static string GetHTMLPortofolioUI(Page page,object[,] tab,WebSession webSession,bool Excel){
			
			#region Pas de données à afficher
			if(tab==null || tab.GetLength(0)==0 ){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}			
			#endregion

			#region variables
			//bool displayCreation=false;
			string firstBaliseA="";
			string endBaliseA="";
			int level=-1;
			string levelProd="";
			string tmp="";
			string creation="";
			string mediaplan="";			
			Int64 idCreation=-1;
			string classCss="";
			bool newLine=false;
			bool showCreation=false;
			bool showMediaPlan=false;
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
			string L1="acl1";
			string L2="acl2";
			string L3="acl3";
			string P2="p2";
			#endregion

			#region style excel
			if(Excel){
				L1="acl11";
				L2="acl21";
				L3="acl31";
			}
			#endregion

			TreeNode tree=new TreeNode();
			tree.Tag=new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.mediaAccess,long.Parse(webSession.GetSelection(webSession.ReferenceUniversMedia,CustomerConstantes.Right.type.mediaAccess)),"");
			webSession.MediaDetailLevel=new TNS.AdExpress.Web.Core.Sessions.MediaLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.media,tree);

			#region Type de module
			// Type de module pour savoir si l'on affiche les créations
			try{
				Module currentModuleDescription=TNS.AdExpress.Web.Core.Navigation.ModulesList.GetModule(webSession.CurrentModule);				
			}
			catch(System.Exception err){
				throw(new WebExceptions.PortofolioUIException("Impossible de déterminer le type de module pour savoir s'il ont doit montrer les créations",err));
			}
			#endregion

			//to exclude creations in case of outdoor media
			if(webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE)
				showCreation=true;

			#region Script
            if (!page.ClientScript.IsClientScriptBlockRegistered("openCreation")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenCreationCompetitorAlert", WebFunctions.Script.OpenCreationCompetitorAlert());
            if (!page.ClientScript.IsClientScriptBlockRegistered("OpenMediaPlanAlert")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenMediaPlanAlert", WebFunctions.Script.OpenMediaPlanAlert());
			if (!page.ClientScript.IsClientScriptBlockRegistered("openGad")){
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openGad", TNS.AdExpress.Web.Functions.Script.OpenGad());
			}
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.PortofolioUIException("La sélection de médias est incorrecte"));
			#endregion

			#region HTML
			try{
				t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 >");
					
				#region libellés colonnes
				// Première ligne
				t.Append("\r\n\t<tr height=\"40px\" >");
				
				t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
				if(!Excel){
					if(!showCreation && WebFunctions.ProductDetailLevel.DisplayCreation(webSession)){
						t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
					}
					t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(150,webSession.SiteLanguage)+"</td>");
				}
				t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1423,webSession.SiteLanguage)+"</td>");				
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
				{
					t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1424,webSession.SiteLanguage)+"</td>");
					t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(943,webSession.SiteLanguage)+"</td>");
					t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(940,webSession.SiteLanguage)+"</td>");
				}
				else if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName)
				{
					t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1604,webSession.SiteLanguage)+"</td>");
				}
				else if(DBClassificationConstantes.Vehicles.names.radio==vehicleName || DBClassificationConstantes.Vehicles.names.tv==vehicleName
					|| DBClassificationConstantes.Vehicles.names.others==vehicleName
					)
				{
					if(Excel==false)t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(942,webSession.SiteLanguage)+"</td>");
					else t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1435,webSession.SiteLanguage)+"</td>");
					t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(939,webSession.SiteLanguage)+"</td>");
				}
				t.Append("</tr>");
				#endregion

				#region total
				classCss="acl1";
				t.Append("\r\n\t<tr align=\"right\"  bgcolor=#ffffff height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+GestionWeb.GetWebWord(1401,webSession.SiteLanguage)+"</td>");								
				if(!Excel){
					if(!showCreation && WebFunctions.ProductDetailLevel.DisplayCreation(webSession)){
						t.Append("<td class=\""+classCss+"\" align=\"center\" bgcolor=#ffffff>&nbsp;</td>");
					}
					if(!showMediaPlan){
						t.Append("<td class=\""+classCss+"\" align=\"center\" bgcolor=#ffffff >&nbsp;</td>");
					}
				}
					
				//Total euros
				if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.EURO_COLUMN_INDEX]!=null){
					//t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+long.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.EURO_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
					t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.EURO_COLUMN_INDEX].ToString(),Constantes.Web.CustomerSessions.Unit.euro,false)+"</td>");
				}
				else t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>&nbsp;</td>");
				
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
					//Total Mm/Col
					if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.MMC_COLUMN_INDEX]!=null){
						//t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+long.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.MMC_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
						t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.MMC_COLUMN_INDEX].ToString(),Constantes.Web.CustomerSessions.Unit.mmPerCol,false)+"</td>");
					}
					else t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>&nbsp;</td>");
					//Total Pages
					if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.PAGES_COLUMN_INDEX]!=null){
						//t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+string.Format("{0:### ### ### ### ##0.###}",double.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.PAGES_COLUMN_INDEX].ToString())/(double)1000)+"</td>");
						t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.PAGES_COLUMN_INDEX].ToString(),Constantes.Web.CustomerSessions.Unit.pages,false)+"</td>");
					}
					else t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>&nbsp;</td>");
				}
//				else if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName)
//				{
//					//Nombre de panneaux
//					if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX]!=null )t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+long.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
//					else t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>&nbsp;</td>");
//					
//				}
				else if(DBClassificationConstantes.Vehicles.names.radio==vehicleName || DBClassificationConstantes.Vehicles.names.tv==vehicleName
					|| DBClassificationConstantes.Vehicles.names.others==vehicleName
					){
					//Total Durée
					if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.DURATION_COLUMN_INDEX]!=null){
						//t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+GetDurationFormatHH_MM_SS(int.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.DURATION_COLUMN_INDEX].ToString())).ToString()+"</td>");
						t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.DURATION_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.duration,false)+"</td>");
					}
					else t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>&nbsp;</td>");
				}
				//Total Insertions/spots
				if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX]!=null ){
//					t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+long.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
					t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.insertion,false)+"</td>");
				}
				else t.Append("\r\n\t<td class=\""+classCss+"\" bgcolor=#ffffff nowrap>&nbsp;</td>");
						
				t.Append("</tr>");
				#endregion

				for(int i=1;i<tab.GetLength(0);i++){
					
					#region switch
										
					#region Level 1
					if(tab[i,ResultConstantes.IDL1_INDEX]!=null && tab[i,ResultConstantes.IDL2_INDEX]==null && tab[i,ResultConstantes.IDL3_INDEX]==null){														
						classCss=L1;
						level=0;
						levelProd="1";
						idCreation = Int64.Parse(tab[i,ResultConstantes.IDL1_INDEX].ToString());
						//Créations
						if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName){
							if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null ){
								if(!showCreation &&  WebFunctions.ProductDetailLevel.DisplayCreation(webSession,ResultCstComp.IDL1_INDEX)){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
								else{creation="";}
							}else{creation="";}
						}else{
							if(!showCreation &&  WebFunctions.ProductDetailLevel.DisplayCreation(webSession,ResultCstComp.IDL1_INDEX))
							creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
							else creation="";
						}
						//Plan média
						if(WebFunctions.ProductDetailLevel.ShowMediaPlanL1(webSession)){mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','"+levelProd+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
						else{mediaplan="";}	
						//Adresse Gad
						if(tab[i,ResultConstantes.ADDRESS_COLUMN_INDEX]!=null){
							tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[i,ResultConstantes.LABELL1_INDEX]+"','"+tab[i,ResultConstantes.ADDRESS_COLUMN_INDEX]+"');";
							firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
							endBaliseA="</a>"; 
						}else{
							firstBaliseA="";
							endBaliseA=""; 		
						}
						//libellé niveau 1																	
						if(!Excel)							
						t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+firstBaliseA+tab[i,ResultConstantes.LABELL1_INDEX]+endBaliseA+"</td>");								
						else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+tab[i,ResultConstantes.LABELL1_INDEX]+"</td>");								
						newLine=true;									 							
					}
					#endregion
					
					#region Level 2
					if(tab[i,ResultConstantes.IDL1_INDEX]==null && tab[i,ResultConstantes.IDL2_INDEX]!=null && tab[i,ResultConstantes.IDL3_INDEX]==null){														
						classCss=L2;
						level=2;
						levelProd="2";
						idCreation = Int64.Parse(tab[i,ResultConstantes.IDL2_INDEX].ToString());
						//Créations
						if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName){
							if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null ){
								if(!showCreation &&  WebFunctions.ProductDetailLevel.DisplayCreation(webSession,ResultCstComp.IDL2_INDEX)){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
								else{creation="";}
							}else{creation="";}
						}else{
							if(!showCreation &&  WebFunctions.ProductDetailLevel.DisplayCreation(webSession,ResultCstComp.IDL2_INDEX))
								creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
							else creation="";
						}
											
						//Plan média
						if(WebFunctions.ProductDetailLevel.ShowMediaPlanL2(webSession)){mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','"+levelProd+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
						else{mediaplan="";}							
						//Adresse Gad
						if(tab[i,ResultConstantes.ADDRESS_COLUMN_INDEX]!=null){
							tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[i,ResultConstantes.LABELL2_INDEX]+"','"+tab[i,ResultConstantes.ADDRESS_COLUMN_INDEX]+"');";
							firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
							endBaliseA="</a>"; 
						}else{
							firstBaliseA="";
							endBaliseA=""; 		
						}
						//libellé niveau 2																					
							if(!Excel)
							t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\"  bgcolor=#D0C8DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[i,ResultConstantes.LABELL2_INDEX]+endBaliseA+"</td>");								
							else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\"  bgcolor=#D0C8DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantes.LABELL2_INDEX]+"</td>");														
						newLine=true;								 							
					}
					#endregion

					#region Level 3
					if(tab[i,ResultConstantes.IDL1_INDEX]==null && tab[i,ResultConstantes.IDL2_INDEX]==null && tab[i,ResultConstantes.IDL3_INDEX]!=null){														
						classCss=L3;
						level=4;
						idCreation = Int64.Parse(tab[i,ResultConstantes.IDL3_INDEX].ToString());
						levelProd="3";
						//Créations
						if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName){
							if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null ){
								if(!showCreation && WebFunctions.ProductDetailLevel.DisplayCreation(webSession,ResultCstComp.IDL3_INDEX)){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
								else{creation="";}
							}else{creation="";}
						}else{
							if(!showCreation && WebFunctions.ProductDetailLevel.DisplayCreation(webSession,ResultCstComp.IDL3_INDEX))creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
							else creation="";
						}		
						//Plan média
						if(!showMediaPlan){mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','"+levelProd+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
						else{mediaplan="";}	
						//Adresse Gad
						if(tab[i,ResultConstantes.ADDRESS_COLUMN_INDEX]!=null){
							tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[i,ResultConstantes.LABELL3_INDEX]+"','"+tab[i,ResultConstantes.ADDRESS_COLUMN_INDEX]+"');";
							firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
							endBaliseA="</a>"; 
						}else{
							firstBaliseA="";
							endBaliseA=""; 		
						}
						//libellé niveau 3																
					
							if(!Excel)
							t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\"  bgcolor=#E1E0DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[i,ResultConstantes.LABELL3_INDEX]+endBaliseA+"</td>");								
							else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\"  bgcolor=#E1E0DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantes.LABELL3_INDEX]+"</td>");														
						newLine=true;									 							
					}
					#endregion						

					if(!Excel){
						if(!showCreation && WebFunctions.ProductDetailLevel.DisplayCreation(webSession)){
							t.Append("<td class=\""+classCss+"\" align=\"center\" >"+creation+"</td>");
						}
						if(!showMediaPlan){
							t.Append("<td class=\""+classCss+"\" align=\"center\" >"+mediaplan+"</td>");
						}
					}
					
					//euros
					if(tab[i,ResultConstantes.EURO_COLUMN_INDEX]!=null ){
//						t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(tab[i,ResultConstantes.EURO_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
						t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.EURO_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					}
					else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
					if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
						//Mm/Col
						if(tab[i,ResultConstantes.MMC_COLUMN_INDEX]!=null ){
//							t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(tab[i,ResultConstantes.MMC_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
							t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.MMC_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.mmPerCol,false)+"</td>");
						}
						else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
						//Pages
						if(tab[i,ResultConstantes.PAGES_COLUMN_INDEX]!=null ){
//							t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+string.Format("{0:### ### ### ### ##0.###}",double.Parse(tab[i,ResultConstantes.PAGES_COLUMN_INDEX].ToString())/(double)1000)+"</td>");						
							t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.PAGES_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.pages,false)+"</td>");						
						}
						else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
					}else if(DBClassificationConstantes.Vehicles.names.radio==vehicleName || DBClassificationConstantes.Vehicles.names.tv==vehicleName
						|| DBClassificationConstantes.Vehicles.names.others==vehicleName
						){
						//Durée
						if(tab[i,ResultConstantes.DURATION_COLUMN_INDEX]!=null ){
							t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.DURATION_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.duration,false)+"</td>");
						}
						else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
					}
					//Insertions/spots
					if(tab[i,ResultConstantes.INSERTIONS_COLUMN_INDEX]!=null ) {
//						 t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(tab[i,ResultConstantes.INSERTIONS_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
						 t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.INSERTIONS_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.insertion,false)+"</td>");
					}
					else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");

						
					if(newLine)t.Append("</tr>");
					newLine=false;
					#endregion
					
				}				
				t.Append("</table>");
			}
			catch(System.Exception err){
				throw(new WebExceptions.PortofolioUIException("Impossible de construire le tableau HTML",err));
			}
			#endregion
			
			return t.ToString();
		}
		#endregion

		#region Chemin de fer popUp
		/// <summary>
		/// Génère le code pour l'affichage du chemin de fer
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="date">date de publication</param>
        /// <param name="parution">date de parution</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="nameMedia">nom du média</param>
		/// <param name="nbrePages">nombre de pages</param>
		/// <param name="pageAnchor">Anchor de la page pour le positionnement de l'image dans son contexte</param>
		/// <returns>Code Html</returns>
		public static string GetPortofolioCreationMedia(WebSession webSession, string date,string parution, string idMedia, string nameMedia, string nbrePages, string pageAnchor){

			#region Variables
			string pathWeb=CstWeb.CreationServerPathes.IMAGES+"/"+idMedia+"/"+parution+"/imagette/";
			string path=CstWeb.CreationServerPathes.LOCAL_PATH_IMAGE+idMedia+@"\"+parution+@"\imagette";
			
			// Pour test en localhost :
			//string path="\\\\localhost\\ImagesPresse\\"+idMedia+"\\"+date+"\\imagette";

			string[] files = Directory.GetFiles(path,"*.jpg");
			string[] endFile;
			StringBuilder t=new StringBuilder(5000);
			int i=1;
			int compteur=0;
			string endBalise="";
			string day;
			string [] filesName=new string[2];
			#endregion

			DateTime dayDT=new DateTime(int.Parse(date.Substring(0,4)),int.Parse(date.Substring(4,2)),int.Parse(date.ToString().Substring(6,2)));
			day=PortofolioDateUI.GetDayOfWeek(webSession,dayDT.DayOfWeek.ToString())+" "+dayDT.ToString("dd/MM/yyyy");	

			t.Append("<table border=1 bordercolor=#644883 cellpadding=0 cellspacing=0 width=100% bgcolor=#E9E6EF ><tr>");
			t.Append("<td class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-BOTTOM-STYLE: none\">"+day+"</td>");
			t.Append("<td align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">"+nameMedia+"</td>");
			t.Append("<td align=right class=\"portofolio1\" style=\"BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">"+GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : "+nbrePages+"</td>");
			t.Append("</tr></table>");

			t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100% bgcolor=#E9E6EF>");
			foreach (string name in files) {
				
				endFile=name.Split('\\');
				// Couverture - Dos
				if(i==1 || i==files.Length){
					t.Append("<tr><td colspan=4 align=center>");
					t.Append("<table border=1 bordercolor=#644883 cellpadding=0 cellspacing=0 width=100%><tr><td align=center>");
					if(i==1) t.Append("<a name=\"C1\"></a><a name=\"C2\"></a>");
					if(i==files.Length) t.Append("<a name=\"C3\"></a><a name=\"C4\"></a>");
					t.Append("<a href=\"javascript:portofolioOneCreation('"+idMedia+"','"+parution+"','"+endFile[endFile.Length-1]+"','');\"><img src='"+pathWeb+endFile[endFile.Length-1]+"' border=\"0\"></a>");
					t.Append("</td></tr></table>");
					t.Append("</td></tr>");
				}				
				else{
					if(compteur==0){
						t.Append("<tr>");
						endBalise="";
					}
					else if(compteur==3){
						compteur=-1;
						endBalise="</tr>";

					}else{
						endBalise="";
					}
					
					t.Append("<td align=center style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
					// Tableau niveau 2
					if(compteur==0 || compteur==2){
						t.Append("<table border=1 bordercolor=#644883 cellpadding=0 cellspacing=0 width=100%><tr><td style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
						filesName[0]=endFile[endFile.Length-1];
						filesName[1]=files[i].Split('\\')[endFile.Length-1];
					}
					// Tableau niveau 1
					t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td>");
					t.Append("<a name=\"#"+i.ToString()+"\" href=\"javascript:portofolioOneCreation('"+idMedia+"','"+parution+"','"+filesName[0]+"','"+filesName[1]+"');\"><img src='"+pathWeb+endFile[endFile.Length-1]+"' border=\"0\"></a>");
					t.Append("</td></tr>");	
					t.Append("</table>");

					if(compteur==1 || compteur==-1){
						t.Append("<tr ><td colspan=2 align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">Pages : "+((int)(i-1)).ToString()+"/"+i.ToString()+"</td></tr>");
						t.Append("</td></tr></table>");					
					}
					
					t.Append("</td>");	
					t.Append(endBalise);
					compteur++;
				}
				i++;
			}
			t.Append("</table>");
			
			// Script location
			t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\ndocument.location='#"+pageAnchor+"';");
			t.Append("\n</script>");

			return t.ToString();
		}

		/// <summary>
		/// Affichage d'une page d'un support
		/// </summary>
		/// <param name="date">date</param>
		/// <param name="idMedia">identifiant média</param>
		///<param name="fileName1">Nom du premier fichier</param>
		///<param name="fileName2">Nom du 2ème fichier</param>
		/// <returns>page d'un support</returns>
		public static string GetPortofolioOneCreationMedia(string date,string idMedia,string fileName1,string fileName2){
			string pathWeb1=CstWeb.CreationServerPathes.IMAGES+"/"+idMedia+"/"+date+"/"+fileName1+"";
			string pathWeb2=CstWeb.CreationServerPathes.IMAGES+"/"+idMedia+"/"+date+"/"+fileName2+"";
			StringBuilder t=new StringBuilder(3000);

			t.Append("<table><tr><td>");
			t.Append("<img src='"+pathWeb1+"' border=\"0\" width=470 height=627>");
			if(fileName2.Length>0)
				t.Append("<img src='"+pathWeb2+"' border=\"0\" width=470 height=627>");
			
			t.Append("</td></tr></table>");

			return t.ToString();
		}

		#endregion

		#region Chemin de fer popUp (version publique pour les alertes push mail)
		/// <summary>
		/// Génère le code pour l'affichage du chemin de fer
		/// </summary>
		/// <param name="date">date</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="nameMedia">nom du média</param>
		/// <param name="languageId">Identifiant de la langue</param>
		/// <returns>Code Html</returns>
		public static string GetPortofolioCreationMedia(string date,string parution, string idMedia, string nameMedia, int languageId){

			#region Variables
            string pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + idMedia + "/" + parution + "/imagette/";
            string path = CstWeb.CreationServerPathes.LOCAL_PATH_IMAGE + idMedia + @"\" + parution + @"\imagette";
			
			// Pour test en localhost :
			//string path="\\\\localhost\\ImagesPresse\\"+idMedia+"\\"+date+"\\imagette";

			string[] files = Directory.GetFiles(path,"*.jpg");
			string[] endFile;
			StringBuilder t=new StringBuilder(5000);
			int i=1;
			int compteur=0;
			string endBalise="";
			string day="";
			string [] filesName=new string[2];
			#endregion

			DateTime dayDT=new DateTime(int.Parse(date.Substring(0,4)),int.Parse(date.Substring(4,2)),int.Parse(date.ToString().Substring(6,2)));
			day=TNS.FrameWork.Date.DateString.dateTimeToDD_MM_YYYY(dayDT,languageId);

			t.Append("<table border=1 bordercolor=#644883 cellpadding=0 cellspacing=0 width=100% bgcolor=#E9E6EF ><tr>");
			t.Append("<td width=\"33%\" class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-BOTTOM-STYLE: none\">"+day+"</td>");
			t.Append("<td width=\"33%\" align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">"+nameMedia+"</td>");
			t.Append("<td width=\"33%\" align=right class=\"portofolio1\" style=\"BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">&nbsp;</td>");
			t.Append("</tr></table>");

			t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100% bgcolor=#E9E6EF>");
			foreach (string name in files) {
				endFile=name.Split('\\');
				// Couverture - Dos
				if(i==1 || i==files.Length){
					t.Append("<tr><td colspan=4 align=center>");
					t.Append("<table border=1 bordercolor=#644883 cellpadding=0 cellspacing=0 width=100%><tr><td align=center>");
					if(i==1) t.Append("<a name=\"C1\"></a><a name=\"C2\"></a>");
					if(i==files.Length) t.Append("<a name=\"C3\"></a><a name=\"C4\"></a>");
                    t.Append("<a href=\"javascript:portofolioOneCreation('" + idMedia + "','" + parution + "','" + endFile[endFile.Length - 1] + "','');\"><img src='" + pathWeb + endFile[endFile.Length - 1] + "' border=\"0\"></a>");
					t.Append("</td></tr></table>");
					t.Append("</td></tr>");
				}				
				else{
					if(compteur==0){
						t.Append("<tr>");
						endBalise="";
					}
					else if(compteur==3){
						compteur=-1;
						endBalise="</tr>";
					}
					else{
						endBalise="";
					}
					t.Append("<td align=center style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
					// Tableau niveau 2
					if(compteur==0 || compteur==2){
						t.Append("<table border=1 bordercolor=#644883 cellpadding=0 cellspacing=0 width=100%><tr><td style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
						filesName[0]=endFile[endFile.Length-1];
						filesName[1]=files[i].Split('\\')[endFile.Length-1];
					}
					// Tableau niveau 1
					t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td>");
                    t.Append("<a name=\"#" + i.ToString() + "\" href=\"javascript:portofolioOneCreation('" + idMedia + "','" + parution + "','" + filesName[0] + "','" + filesName[1] + "');\"><img src='" + pathWeb + endFile[endFile.Length - 1] + "' border=\"0\"></a>");
					t.Append("</td></tr>");	
					t.Append("</table>");

					if(compteur==1 || compteur==-1){
						t.Append("<tr ><td colspan=2 align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">Pages : "+((int)(i-1)).ToString()+"/"+i.ToString()+"</td></tr>");
						t.Append("</td></tr></table>");	
					}
					t.Append("</td>");	
					t.Append(endBalise);
					compteur++;
				}
				i++;
			}
			t.Append("</table>");

			return t.ToString();
		}
		#endregion

		#region Nouveautés
		/// <summary>
		/// Génère le code HTML pour la planche Nouveauté
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="idVehicle">identifiant du véhicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="excel">utilisé pour les excel si true</param>
		/// <returns>Code HTML</returns>
		public static string GetHTMLNoveltyUI(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,bool excel){
			
			DataSet dsNewProduct=PortofolioDataAccess.GetNewProduct(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			DataTable dtNewProduct=dsNewProduct.Tables[0];	
		
			#region Pas de données à afficher
			if(dtNewProduct==null || dtNewProduct.Rows.Count==0 ){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}			
			#endregion

			string HTML="";

			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				case DBClassificationConstantes.Vehicles.names.press:
					HTML=GetHTMLNoveltyPress(webSession,dtNewProduct,idVehicle,idMedia,excel);
					return HTML;
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					HTML=GetHTMLNoveltyPress(webSession,dtNewProduct,idVehicle,idMedia,excel);
					return HTML;
				case DBClassificationConstantes.Vehicles.names.radio:							
				case DBClassificationConstantes.Vehicles.names.tv:	
				case DBClassificationConstantes.Vehicles.names.others:
					HTML=GetHTMLNoveltyRadioTV(webSession,dtNewProduct,idVehicle,idMedia,excel);
					return HTML;
//				case DBClassificationConstantes.Vehicles.names.outdoor:
//					HTML=getHTMLNoveltyOutdoor(webSession,dtNewProduct,idVehicle,idMedia,excel);
//					return HTML;
									
				default:
					throw new Exceptions.PortofolioUIException("-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}		
		}

		#region Radio-télé
		/// <summary>
		/// Génère le code HTML pour la TV et la Presse
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="table">table avec les données</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="excel">utilisé pour les excel si true</param>
		/// <returns>Code Html</returns>
		protected static string GetHTMLNoveltyRadioTV(WebSession webSession,DataTable table ,Int64 idVehicle, Int64 idMedia,bool excel){
		
			#region Variables
			StringBuilder t=new StringBuilder(5000);
			string classStyleTitle="";
			string classStyleValue="";
			string duree="";
			#endregion			

			#region Génération du code
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");			
			t.Append("\r\n\t<tr height=\"20px\" >");
			// Nom du produit	
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1418,webSession.SiteLanguage)+"</td>");
			if(!excel){
				// Création
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
			}
			// Nombre de spot
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1404,webSession.SiteLanguage)+"</td>");
			// Valeur
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1419,webSession.SiteLanguage)+"</td>");
			// durée
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(861,webSession.SiteLanguage)+"</td>");
			t.Append("</tr>");
			
			#region Parcours du tableau
			foreach(DataRow row in table.Rows){				
				classStyleTitle="acl2";
				classStyleValue="acl2";	
				
				t.Append("<tr  onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA>");
				// Nom du produit
				t.Append("<td class=\""+classStyleTitle+"\" nowrap>"+row["produit"].ToString()+"</td>");
				if(!excel){
					// Création
					t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+idVehicle+",-1,"+idMedia+",-1,"+int.Parse(row["id_product"].ToString())+"','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
				}
				// Nombre d'insertion
//				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["insertion"]+"</td>");
				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["insertion"].ToString(),WebConstantes.CustomerSessions.Unit.insertion,false)+"</td>");
				// Valeur
//				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+int.Parse(row["valeur"].ToString()).ToString("### ### ###")+"</td>");
				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["valeur"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
				// Durée
				//duree=GetDurationFormatHH_MM_SS(int.Parse(row["duree"].ToString()));
				duree=WebFunctions.Units.ConvertUnitValueAndPdmToString(row["duree"].ToString(),WebConstantes.CustomerSessions.Unit.duration,false);
				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+duree+"</td>");
				t.Append("</tr>");			
			}
			#endregion

			t.Append("</table>");
			#endregion

			return (t.ToString());
		}

		#endregion

		#region Presse
		/// <summary>
		/// Génère le code HTML pour la Presse
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="table">table avec les données</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="excel">utilisé pour les excel si true</param>
		/// <returns>Code Html</returns>
		protected static string GetHTMLNoveltyPress(WebSession webSession,DataTable table ,Int64 idVehicle, Int64 idMedia,bool excel){
		
			#region Variables
			StringBuilder t=new StringBuilder(5000);
			string classStyleTitle="";
			string classStyleValue="";
			#endregion			

			#region Génération du code
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");			
			t.Append("\r\n\t<tr height=\"20px\" >");
			// Nom du produit
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1418,webSession.SiteLanguage)+"</td>");
			if(!excel){
				// Création
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
			}
			// Nombre d'insertion
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+"</td>");
			// Valeur
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1419,webSession.SiteLanguage)+"</td>");	
			t.Append("</tr>");
			
			#region Parcours du tableau
			foreach(DataRow row in table.Rows){				
				classStyleTitle="acl2";
				classStyleValue="acl2";
				
				t.Append("<tr  onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA>");
				// Nom du produit
				t.Append("<td class=\""+classStyleTitle+"\" nowrap>"+row["produit"].ToString()+"</td>");
				if(!excel){
					// Création
					t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+idVehicle+",-1,"+idMedia+",-1,"+int.Parse(row["id_product"].ToString())+"','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
				}
				// Nombre d'insertion
				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["insertion"]+"</td>");
				// Valeur
//				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+int.Parse(row["valeur"].ToString()).ToString("### ### ###")+"</td>");		
				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["valeur"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");		
				t.Append("</tr>");			
			}
			#endregion

			t.Append("</table>");
			#endregion

			return (t.ToString());
		}

		#endregion

		#region Affichage
//		/// <summary>
//		/// Génère le code HTML pour la Presse
//		/// </summary>
//		/// <param name="webSession">Session Client</param>
//		/// <param name="table">table avec les données</param>
//		/// <param name="idVehicle">identifiant vehicle</param>
//		/// <param name="idMedia">identifiant média</param>
//		/// <param name="excel">utilisé pour les excel si true</param>
//		/// <returns>Code Html</returns>
//		protected static string getHTMLNoveltyOutdoor(WebSession webSession,DataTable table ,Int64 idVehicle, Int64 idMedia,bool excel)
//		{
//		
//			#region Variables
//			StringBuilder t=new StringBuilder(5000);
//			string classStyleTitle="";
//			string classStyleValue="";
//			string duree="";
//			#endregion			
//
//			#region Génération du code
//			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");			
//			t.Append("\r\n\t<tr height=\"20px\" >");
//			// Nom du produit	
//			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1418,webSession.SiteLanguage)+"</td>");
//			if(!excel)
//			{
//				// Création
//				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
//			}
//			// Nombre de panneaux
//			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1604,webSession.SiteLanguage)+"</td>");
//			// Valeur
//			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\">"+GestionWeb.GetWebWord(1419,webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");
//			
//			#region Parcours du tableau
//			foreach(DataRow row in table.Rows)
//			{				
//				classStyleTitle="acl2";
//				classStyleValue="acl2";	
//				
//				t.Append("<tr  onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA>");
//				// Nom du produit
//				t.Append("<td class=\""+classStyleTitle+"\" nowrap>"+row["produit"].ToString()+"</td>");
//				if(!excel)
//				{
//					// Création
//					t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+idVehicle+",-1,"+idMedia+",-1,"+int.Parse(row["id_product"].ToString())+"','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//				}
//				// Nombre de panneaux
//				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["number_board"]+"</td>");
//				// Valeur
//				t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+int.Parse(row["valeur"].ToString()).ToString("### ### ###")+"</td>");
//				t.Append("</tr>");			
//			}
//			#endregion
//
//			t.Append("</table>");
//			#endregion
//
//			return (t.ToString());
//		}

		#endregion

		#endregion
		
		#region Détail d'un support

		#region Première page
		/// <summary>
		/// Méthode globale
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="excel">true si export Excel</param>
		/// <returns>Liste des supports</returns>
		public static string DetailMediaUI(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,bool excel){		
			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				case DBClassificationConstantes.Vehicles.names.press:
					return DetailMediaPressUI(webSession,idVehicle,idMedia,dateBegin,dateEnd);
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return DetailMediaPressUI(webSession,idVehicle,idMedia,dateBegin,dateEnd);
				case DBClassificationConstantes.Vehicles.names.radio:					
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return DetailMediaTvRadioUI(webSession,idVehicle,idMedia,dateBegin,dateEnd,excel);
				default:
					throw new Exceptions.PortofolioUIException("Le cas de ce média n'est pas gérer.");
			}
		}
	
		#region Press
		/// <summary>
		///  Génère le code Html pour le detail portefeuille dans le media press
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>Code Html</returns>
		protected static string DetailMediaPressUI(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
			#region Variables
			DataSet dsVisuel=PortofolioDateDataAccess.GetListDate(webSession,idVehicle,idMedia,dateBegin,dateEnd,true);
			DataTable dtVisuel=dsVisuel.Tables[0];
			Hashtable htValue=PortofolioDataAccess.GetInvestmentByMedia(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			StringBuilder t=new StringBuilder(5000);
			string support="";
			
			int compteur=0;
			string endBalise="";
			string day="";
			string pathWeb="";
			#endregion

			DataSet dsCategory=PortofolioDataAccess.GetCategoryMediaSellerData(webSession,idVehicle,idMedia);
			DataTable dtCategory=dsCategory.Tables[0];

			foreach(DataRow row in dtCategory.Rows){
				support=row["support"].ToString();				
			}

			//Ensemble du spot à spot sur la période intérrogée						
			GetAllPeriodInsertions(t,GestionWeb.GetWebWord(1837,webSession.SiteLanguage),webSession,idMedia.ToString());

            t.Append("<table  border=1 cellpadding=0 cellspacing=0 width=600 class=\"paleVioletBackGroundV2 violetBorder\">");
			//Chemin de fer
            t.Append("\r\n\t<tr height=\"25px\" ><td colspan=3 class=\"txtBlanc12Bold violetBackGround portofolioSynthesisBorder\" align=\"center\" >" + support + "</td></tr>");
			for(int i=0;i<dtVisuel.Rows.Count;i++) {
				//date_media_num

                if (dtVisuel.Rows[i]["disponibility_visual"] != System.DBNull.Value && int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString()) >= 10)
                {
					pathWeb=CstWeb.CreationServerPathes.IMAGES+"/"+idMedia.ToString()+"/"+dtVisuel.Rows[i]["date_cover_num"].ToString()+"/Imagette/"+CstWeb.CreationServerPathes.COUVERTURE+"";
				}
				else{
					pathWeb="/Images/"+webSession.SiteLanguage+"/Others/no_visuel.gif";
				}
				DateTime dayDT=new DateTime(int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(0,4)),int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(4,2)),int.Parse(dtVisuel.Rows[i]["date_media_num"].ToString().Substring(6,2)));
				day=PortofolioDateUI.GetDayOfWeek(webSession,dayDT.DayOfWeek.ToString())+" "+WebFunctions.Dates.dateToString(dayDT,webSession.SiteLanguage);	

				if(compteur==0){
					t.Append("<tr>");
					compteur=1;
					endBalise="";
				}
				else if(compteur==1){
					compteur=2;	
					endBalise="";
				}
				else{
					compteur=0;
					endBalise="</td></tr>";

				}
                t.Append("<td class=\"portofolioSynthesisBorder\"><table  border=0 cellpadding=0 cellspacing=0 width=100% >");
                t.Append("<tr><td class=\"portofolioSynthesis\" align=center>" + day + "</td><tr>");
                t.Append("<tr><td align=\"center\" class=\"portofolioSynthesis\">");
				//if(int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString())>=10){	
					//t.Append("<a href=\"/Private/Results/PortofolioCreationMediaPopUp.aspx?idSession="+webSession.IdSession+"&idMedia="+idMedia+"&date="+dtVisuel.Rows[i]["date_media_num"].ToString()+"\">");
					t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','"+dtVisuel.Rows[i]["date_media_num"].ToString()+"','');\" >");
				//}
				t.Append(" <img alt=\""+GestionWeb.GetWebWord(1409,webSession.SiteLanguage)+"\" src='"+pathWeb+"' border=\"0\" width=180 height=220>");
				//if(int.Parse(dtVisuel.Rows[i]["disponibility_visual"].ToString())>=10){			
					t.Append("</a>");
				//}
				t.Append("</td></tr>");
				if(htValue.Count>0){
                    if (htValue.ContainsKey(dtVisuel.Rows[i]["date_cover_num"])) {
                        t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + " : " + ((string[])htValue[dtVisuel.Rows[i]["date_cover_num"]])[1] + "</td><tr>");
                        t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1399, webSession.SiteLanguage) + " :" + int.Parse(((string[])htValue[dtVisuel.Rows[i]["date_cover_num"]])[0]).ToString("### ### ### ###") + "</td><tr>");
                    }
                    else {
                        t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + " : 0</td><tr>");
                        t.Append("<tr><td class=\"portofolioSynthesis\" align=\"center\">" + GestionWeb.GetWebWord(1399, webSession.SiteLanguage) + " : 0</td><tr>");
                    }
				}
				t.Append("</table></td>");
				t.Append(endBalise);								
			}
			if(compteur!=0)
				t.Append("</tr>");	
			
			t.Append("</table>");

			return t.ToString();


		}
		
		#endregion

		#region TV-Radio
		/// <summary>
		/// Détail portefeuille pour la télévision et la radio
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="excel">true si sortie Excel</param>
		/// <returns>Liste des supports pour la télé et la radio</returns>
		private static string DetailMediaTvRadioUI(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,bool excel){
			
		//	string classStyleTitle="acl2";
			string classStyleValue="acl2";
			bool color=true;
			bool isTvNatThematiques=false;
            //string style ="style=\"cursor : hand\"";
            string style = "cursorHand";

			StringBuilder t=new StringBuilder(20000);
			string nbrInsertion="";
			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				
				case DBClassificationConstantes.Vehicles.names.radio:
					nbrInsertion=GestionWeb.GetWebWord(939,webSession.SiteLanguage);
					break;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					nbrInsertion=GestionWeb.GetWebWord(939,webSession.SiteLanguage);
					break;
				default:
					break;
 
			}
			
			int[,] tab=TNS.AdExpress.Web.Rules.Results.PortofolioRules.GetFormattedTableDetailMedia(webSession,idVehicle,idMedia,dateBegin,dateEnd);

			#region aucune données
			if(tab.GetLength(0)==0){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}

			#endregion
						 
			//Vérifie si le support appartient à la TV Nat Thématiques
			isTvNatThematiques = WebDataAccess.Functions.IsBelongToTvNatThematiques(webSession,idMedia.ToString());
			if(isTvNatThematiques)style="";
			if(!excel && !isTvNatThematiques){				
				//Ensemble du spot à spot sur la période intérrogée
				GetAllPeriodInsertions(t,GestionWeb.GetWebWord(1836,webSession.SiteLanguage),webSession,idMedia.ToString());
			}
			
	
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");			
			
			#region Première ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
            t.Append("<td class=\"p2 violetBorderTop\" colspan=2>&nbsp;</td>");
			// Lundi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(654, webSession.SiteLanguage) + "</td>");
			// Mardi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(655, webSession.SiteLanguage) + "</td>");
			// Mercredi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(656, webSession.SiteLanguage) + "</td>");
			// Jeudi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(657, webSession.SiteLanguage) + "</td>");
			// Vendredi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(658, webSession.SiteLanguage) + "</td>");
			// Samedi
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(659, webSession.SiteLanguage) + "</td>");
			// Dimanche
            t.Append("<td class=\"p2 violetBorderTop\">" + GestionWeb.GetWebWord(660, webSession.SiteLanguage) + "</td>");
			t.Append("</tr>");
			#endregion

			for(int i=0;i<tab.GetLength(0) && int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN].ToString())>=0;i++){
			
				if(color) {
                    t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" class=\"violetBackGroundV2\">");
				}
				else{
                    t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='greyBackGround';\" class=\"greyBackGround\">");
				}
				// code écran
				t.Append("<td class=\"p2\" rowspan=2 align=\"left\" nowrap>"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"</td>");
				
				if(color) {
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Monday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
					
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_VALUE].ToString()).ToString("### ### ###")+"</td>");
					t.Append("<td class=\""+classStyleValue + (style.Length>0 ? " " + style + "" : "" ) + "\" align=\"right\" nowrap title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_VALUE].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}

				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Tuesday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
					
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_VALUE].ToString()).ToString("### ### ###")+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_VALUE].ToString(), WebConstantes.CustomerSessions.Unit.euro, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
					
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Wednesday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_VALUE].ToString()).ToString("### ### ###")+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_VALUE].ToString(), WebConstantes.CustomerSessions.Unit.euro, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Thursday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_VALUE].ToString()).ToString("### ### ###")+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_VALUE].ToString(), WebConstantes.CustomerSessions.Unit.euro, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Friday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
					
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_VALUE].ToString()).ToString("### ### ###")+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_VALUE].ToString(), WebConstantes.CustomerSessions.Unit.euro, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Saturday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_VALUE].ToString()).ToString("### ### ###")+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_VALUE].ToString(), WebConstantes.CustomerSessions.Unit.euro, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Sunday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+int.Parse(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_VALUE].ToString()).ToString("### ### ###")+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_VALUE].ToString(), WebConstantes.CustomerSessions.Unit.euro, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				t.Append("</tr>");

				if(color){
                    t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV2';\" class=\"violetBackGroundV2\">");
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+GestionWeb.GetWebWord(939,webSession.SiteLanguage)+"</td>");
					color=!color;
				}
				else{
                    t.Append("<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='greyBackGround';\" class=\"greyBackGround\">");
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+GestionWeb.GetWebWord(939,webSession.SiteLanguage)+"</td>");
					color=!color;
				}
				
				//t.Append("<td class=\""+classStyleValue+"\" rowspan=2 align=\"right\" nowrap>"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"</td>");
				
				
				// Partie Nombre de spot

				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Monday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
					
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION]+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.MONDAY_INSERTION].ToString(), WebConstantes.CustomerSessions.Unit.spot, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Tuesday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");

//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION]+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.TUESDAY_INSERTION].ToString(), WebConstantes.CustomerSessions.Unit.spot, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Wednesday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION]+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.WEDNESDAY_INSERTION].ToString(), WebConstantes.CustomerSessions.Unit.spot, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Thursday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION]+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.THURSDAY_INSERTION].ToString(), WebConstantes.CustomerSessions.Unit.spot, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Friday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION]+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.FRIDAY_INSERTION].ToString(), WebConstantes.CustomerSessions.Unit.spot, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Saturday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION]+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SATURDAY_INSERTION].ToString(), WebConstantes.CustomerSessions.Unit.spot, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				if(tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString()!="0"){
					if(!excel && !isTvNatThematiques)
						t.Append("<a href=\"javascript:portofolioDetailMedia('"+webSession.IdSession+"','"+idMedia+"','Sunday','"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.ECRAN]+"');\" >");
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap style=\"cursor : hand\" title=\""+GestionWeb.GetWebWord(1429,webSession.SiteLanguage)+"\">"+tab[i,FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION]+"</td>");
                    t.Append("<td class=\"" + classStyleValue + (style.Length > 0 ? " " + style + "" : "") + "\" align=\"right\" nowrap title=\"" + GestionWeb.GetWebWord(1429, webSession.SiteLanguage) + "\">" + WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i, FrameWorkConstantes.Results.PortofolioDetailMedia.SUNDAY_INSERTION].ToString(), WebConstantes.CustomerSessions.Unit.spot, false) + "</td>");
					if(!excel && !isTvNatThematiques)
						t.Append("</a>");
				}
				else{
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>&nbsp;</td>");
				}
				t.Append("</tr>");
			
			}

			t.Append("</table>");
			return t.ToString();
		
		}

		#endregion	
		
		#endregion

		#region PopUp
		/// <summary>
		/// Génère le code html pour la popup dans le détail support
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="table">table avec la liste des pubs</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="excel">true si export Excel</param>
		/// <param name="code_ecran">Code écran</param>
		/// <param name="date">jour de la semaine</param>
		/// <param name="page">Page</param>
		///	<param name="allPeriod">vrai si on présente les spots de toute la période</param>
		/// <returns>Code Html</returns>
		public static string GetHTMLDetailMediaPopUpUI(Page page,WebSession webSession,DataTable table,Int64 idVehicle,Int64 idMedia,string code_ecran,string date,bool excel,bool allPeriod){


			if(table.Rows.Count==0){
				return("<table bgcolor=#E9E6EF width=100%><tr><td align=\"center\" bgcolor=#E9E6EF class=\"txtNoir11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</td></tr></table>");
			}

			string mediaAgencyYear="";
			DataTable dt=TNS.AdExpress.Web.DataAccess.Results.MediaAgencyDataAccess.GetListYear(webSession).Tables[0];
				
			if(dt!=null && dt.Rows.Count>0){
				//On récupère la dernière année des agences médias
				mediaAgencyYear = dt.Rows[0]["year"].ToString();
			}

			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				case DBClassificationConstantes.Vehicles.names.press:
					return GetHTMLPressDetailMediaPopUpUI(page,webSession,table,idMedia,excel,date,allPeriod,int.Parse(idVehicle.ToString()),mediaAgencyYear);
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return GetHTMLPressDetailMediaPopUpUI(page,webSession,table,idMedia,excel,date,allPeriod,int.Parse(idVehicle.ToString()),mediaAgencyYear);
				case DBClassificationConstantes.Vehicles.names.radio:
					return GetHTMLRadioDetailMediaPopUpUI(page,webSession,table,idVehicle,idMedia,excel,date,allPeriod,"",mediaAgencyYear);
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return GetHTMLTVDetailMediaPopUpUI(page,webSession,table,idVehicle,idMedia,excel,date,allPeriod,"",mediaAgencyYear);
				default:
					throw new Exceptions.PortofolioUIException("Le cas de ce média n'est pas gérer.");
			}
		}

		#region Presse
		/// <summary>
		/// Génère la popup avec la liste des pubs
		/// pour le média sélectionné
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="table">table avec la liste des pubs</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="excel">true si utilisé pour l'export Excel</param>
		/// <param name="date">jour de la semaine</param>
		/// <param name="page">page</param>
		///	<param name="allPeriod">vrai si on présente les spots de toute la période</param>
		///	<param name="idVehicle">Identifiant du media</param>
        /// <param name="mediaAgencyYear">Année agence média</param>
		/// <returns>Code Html</returns>
		public static string GetHTMLPressDetailMediaPopUpUI(Page page,WebSession webSession,DataTable table,Int64 idMedia,bool excel,string date,bool allPeriod,int idVehicle,string mediaAgencyYear){

			StringBuilder t=new StringBuilder(5000);			
			string classStyleValue="";
			int [] sqlLocation=new int[table.Rows.Count+1];			
			string[] files;
			string listVisual="";
			string dateParution="";
			string colSpan="13";
			DateTime dayOfWeek=DateTime.Now;
			DateTime dayOfParution=DateTime.Now;
			string periodBeginningDate="";
			string periodEndDate="";
			
			if(!allPeriod){
				dayOfWeek=new DateTime(int.Parse(date.ToString().Substring(0,4)),int.Parse(date.Substring(4,2)),int.Parse(date.ToString().Substring(6,2)));
			}
			if(allPeriod){
				periodBeginningDate=WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType),webSession.SiteLanguage);
				periodEndDate = WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType),webSession.SiteLanguage);				

			}

			#region Paramètres du tableau
			if(excel){
				if(allPeriod){
					// Toute la période
					t.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromPortofolio(webSession,true,webSession.PeriodBeginningDate,webSession.PeriodEndDate,(int)idVehicle,(int)idMedia,allPeriod));
				}
				else{
					// Date de parution
					t.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromPortofolio(webSession,false,date,date,(int)idVehicle,(int)idMedia,allPeriod));
				}
			}
			#endregion


			t.Append("<table border=0 cellpadding=0 cellspacing=0 ><tr bgcolor=#ffffff><td>");
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");
			if(!excel){
				t.Append("<tr valign=\"top\" height=\"30\">");
				
				if(!allPeriod)t.Append("<td colSpan=\""+colSpan+"\" valign=middle class=txtViolet12Bold>"+table.Rows[0]["media"].ToString()+"-"+WebFunctions.Dates.dateToString(dayOfWeek.Date,webSession.SiteLanguage)+"</td>");	
				else{										
					colSpan="13";
					t.Append("<td colSpan=\""+colSpan+"\" valign=middle class=txtViolet12Bold>"+table.Rows[0]["media"].ToString()+" - ");	
					t.Append(periodBeginningDate);
					if(!periodBeginningDate.Equals(periodEndDate))t.Append(" "+GestionWeb.GetWebWord(1730,webSession.SiteLanguage)+" "+periodEndDate);
					t.Append(" </td> ");
				}
				t.Append("</tr>");
			}

			webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product;
			webSession.Save();

			#region script
            if (!page.ClientScript.IsClientScriptBlockRegistered("OpenMediaPlanAlert")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenMediaPlanAlert", WebFunctions.Script.OpenMediaPlanAlert());
			#endregion

			#region Première ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			
			if(!excel){
				// Création
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
			}
			// date de parution
			if(allPeriod)t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1381,webSession.SiteLanguage)+"</td>");
			// Annonceur
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1146,webSession.SiteLanguage)+"</td>");
			// Agence média
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null && table.Columns.Contains("advertising_agency"))
			{
//				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1448,webSession.SiteLanguage)+"</td>");
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1448,webSession.SiteLanguage)+((mediaAgencyYear.Length>0)?" ("+mediaAgencyYear+")":"")+"</td>");

			}
			if(!excel){
				// plan média produit
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1478,webSession.SiteLanguage)+"</td>");
			}
			// Produit
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
			// Famille
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1103,webSession.SiteLanguage)+"</td>");
			// groupe
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
			// Numéro de page
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(894,webSession.SiteLanguage)+"</td>");
			// Surface en page
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(593,webSession.SiteLanguage)+"</td>");
			// Surface en mmc
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(595,webSession.SiteLanguage)+"</td>");
			// Prix
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");
			// Descriptifs
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1425,webSession.SiteLanguage)+"</td>");
			// Format
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1420,webSession.SiteLanguage)+"</td>");
			// Couleur
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(561,webSession.SiteLanguage)+"</td>");
			// Rang Famille
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1426,webSession.SiteLanguage)+"</td>");
			// Rang groupe
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1427,webSession.SiteLanguage)+"</td>");
			// Rang support
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1428,webSession.SiteLanguage)+"</td>");
			t.Append("</tr>");

			#endregion

			Int64 idOldLine=-1;
			Int64 idLine=-1;
			
			classStyleValue="acl2";
			int i=0;			

			#region Parcours du tableau
			foreach(DataRow row in table.Rows){
				if(date==row["date_media_num"].ToString() || allPeriod){
					idLine=(long)row["id_advertisement"];
					if(idLine!=idOldLine){
						i++;
						t.Append("<tr  onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA>");
						if(!excel){
						
							if (row["disponibility_visual"]!=System.DBNull.Value 
								&& int.Parse(row["disponibility_visual"].ToString())<=10
								&& row["visual"].ToString()!=""
								//&& webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_CREATION_ACCESS_FLAG)!=null
                                && webSession.CustomerLogin.ShowCreatives((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()))
								){
								// Création
								files=row["visual"].ToString().Split(',');						
								foreach(string str in files){
									listVisual+="/ImagesPresse/"+idMedia+"/"+row["date_media_num"]+"/"+str+",";
								}
								if(listVisual.Length>0){
									listVisual=listVisual.Substring(0,listVisual.Length-1);
								}						
								t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:openPressCreation('"+listVisual+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
							}
							else{
								t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap>&nbsp;</td>");
							}
							listVisual="";
						}
						//Date de parution
						if(allPeriod){
							if(row["date_media_num"]!= System.DBNull.Value){
								dayOfParution = new DateTime(int.Parse(row["date_media_num"].ToString().Substring(0,4)),int.Parse(row["date_media_num"].ToString().Substring(4,2)),int.Parse(row["date_media_num"].ToString().Substring(6,2)));
								dateParution=WebFunctions.Dates.dateToString(dayOfParution,webSession.SiteLanguage);
							}
							else dateParution="&nbsp;";
							 t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+dateParution+"</td>");
						}
						// Annonceur 
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["advertiser"]+"</td>");
						// Agence Média
						if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null && table.Columns.Contains("advertising_agency"))
						{
							t.Append("<td class=\""+classStyleValue+"\" align=\"left\"	>"+row["advertising_agency"]+"</td>");
						}
						//Plan media du produit
						if(!excel)
						t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+row["id_product"]+"','1');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");

						// Produit						
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>");
						//if(!excel)
						//	t.Append("<a class=\""+classStyleValue+"\" title=\""+GestionWeb.GetWebWord(1447,webSession.SiteLanguage)+"\" href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+row["id_product"]+"','1');\">");
						
						t.Append(row["product"]);
					//	if(!excel)
					//		t.Append("</a>");
						t.Append("</td>");
						// Famille
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["sector"]+"</td>");
						// Groupe
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["group_"]+"</td>");
						// Numéro de page
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["media_paging"]+"</td>");
						// Surface en page
//						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+((double)(double.Parse(row["area_page"].ToString())/1000)).ToString("0.000")+"</td>");
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["area_page"].ToString(),WebConstantes.CustomerSessions.Unit.pages,false)+"</td>");
						// Surface en mmc
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["area_mmc"]+"</td>");
						// Prix
//						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+int.Parse(row["expenditure_euro"].ToString()).ToString("### ### ###")+"</td>");
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["expenditure_euro"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					
						// Descriptifs
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["location"]+"</td>");
						sqlLocation[i]=t.Length-5;
						// Format
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["format"]+"</td>");
						// Couleur
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["color"]+"</td>");
						// Rang famille
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["rank_sector"]+"</td>");
						// Rang groupe
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["rank_group_"]+"</td>");
						// Rang support
						t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["rank_media"]+"</td>");
					
						idOldLine=idLine;
						t.Append("</tr>");
					}
					else{
						t.Insert(sqlLocation[i],"-"+row["location"].ToString());				
					}	
				}				
			}	
			#endregion

			t.Append("</table>");
			t.Append("</td></tr></table>");

			return t.ToString();
		}
		#endregion
		
		#region Radio
		/// <summary>
		/// Fournit la liste des pubs pour un code-écran
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="table">table</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="excel">Si true, export excel</param>
		/// <param name="date">Jour de la semaine</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="page">page</param>
		/// <param name="allPeriod">vrai si on présente les spots de toute la période</param>
		/// <param name="codeEcran">Code écran</param>
        /// <param name="mediaAgencyYear">Année agence média</param>
		/// <returns>Code Html</returns>
		public static string GetHTMLRadioDetailMediaPopUpUI(Page page,WebSession webSession,DataTable table,Int64 idVehicle,Int64 idMedia,bool excel,string date,bool allPeriod, string codeEcran,string mediaAgencyYear){
		
			StringBuilder t=new StringBuilder(5000);
			string classStyleValue="";			
			classStyleValue="acl2";
			DateTime dayOfWeek;
			DateTime dayOfDiffusion;
			string dateDiffusion="";
			webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product;
			webSession.Save();

			#region script
            if (!page.ClientScript.IsClientScriptBlockRegistered("OpenMediaPlanAlert")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenMediaPlanAlert", WebFunctions.Script.OpenMediaPlanAlert());
			#endregion

			#region Paramètres du tableau
			if(excel){
				t.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromPortofolio(webSession,false,webSession.PeriodBeginningDate,webSession.PeriodEndDate,(int)idVehicle,(int)idMedia,allPeriod,date,codeEcran));
			}
			#endregion
			
			t.Append("<table border=0 cellpadding=0 cellspacing=0 ><tr bgcolor=#ffffff><td>");
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");
			
			if(!excel){
				t.Append("<tr valign=\"top\" height=\"30\">");
				t.Append("<td colSpan=\"13\" valign=middle class=txtViolet12Bold>"+table.Rows[0]["media"].ToString()+"</td>");	
				t.Append("</tr>");

			}
			
			#region Première ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			if(!excel){
				// Message audio
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
			}
			// date de diffusion
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1380,webSession.SiteLanguage)+"</td>");
			
			// Annonceur
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1146,webSession.SiteLanguage)+"</td>");
			// Agence média
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null)
			{
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1448,webSession.SiteLanguage)+((mediaAgencyYear.Length>0)?" ("+mediaAgencyYear+")":"")+"</td>");
			}
			if(!excel){
				// plan média produit
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1478,webSession.SiteLanguage)+"</td>");
			}
			// Produit
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
			// Famille
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1103,webSession.SiteLanguage)+"</td>");
			// groupe
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
			// Top de diffusion
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(860,webSession.SiteLanguage)+"</td>");
			// Durée
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1430,webSession.SiteLanguage)+"</td>");
			// Code Ecran
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1431,webSession.SiteLanguage)+"</td>");
			// Position
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(862,webSession.SiteLanguage)+"</td>");
			// Durée écran
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1432,webSession.SiteLanguage)+"</td>");
			// Spots écran 
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1433,webSession.SiteLanguage)+"</td>");
			// Position hap
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(865,webSession.SiteLanguage)+"</td>");
			// Durée écran hap
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(866,webSession.SiteLanguage)+"</td>");
			// Spots écran hap
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(867,webSession.SiteLanguage)+"</td>");
			// Prix
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");
			
			t.Append("</tr>");
		
			#endregion

			#region Parcours du tableau
			foreach(DataRow row in table.Rows){

				dayOfWeek=new DateTime(int.Parse(row["dateOfWeek"].ToString().Substring(0,4)),int.Parse(row["dateOfWeek"].ToString().Substring(4,2)),int.Parse(row["dateOfWeek"].ToString().Substring(6,2)));
				// Vérifie le jour de la semaine
				if(dayOfWeek.DayOfWeek.ToString()==date || allPeriod){
					t.Append("<tr  onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA>");
					if(!excel){
						// Message audio
						if(webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG)!=null
							&& row["fileData"].ToString().Length>0
							){
							t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:openDownload('"+row["fileData"]+"','"+webSession.IdSession+"','"+idVehicle+"');\"><img border=0 src=\"/Images/Common/Picto_Radio.gif\"></a></td>");
						}
						else {
							t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap></td>");
						}
					}
					// Date de diffusion
					if(row["dateOfweek"]!=System.DBNull.Value){
						dayOfDiffusion=new DateTime(int.Parse(row["dateOfweek"].ToString().Substring(0,4)),int.Parse(row["dateOfweek"].ToString().Substring(4,2)),int.Parse(row["dateOfweek"].ToString().Substring(6,2)));
						dateDiffusion=WebFunctions.Dates.dateToString(dayOfDiffusion,webSession.SiteLanguage);
						//dateDiffusion=dateDiffusion.Substring(6,2)+"/"+dateDiffusion.Substring(4,2)+"/"+dateDiffusion.Substring(0,4);
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+dateDiffusion+"</td>");
					}else{
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>&nbsp;</td>");
					}
					
					// Annonceur
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["advertiser"]+"</td>");
					// Agence Média
					if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null && table.Columns.Contains("advertising_agency"))
					{
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["advertising_agency"]+"</td>");
					}
					//Plan media du produit
					if(!excel)
						t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+row["id_product"]+"','1');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");

					// Produit					
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>");
					//if(!excel)
					//	t.Append("<a class=\""+classStyleValue+"\" title=\""+GestionWeb.GetWebWord(1447,webSession.SiteLanguage)+"\" href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+row["id_product"]+"','1');\">");
					
					t.Append(row["product"]);
					//if(!excel)
					//	t.Append("</a>");
					t.Append("</td>");
					// Famille
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["sector"]+"</td>");
					// Groupe
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["group_"]+"</td>");
					// Top de diffusion
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+(new TimeSpan(
						row["top_diffusion"].ToString().Length>=5?int.Parse(row["top_diffusion"].ToString().Substring(0,row["top_diffusion"].ToString().Length-4)):0,
						row["top_diffusion"].ToString().Length>=3?int.Parse(row["top_diffusion"].ToString().Substring(System.Math.Max(row["top_diffusion"].ToString().Length-4,0),System.Math.Min(row["top_diffusion"].ToString().Length-2,2))):0,
						int.Parse(row["top_diffusion"].ToString().Substring(System.Math.Max(row["top_diffusion"].ToString().Length-2,0),System.Math.Min(row["top_diffusion"].ToString().Length,2))))
						).ToString()+"</td>");					
					
					// Durée
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["duration"]+"</td>");
					// écran
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["ecran"]+"</td>");
					// Position
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["rank"]+"</td>");
					// Duree écran
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["duration_commercial_break"]+"</td>");
					// spot ecran
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["number_spot_com_break"]+"</td>");				
					// Position hap
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["rank_wap"]+"</td>");
					// Duree ecran hap
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["duration_com_break_wap"]+"</td>");
					// Spots ecrans hap
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["number_spot_com_break_wap"]+"</td>");				
					// Prix
//					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+int.Parse(row["expenditure_euro"].ToString()).ToString("### ### ###")+"</td>");
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["expenditure_euro"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					
					// prix du 30 sec
					//	t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["expenditure_30s_euro"]+"</td>");
				
					t.Append("</tr>");
				}			
			}
			#endregion

			t.Append("</table>");
			t.Append("</td></tr></table>");

			return t.ToString();
		
		}
		#endregion

		#region Télévision
		/// <summary>
		/// Affiche le code pour la pop-up
		/// </summary>
		/// <param name="webSession">Session Client</param>
		/// <param name="table">Table avec la liste des pubs</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="excel">indique s'il s'agit du fichier excel</param>
		/// <param name="date">date</param>
		///	<param name="page">page</param>	
		///	<param name="allPeriod">vrai si on présente les spots de toute la période</param>
		///	<param name="codeEcran">Code écran</param>
        /// <param name="mediaAgencyYear">Année agence média</param>
		/// <returns>Code Html</returns>
		public static string GetHTMLTVDetailMediaPopUpUI(Page page,WebSession webSession,DataTable table,Int64 idVehicle,Int64 idMedia,bool excel,string date,bool allPeriod, string codeEcran,string mediaAgencyYear){
		
			StringBuilder t=new StringBuilder(5000);
			string classStyleValue="";
			classStyleValue="acl2";
			DateTime dayOfWeek;
			DateTime dayOfDiffusion;
			string dateDiffusion="";
			webSession.PreformatedProductDetail=WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product;
			webSession.Save();
			
			#region script
            if (!page.ClientScript.IsClientScriptBlockRegistered("OpenMediaPlanAlert")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenMediaPlanAlert", WebFunctions.Script.OpenMediaPlanAlert());
			#endregion

			#region Paramètres du tableau
			if(excel){
				t.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromPortofolio(webSession,false,webSession.PeriodBeginningDate,webSession.PeriodEndDate,(int)idVehicle,(int)idMedia,allPeriod,date,codeEcran));
			}
			#endregion

			t.Append("<table border=0 cellpadding=0 cellspacing=0 ><tr bgcolor=#ffffff><td>");
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");
			
			if(!excel){
				t.Append("<tr valign=\"top\" height=\"30\">");
			
				t.Append("<td colSpan=\"13\" valign=middle class=txtViolet12Bold>"+table.Rows[0]["media"].ToString()+"</td>");	
				t.Append("</tr>");

			}
			
			#region Première ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			if(!excel
				//	&& (int)idVehicle!=DBClassificationConstantes.Vehicles.names.others.GetHashCode()
				){
				// Message Video
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1445,webSession.SiteLanguage)+"</td>");
			}
			// date de diffusion
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1380,webSession.SiteLanguage)+"</td>");
			// Annonceur
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1146,webSession.SiteLanguage)+"</td>");
			// Agence média
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null && table.Columns.Contains("advertising_agency"))
			{
//				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1448,webSession.SiteLanguage)+"</td>");
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1448,webSession.SiteLanguage)+((mediaAgencyYear.Length>0)?" ("+mediaAgencyYear+")":"")+"</td>");
			}
			if(!excel){
				// plan média produit
				t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1478,webSession.SiteLanguage)+"</td>");
			}
			// Produit
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
			// Famille
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1103,webSession.SiteLanguage)+"</td>");
			// groupe
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
			// Top de diffusion
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(860,webSession.SiteLanguage)+"</td>");
			// Durée
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1430,webSession.SiteLanguage)+"</td>");
			// Code Ecran
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1431,webSession.SiteLanguage)+"</td>");
			// Position
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(862,webSession.SiteLanguage)+"</td>");
			// Durée écran
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1432,webSession.SiteLanguage)+"</td>");
			// Spots écran 
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1433,webSession.SiteLanguage)+"</td>");
			// Prix
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");
			// Prix du 30 sec 
			t.Append("<td class=\"p2\" style=\"BORDER-TOP: #644883 1px solid;  BORDER-BOTTOM: #644883 1px solid\" nowrap>"+GestionWeb.GetWebWord(1446,webSession.SiteLanguage)+"</td>");
			t.Append("</tr>");
		
			#endregion

			#region Parcours du tableau
			foreach(DataRow row in table.Rows){

				
				dayOfWeek=new DateTime(int.Parse(row["dateOfWeek"].ToString().Substring(0,4)),int.Parse(row["dateOfWeek"].ToString().Substring(4,2)),int.Parse(row["dateOfWeek"].ToString().Substring(6,2)));
				// Vérifie le jour de la semaine
				if(dayOfWeek.DayOfWeek.ToString()==date || allPeriod){
					t.Append("<tr  onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA>");
					if(!excel){
						// Message audio
						if(webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG)!=null
							&& row["fileData"].ToString().Length>0
							){
							t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:openDownload('"+row["fileData"]+"','"+webSession.IdSession+"','"+idVehicle+"');\"><img border=0 src=\"/Images/Common/Picto_pellicule.gif\"></a></td>");
						}
						else if((int)idVehicle!=DBClassificationConstantes.Vehicles.names.others.GetHashCode()) {
							t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap></td>");
						}
					}
					// Date de diffusion
					if(row["dateOfweek"]!=System.DBNull.Value){
						dayOfDiffusion=new DateTime(int.Parse(row["dateOfweek"].ToString().Substring(0,4)),int.Parse(row["dateOfweek"].ToString().Substring(4,2)),int.Parse(row["dateOfweek"].ToString().Substring(6,2)));
						dateDiffusion=WebFunctions.Dates.dateToString(dayOfDiffusion,webSession.SiteLanguage);
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+dateDiffusion+"</td>");
					}else{
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>&nbsp;</td>");
					}
					
					// Annonceur
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["advertiser"]+"</td>");
					// Agence Média
					if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null && table.Columns.Contains("advertising_agency")) {
						t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["advertising_agency"]+"</td>");
					}
					//Plan media du produit
					if(!excel)
						t.Append("<td class=\""+classStyleValue+"\" align=\"center\" nowrap><a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+row["id_product"]+"','1');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
					// Produit
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>");
					t.Append(row["product"]);
					t.Append("</td>");
					// Famille
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["sector"]+"</td>");
					// Groupe
					t.Append("<td class=\""+classStyleValue+"\" align=\"left\" nowrap>"+row["group_"]+"</td>");
					// Top diffusion
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+(new TimeSpan(
						row["top_diffusion"].ToString().Length>=5?int.Parse(row["top_diffusion"].ToString().Substring(0,row["top_diffusion"].ToString().Length-4)):0,
						row["top_diffusion"].ToString().Length>=3?int.Parse(row["top_diffusion"].ToString().Substring(System.Math.Max(row["top_diffusion"].ToString().Length-4,0),System.Math.Min(row["top_diffusion"].ToString().Length-2,2))):0,
						int.Parse(row["top_diffusion"].ToString().Substring(System.Math.Max(row["top_diffusion"].ToString().Length-2,0),System.Math.Min(row["top_diffusion"].ToString().Length,2))))
						).ToString()+"</td>");	
					// Durée
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["duration"]+"</td>");
					// écran
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["ecran"]+"</td>");
					// Position
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["rank"]+"</td>");
					// Duree écran
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["duration_commercial_break"]+"</td>");
					// spot ecran
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+row["number_message_commercial_brea"]+"</td>");
					// Prix
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["expenditure_euro"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					// Prix du 30 sec 
					t.Append("<td class=\""+classStyleValue+"\" align=\"right\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(row["expenditure_30s_euro"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					t.Append("</tr>");
				}	
			}
			#endregion

			t.Append("</table>");
			t.Append("</td></tr></table>");

			return t.ToString();
		
		}

		#endregion
		
		#endregion

		#endregion

		#region Structure
		/// <summary>
		/// Obtient le tableau de résultats pour la structure d'une  alerte de portefeuille
		/// </summary>
		/// <param name="page">page courante</param>
		/// <param name="webSession">session du client</param>
		/// <param name="Excel">vrai pour sortie au format ecel</param>
		/// <returns>code html du tableau de résultats à afficher</returns>
		public static string  GetHTMLStructureUI(Page page,WebSession webSession,bool Excel){
			
			#region Variables
			
			object[,] tab=null;
			StringBuilder t=new StringBuilder(5000);
			DataTable dtFormat;
			DataTable dtColor;
			DataTable dtInsert;
			DataTable dtLocation;
		//	string L1="acl1";
			string L2="acl2";
		//	string L3="acl3";					
			#region style excel
			if(Excel){
			//	L1="acl11";
				L2="acl21";
			//	L3="acl31";
			}
			string classCss=L2;
			#endregion
			#endregion

			#region Paramétrage des dates
			int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
			#endregion	
			
			//id Média
			string idVehicle=webSession.GetSelection(webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
			
			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				case DBClassificationConstantes.Vehicles.names.internationalPress :
					//UI presse inter
					PortofolioRules.GetStructPressTab(webSession,dateBegin,dateEnd,out dtFormat,out dtColor,out dtLocation,out dtInsert);
					if(dtFormat==null && dtColor==null && dtInsert==null && dtLocation==null){
						#region Pas de données à afficher
						return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
							+"</div>");
						#endregion
					}					
					t.Append(GetPressTab(webSession,dtFormat,dtColor,dtInsert,dtLocation,classCss));					
					break;
				case DBClassificationConstantes.Vehicles.names.press :
					//UI presse
					PortofolioRules.GetStructPressTab(webSession,dateBegin,dateEnd,out dtFormat,out dtColor,out dtLocation,out dtInsert);
					if(dtFormat==null && dtColor==null && dtInsert==null && dtLocation==null){
						#region Pas de données à afficher
						return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
							+"</div>");
						#endregion
					}					
					t.Append(GetPressTab(webSession,dtFormat,dtColor,dtInsert,dtLocation,classCss));					
					break;
				case DBClassificationConstantes.Vehicles.names.radio :
					//UI radio
					tab = PortofolioRules.GetStructRadioTab(webSession,dateBegin,dateEnd);
					#region Pas de données à afficher
					if(tab==null || tab.GetLength(0)==0 ){
						return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
							+"</div>");
					}			
					#endregion
				
					if(tab!=null)
						t.Append(GetTvOrRadioTab(webSession,tab,idVehicle,classCss));
					break;
				case DBClassificationConstantes.Vehicles.names.tv :
				case DBClassificationConstantes.Vehicles.names.others :
					//UI télé
					tab = PortofolioRules.GetStructTVTab(webSession,dateBegin,dateEnd);
					#region Pas de données à afficher
					if(tab==null || tab.GetLength(0)==0 ){
						return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
							+"</div>");
					}			
					#endregion				
					if(tab!=null)
						t.Append(GetTvOrRadioTab(webSession,tab,idVehicle,classCss));					
					break;
				default :				
					throw new Exceptions.PortofolioUIException("Le cas de ce média n'est pas gérer.");
			}
			return t.ToString();
		}
		#endregion

		#region Performances
		/// <summary>
		/// Génère le code html pour afficher les performances du portefeuille d'un support
		/// </summary>
		/// <param name="page">Page utilisée pour montrer le résultat</param>		
		/// <param name="webSession">Session du client</param>
		/// <param name="Excel">vrai pour sortie au format excel</param>
		/// <returns>Code HTML</returns>
		public static string GetHTMLPerformancesUI(Page page,WebSession webSession,bool Excel){
			
			#region variables
			DataSet ds=null;				
			StringBuilder t=new StringBuilder(5000);
		//	string L1="acl1";
			string L2="acl2";
		//	string L3="acl3";
				

			#region style excel
			if(Excel){
			//	L1="acl11";
				L2="acl21";
			//	L3="acl31";
			}
			#endregion
			string classCss=L2;
			string creation="";
			string bgcolor="#B1A3C1";
			string P2="p2";
			string parutionDate="";
			DateTime dtParutionDate=DateTime.Now;
			#endregion
			
			#region Paramétrage des dates
			int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
			#endregion

			#region script
            if (!page.ClientScript.IsClientScriptBlockRegistered("openCreationByParution")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openCreation", WebFunctions.Script.OpenCreationByParution());			
			#endregion

			//Chargement des données
			ds=PortofolioDataAccess.GetPerformancesData(webSession,dateBegin,dateEnd);
			
			#region Pas de données à afficher
			if(ds==null && ds.Tables[0].Rows.Count==0){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}
			#endregion

			//id Média
			string idVehicle=webSession.GetSelection(webSession.SelectionUniversMedia,TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
		
			
			//Debut du tableau
			t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 >");					
			
			#region libellés colonnes
			// Première ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1453,webSession.SiteLanguage)+"</td>");					
			if(!Excel)t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");			
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1423,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1424,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(943,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+"</td>");			
			t.Append("</tr>");	
			#endregion	
			
			//1 ligne par parution
			foreach(DataRow dr in ds.Tables[0].Rows){								
				t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='"+bgcolor+"';\"  bgcolor="+bgcolor+" height=\"20px\" >");
					
				#region Parution
				parutionDate=dr["date_media_num"].ToString();
				if(parutionDate.Length>0){
					dtParutionDate=new DateTime(int.Parse(parutionDate.Substring(0,4)),int.Parse(parutionDate.Substring(4,2)),int.Parse(parutionDate.Substring(6,2)));
				}
				#endregion
				
				t.Append("\r\n\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+dtParutionDate.Date.ToString("dd/MM/yyyy")+"</td>");
				if(!Excel){
					//Créations
					creation="<a href=\"javascript:openCreationByParution('"+webSession.IdSession+"','"+idVehicle+",-1,"+((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID+",-1','"+dr["date_media_num"].ToString()+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
					t.Append("\r\n\t<td class=\""+classCss+"\" align=\"center\" nowrap>"+creation+"</td>");
				}
				//Euros						
//				t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(dr["euro"].ToString()).ToString("### ### ### ### ##0")+"</td>");
				t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["euro"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					
				//mmcpercol						
//				t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(dr["mmpercol"].ToString()).ToString("### ### ### ### ##0")+"</td>");
				t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["mmpercol"].ToString(),WebConstantes.CustomerSessions.Unit.mmPerCol,false)+"</td>");
				
				//pages
				t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["pages"].ToString(),WebConstantes.CustomerSessions.Unit.pages,false)+"</td>");
				
				//insertion						
//				t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(dr["insertion"].ToString()).ToString("### ### ### ### ##0")+"</td>");
				t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["insertion"].ToString(),WebConstantes.CustomerSessions.Unit.insertion,false)+"</td>");
				t.Append("</tr>");								
			}

			//Fin du tableau
			t.Append("</table>");

			return t.ToString();
		}
		#endregion

		#region Calendrier d'action
		/// <summary>
		/// Génère le code HTML pour le calendrier d'action
		/// </summary>
		/// <param name="page">page</param>
		/// <param name="tab">tab</param>
		/// <param name="webSession">Session client</param>
		/// <returns>code HTML</returns>
		/// <param name="excel">True pour fichier Excel</param>
		public static string GetHTMLCalendarUI(Page page,object[,] tab,WebSession webSession,bool excel){
		
			#region Pas de données à afficher
			if(tab==null || tab.GetLength(0)==0 ){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}			
			#endregion

			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);	
			string classCss="";
			string L1="acl1";
			string L2="acl2";
			string L3="acl3";
			string P2="p2";
			bool newLine=false;
			int length=tab.GetLength(1);
			string totalUnit="";
			int i=0;
			string tmp="";
			string firstBaliseA="";
			string endBaliseA="";
			//bool showMediaPlan=false;
			string mediaplan="";
			Int64 idCreation=-1;
			string levelProd="";
			#endregion
			
			#region style excel
			if(excel){
				L1="acl11";
				L2="acl21";
				L3="acl31";
			}
			#endregion

			#region libellés colonnes
			t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0>");
			// Première ligne
			t.Append("\r\n\t<tr height=\"40px\" >");
			// Support
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
			// Plan média
			if((WebFunctions.ProductDetailLevel.ShowMediaPlanL3(webSession) ||  WebFunctions.ProductDetailLevel.ShowMediaPlanL2(webSession)) && !excel){
				t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(751,webSession.SiteLanguage)+"</td>");
			}
			// Total
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1401,webSession.SiteLanguage)+"</td>");
			//PDM
			t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1236,webSession.SiteLanguage)+"</td>");
			
			// Parution 
			for(i=ResultConstantesCalendar.TOTAL_INDEX;i<length;i++){
				t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY((int)tab[ResultConstantesCalendar.DATE_INDEX,i],webSession.SiteLanguage)+"</td>");
			}
			#endregion

			#region script
			if (!page.ClientScript.IsClientScriptBlockRegistered("openGad")){
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openGad", TNS.AdExpress.Web.Functions.Script.OpenGad());
			}
            if (!page.ClientScript.IsClientScriptBlockRegistered("OpenMediaPlanAlert")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenMediaPlanAlert", WebFunctions.Script.OpenMediaPlanAlert());
			#endregion

			#region Ligne total
			t.Append("\r\n\t<tr height=\"20px\" >");
			// Texte Total
			t.Append("<td align= \"left\" class=\"acl1\" nowrap>"+GestionWeb.GetWebWord(1401,webSession.SiteLanguage)+"</td>");
			// Plan média
			if((WebFunctions.ProductDetailLevel.ShowMediaPlanL3(webSession) ||  WebFunctions.ProductDetailLevel.ShowMediaPlanL2(webSession)) && !excel){
				t.Append("<td class=\"acl1\" align=\"right\" bgcolor=#ffffff nowrap></td>");
			}
			// Total
			totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantesCalendar.TOTAL_LINE,ResultConstantesCalendar.TOTAL_VALUE_COLUMN_INDEX].ToString(),webSession.Unit,webSession.Percentage);
			if(webSession.Percentage)
				totalUnit="100";
			t.Append("<td class=\"acl1\" align=\"right\" nowrap>"+totalUnit+"</td>");
			//PDM
			t.Append("<td class=\"acl1\" align=\"right\" nowrap>100</td>");
			
			// Parution 
			for(i=ResultConstantesCalendar.TOTAL_INDEX;i<length;i++){
				totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantesCalendar.TOTAL_LINE,i].ToString(),webSession.Unit,webSession.Percentage);
				t.Append("<td  class=\"acl1\" align=\"right\" nowrap>"+totalUnit+"</td>");
			}
			#endregion

			for(i=ResultConstantesCalendar.DATE_INDEX+2;i<tab.GetLength(0);i++){					
										
				#region Level 1
				if(tab[i,ResultConstantesCalendar.IDL1_INDEX]!=null && tab[i,ResultConstantesCalendar.IDL2_INDEX]==null && tab[i,ResultConstantesCalendar.IDL3_INDEX]==null){														
					classCss=L1;
					//level=0;
					levelProd="1";
					idCreation = Int64.Parse(tab[i,ResultConstantesCalendar.IDL1_INDEX].ToString());
					//Créations
					//if(!showCreation){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
					//else{creation="";}
					//Plan média
					if(WebFunctions.ProductDetailLevel.ShowMediaPlanL1(webSession)){mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','"+levelProd+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
					else{mediaplan="";}	
					//Adresse Gad
					if(tab[i,ResultConstantesCalendar.ADDRESS_COLUMN_INDEX]!=null){
						tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[i,ResultConstantesCalendar.LABELL1_INDEX]+"','"+tab[i,ResultConstantesCalendar.ADDRESS_COLUMN_INDEX]+"');";
						firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
						endBaliseA="</a>"; 
					}else{
						firstBaliseA="";
						endBaliseA=""; 		
					}
					//libellé niveau 1																	
					if(!excel)							
						t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+firstBaliseA+tab[i,ResultConstantes.LABELL1_INDEX]+endBaliseA+"</td>");								
					else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+tab[i,ResultConstantesCalendar.LABELL1_INDEX]+"</td>");								
					newLine=true;									 							
				}
				#endregion
					
				#region Level 2
				if(tab[i,ResultConstantesCalendar.IDL1_INDEX]==null && tab[i,ResultConstantesCalendar.IDL2_INDEX]!=null && tab[i,ResultConstantesCalendar.IDL3_INDEX]==null){														
					classCss=L2;
				//	level=2;
					levelProd="2";
					idCreation = Int64.Parse(tab[i,ResultConstantesCalendar.IDL2_INDEX].ToString());
					//Créations
				//	if(!showCreation){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
				//	else{creation="";}						
					//Plan média
					if(WebFunctions.ProductDetailLevel.ShowMediaPlanL3(webSession)){mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','"+levelProd+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
					else{mediaplan="";}							
					//Adresse Gad
					if(tab[i,ResultConstantesCalendar.ADDRESS_COLUMN_INDEX]!=null){
						tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[i,ResultConstantesCalendar.LABELL2_INDEX]+"','"+tab[i,ResultConstantesCalendar.ADDRESS_COLUMN_INDEX]+"');";
						firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
						endBaliseA="</a>"; 
					}else{
						firstBaliseA="";
						endBaliseA=""; 		
					}
					//libellé niveau 2																
					
				//	if(boolProductL2(webSession)){
						if(!excel)
							t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\"  bgcolor=#D0C8DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[i,ResultConstantesCalendar.LABELL2_INDEX]+endBaliseA+"</td>");								
						else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\"  bgcolor=#D0C8DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantesCalendar.LABELL2_INDEX]+"</td>");								
				//	}
				//	else{
					//	if(!excel)
					//		t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\"  bgcolor=#D0C8DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantesCalendar.LABELL2_INDEX]+"</td>");								
					//	else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\"  bgcolor=#D0C8DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantesCalendar.LABELL2_INDEX]+"</td>");								
				//	}
						
					newLine=true;								 							
				}
				#endregion

				#region Level 3
				if(tab[i,ResultConstantesCalendar.IDL1_INDEX]==null && tab[i,ResultConstantesCalendar.IDL2_INDEX]==null && tab[i,ResultConstantesCalendar.IDL3_INDEX]!=null){														
					classCss=L3;
				//	level=4;
					idCreation = Int64.Parse(tab[i,ResultConstantesCalendar.IDL3_INDEX].ToString());
					levelProd="3";
					//Créations
				//	if(!showCreation){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
				//	else{creation="";}						
					//Plan média
					if(WebFunctions.ProductDetailLevel.ShowMediaPlanL3(webSession)){mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','"+levelProd+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
					else{mediaplan="";}	
					//Adresse Gad
					if(tab[i,ResultConstantesCalendar.ADDRESS_COLUMN_INDEX]!=null){
						tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[i,ResultConstantesCalendar.LABELL3_INDEX]+"','"+tab[i,ResultConstantesCalendar.ADDRESS_COLUMN_INDEX]+"');";
						firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
						endBaliseA="</a>"; 
					}else{
						firstBaliseA="";
						endBaliseA=""; 		
					}
					//libellé niveau 3																
							
					if(CanShowMediaPlanL3(webSession)){
						if(!excel)
							t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\"  bgcolor=#E1E0DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[i,ResultConstantesCalendar.LABELL3_INDEX]+endBaliseA+"</td>");								
						else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\"  bgcolor=#E1E0DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantesCalendar.LABELL3_INDEX]+"</td>");								 
					}
					else{
				//		if(!Excel)
							t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\"  bgcolor=#E1E0DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantesCalendar.LABELL3_INDEX]+"</td>");								
				//		else t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\"  bgcolor=#E1E0DA height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[i,ResultConstantes.LABELL3_INDEX]+"</td>");								
					}
						
					newLine=true;									 							
				}
				#endregion						

				// Plan média
				if((WebFunctions.ProductDetailLevel.ShowMediaPlanL3(webSession) || WebFunctions.ProductDetailLevel.ShowMediaPlanL2(webSession)) && !excel){
					t.Append("<td class=\""+classCss+"\" align=\"center\" >"+mediaplan+"</td>");
					mediaplan="";
				}
				
				totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantesCalendar.TOTAL_VALUE_COLUMN_INDEX].ToString(),webSession.Unit,webSession.Percentage);
				

				if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" || totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="";
				// Total
				if(tab[i,ResultConstantesCalendar.TOTAL_VALUE_COLUMN_INDEX]!=null){
					// Si PDM le total vaut 100
					if(webSession.Percentage){
						totalUnit="100";
					}
					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+totalUnit+"</td>");
				}
				else{
					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
				}				
				// PDM
				if(tab[i,ResultConstantesCalendar.PDM_COLUMN_INDEX]!=null){
					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,ResultConstantesCalendar.PDM_COLUMN_INDEX].ToString()).ToString("0.00")+"</td>");
				}
				else{
					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
				}

				for(int j=ResultConstantesCalendar.TOTAL_INDEX;j<length;j++){

					if(tab[i,j]!=null ){
						totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
						if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" || totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="";
						t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+totalUnit+"</td>");
					}
					else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
				}
				if(newLine)t.Append("</tr>");
				newLine=false;

			}
			t.Append("</table>");

			return t.ToString();
		}
		#endregion

		#region Méthodes Internes
		/// <summary>
		/// Pour connaître le style du libellé à utiliser
		/// </summary>
		/// <param name="classStyleTitle">css en cours</param>
		/// <returns>nouveau style</returns>
		protected static string InversClassStyleTitle(string classStyleTitle){
			string valeur="";
			if(classStyleTitle==portofolioTitle1){
				valeur=portofolioTitle2;
			}else {
				valeur=portofolioTitle1;
			}
			return valeur;
		}

		/// <summary>
		/// Pour connaître le style de la valeur à utiliser
		/// </summary>
		/// <param name="classStyleValue">css en cours</param>
		/// <returns>nouveau style</returns>
		protected static string InversClassStyleValue(string classStyleValue){
			string valeur="";
			if(classStyleValue==portofolioValue1){
				valeur=portofolioValue2;
			}else {
				valeur=portofolioValue1;
			}
			return valeur;
		}

		/// <summary>
		/// Affiche le lien vers le détail des insertions sur la période sélectionnée.
		/// </summary>
		/// <param name="t">Constructeur du lien</param>
		/// <param name="linkText">texte du lien</param>
        /// <param name="webSession">session client</param>
		/// <param name="idMedia">identifiant support</param>
        private static void GetAllPeriodInsertions(StringBuilder t, string linkText, WebSession webSession, string idMedia) {
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			t.Append("<table border=0 cellpadding=0 cellspacing=0 >");
            t.Append("<TR height=10><TD ><a class=\"roll03\" href=\"javascript:portofolioDetailMedia('" + webSession.IdSession + "','" + idMedia + "','','');\" ");
            t.Append(" onmouseover=\"detailSpotButton.src='/App_Themes/"+themeName+"/Images/Common/detailSpot_down.gif';\" onmouseout=\"detailSpotButton.src='/App_Themes/"+themeName+"/Images/Common/detailSpot_up.gif';\" ");
            t.Append("><IMG NAME=\"detailSpotButton\" src=\"/App_Themes/" + themeName + "/Images/Common/detailSpot_up.gif\" BORDER=0 align=absmiddle alt=\"" + linkText + "\">");
			t.Append("&nbsp;"+linkText);
			t.Append("</a></TD></TR>");
			t.Append("</table>");
			t.Append("<br><br>");
		}

		/// <summary>
		/// Génère le code HTML du tableau de structure pour la télé et la radio
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="tab">tableau à afficher</param>
		/// <param name="idVehicle">identifiant du média</param>
		/// <param name="classCss">classe de style</param>
		/// <returns>code html du tableau à afficher</returns>
		private static string GetTvOrRadioTab(WebSession webSession, object[,] tab,string idVehicle,string classCss){
			
			StringBuilder t=new StringBuilder(5000);
			string P2="p2";
            string backGround = "whiteBackGround";

            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");					
			
			#region libellés colonnes
			// Première ligne
			t.Append("\r\n\t<tr height=\"20px\" >");
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.radio)
				t.Append("<td class=\""+P2+" whiteBackGround\" nowrap>"+GestionWeb.GetWebWord(1299,webSession.SiteLanguage)+"</td>");
			else if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.tv
				|| (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())==DBClassificationConstantes.Vehicles.names.others
				)
				t.Append("<td class=\""+P2+" whiteBackGround\" nowrap>"+GestionWeb.GetWebWord(1451,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+" whiteBackGround\" nowrap>"+GestionWeb.GetWebWord(1423,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+" whiteBackGround\" nowrap>"+GestionWeb.GetWebWord(869,webSession.SiteLanguage)+"</td>");
			t.Append("<td class=\""+P2+" whiteBackGround\" nowrap>"+GestionWeb.GetWebWord(1435,webSession.SiteLanguage)+"</td>");						
			t.Append("</tr>");	
			#endregion	

			//1 ligne par tranche horaire
			for(int i=0;i<tab.GetLength(0);i++){
				classCss="acl1";
				if(tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX]!=null && tab[i,PortofolioStructure.EUROS_COLUMN_INDEX]!=null && 
					tab[i,PortofolioStructure.EUROS_COLUMN_INDEX]!=null && tab[i,PortofolioStructure.SPOT_COLUMN_INDEX]!=null &&
					tab[i,PortofolioStructure.DURATION_COLUMN_INDEX]!=null ){
                    t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='" + backGround + "';\"  class=\"" + backGround + "\" height=\"20px\" >");
					
					//tranche horaire										
					t.Append("\r\n\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+tab[i,PortofolioStructure.MEDIA_HOURS_COLUMN_INDEX].ToString()+"</td>");
					//else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
					
					//Euros						
//					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(tab[i,PortofolioStructure.EUROS_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,PortofolioStructure.EUROS_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.euro,false)+"</td>");
					
					//Spot						
//					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+long.Parse(tab[i,PortofolioStructure.SPOT_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,PortofolioStructure.SPOT_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.spot,false)+"</td>");
					//else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
					
					//Durée
					t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,PortofolioStructure.DURATION_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.duration,false)+"</td>");
					//else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
					t.Append("</tr>");
				}
				classCss="acl2";
                backGround = "violetBackGroundV3";
			}
			t.Append("</table>");

			return t.ToString();
			
		}
		/// <summary>
		///  Génère le code HTML du tableau de structure pour la presse
		/// </summary>
		/// <param name="dtFormat">table de données pour format</param>
		/// <param name="dtColor">table de données pour couleur</param>
		/// <param name="dtInsert">table de données pour encarts</param>
		/// <param name="dtLocation">table de données pour emplacements</param>
		/// <param name="classCss">classe de style</param>
		/// <param name="webSession">session du client</param>
		/// <returns>code html pour résultats de structure presse</returns>
		private static string GetPressTab(WebSession webSession, DataTable dtFormat,DataTable dtColor,DataTable dtInsert,DataTable dtLocation,string classCss){
			
			StringBuilder t=new StringBuilder(8000);			
			//string P2="p2";	

            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 >");	
			
			#region libellés colonnes
			// Première ligne
//			t.Append("\r\n\t<tr height=\"20px\" >");
//			t.Append("<td bgcolor=#ffffff nowrap></td>"); 
//			t.Append("<td colspan=2 class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+"</td>");			
//			t.Append("</tr>");	
			#endregion	

			//format
			if(dtFormat!=null && dtFormat.Rows.Count>0)
				t.Append(GetVentilationLines(webSession,dtFormat,1420,classCss,PortofolioStructure.Ventilation.format));
			//couleur
			if(dtColor!=null && dtColor.Rows.Count>0)
				t.Append(GetVentilationLines(webSession,dtColor,1438,classCss,PortofolioStructure.Ventilation.color));
			//Emplacements
			if( dtLocation!=null && dtLocation.Rows.Count>0){
				t.Append(GetVentilationLines(webSession,dtLocation,1439,classCss,PortofolioStructure.Ventilation.location));
			}
			//Encarts
			if(dtInsert!=null && dtInsert.Rows.Count>0)
				t.Append(GetVentilationLines(webSession,dtInsert,1440,classCss,PortofolioStructure.Ventilation.insert));			
			t.Append("</table>");

			return t.ToString();
		}

		/// <summary>
		/// Obtient les lignes format ou couleur ou emplacements ou encarts
		/// </summary>
		/// <param name="dt">table de données</param>
		/// <param name="labelcode">code du libéllé de la ventilation</param>
		/// <param name="classCss">classe de style</param>
		/// <param name="webSession">session du client</param>
		/// <param name="ventilation">format ou couleur ou emplacements ou encarts</param>
		/// <returns>code html lignes </returns>
		private static string GetVentilationLines(WebSession webSession,DataTable dt,int labelcode,string classCss,PortofolioStructure.Ventilation ventilation){
			StringBuilder t=new StringBuilder(5000);	
			string ventilationType="";
			switch(ventilation){
				case PortofolioStructure.Ventilation.color :
					ventilationType="color";
					break;
				case PortofolioStructure.Ventilation.format :
					ventilationType="format";
					break;
				case PortofolioStructure.Ventilation.insert :
					ventilationType="inset";
					break;
				case PortofolioStructure.Ventilation.location :
					ventilationType="location";
					break;
				default :				
					throw new Exceptions.PortofolioUIException("GetVentilationLines(WebSession webSession,DataTable dt,int labelcode,string classCss,PortofolioStructure.Ventilation ventilation)-->Le type de ventilation ne peut pas être déterminé.");
			}
			//libellé
            t.Append("\r\n\t<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");
            t.Append("\r\n\t<td align=\"left\" class=\"p2\" class=\"whiteBackGround\" nowrap><b>" + GestionWeb.GetWebWord(labelcode, webSession.SiteLanguage) + "</b></td>"); 
			t.Append("\r\n\t<td  class=\"p2\"  nowrap>"+GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+"</td>");
			t.Append("</tr>");
			//Nombre d'insertions
			foreach(DataRow dr in dt.Rows){
                t.Append("\r\n\t<tr  onmouseover=\"this.className='whiteBackGround';\" onmouseout=\"this.className='violetBackGroundV3';\"  class=\"violetBackGroundV3\" height=\"20px\" >");								
				if(dr[ventilationType]!=null )
					t.Append("\r\n\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+dr[ventilationType].ToString()+"</td>");
				else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
				if(dr["insertion"]!=null ){
//					t.Append("\r\n\t<td align=\"right\" class=\""+classCss+"\" nowrap>"+long.Parse(dr["insertion"].ToString()).ToString("### ### ### ### ##0")+"</td>");
					t.Append("\r\n\t<td align=\"right\" class=\""+classCss+"\" nowrap>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(dr["insertion"].ToString(),WebConstantes.CustomerSessions.Unit.insertion,false)+"</td>");
				}
				else t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>&nbsp;</td>");
				t.Append("</tr>");
			}
			return t.ToString();
		}
		
		#endregion
		
		#region Afficher raccourci vers plan média
		/// <summary>
		/// Détermine de récupérer les données pour le niveau produit 3
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>vrai</returns>
		protected static bool CanShowMediaPlanL3(WebSession webSession){
				switch(webSession.PreformatedProductDetail){												
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:				
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:	
				case CstWeb.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:		
					return(true);								
				default:
					return(false);
			}
		}
		#endregion

		#region Excel

		#region Synthèse publicitaire
		/// <summary>
		/// Génère le fichier Excel pour la planche synthèse
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="moduleType">type de module</param>
		/// <returns>Code Html Pour Excel</returns>
		public static string SynthesisExcel(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,TNS.AdExpress.Constantes.Web.Module.Type moduleType){
			
			#region Variables 
			StringBuilder t=new StringBuilder(5000);

			#endregion

			#region Rappel des paramètres
			// Paramètres du tableau
            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1410,webSession.SiteLanguage)));

			#endregion			

			switch(webSession.CurrentModule){
				case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:								
					t.Append(Synthesis(webSession,idVehicle,idMedia,dateBegin,dateEnd,true));
					break;
				case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:				
					t.Append(TNS.AdExpress.Web.UI.Results.PortofolioUI.SynthesisAnalysis(webSession,idVehicle,idMedia,dateBegin,dateEnd,true));
					break;
			}
			t.Append(ExcelFunction.GetFooter(webSession));
			return Convertion.ToHtmlString(t.ToString());		
		}
		#endregion

		#region Nouveauté
		/// <summary>
		/// Génère le fichier Excel pour la planche nouveauté
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant du vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>Code Html Pour Excel</returns>
		public static string GetExcelNoveltyUI(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
							
			#region Variables 
			StringBuilder t=new StringBuilder(5000);
			#endregion

			#region Rappel des paramètres
			// Paramètres du tableau
            t.Append(ExcelFunction.GetLogo(webSession));
			if(webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.NOVELTY)
			{
				t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1310,webSession.SiteLanguage)));
			}
			else
				t.Append(ExcelFunction.GetExcelHeader(webSession,""));
			#endregion			

			t.Append(GetHTMLNoveltyUI(webSession,idVehicle,idMedia,dateBegin,dateEnd,true));
			t.Append(ExcelFunction.GetFooter(webSession));
			return t.ToString();		
		}
		#endregion
		
		#region Détail support
		/// <summary>
		/// Export Excel pour le détail support
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>Export Excel</returns>
		public static string GetExcelDetailMediaUI(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		          
			#region Variables 
			StringBuilder t=new StringBuilder(5000);
			#endregion
			
			#region Rappel des paramètres
			// Paramètres du tableau
			t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1458,webSession.SiteLanguage)));
//			t.Append("<table border=0 cellpadding=0 cellspacing=0>");
//			// Alerte Portefeuille
//			t.Append("<tr><td>"+GestionWeb.GetWebWord(1062,webSession.SiteLanguage)+"</td></tr>");
//			t.Append("<tr>");
//			t.Append("<td>");		
//			t.Append(""+GestionWeb.GetWebWord(1458,webSession.SiteLanguage)+"");					
//			t.Append("</td>");
//			t.Append("</tr>");
//			// Période d'étude
//			t.Append("<tr><td>"+GestionWeb.GetWebWord(119,webSession.SiteLanguage)+" : "+DateString.YYYYMMDDToDD_MM_YYYY(dateBegin,webSession.SiteLanguage)+"-"+DateString.YYYYMMDDToDD_MM_YYYY(dateEnd,webSession.SiteLanguage)+"</td></tr>");
//			t.Append("<tr><td>&nbsp;</td></tr>");
//			t.Append("</table>");
			#endregion		
			
			t.Append(TNS.AdExpress.Web.UI.Results.PortofolioUI.DetailMediaUI(webSession,((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID,((LevelInformation)webSession.ReferenceUniversMedia.FirstNode.Tag).ID,dateBegin.ToString(),dateEnd.ToString(),true));
			
			return t.ToString();
		}
		
		/// <summary>
		/// Génère l'export Excel pour le detail support du portefeuille
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="table">table provenant du rule</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="date">jour de parution</param>
		/// <param name="excel">True si export excel</param>
		/// <param name="page">page</param>
		///	<param name="allPeriod">vrai si on présente les spots de toute la période</param>
		///	<param name="codeEcran">Code écran</param>
		/// <returns>HTML</returns>
		public static string GetExcelDetailMediaPopUpUI(Page page,WebSession webSession,DataTable table,Int64 idVehicle,Int64 idMedia,string date,bool excel,bool allPeriod, string codeEcran){
			
			string mediaAgencyYear="";
			DataTable dt=TNS.AdExpress.Web.DataAccess.Results.MediaAgencyDataAccess.GetListYear(webSession).Tables[0];
				
			if(dt!=null && dt.Rows.Count>0){
				//On récupère la dernière année des agences médias
				mediaAgencyYear = dt.Rows[0]["year"].ToString();
			}

			switch((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())){
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return GetHTMLPressDetailMediaPopUpUI(page,webSession,table,idMedia,excel,date,allPeriod,int.Parse(idVehicle.ToString()),mediaAgencyYear);
				case DBClassificationConstantes.Vehicles.names.press:
					return GetHTMLPressDetailMediaPopUpUI(page,webSession,table,idMedia,excel,date,allPeriod,int.Parse(idVehicle.ToString()),mediaAgencyYear);
				case DBClassificationConstantes.Vehicles.names.radio:
					return GetHTMLRadioDetailMediaPopUpUI(page,webSession,table,idVehicle,idMedia,excel,date,allPeriod,codeEcran,mediaAgencyYear);
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return GetHTMLTVDetailMediaPopUpUI(page,webSession,table,idVehicle,idMedia,excel,date,allPeriod,codeEcran,mediaAgencyYear);
				default:
					throw new Exceptions.PortofolioUIException("Le cas de ce média n'est pas gérer.");
			}
		}
		#endregion
		
		#region Détail portefeuille
		/// <summary>
		/// Génère l'export Excel pour le détail portefeuille
		/// </summary>
		/// <param name="page">PAGE courante</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="webSession">session du client</param>
		/// <param name="Excel">vrai si sortie excel</param>
		/// <returns>code html pour sortie excel</returns>
		public static string GetExcelPortofolioUI(Page page,object[,] tab,WebSession webSession,bool Excel){
			#region Variables 
			StringBuilder t=new StringBuilder(5000);
			#endregion

			// Paramètres du tableau
			t.Append(ExcelFunction.GetExcelHeader(webSession,true,false,false,true,GestionWeb.GetWebWord(1597,webSession.SiteLanguage)));

//			t.Append("<table border=0 cellpadding=0 cellspacing=0>");
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(464,webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(119,webSession.SiteLanguage)+" : "+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage)+" - "
//				+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");			
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(1124,webSession.SiteLanguage)+" "+WebFunctions.ProductDetailLevel.LevelProductToExcelString(webSession)+"</td>");
//			t.Append("</tr>");
//			t.Append("<tr>");
//			t.Append("<td>&nbsp;</td>");
//			t.Append("</tr>");
//			t.Append("</table>");

			t.Append(GetHTMLPortofolioUI(page,tab,webSession,true));

			return t.ToString();	
		}
		#endregion

		#region structure (excel)
		/// <summary>
		///Génère l'export Excel pour la STRUCTURE d'un portefeuille
		/// </summary>
		/// <param name="page">page courante</param>
		/// <param name="webSession">sessioin du client</param>
		/// <param name="Excel">vrai si sortie excel</param>
		/// <returns>tableau formatté pour affichage sous excel</returns>
		public static string  GetExcelStructureUI(Page page,WebSession webSession,bool Excel){
			#region Variables 
			StringBuilder t=new StringBuilder(5000);

			// Paramètres du tableau
            t.Append(ExcelFunction.GetLogo(webSession));
			t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1379,webSession.SiteLanguage)));

			#endregion

			t.Append(GetHTMLStructureUI(page,webSession,true));
			t.Append(ExcelFunction.GetFooter(webSession));
			return t.ToString();	
		}
		#endregion

		#region performances
		/// <summary>
		/// Excel pour performance
		/// </summary>
		/// <param name="page">page</param>
		/// <param name="webSession">Session client</param>
		/// <param name="Excel">true si excel</param>
		/// <returns>Excel pour les performances</returns>
		public static string GetExcelPerformancesUI(Page page,WebSession webSession,bool Excel){
			#region Variables 
			StringBuilder t=new StringBuilder(5000);

			// Paramètres du tableau
			t.Append(ExcelFunction.GetExcelHeader(webSession,GestionWeb.GetWebWord(1452,webSession.SiteLanguage)));
//			t.Append("<table border=0 cellpadding=0 cellspacing=0>");
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(1452,webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(464,webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(119,webSession.SiteLanguage)+" : "+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage)+" - "
//				+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");						
//			t.Append("<tr>");
//			t.Append("<td>&nbsp;</td>");
//			t.Append("</tr>");
//			t.Append("</table>");
			#endregion

			t.Append(GetHTMLPerformancesUI(page,webSession,true));

			return t.ToString();
		}
		#endregion

		#region Calendrier d'action
		/// <summary>
		/// Génère l'export Excel pour le calendrier d'actions
		/// </summary>
		/// <param name="page">PAGE courante</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="webSession">session du client</param>
		/// <param name="Excel">vrai si sortie excel</param>
		/// <returns>code html pour sortie excel</returns>
		public static string GetExcelCalendarUI(Page page,object[,] tab,WebSession webSession,bool Excel){
			#region Variables 
			StringBuilder t=new StringBuilder(5000);
			#endregion

			// Paramètres du tableau
			t.Append(ExcelFunction.GetExcelHeader(webSession,true,true,false,true,GestionWeb.GetWebWord(1474,webSession.SiteLanguage)));
//			t.Append("<table border=0 cellpadding=0 cellspacing=0>");
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(1474,webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(119,webSession.SiteLanguage)+" : "+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage)+" - "
//				+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage)+"</td>");
//			t.Append("</tr>");			
//			t.Append("<tr>");
//			t.Append("<td>"+GestionWeb.GetWebWord(1124,webSession.SiteLanguage)+" "+stringLevelProduct(webSession)+"</td>");
//			t.Append("</tr>");
//			t.Append("<tr>");
//			t.Append("<td>&nbsp;</td>");
//			t.Append("</tr>");
//			t.Append("</table>");
			t.Append(GetHTMLCalendarUI(page,tab,webSession,true));

			return t.ToString();	
		}
		#endregion

		#endregion

		#region Excel des données brutes

		#region Détail portefeuille
		/// <summary>
		/// Génère l'export Excel des données brutes pour le détail portefeuille
		/// </summary>
		/// <param name="page">PAGE courante</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="webSession">session du client</param>
		/// <returns>code html pour sortie excel</returns>
		public static string GetRawExcelPortofolioUI(Page page, object[,] tab,WebSession webSession){

			#region Pas de données à afficher
			if(tab==null || tab.GetLength(0)==0 ){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}			
			#endregion
			
			#region variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);						
			int nbColTab=tab.GetLength(0);
			int nbline=tab.GetLength(1);
			int i,j,m;
			#endregion

			#region Nombre de niveaux
			int nbLevels=WebFunctions.ProductDetailLevel.GetLevelNumber(webSession);
			string levels = WebFunctions.ProductDetailLevel.LevelProductToExcelString(webSession);
			string[] levelsTab = levels.Split('/');
			#endregion

			#region Paramètres du tableau
			t.Append(ExcelFunction.GetExcelHeader(webSession,true,false,false,true,GestionWeb.GetWebWord(1597,webSession.SiteLanguage)));
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.PortofolioUIException("La sélection de médias est incorrecte"));
			#endregion

			#region HTML
			try{
				t.Append("<table border=1 cellpadding=0 cellspacing=0 >");

//				for(i=0;i<nbColTab;i++){
//					t.Append("<tr>");
//						for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){	
//						t.Append("<td>"+tab[i,j]+"</td>");
//					}
//					t.Append("</tr>");
//				}

				#region Première ligne
				t.Append("<tr>");
				for(int k=0;k<nbLevels;k++){
					t.Append("<td>"+levelsTab[k].ToString()+"</td>");
				}
				// Total
				t.Append("<td>"+GestionWeb.GetWebWord(1423,webSession.SiteLanguage)+"</td>");
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName) {
					t.Append("<td>"+GestionWeb.GetWebWord(1424,webSession.SiteLanguage)+"</td>");
					t.Append("<td>"+GestionWeb.GetWebWord(943,webSession.SiteLanguage)+"</td>");
					t.Append("<td>"+GestionWeb.GetWebWord(940,webSession.SiteLanguage)+"</td>");
				}
				else if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName) {
					t.Append("<td>"+GestionWeb.GetWebWord(1604,webSession.SiteLanguage)+"</td>");
				}
				else if(DBClassificationConstantes.Vehicles.names.radio==vehicleName || DBClassificationConstantes.Vehicles.names.tv==vehicleName || DBClassificationConstantes.Vehicles.names.others==vehicleName) {
					t.Append("<td>"+GestionWeb.GetWebWord(1435,webSession.SiteLanguage)+"</td>");
					t.Append("<td>"+GestionWeb.GetWebWord(939,webSession.SiteLanguage)+"</td>");
				}
				t.Append("</tr>");
				#endregion

				#region Total
				t.Append("<tr>");
				for(i=0;i<nbLevels;i++){
					t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
				}
				//Total euros
				if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.EURO_COLUMN_INDEX]!=null)
					//t.Append("<td>"+tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.EURO_COLUMN_INDEX]+"</td>");
					t.Append("<td>"+long.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.EURO_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
				else t.Append("<td>0</td>");
				
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
					//Total Mm/Col
					if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.MMC_COLUMN_INDEX]!=null)
						//t.Append("<td>"+tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.MMC_COLUMN_INDEX]+"</td>");
						t.Append("<td>"+long.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.MMC_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
					else t.Append("<td>0</td>");
					//Total Pages
					if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.PAGES_COLUMN_INDEX]!=null)
						//t.Append("<td>"+((double)tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.PAGES_COLUMN_INDEX]/(double)1000)+"</td>");
//						t.Append("<td>"+string.Format("{0:### ### ### ### ##0.###}",double.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.PAGES_COLUMN_INDEX].ToString())/(double)1000)+"</td>");
						t.Append("<td>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.PAGES_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.pages,false)+"</td>");
					else t.Append("<td>0</td>");
				}

				else if(DBClassificationConstantes.Vehicles.names.radio==vehicleName || DBClassificationConstantes.Vehicles.names.tv==vehicleName || DBClassificationConstantes.Vehicles.names.others==vehicleName){
					//Total Durée
					if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.DURATION_COLUMN_INDEX]!=null)
						t.Append("<td>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.DURATION_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.duration,false)+"</td>");
					else t.Append("<td>0</td>");
				}
				//Total Insertions/spots
				if(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX]!=null)
					//t.Append("<td>"+tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX]+"</td>");
					t.Append("<td>"+long.Parse(tab[ResultConstantes.TOTAL_LINE_INDEX,ResultConstantes.INSERTIONS_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
				else t.Append("<td>0</td>");
				t.Append("</tr>");
				#endregion

				#region Résultats
				string oldTextLevel1="";
				string oldTextLevel2="";
				string oldTextLevel3="";
				for(i=1;i<nbColTab;i++){
					t.Append("<tr>");
					for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){	
						switch(j){

							#region Niveau 1
							case ResultConstantes.LABELL1_INDEX:
								if(tab[i,j]!=null){
									t.Append("<td>"+tab[i,ResultConstantes.LABELL1_INDEX]+"</td>");
									oldTextLevel1 = tab[i,ResultConstantes.LABELL1_INDEX].ToString();
									oldTextLevel2=null;
								}
								else t.Append("<td>"+oldTextLevel1+"</td>");
								break;
							#endregion
								
							#region Niveau 2
							case ResultConstantes.LABELL2_INDEX:
								if(tab[i,j]!=null){
									t.Append("<td>"+tab[i,ResultConstantes.LABELL2_INDEX]+"</td>");
									oldTextLevel2 = tab[i,ResultConstantes.LABELL2_INDEX].ToString();
								}
								else{
									if(oldTextLevel2!=null){
										t.Append("<td>"+oldTextLevel2+"</td>");
									}
									else{ 
										// Libellé TOTAL
										if(nbLevels>1)t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
									}
								}
								break;
							#endregion

							#region Niveau 3
							case ResultConstantes.LABELL3_INDEX:
								if(tab[i,j]!=null){
									t.Append("<td>"+tab[i,ResultConstantes.LABELL3_INDEX]+"</td>");
									oldTextLevel3 = tab[i,ResultConstantes.LABELL3_INDEX].ToString();
								}
								else {
									// Libellé TOTAL
									if(nbLevels>2)t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
								}
								break;
							#endregion

							#region Colonnes résultats
							case ResultConstantes.EURO_COLUMN_INDEX:
								for(m=ResultConstantes.EURO_COLUMN_INDEX; m<nbline;m++){
									switch(m){
											// Toujours affiché
										case ResultConstantes.EURO_COLUMN_INDEX:
											//if(tab[i,m]!=null) t.Append("<td>"+tab[i,ResultConstantes.EURO_COLUMN_INDEX]+"</td>");
											if(tab[i,m]!=null) t.Append("<td>"+long.Parse(tab[i,ResultConstantes.EURO_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
											else t.Append("<td>0</td>");
											break;
										
											// Affiché en Presse uniquement
										case ResultConstantes.MMC_COLUMN_INDEX:
											if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
												//if(tab[i,m]!=null) t.Append("<td>"+tab[i,ResultConstantes.MMC_COLUMN_INDEX]+"</td>");
												if(tab[i,m]!=null) t.Append("<td>"+long.Parse(tab[i,ResultConstantes.MMC_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
												else t.Append("<td>0</td>");
											}
											break;
										case ResultConstantes.PAGES_COLUMN_INDEX:
											if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName){
												//if(tab[i,m]!=null) t.Append("<td>"+((double)tab[i,ResultConstantes.PAGES_COLUMN_INDEX]/(double)1000)+"</td>");
												
//												if(tab[i,m]!=null) t.Append("<td>"+string.Format("{0:### ### ### ### ##0.###}",double.Parse(tab[i,ResultConstantes.PAGES_COLUMN_INDEX].ToString())/(double)1000)+"</td>");
												if(tab[i,m]!=null) t.Append("<td>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.PAGES_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.pages,false)+"</td>");
												else t.Append("<td>0</td>");
											}
											break;
										
											// Affiché en Radio / TV uniquement
										case ResultConstantes.DURATION_COLUMN_INDEX:
											if(DBClassificationConstantes.Vehicles.names.radio==vehicleName || DBClassificationConstantes.Vehicles.names.tv==vehicleName || DBClassificationConstantes.Vehicles.names.others==vehicleName){
												//if(tab[i,m]!=null) t.Append("<td>"+tab[i,ResultConstantes.DURATION_COLUMN_INDEX]+"</td>");
												if(tab[i,m]!=null){
													//t.Append("<td>"+GetDurationFormatHH_MM_SS(int.Parse(tab[i,ResultConstantes.DURATION_COLUMN_INDEX].ToString())).ToString()+"</td>");
													t.Append("<td>"+WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantes.DURATION_COLUMN_INDEX].ToString(),WebConstantes.CustomerSessions.Unit.duration,false)+"</td>");
												}
												else t.Append("<td>0</td>");
											}
											break;

											// Toujours affiché
										case ResultConstantes.INSERTIONS_COLUMN_INDEX:
											//if(tab[i,m]!=null) t.Append("<td>"+tab[i,ResultConstantes.INSERTIONS_COLUMN_INDEX]+"</td>");
											if(tab[i,m]!=null) t.Append("<td>"+long.Parse(tab[i,ResultConstantes.INSERTIONS_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0")+"</td>");
											else t.Append("<td>0</td>");
											break;
									}
								}
								break;
							#endregion

						}
					}
					t.Append("</tr>");
				}
				#endregion

				t.Append("</table>");
			}
			catch(System.Exception err){
				throw(new WebExceptions.PortofolioUIException("Impossible de construire le tableau HTML: "+err.Message));
			}
			#endregion

			return t.ToString();	
		}
		#endregion

		#region Calendrier d'action
		/// <summary>
		/// Génère l'export Excel des données brutes pour le calendrier d'actions
		/// </summary>
		/// <param name="page">PAGE courante</param>
		/// <param name="tab">tableau de résultats</param>
		/// <param name="webSession">session du client</param>
		/// <returns>code html pour sortie excel</returns>
		public static string GetRawExcelCalendarUI(Page page, object[,] tab, WebSession webSession){

			#region Pas de données à afficher
			if(tab==null || tab.GetLength(0)==0 ){
				return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
					+"</div>");
			}			
			#endregion
			
			#region variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(20000);						
			int nbColTab=tab.GetLength(0);
			int nbline=tab.GetLength(1);
			int i,j,m;
			string totalUnit="";
			#endregion

			#region Nombre de niveaux
			int nbLevels=WebFunctions.ProductDetailLevel.GetLevelNumber(webSession);
			string levels = WebFunctions.ProductDetailLevel.LevelProductToExcelString(webSession);
			string[] levelsTab = levels.Split('/');
			#endregion

			#region Paramètres du tableau
			t.Append(ExcelFunction.GetExcelHeader(webSession,true,false,false,true,GestionWeb.GetWebWord(1597,webSession.SiteLanguage)));
			#endregion

			#region Sélection du vehicle
			string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
			DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
			if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.PortofolioUIException("La sélection de médias est incorrecte"));
			#endregion

			#region HTML
			try{
				t.Append("<table border=1 cellpadding=0 cellspacing=0 >");

//				for(i=0;i<nbColTab;i++){
//					t.Append("<tr>");
//						for(j=ResultConstantesCalendar.DATE_INDEX;j<nbline;j++){	
//						t.Append("<td>"+tab[i,j]+"</td>");
//					}
//					t.Append("</tr>");
//				}

				#region Première ligne
				t.Append("<tr>");
				for(int k=0;k<nbLevels;k++){
					t.Append("<td>"+levelsTab[k].ToString()+"</td>");
				}
				// Total
				t.Append("<td>"+GestionWeb.GetWebWord(1423,webSession.SiteLanguage)+"</td>");
				// Pdm
				t.Append("<td>"+GestionWeb.GetWebWord(1236,webSession.SiteLanguage)+"</td>");
				// Parution 
				for(i=ResultConstantesCalendar.TOTAL_INDEX;i<nbline;i++){
					t.Append("<td>"+TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY((int)tab[ResultConstantesCalendar.DATE_INDEX,i],webSession.SiteLanguage)+"</td>");
				}
				t.Append("</tr>");
				#endregion

				#region Total
				t.Append("<tr>");
				for(i=0;i<nbLevels;i++){
					t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
				}
				// Total
				totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantesCalendar.TOTAL_LINE,ResultConstantesCalendar.TOTAL_VALUE_COLUMN_INDEX].ToString(),webSession.Unit,webSession.Percentage);
				if(webSession.Percentage) totalUnit="100";
				t.Append("<td>"+totalUnit+"</td>");

				//PDM
				t.Append("<td>100</td>");
			
				// Parutions
				for(i=ResultConstantesCalendar.TOTAL_INDEX;i<nbline;i++){
					totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[ResultConstantesCalendar.TOTAL_LINE,i].ToString(),webSession.Unit,webSession.Percentage);
					t.Append("<td>"+totalUnit+"</td>");
				}
				t.Append("</tr>");
				#endregion

				#region Résultats
				string oldTextLevel1="";
				string oldTextLevel2="";
				string oldTextLevel3="";
				for(i=2;i<nbColTab;i++){
					t.Append("<tr>");
					for(j=ResultConstantesCalendar.DATE_INDEX;j<nbline;j++){
						switch(j){

							#region Niveau 1
							case ResultConstantesCalendar.LABELL1_INDEX:
								if(tab[i,j]!=null){
									t.Append("<td>"+tab[i,ResultConstantesCalendar.LABELL1_INDEX]+"</td>");
									oldTextLevel1 = tab[i,ResultConstantesCalendar.LABELL1_INDEX].ToString();
									oldTextLevel2=null;
								}
								else t.Append("<td>"+oldTextLevel1+"</td>");
								break;
							#endregion

							#region Niveau 2
							case ResultConstantesCalendar.LABELL2_INDEX:
								if(tab[i,j]!=null){
									t.Append("<td>"+tab[i,ResultConstantesCalendar.LABELL2_INDEX]+"</td>");
									oldTextLevel2 = tab[i,ResultConstantesCalendar.LABELL2_INDEX].ToString();
								}
								else{
									if(oldTextLevel2!=null){
										t.Append("<td>"+oldTextLevel2+"</td>");
									}
									else{ 
										// Libellé TOTAL
										if(nbLevels>1)t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
									}
								}
								break;
							#endregion

							#region Niveau 3
							case ResultConstantesCalendar.LABELL3_INDEX:
								if(tab[i,j]!=null){
									t.Append("<td>"+tab[i,ResultConstantesCalendar.LABELL3_INDEX]+"</td>");
									oldTextLevel3 = tab[i,ResultConstantesCalendar.LABELL3_INDEX].ToString();
								}
								else {
									// Libellé TOTAL
									if(nbLevels>2)t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
								}
								break;
							#endregion

							#region Total
							case ResultConstantesCalendar.TOTAL_VALUE_COLUMN_INDEX:
								if(tab[i,j]!=null){
									totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ResultConstantesCalendar.TOTAL_VALUE_COLUMN_INDEX].ToString(),webSession.Unit,webSession.Percentage);
									if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" || totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="";
									if(webSession.Percentage) totalUnit="100";
									t.Append("<td>"+totalUnit+"</td>");
									//t.Append("<td>"+tab[i,ResultConstantesCalendar.TOTAL_VALUE_COLUMN_INDEX]+"</td>");
								}
								break;
							#endregion

							#region Pourcentage
							case ResultConstantesCalendar.PDM_COLUMN_INDEX:
								if(tab[i,j]!=null){
									//t.Append("<td>"+tab[i,ResultConstantesCalendar.PDM_COLUMN_INDEX]+"</td>");
									t.Append("<td>"+double.Parse(tab[i,ResultConstantesCalendar.PDM_COLUMN_INDEX].ToString()).ToString("0.00")+"</td>");
								}
								break;
							#endregion

							#region Parutions
							case ResultConstantesCalendar.TOTAL_INDEX:
								for(m=ResultConstantesCalendar.TOTAL_INDEX;m<nbline;m++){
									if(tab[i,m]!=null ){
										totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,m].ToString(),webSession.Unit,webSession.Percentage);
										if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" || totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="";
										t.Append("<td>"+totalUnit+"</td>");
									}
									else t.Append("<td>0</td>");
								}
								break;
							#endregion

						}
					}
				}
				#endregion

				t.Append("</table>");
			}
			catch(System.Exception err){
				throw(new WebExceptions.CompetitorUIException("Impossible de construire le tableau HTML",err));
			}
			#endregion

			return t.ToString();
		}
		#endregion

		#endregion

		#region Syntèse pour l'analyse
		/// <summary>
		/// Synthèse portefeuille
		/// </summary>
		/// <param name="webSession">session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		///<param name="excel">booléen à true si excel</param>
		/// <returns>Code Html</returns>
		public static string SynthesisAnalysis(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,bool excel){
			
			#region Variables
			string investment="";
			string firstDate="";
			string lastDate="";
			string support="";	
			string periodicity="";
			string category="";
			string classStyleTitle="";	
			string classStyleValue="";	
			string regie="";
			string interestCenter="";
			string pageNumber="";
			string adNumber="";
			string ojd="";			
			string nbrSpot="";
            string volume = "";
			//string nbrEcran="";
			//decimal averageDurationEcran=0;
			//decimal nbrSpotByEcran=0;
			string totalDuration="";
			
			StringBuilder t=new StringBuilder(5000);		
	
			
			#endregion

			#region Formattage des dates 
			dateBegin = TNS.AdExpress.Web.Rules.Results.PortofolioRules.GetDateBegin(webSession);
			dateEnd = TNS.AdExpress.Web.Rules.Results.PortofolioRules.GetDateEnd(webSession);
			#endregion
			
			#region Période
			DateTime dtFirstDate=DateTime.Today;
			DateTime dtLastDate=DateTime.Today;
			if(dateBegin.Length>0){
				dtFirstDate=WebFunctions.Dates.getPeriodBeginningDate(dateBegin, webSession.PeriodType);
				dtLastDate=WebFunctions.Dates.getPeriodEndDate(dateEnd, webSession.PeriodType);

			}
			#endregion

			#region Accès aux tables
			//DataSet dsInvestment=PortofolioAnalysisDataAccess.GetDataForAnalysisPortofolio(webSession,idVehicle,idMedia,dateBegin,dateEnd);
            DataSet dsInvestment = PortofolioAnalysisDataAccess.GetGenericDataForAnalysisPortofolio(webSession, idVehicle, idMedia);
			DataTable dtInvestment=dsInvestment.Tables[0];

			DataSet dsCategory=PortofolioDataAccess.GetCategoryMediaSellerData(webSession,idVehicle,idMedia);
			DataTable dtCategory=dsCategory.Tables[0];

			DataSet dsPage = null;
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())!= DBClassificationConstantes.Vehicles.names.internet)
				dsPage = PortofolioDataAccess.GetPage(webSession,idVehicle,idMedia,dtFirstDate.ToString("yyyyMMdd"),dtLastDate.ToString("yyyyMMdd"));
			
			DataTable dtPage = null;
			if(dsPage != null)
			 dtPage=dsPage.Tables[0];	

			//object [] tab=PortofolioAnalysisDataAccess.NumberProductAdvertiser(webSession,idVehicle,idMedia,dateBegin,dateEnd);
         object[] tab = PortofolioAnalysisDataAccess.NumberProductAdvertiser(webSession, idVehicle, idMedia);
			object [] tabEncart=null;
			

			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
				){			
				tabEncart=PortofolioDataAccess.NumberPageEncart(webSession,idVehicle,idMedia,dateBegin,dateEnd);
			}
			#endregion

			#region Parcours des tableaux
			foreach(DataRow row in dtInvestment.Rows){
				investment=row["investment"].ToString();
				if(dtInvestment.Columns.Contains("page") && row["page"].ToString().Length>0)
				adNumber=WebFunctions.Units.ConvertUnitValueAndPdmToString(row["page"].ToString(),WebConstantes.CustomerSessions.Unit.pages,false);
				if(dtInvestment.Columns.Contains("duree"))totalDuration=row["duree"].ToString();
				if(dtInvestment.Columns.Contains("insertion"))nbrSpot=row["insertion"].ToString();
                if (dtInvestment.Columns.Contains("volume")) {
                    if (row["volume"].ToString().Length > 0){
                        volume = Convert.ToString(Math.Round(decimal.Parse(row["volume"].ToString())));
                        volume = WebFunctions.Units.ConvertUnitValueAndPdmToString(volume, WebConstantes.CustomerSessions.Unit.volume, false);
                    }
                    else
                        volume = "0";
                }
				//firstDate=row["firstDate"].ToString(); 
				//lastDate=row["lastDate"].ToString(); 
			}

			foreach(DataRow row in dtCategory.Rows){
				support=row["support"].ToString();
				category=row["category"].ToString();
				regie=row["media_seller"].ToString();
				interestCenter=row["interest_center"].ToString();
				if(dtCategory.Columns.Contains("periodicity"))
					periodicity=row["periodicity"].ToString();
				if(dtCategory.Columns.Contains("ojd"))
					ojd=row["ojd"].ToString();
			}

			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
				){
				foreach(DataRow row in dtPage.Rows){
					pageNumber=row["page"].ToString();				
			
				}
			}
			#endregion

			#region HTML
			t.Append("<table  border=0 cellpadding=0 cellspacing=0 width=600 >");			
			
			//Titre du support
			t.Append("\r\n\t<tr height=\"30px\" ><td colspan=2 class=\"p2\" align=\"center\" style=\"BORDER-RIGHT: #644883 1px solid; BORDER-TOP: #644883 1px solid; BORDER-LEFT: #644883 1px solid; BORDER-BOTTOM: #644883 1px solid;font-size: 16px\">"+support+"</td></tr>");	
			// Date de parution ou diffusion
			if(dateBegin.Length>0){
				if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
					|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
					){
					//Cas Presse : Période sélectionnée 
					t.Append("\r\n\t<tr  ><td class=\""+portofolioTitle1+"\" width=50%>"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+"</td>");
				}
				else{
					// Cas TV-Radio : Période sélectionnée
					t.Append("\r\n\t<tr ><td class=\""+portofolioTitle1+"\" width=50%>"+GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+"</td>");
				}
				if(dateBegin==dateEnd){
					t.Append("<td class=\""+portofolioValue1+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+dtFirstDate.Date.ToString("dd/MM/yyyy")+"</td></tr>");
				}
				else{
					t.Append("<td class=\""+portofolioValue1+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;Du "+dtFirstDate.Date.ToString("dd/MM/yyyy")+" au "+dtLastDate.Date.ToString("dd/MM/yyyy")+"</td></tr>");
				}
			}

			classStyleTitle=portofolioTitle2;	
			classStyleValue=portofolioValue2;
			// Périodicité
			if(dtCategory.Columns.Contains("periodicity")){
				t.Append("<tr><td class=\""+portofolioTitle2+"\" width=50%>"+GestionWeb.GetWebWord(1450,webSession.SiteLanguage)+"</td><td class=\""+portofolioValue2+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+periodicity+"</td></tr>");
				classStyleTitle=portofolioTitle1;	
				classStyleValue=portofolioValue1;	
			}
			// Categorie
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1416,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+category+"</td></tr>");
			
			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);	
			// Régie
            if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) != DBClassificationConstantes.Vehicles.names.directMarketing) {
                t.Append("<tr><td class=\"" + classStyleTitle + "\" width=50%>" + GestionWeb.GetWebWord(1417, webSession.SiteLanguage) + "</td><td class=\"" + classStyleValue + "\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;" + regie + "</td></tr>");
            }

            classStyleTitle = InversClassStyleTitle(classStyleTitle);
            classStyleValue = InversClassStyleValue(classStyleValue);
            
            // Cas du Marketing Direct
            if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.directMarketing &&
                webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT) != null) {
                // Volume
                classStyleTitle = InversClassStyleTitle(classStyleTitle);
                classStyleValue = InversClassStyleValue(classStyleValue);
                t.Append("<tr><td class=\"" + classStyleTitle + "\" width=50%>" + GestionWeb.GetWebWord(2216, webSession.SiteLanguage) + "</td><td class=\"" + classStyleValue + "\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;" + volume + "</td></tr>");
            }

            // Centre d'intérêt
            if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) != DBClassificationConstantes.Vehicles.names.directMarketing) {
                t.Append("<tr><td class=\"" + classStyleTitle + "\" width=50%>" + GestionWeb.GetWebWord(1411, webSession.SiteLanguage) + "</td><td class=\"" + classStyleValue + "\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;" + interestCenter + "</td></tr>");
            }

			// Cas de la presse
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.press
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.internationalPress
				){
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);	
				// Nombre de page
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+pageNumber+"</td></tr>");
				
					classStyleTitle=InversClassStyleTitle(classStyleTitle);
					classStyleValue=InversClassStyleValue(classStyleValue);
					// Nombre de page pub
				if(adNumber.Length==0){
					adNumber="0";
				}
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1386,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+adNumber+"</td></tr>");
	
					classStyleTitle=InversClassStyleTitle(classStyleTitle);
					classStyleValue=InversClassStyleValue(classStyleValue);
				if(pageNumber.Length>0){
					// Ratio
					t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1387,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+((decimal)(decimal.Parse(adNumber)/decimal.Parse(pageNumber)*100)).ToString("0.###")+" %</td></tr>");	
				}
			}

			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.outdoor)
			{
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);					
				//Nombre de Panneaux
				if(nbrSpot.Length==0)
				{
					nbrSpot="0";
				}
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1604,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+nbrSpot+"</td></tr>");
			}
			// Cas tv, presse
			if((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.radio
				|| 	(DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.tv
				|| (DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString())== DBClassificationConstantes.Vehicles.names.others
				
				)
			{
				
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);					
				//Nombre de spot
				if(nbrSpot.Length==0)
				{
					nbrSpot="0";
				}
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1404,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+nbrSpot+"</td></tr>");
			
				classStyleTitle=InversClassStyleTitle(classStyleTitle);
				classStyleValue=InversClassStyleValue(classStyleValue);

				if(totalDuration.Length>0) 
				{
					totalDuration=WebFunctions.Units.ConvertUnitValueAndPdmToString(totalDuration,WebConstantes.CustomerSessions.Unit.duration,false);
				}
				else
				{
					totalDuration="0";
				}
				// total durée
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1413,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+totalDuration+"</td></tr>");
			}


			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);
			// Total investissements
			if(investment.Length>0){
				t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1390,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;"+WebFunctions.Units.ConvertUnitValueAndPdmToString(investment,WebConstantes.CustomerSessions.Unit.euro,false)+"</td></tr>");	
			}
            else
                t.Append("<tr><td class=\"" + classStyleTitle + "\" width=50%>" + GestionWeb.GetWebWord(1390, webSession.SiteLanguage) + "</td><td class=\"" + classStyleValue + "\" width=50%>&nbsp;&nbsp;&nbsp; 0</td></tr>");
			
			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);
			//Nombre de produits
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1393,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+(int)tab[0]+"</td></tr>");
	
			classStyleTitle=InversClassStyleTitle(classStyleTitle);
			classStyleValue=InversClassStyleValue(classStyleValue);
			//Nombre d'annonceurs
			t.Append("<tr><td class=\""+classStyleTitle+"\" width=50%>"+GestionWeb.GetWebWord(1396,webSession.SiteLanguage)+"</td><td class=\""+classStyleValue+"\" width=50%>&nbsp;&nbsp;&nbsp;&nbsp;"+(int)tab[1]+"</td></tr>");	

			
			t.Append("</table>");
				
			
			#endregion

			return t.ToString();
		
		}
		#endregion

	}
}
