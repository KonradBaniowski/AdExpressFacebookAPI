using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using ClassificationDA=TNS.AdExpress.Classification.DataAccess;
using ClassificationTable=TNS.AdExpress.Constantes.Classification.DB.Table;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using AjaxPro;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Description résumée de CountItemsInClassificationWebControl.
	/// </summary>
	///  <author>G. Facon</author>
	[DefaultProperty("Text"), 
	ToolboxData("<{0}:CountItemsInClassificationWebControl runat=server></{0}:CountItemsInClassificationWebControl>"),
	ParseChildren(false),
	PersistChildren(false),
	Designer(typeof(CountItemsInClassificationWebControlDesigner)),
	ControlBuilder(typeof(CountItemsInClassificationWebControlBuilder))
	]
	public class CountItemsInClassificationWebControl : Control,IAttributeAccessor{
		
		#region Structure


		///<summary>
		/// Résultat d'un niveau
		/// </summary>
		///  <author>G. Facon</author>
		///  <since>22/06/2006</since>
		///  <stereotype>struct</stereotype>
		private struct ResultItem {
			/// <summary>
			/// Libellé du résultat
			/// </summary>
			public string Label;
			/// <summary>
			/// Nombre de résultat pour cet élément
			/// </summary>
			public string NbItem;
			/// <summary>
			/// Constructeur
			/// </summary>
			/// <param name="label">Libellé du résultat</param>
			/// <param name="nbItem">Nombre d'éléments pour le résultat</param>
			public ResultItem(string label,string nbItem){
				Label=label;
				NbItem=nbItem;
			}
		}
		#endregion

		#region Variables

		///<supplierCardinality>1</supplierCardinality>
		///  <directed>True</directed>
		private ClassificationDescriptionCollection _levels;
		/// <summary>
		/// The internal 
		/// <see cref="System.Collections.Specialized.StringDictionary">StringDictionary</see>
		/// object representing custom attributes.
		/// </summary>
		protected StringDictionary _customAttributes;
		/// <summary>
		/// Objet session
		/// </summary>
		protected WebSession _webSession=null;
		/// <summary>
		/// Mot à rechercher
		/// </summary>
		protected string _wordToSearch="";
		/// <summary>
		/// Classe CSS des textes
		/// </summary>
		protected string _textCss="";
		/// <summary>
		/// Résultats
		/// </summary>
		protected System.Collections.ArrayList _result;
		#endregion

		#region Constructeur
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit le mot recherché
		/// </summary>
		[Bindable(true), 
		Category("Appearance"),
		Description("Classe Css des Textes")
		]
		public string TextCss{
			get{return _textCss;}
			set{_textCss=value;}
		}

		/// <summary>
		/// Obtient ou définit le mot recherché
		/// </summary>
		public string WordToSearch{
			get{return _wordToSearch;}
			set{_wordToSearch=value;}
		}

		/// <summary>
		/// Obtient ou définit la session du client 
		/// </summary>
		public WebSession CustomerWebSession{
			get{return _webSession;}
			set{_webSession=value;}
		}

		/// <summary>
		/// Définit les attributs
		/// </summary>
		[
		Browsable(false)
		, Description("Collection of custom attributes")
		, NotifyParentProperty(true)
		//, PersistenceMode(PersistenceMode.Attribute)
		, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		, Editor(typeof(System.ComponentModel.Design.CollectionEditor), typeof(System.Drawing.Design.UITypeEditor))
		]
		public StringDictionary Attributes {
			get {
				if (_customAttributes == null)
					_customAttributes
						= new StringDictionary();

				return _customAttributes;
			}
		}
	
		/// <summary>
		/// Définit ou définit les niveaux de recherche
		/// </summary>
		[Bindable(true), 
		Category("Appearance"),
		Browsable(true),
		PersistenceMode(PersistenceMode.InnerDefaultProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		] 
		public ClassificationDescriptionCollection Levels{
			get{
				if(_levels==null)_levels=new ClassificationDescriptionCollection();
				return _levels;
			}

			set{
				_levels = value;
			}
		}
		#endregion

		#region JavaScript
		private string GetCountItemsScript(){
			StringBuilder js=new StringBuilder(2000);
			js.Append("\r\n<SCRIPT language=javascript>\r\n<!--");
			js.Append("\r\nAjaxPro.timeoutPeriod=240*1000;"); 
			foreach(ClassificationDescription currentDescription in Levels){	
				js.Append("\r\nfunction count_"+currentDescription.LevelType.ToString()+"(){");
				js.Append("\r\n\tTNS.AdExpress.Web.Controls.Results.CountItemsInClassificationWebControl.GetItem("+currentDescription.LevelType.GetHashCode()+",'"+_wordToSearch+"','"+_webSession.IdSession+"',count_"+currentDescription.LevelType.ToString()+"_callback);");
				js.Append("\r\n}");
				js.Append("\r\nfunction count_"+currentDescription.LevelType.ToString()+"_callback(res){");
				js.Append("\r\n\tvar oN=document.getElementById('res_"+currentDescription.LevelType.ToString()+"');");
				js.Append("\r\n\toN.innerHTML=res.value;");
				js.Append("\r\n}\r\n");
				js.Append("\r\naddEvent(window, \"load\", count_"+currentDescription.LevelType.ToString()+");");
				
				//js.Append("count_"+currentDescription.LevelType.ToString()+"();");
			}
			js.Append("\r\nfunction countAll(){");
			foreach(ClassificationDescription currentDescription in Levels){
				js.Append("\r\n\tcount_"+currentDescription.LevelType.ToString()+"();");
			}
			js.Append("\r\n}");
			//js.Append("addEvent(window, \"load\", count_"+currentDescription.LevelType.ToString()+");");
			//js.Append("\r\naddEvent(window, \"load\", countAll);");
			
			

			js.Append("\r\n-->\r\n</SCRIPT>");
			return(js.ToString());
		}
		#endregion

		#region Load
		/// <summary>
		/// Chargement du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnLoad(EventArgs e) {
			AjaxPro.Utility.RegisterTypeForAjax(typeof(CountItemsInClassificationWebControl));
			base.OnLoad (e);
		}
		#endregion

		/// <summary>
		/// Ajoute un sous éléments
		/// </summary>
		/// <param name="obj">Objet à ajouter</param>
		protected override void AddParsedSubObject(object obj){
			if (obj is ClassificationDescriptionHelper) {
				ClassificationDescriptionHelper h = obj as ClassificationDescriptionHelper;
				this.Levels.Add(new ClassificationDescription((ClassificationTable.name)h.LevelType,Int64.Parse(h.LabelTextId)));
			}
			else
				base.AddParsedSubObject (obj);
		}

		#region IAttributeAccessor Members

		/// <summary>
		/// Supports accessing custom attributes
		/// </summary>
		public string GetAttribute(string key) {
			return Attributes[key];
		}

		/// <summary>
		/// Supports setting custom attributes
		/// </summary>
		public void SetAttribute(string key, string value) {
			Attributes[key] = value;
		}

		#endregion

		/// <summary>
		/// Obtient le nombre d'élements contenant le terme wordToSearchTmp pour le niveau tableId
		/// </summary>
		/// <param name="tableId">Niveau de recherche</param>
		/// <param name="wordToSearchTmp">Treme recherché</param>
		/// <param name="sessionId">Identifiant de la session du client</param>
		/// <returns>Nombre d'élements contenant le terme wordToSearchTmp</returns>
		[AjaxPro.AjaxMethod]
		public string GetItem(int tableId,string wordToSearchTmp,string sessionId){
			try{
				WebSession sessionTmp=(WebSession)WebSession.Load(sessionId);
				TNS.AdExpress.Constantes.Classification.DB.Table.name tableName=(TNS.AdExpress.Constantes.Classification.DB.Table.name)tableId;
				//return(tableId.ToString());
				return(TNS.AdExpress.Web.DataAccess.Results.SearchLevelDataAccess.CountItems(tableName,wordToSearchTmp,sessionTmp).ToString());
			}
			catch(System.Exception err){
				return(err.Message);
			}
		}

		#region PréRender
		/// <summary>
		/// Prérendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender (e);
			if(!this.Page.ClientScript.IsClientScriptBlockRegistered("GetCountItemsScript"))
				this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"GetCountItemsScript",GetCountItemsScript());
			_result=new ArrayList();
			//			foreach(ClassificationDescription currentDescription in Levels){
			//				_result.Add(new ResultItem(GestionWeb.GetWebWord(currentDescription.labelTextId,_webSession.SiteLanguage)+" :",TNS.AdExpress.Web.DataAccess.Results.SearchLevelDataAccess.CountItems(currentDescription.LevelType,_wordToSearch,_webSession).ToString()));
			//			}
		}
		#endregion

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			
			StringBuilder html=new StringBuilder(2000);
			string  textCss="";
			if(_textCss.Length>0)
				textCss=" class=\""+_textCss+"\" ";

			if(Levels.Count>0 && _webSession!=null){
//				html.Append("\n<table>");
//				foreach(ClassificationDescription currentDescription in Levels){
//					html.Append("\n\t<tr>\n\t\t<td "+textCss+">");
//					html.Append(GestionWeb.GetWebWord(currentDescription.labelTextId,_webSession.SiteLanguage)+" :");
//					html.Append("\n\t\t</td>");
//					html.Append("\n\t\t<td "+textCss+">");
//					html.Append(TNS.AdExpress.Web.DataAccess.Results.SearchLevelDataAccess.CountItems(currentDescription.LevelType,_wordToSearch,_webSession).ToString());
//					html.Append("\n\t\t</td>\n\t</tr>");
//				}
//				html.Append("\n</table>");
//				html.Append("\n<table>");
//				foreach(ResultItem currentLevelResult in _result){
//					html.Append("\n\t<tr>\n\t\t<td "+textCss+">");
//					html.Append(currentLevelResult.Label);
//					html.Append("\n\t\t</td>");
//					html.Append("\n\t\t<td "+textCss+">");
//					html.Append(currentLevelResult.NbItem);
//					html.Append("\n\t\t</td>\n\t</tr>");
//				}
//				html.Append("\n</table>");
				html.Append("\n<table>");
				foreach(ClassificationDescription currentDescription in Levels){
					html.Append("\n\t<tr>\n\t\t<td "+textCss+">");
					html.Append(GestionWeb.GetWebWord(currentDescription.labelTextId,_webSession.SiteLanguage)+" :");
					html.Append("\n\t\t</td>");
					html.Append("\n\t\t<td "+textCss+">");
					html.Append("<div id=\"res_"+currentDescription.LevelType.ToString()+"\"><img src=\"/Images/Common/await.gif\"></div>");
					html.Append("\n\t\t</td>\n\t</tr>");
				}
				html.Append("\n</table>");
				//html.Append("\n<INPUT id=\"test\" type=\"text\" value=\"ggg\">");
			}
			else
				html.Append("<div "+textCss+">Count tool</div>");
			output.Write(html.ToString());
		}
		#endregion
	}
	
}
