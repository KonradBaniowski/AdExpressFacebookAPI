///////////////////////////////////////////////////////////
//  BastetExcel.cs
//  Implementation of the Class Excel
//  Generated by Enterprise Architect
//  Created on:      17-nov.-2005 16:51:12
//  Original author: D.V. Mussuma
///////////////////////////////////////////////////////////


using System;
using System.IO;
using Aspose.Excel;
using System.Drawing;
using System.Data;
using TNS.AdExpress.Anubis.Bastet;
using BastetCommon=TNS.AdExpress.Bastet.Common;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes.DB;
using BastetExceptions=TNS.AdExpress.Anubis.Bastet.Exceptions;


namespace TNS.AdExpress.Anubis.Bastet.UI {
	/// <summary>
	/// Composant Excel
	/// </summary>
	public  class BastetExcel {

		/// <summary>
		/// Composant excel
		/// </summary>
		protected Excel _excel;
		/// <summary>
		/// Licence Aspose Excel
		/// </summary>
		License _license;
				
		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public BastetExcel(){
			_excel = new Excel();
			_license = new License();
			_license.SetLicense("Aspose.Excel.lic");
			//Ajout de couleur			
			AddColor(Color.FromArgb(128,128,192));
		}
		#endregion
		
		#region Destructeur
		/// <summary>
		/// Destructeur
		/// </summary>
		~BastetExcel(){
			_excel=null;
			_license=null;
		}
		#endregion

		public virtual void Dispose(){

		}

	
		#region Dur�e moyenne des connections
		/// <summary>
		/// Dur�e moyenne des connection par client et par modules et gourpes de module
		/// </summary>
		/// <param name="parameters">param�tres des statistiques</param>
		protected void ConnexionDurationAverage(BastetCommon.Parameters parameters){
			UI.Duration.ConnexionAverage(_excel,parameters);
		}
		#endregion

