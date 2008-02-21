#region Informations
// Author: G. Facon
// Creation date:
// Modification date:
#endregion


using System;
using System.Collections;
using System.Xml;
using System.IO;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
using CoreExceptions=TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using ConstantesWeb=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Core.DataAccess {

	///	 <since>28/03/2006</since>
	///  <author>G. Facon</author>
	///  <summary>Load default detail level</summary>
	///  <stereotype>utility</stereotype>
	public class DetailLevelsInformationDataAccess {

		///	 <author>G. Facon</author>
		///  <since>28/03/2006</since>
		///  <summary>Load default detail level</summary>
		///  <param name="source">Data Source</param>
		///  <returns>Hashtable contains detail levels</returns>
		///  <example>This example shows the contents of the XML file
		///		<detailLevels>
		///			<detailLevel id="1" name="media\catgorie">
		///				<levelItem id="1" name="Media"/>
		///				<levelItem id="2" name="Category"/>
		///			</detailLevel>
		///			<detailLevel id="2" name="media\catgorie\support">
		///				<levelItem id="1" name="Media"/>
		///				<levelItem id="2" name="Category"/>
		///				<levelItem id="3" name="Support"/>
		///			</detailLevel>
		///			<detailLevel id="3" name="media\support">
		///				<levelItem id="1" name="Media"/>
		///				<levelItem id="3" name="Support"/>
		///			</detailLevel>
		///		 </detailLevels>
		///	 </example>
		/// <exception cref="XmlException">Thrown when the XmlTextReader read an invalid attribute for levelItem</exception>
		/// <exception cref="System.Exception">Thrown when is impossible to load the GenericDetailLevel XML file</exception>
		public static Hashtable Load(IDataSource source){
			Hashtable list=new Hashtable();
			ArrayList levelIds=null;
			XmlTextReader reader=null;
			int id=0;
			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "detailLevel":
								if(id!=0){
									list.Add(id,new GenericDetailLevel(levelIds));
								}
								id=0;
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0)){
									id=int.Parse(reader.GetAttribute("id"));
									levelIds=new ArrayList();
								}
								else{
									throw(new XmlException("Invalide Attribute for detailLevel"));
								}
								break;
							case "levelItem":
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0)){
									levelIds.Add(int.Parse(reader.GetAttribute("id")));
								}
								else{
									throw(new XmlException("Invalide Attribute for levelItem"));
								}
								break;
						}
					}
				}
				if(id!=0 && levelIds.Count>0)list.Add(id,new GenericDetailLevel(levelIds));
				source.Close();
				return(list);
			}
			catch(System.Exception err){
				source.Close();
				throw(new CoreExceptions.DetailLevelsInformationDataAccessException("Impossible to load the GenericDetailLevel XML file",err));
			}
		}

		/// <summary>
		/// Load allowed detail levels
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <returns>Allowed detail level list</returns>
		/// <example>This example shows the contents of the XML file
		///		<allowedMediaLevelItems>				
		///			<allowedMediaLevelItem id="2" name="Categorie"/>
		///			<allowedMediaLevelItem id="3" name="Support"/>
		///			<allowedMediaLevelItem id="4" name="Centre d'interet"/>
		///			<allowedMediaLevelItem id="5" name="Régie"/>
		///			<allowedMediaLevelItem id="6" name="Version"/>
		///			<allowedMediaLevelItem id="7" name="Groupe de société"/>
		///			<allowedMediaLevelItem id="8" name="Annonceur"/>
		///			<allowedMediaLevelItem id="9" name="Marque"/>	
		///			<allowedMediaLevelItem id="10" name="Produit"/>
		///			<allowedMediaLevelItem id="11" name="Famille"/>
		///			<allowedMediaLevelItem id="12" name="Classe"/>	
		///			<allowedMediaLevelItem id="13" name="Groupe"/>
		///			<allowedMediaLevelItem id="14" name="Variété"/>
		///			<allowedMediaLevelItem id="18" name="Date"/>
		///			<allowedMediaLevelItem id="20" name="Format"/>
		///		</allowedMediaLevelItems>
		/// </example>
		/// <exception cref="XmlException">Thrown when the XmlTextReader read an invalid attribute for allowedMediaLevelItem</exception>
		/// <exception cref="System.Exception">Thrown when is impossible to load the MediaPlanInsertionConfiguration XML file</exception>
		public static ArrayList LoadDetailLevel(IDataSource source){
			ArrayList levelIds=null;
			XmlTextReader reader=null;
			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "allowedMediaLevelItem":
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0)){
									levelIds.Add(DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								}
								else{
									throw(new XmlException("Invalid Attribute for allowedMediaLevelItem"));
								} 
								break;
						}
					}
				}
				source.Close();
				return(levelIds);
			}
			catch(System.Exception err){
				source.Close();
				throw(new CoreExceptions.DetailLevelsInformationDataAccessException("Impossible to load the MediaPlanInsertionConfiguration XML file",err));
			}
		}
	}
}
