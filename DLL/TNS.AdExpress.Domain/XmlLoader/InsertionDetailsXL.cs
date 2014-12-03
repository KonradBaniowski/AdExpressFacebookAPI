#region Informations
// Author:
// Creation date:
// Modification date:
/*
 *      24/09/2008 - G Ragneau - moved from TNS.AdExpress.Web.Core
 * 
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.IO;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;

using TNS.AdExpress.Constantes;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Level;
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.XmlLoader{

	///<summary>
	/// This class is used to initialize 4 hash tables, the first for allowed module by detail column, the second for column list, 
	/// the third for default media detail level and the fourth for allowed media detail level
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class InsertionDetailsXL {

		#region Parcours du fichier XML
		/// <summary>
		/// This static method is used to initialize 4 hash tables, the first for allowed module by detail column, 
		/// the second for column list, the third for default media detail level 
		/// and the fourth for allowed media detail level
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <param name="allowedModuleByDetailColums">Hashtable contains allowed module by detail column</param>
		/// <param name="detailColumnsList">Hashtable contains detail column</param>
		/// <param name="allowedMediaLevelItems">Hashtable contains default media detail level</param>
		/// <param name="defaultMediaDetailLevels">Hashtable contains allowed media detail level</param>
		/// <example> This example shows the contents of the XML file
		///		<vehicle id="1" name="presse">
		///			<detailColumns>
		///				<detailColumn id="1">
		///					 <allowedModules>
		///						<allowedModule name="Alerte Plan M�dia" privilegecode="198"/>
		///						<allowedModule name="Alerte Plan M�dia Concurrentiel" privilegecode="199"/>
		///						<allowedModule name="Alerte concurrentielle" privilegecode="277"/>
		///						<allowedModule name="Alerte de potentiels" privilegecode="280"/>
		///						<allowedModule name="Analyse Plan M�dia" privilegecode="196"/>
		///						<allowedModule name="Analyse Plan M�dia Annonceur Concurrentiel" privilegecode="184"/>
		///						<allowedModule name="Analyse concurrentielle" privilegecode="278"/>
		///						<allowedModule name="Analyse dynamique" privilegecode="197"/>
		///						<allowedModule name="Analyse de potentiels" privilegecode="281"/>
		///						<allowedModule name="Alerte Portefeuille d'un support" privilegecode="282"/>
		///						<allowedModule name="Portefeuille d'un support" privilegecode="283"/>
		///					</allowedModules>
		///				</detailColumn>
		///			</detailColumns>
		///			<MediaDetailLevel>			
		///				<defaultMediaDetailLevels>
		///					<defaultMediaDetailLevel id="9" name="cat�gorie\support\date" />		
		///				</defaultMediaDetailLevels>
		///				<allowedMediaLevelItems>				
		///					<allowedMediaLevelItem id="2" name="Categorie"/>
		///					<allowedMediaLevelItem id="3" name="Support"/>
		///					<allowedMediaLevelItem id="4" name="Centre d'interet"/>
		///					<allowedMediaLevelItem id="5" name="R�gie"/>
		///					<allowedMediaLevelItem id="6" name="Version"/>
		///					<allowedMediaLevelItem id="7" name="Groupe de soci�t�"/>
		///					<allowedMediaLevelItem id="8" name="Annonceur"/>
		///					<allowedMediaLevelItem id="9" name="Marque"/>	
		///					<allowedMediaLevelItem id="10" name="Produit"/>
		///					<allowedMediaLevelItem id="11" name="Famille"/>
		///					<allowedMediaLevelItem id="12" name="Classe"/>	
		///					<allowedMediaLevelItem id="13" name="Groupe"/>
		///					<allowedMediaLevelItem id="14" name="Vari�t�"/>
		///					<allowedMediaLevelItem id="18" name="Date"/>
		///					<allowedMediaLevelItem id="20" name="Format"/>
		///				</allowedMediaLevelItems>
		///			</MediaDetailLevel>
		///		</vehicle>
		/// </example>
 		/// <exception cref="System.Exception">Thrown when is impossible to load the MediaPlanInsertionConfiguration XML file</exception>
        public static void Load(IDataSource source, Dictionary<Int64, Dictionary<Int64, Int64>> allowedModuleByDetailColums, Dictionary<Int64, List<GenericColumnItemInformation>> detailColumnsList, Dictionary<Int64, List<GenericDetailLevel>> defaultMediaDetailLevels, Dictionary<Int64, List<DetailLevelItemInformation>> allowedMediaLevelItems)
        {
			XmlTextReader Reader;
            List <GenericDetailLevel> defaultMediaDetailLevelList = null;
            List<DetailLevelItemInformation> allowedMediaLevelItemList = null;
            Int64 id = Int64.MinValue;
			Int64 idDetailColumn=0;
			Dictionary<Int64, Int64> idModuleIdDetailColumnTable=null;
					
			try{
					Reader=(XmlTextReader)source.GetSource();								
					
				while(Reader.Read()){
						
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "vehicle":
                                if (id != Int64.MinValue) {
									defaultMediaDetailLevels.Add(id,defaultMediaDetailLevelList);
									allowedMediaLevelItems.Add(id,allowedMediaLevelItemList);
								}
                                id = Int64.MinValue;
									
								if(Reader.GetAttribute("id")!=null ) {
                                    defaultMediaDetailLevelList = new List<GenericDetailLevel>();
                                    allowedMediaLevelItemList = new List<DetailLevelItemInformation>();
                                    idModuleIdDetailColumnTable = new Dictionary<Int64, Int64>();
									id=Int64.Parse(Reader.GetAttribute("id"));
									allowedModuleByDetailColums.Add(Int64.Parse(Reader.GetAttribute("id")),idModuleIdDetailColumnTable);										
								}
								break;	
							case "detailColumn":
								if(Reader.GetAttribute("id")!=null){
									idDetailColumn = Int64.Parse(Reader.GetAttribute("id"));
									if(!detailColumnsList.ContainsKey(idDetailColumn))detailColumnsList.Add(idDetailColumn, WebApplicationParameters.GenericColumnsInformation.GetGenericColumnItemInformationList(idDetailColumn));										
								}
								break;
							case "allowedModule":
								if(Reader.GetAttribute("privilegecode")!=null && idDetailColumn>0){
									idModuleIdDetailColumnTable.Add(Int64.Parse(Reader.GetAttribute("privilegecode")),idDetailColumn);										
								}
								break;
							case "defaultMediaDetailLevel":
								if(Reader.GetAttribute("id")!=null){
									defaultMediaDetailLevelList.Add(DetailLevelsInformation.Get(int.Parse(Reader.GetAttribute("id"))));
								}
								break;
							case "allowedMediaLevelItem":
								if(Reader.GetAttribute("id")!=null){
									allowedMediaLevelItemList.Add(DetailLevelItemsInformation.Get(int.Parse(Reader.GetAttribute("id"))));
								}
								break;
						}					
					}				
				}
                if (id != Int64.MinValue && defaultMediaDetailLevelList != null && defaultMediaDetailLevelList.Count > 0) defaultMediaDetailLevels.Add(id, defaultMediaDetailLevelList);
                if (id != Int64.MinValue && allowedMediaLevelItemList != null && allowedMediaLevelItemList.Count > 0) allowedMediaLevelItems.Add(id, allowedMediaLevelItemList);
		
				}
		
			catch(System.Exception e){
				throw(new System.Exception(" Erreur : ",e)); 
			}		
			
		}
		#endregion

	}
}