		#region Styles du fichier excel
//		/// <summary>
//		/// Styles excel
//		/// </summary>
//		/// <param name="excel"></param>
//		private void AddStyles(Excel excel) {
//			Aspose.Excel.Style style = null;
//			for (int i = 0;i < 28;i++) {
//				int index = excel.Styles.Add();
//				style = excel.Styles[index];
//				style.Name = "Custom_Style" + ((int)(i + 1)).ToString();
//				style.ForegroundColor = Color.White;
//				style.HorizontalAlignment = TextAlignmentType.Center;
//				style.VerticalAlignment = TextAlignmentType.Bottom;
//				style.Font.Name = "Arial";
//				style.Font.Size = 10;
//			}
//			//Custom_Style1
//			style = excel.Styles["Custom_Style1"];
//			style.Font.IsBold = true;
//			style.Number = 37;	//#,##0;-#,##0
//			style.HorizontalAlignment = TextAlignmentType.Right;
//
//			//Custom_Style2
//			style = excel.Styles["Custom_Style2"];
//			style.Font.IsBold = true;
//			style.Custom = "\"$\"#,##0.00";
//
//			//Custom_Style3
//			style = excel.Styles["Custom_Style3"];
//			style.Font.IsItalic = true;
//			style.Custom = "\"$\"#,##0";
//			style.ForegroundColor = Color.Silver;
//
//			//Custom_Style4
//			style = excel.Styles["Custom_Style4"];
//			style.Font.IsItalic = true;
//			style.Font.Underline = FontUnderlineType.Single;
//			style.ForegroundColor = Color.Silver;
//
//			//Custom_Style5
//			style = excel.Styles["Custom_Style5"];
//			style.Font.IsBold = true;
//			style.Number = 10;	//0.00%
//
//			//Custom_Style6
//			style = excel.Styles["Custom_Style6"];
//			style.Font.IsBold = true;
//			style.Number = 9;	//0%
//			style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
//
//			//Custom_Style7
//			style = excel.Styles["Custom_Style7"];
//			style.Font.IsBold = true;
//			style.Number = 38;	//#,##0;[Red]-#,##0
//			style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
//
//			//Custom_Style8
//			style = excel.Styles["Custom_Style8"];
//			style.Font.IsBold = true;
//			style.VerticalAlignment = TextAlignmentType.Top;
//			style.Custom = "\"$\"#,##0_);[Red](\"$\"#,##0)";
//			style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
//			style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
//
//			//Custom_Style9
//			style = excel.Styles["Custom_Style9"];
//			style.Number = 10;	//0.00%
//
//			//Custom_Style10
//			style = excel.Styles["Custom_Style10"];
//			style.Font.IsItalic = true;
//			style.Custom = "\"$\"#,##0_);[Red](\"$\"#,##0)";
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//
//			//Custom_Style11
//			style = excel.Styles["Custom_Style11"];
//			style.Font.IsItalic = true;
//			style.Number = 38;	//#,##0;[Red]-#,##0
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//
//			//Custom_Style12
//			style = excel.Styles["Custom_Style12"];
//			style.Custom = "\"$\"#,##0_);[Red](\"$\"#,##0)";
//			style.HorizontalAlignment = TextAlignmentType.Right;
//
//			//Custom_Style13
//			style = excel.Styles["Custom_Style13"];
//			style.Number = 38;	//#,##0;[Red]-#,##0
//			style.HorizontalAlignment = TextAlignmentType.Right;
//
//			//Custom_Style14
//			style = excel.Styles["Custom_Style14"];
//			style.Font.IsItalic = true;
//			style.Number = 9;	//0%
//			style.ForegroundColor = Color.Silver;
//
//			//Custom_Style15
//			style = excel.Styles["Custom_Style15"];
//			style.Number = 38;	//#,##0;[Red]-#,##0
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//
//			//Custom_Style16
//			style = excel.Styles["Custom_Style16"];
//			style.Font.IsBold = true;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//			style.Number = 37;	//#,##0;-#,##0
//
//			//Custom_Style17
//			style = excel.Styles["Custom_Style17"];
//			style.Custom = "\"$\"#,##0_);[Red](\"$\"#,##0)";
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//
//			//Custom_Style18
//			style = excel.Styles["Custom_Style18"];
//			style.ForegroundColor = Color.Black;
//
//			//Custom_Style19
//			style = excel.Styles["Custom_Style19"];
//			style.Custom = "0.0%";
//
//			//Custom_Style20
//			style = excel.Styles["Custom_Style20"];
//			style.Number = 10;	//0.00%
//			style.ForegroundColor = Color.Silver;
//
//			//Custom_Style21
//			style = excel.Styles["Custom_Style21"];
//			style.Custom = "\"$\"#,##0_);[Red](\"$\"#,##0)";
//			style.ForegroundColor = Color.Silver;
//
//			//Custom_Style22
//			style = excel.Styles["Custom_Style22"];
//			style.Number = 38;	//#,##0;[Red]-#,##0
//
//			//Custom_Style23
//			style = excel.Styles["Custom_Style23"];
//			style.Custom = "\"$\"#,##0.00_);[Red](\"$\"#,##0.00)";
//			style.ForegroundColor = Color.Silver;
//
//			//Custom_Style24
//			style = excel.Styles["Custom_Style24"];
//			style.Custom = "\"$\"#,##0_);[Red](\"$\"#,##0)";
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//			style.IndentLevel = 2;
//
//			//Custom_Style25
//			style = excel.Styles["Custom_Style25"];
//			style.Custom = "\"$\"#,##0_);[Red](\"$\"#,##0)";
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//			style.IndentLevel = 1;
//
//			//Custom_Style26
//			style = excel.Styles["Custom_Style26"];
//			style.Number = 38;	//#,##0;[Red]-#,##0
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//			style.IndentLevel = 1;
//
//			//Custom_Style27
//			style = excel.Styles["Custom_Style27"];
//			style.Number = 38;	//#,##0;[Red]-#,##0
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//			style.IndentLevel = 3;
//
//			//Custom_Style28
//			style = excel.Styles["Custom_Style28"];
//			style.Number = 38;	//#,##0;[Red]-#,##0
//			style.ForegroundColor = Color.Silver;
//			style.HorizontalAlignment = TextAlignmentType.Right;
//			style.IndentLevel = 1;
//			
//		}
		#endregion

		#region Rappel des s�lection
		/// <summary>
		/// Rappel des s�lection effectu�es par le demandeur des statistiques d'adexpress
		/// </summary>
		/// <param name="parameters">param�tres des statistiques</param>
		/// <param name="idLogins">identifiant du demandeur</param>
		protected void CustomerSelelection(BastetCommon.Parameters parameters,string idLogins){
			try{
				Worksheet sheet = this._excel.Worksheets[0];
				sheet.Name=" Rappel des s�lections";
				string[] loginsArray =null;
				Cells cells = sheet.Cells;
				if(parameters.Logins!=null && parameters.Logins.Length>0)
					loginsArray = parameters.Logins.Split(',');
				//Saut de page horizontal
				int nbPages=0;
				int nbRows=0;
				const int nbMaxRowByPage=40;			
				if(parameters.EmailsRecipient!=null && parameters.EmailsRecipient.Count>0)
					nbRows+=parameters.EmailsRecipient.Count;
				if(loginsArray!=null && loginsArray.Length>0)
					nbRows+=loginsArray.Length;
				nbPages=(int)Math.Ceiling(nbRows*1.0/nbMaxRowByPage);
				for(int s=1;s<=nbPages+1;s++){
					sheet.HPageBreaks.Add(nbMaxRowByPage*s,0,8);
				}	
				int cellRow =5;

				//Chargement des donn�es
				DataTable dt = DataAccess.Client.Name(parameters,idLogins);
			
				//Ajout du logo TNS
				Pictures pics = sheet.Pictures;
				string tnsLogoPath=@"Images\logoTNSMedia.gif";	
				string logoPath = System.IO.Path.GetFullPath(tnsLogoPath);
				pics.Add(0, 0,logoPath);
				string bastetLogoPath=@"Images\Bastet.gif";
				string bastetImagePath = System.IO.Path.GetFullPath(bastetLogoPath);
				pics.Add(0, 5,bastetImagePath);

				sheet.IsGridlinesVisible = false;
				sheet.PageSetup.Orientation = PageOrientationType.Landscape;
				Aspose.Excel.PageSetup pageSetup =sheet.PageSetup;
			
				//Set margins, in unit of inches 					
				pageSetup.TopMarginInch = 0.3; 
				pageSetup.BottomMarginInch = 0.3; 
				pageSetup.HeaderMarginInch = 0.3; 
				pageSetup.FooterMarginInch = 0.3; 

				//Set current date and current time at the center section of header and change the font of the header
				pageSetup.SetFooter(2, "&\"Times New Roman,Bold\"&D-&T");		
					
				//Set current page number at the center section of footer
				pageSetup.SetFooter(1, "&A"+" - Page "+"&P"+" sur "+"&N");

				//intitul� du fichier excel			
				cells["D"+cellRow].PutValue("Statistiques AdExpress ");
				cells["D"+cellRow].Style.Font.Size =15;
				cells["D"+cellRow].Style.Font.Color = Color.DarkViolet;
				cells["D"+cellRow].Style.Font.IsBold = true;
				cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;					
				cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cellRow++;

				//Date de cr�ation
				cells["D"+cellRow].PutValue(" Cr�� le "+DateTime.Now.ToString("ddd dd MMM yyyy"));
				cells["D"+cellRow].Style.Font.Size =10;
				cells["D"+cellRow].Style.Font.Color = Color.Black;
				cells["D"+cellRow].Style.Font.IsBold = true;
				cells["D"+cellRow].Style.HorizontalAlignment = TextAlignmentType.Center;
				cellRow++;			

			
				//E10 to J10
				cellRow=cellRow+2;
				cells["D"+cellRow].PutValue(GestionWeb.GetWebWord(1752,Language.FRENCH.GetHashCode()));
				cells["D"+cellRow].Style.Font.IsBold = true;
				cells["D"+cellRow].Style.Font.Color = Color.White;
				cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;			
				cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.ForegroundColor = Color.FromArgb(128,128,192);
				cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells["E"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;					
				cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cells["E"+cellRow].Style.ForegroundColor =  Color.FromArgb(128,128,192);
				cellRow++;		

				//Exp�diteur						
				cells["D"+cellRow].PutValue(" Exp�diteur  ");// A mettre dans web word
				cells["D"+cellRow].Style.Font.IsBold = true;
				cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;	
				cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
				cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;				
				cells["E"+cellRow].PutValue(dt.Rows[0]["FIRST_NAME"].ToString()+"  "+dt.Rows[0]["NAME"].ToString());
				cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cellRow++;		

				//Emails destinataires
				for(int i=0; i<parameters.EmailsRecipient.Count;i++){
					if(i==0)cells["D"+cellRow].PutValue(" Destinataire(s) ");// A mettre dans web word
					cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
					cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
					cells["D"+cellRow].Style.Font.IsBold = true;
					cells["E"+cellRow].PutValue(parameters.EmailsRecipient[i].ToString());
					cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;			
					cellRow++;
				}
			
				

				//P�riode �tudi�e
				cells["D"+cellRow].PutValue(" P�riode  ");// A mettre dans web word
				cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Font.IsBold = true;
			
				string period ="";
				if(!parameters.PeriodBeginningDate.Equals(parameters.PeriodEndDate))
					period = " Du "+parameters.PeriodBeginningDate.Substring(6,2)+"/"+parameters.PeriodBeginningDate.Substring(4,2)+"/"+parameters.PeriodBeginningDate.Substring(0,4)
						+" au "+parameters.PeriodEndDate.Substring(6,2)+"/"+parameters.PeriodEndDate.Substring(4,2)+"/"+parameters.PeriodEndDate.Substring(0,4); 
				else period = parameters.PeriodBeginningDate.Substring(6,2)+"/"+parameters.PeriodBeginningDate.Substring(4,2)+"/"+parameters.PeriodBeginningDate.Substring(0,4);
											 
				cells["E"+cellRow].PutValue(period);
				cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				cells["E"+cellRow].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cellRow++;	

				//Logins clients
				cells["D"+cellRow].PutValue(" Logins  ");// A mettre dans web word
				cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
				cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;	
				cells["D"+cellRow].Style.Font.IsBold = true;

				cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
			
			
				if(parameters.Logins!=null && parameters.Logins.Length>0){				
					for(int i=0; i<loginsArray.Length;i++){
						dt = DataAccess.Client.Name(parameters,loginsArray[i].ToString());
						if(dt!=null && dt.Rows.Count>0){
							cells["E"+cellRow].PutValue(dt.Rows[0]["login"].ToString());
							cells["E"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
							cells["D"+cellRow].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;

							cellRow++;
						}
					}
					cellRow--;
					cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				}else{
					cells["E"+cellRow].PutValue(" Tous ");
				
					cells["D"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
					cells["E"+cellRow].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
				}
			
		
				sheet.AutoFitRow(4);		
				sheet.AutoFitColumn(3);
				sheet.AutoFitColumn(4);
				sheet.AutoFitColumn(7);
			}catch(Exception err){
				throw (new  BastetExceptions.BastetExcelException(" CustomerSelelection : Impossible d'obtenir le rappel des s�lections. ", err));
			}
		}
		#endregion

		#region Top connection clients
		/// <summary>
		/// Top des clients qui se connectent le plus
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopConnectedCustomer(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Client.TopConnected(_excel,parameters);

		}
		#endregion

		#region Top connexion par type de clients
		/// <summary>
		/// Top connexion par type de clients
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopTypeConnectedCustomer(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Client.TopTypeConnected(_excel,parameters);

		}
		#endregion

		#region Top connexion client par mois
		/// <summary>
		/// Top connexion client par mois
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopConnectedCustomerByMonth(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Client.TopConnectedByMonth(_excel,parameters);

		}
		#endregion

		#region Top connection par type de client et par mois
		/// <summary>
		/// Top connection paf type de client et par mois
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopTypeConnectedCustomerByMonth(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Client.TopTypeConnectedByMonth(_excel,parameters);

		}
		#endregion

		#region Top connexion client par jour nomm�
		/// <summary>
		/// Top connexion client par par jour nomm�
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopConnectedCustomerByDay(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Client.TopConnectedByDay(_excel,parameters);

		}
		#endregion

		#region Top connection par type de client et par jour nomm�
		/// <summary>
		/// Top connection paf type de client et par jour nomm�
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopTypeConnectedCustomerByDay(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Client.TopTypeConnectedByDay(_excel,parameters);

		}
		#endregion
		
		#region IP Adresse par client
			/// <summary>
			/// IP Adresse par client
			/// </summary>
			///<param name="excelpath">chemin fichier excel</param>
			///<param name="parameters">param�tres des statistiques</param>
			protected void IPAdresseByClient(string excelpath,BastetCommon.Parameters parameters){

				_excel=UI.Client.IPAddress(_excel,parameters);

			}
		#endregion

		#region Top connexion soci�t�s
		/// <summary>
		/// Top des soci�t�s qui se connectent le plus
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopConnectedCompany(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Company.TopConnected(_excel,parameters);

		}
		#endregion

		#region Top connexion soci�t�s par mois
		/// <summary>
		/// Top des soci�t�s qui se connectent le plus par mois
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopConnectedCompanyByMonth(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Company.TopConnectedByMonth(_excel,parameters);

		}
		#endregion				

		#region Top connection soci�t�s par jour nomm�
		/// <summary>
		/// Top des soci�t�s qui se connectent le plus par jour nomm�
		/// </summary>
		///<param name="excelpath">chemin fichier excel</param>
		///<param name="parameters">param�tres des statistiques</param>
		protected void TopConnectedCompanyByDay(string excelpath,BastetCommon.Parameters parameters){

			_excel=UI.Company.TopConnectedByDay(_excel,parameters);

		}
		#endregion				
				
		#region Top utilisation du fichier AGM et GAD
		/// <summary>
		/// Top utilisation du fichier AGM et GAD
		/// </summary>
		protected void TopFileUsing(BastetCommon.Parameters parameters){
			UI.File.TopUsers(_excel,parameters);
		}
		#endregion

		#region Top des modules et groupes de modules utilis�s
		/// <summary>
		///Top des modules et groupes de modules utilis�s 
		/// </summary>
		/// <param name="parameters">param�tres des statistiques</param>
		protected void TopUsedModules(BastetCommon.Parameters parameters){
			UI.Module.TopUsed(_excel,parameters);
		}
		#endregion

		#region Top export de fichiers
		/// <summary>
		/// Top export de fichiers
		/// </summary>
		///<param name="parameters">param�tres</param>
		protected void TopExport(BastetCommon.Parameters parameters){
			UI.Export.Top(_excel,parameters);
		}	
		#endregion

		#region Top otpions utilis�es
		/// <summary>
		/// Top otpions utilis�es
		/// </summary>
		/// <param name="parameters">param�tres</param>
		protected void TopUsedTab(BastetCommon.Parameters parameters){
			UI.Tab.TopUsed(_excel,parameters);
		}	
		#endregion

		#region Top clients utilisant le plus les requ�tes sauvegard�es
		/// <summary>
		/// Top clients utilisant le plus les requ�tes sauvegard�es
		/// </summary>
		/// <param name="parameters">param�tres</param>
		protected void TopUsingSavedSession(BastetCommon.Parameters parameters){
			UI.Request.TopClient(_excel,parameters);
		}
		#endregion
		
		#region Top unit�s utilis�es
		/// <summary>
		/// Top unit�s utilis�es
		/// </summary>
		/// <param name="parameters">param�tres</param>
		protected void TopUsedUnits(BastetCommon.Parameters parameters){
			UI.Unit.TopUsed(_excel,parameters);
		}
		#endregion

		#region Top P�riodes utilis�es
		/// <summary>
		/// Top p�riodes utilis�es
		/// </summary>
		///  <param name="parameters">param�tres</param>
		protected void  TopUsedPeriod(BastetCommon.Parameters parameters){
			UI.Period.TopUsed(_excel,parameters);
		}
		#endregion

		#region Top m�dias utilis�s
		/// <summary>
		/// Top m�dias utilis�s
		/// </summary>
		/// <param name="parameters">param�tres</param>
		protected void TopVehicle(BastetCommon.Parameters parameters){
			UI.Vehicle.TopUsed(_excel,parameters);
		}
		#endregion

		#region Top m�dias utilis�s par module
		/// <summary>
		/// Top m�dias utilis�s par module
		/// </summary>
		/// <param name="parameters">param�tres</param>
		protected void TopVehicleByModule(BastetCommon.Parameters parameters){
			UI.Vehicle.TopByModule(_excel,parameters);
		}
		#endregion
		
		#region Sauvegarde du fichier
		/// <summary>
		/// Sauvegarde du fichier excel
		/// </summary>
		/// <param name="excelpath"></param>
		protected void Save(string excelpath){
			_excel.Save(excelpath);
		}
		#endregion

		#region M�thodes internes
		/// <summary>
		/// Code couleur
		/// </summary>
		private int    m_indexCouleur=55;
  
		/// <summary>
		/// Cr�ation �ventuel de la couleur
		/// </summary>
		/// <param name="couleur">Couleur existante ? si non cr�� la</param>
		private void AddColor(Color couleur) {
			// Cr�ation de la couleur
			if(!_excel.IsColorInPalette(couleur)) {
				if (m_indexCouleur>=0) 
					_excel.ChangePalette(couleur, m_indexCouleur--);
			}
		}


		#endregion

	}//end Excel

}//end namespace UI