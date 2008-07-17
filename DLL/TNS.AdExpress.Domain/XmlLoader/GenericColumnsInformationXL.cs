#region Informations
// Author: D. Mussuma, Y. Rkaina
// Creation date: 25/04/2006
// Modification date:
#endregion

using System;
using System.Collections;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Exceptions;
using ConstantesWeb = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Level;


namespace TNS.AdExpress.Domain.XmlLoader{

	///<summary>
	/// This class is used to load default detail column
	/// </summary>
	/// <stereotype>utility</stereotype>
	public class GenericColumnsInformationXL {		

		/// <summary>Load default detail columns by media</summary>
		/// <param name="source">Data Source</param>	
		/// <returns>Generic columns list by media</returns>	
		///	<example>  This example shows the contents of the XML file
		///		<detailColumn id="1" name="Page\Visuel\Annonceur\Groupe\Produit\Version\Centre d'interet\Régie\Format\Surface\Couleur\Prix\Descriptif">						
		///			<columnItem id="35" name="Visuel" notInExcelExport="true"/>		
		///			<columnItem id="34" name="Page"/> 						
		///			<columnItem id="7" name="Annonceur" idDetailLevelMatching="8"/>
		///			<columnItem id="8" name="Groupe" idDetailLevelMatching="13"/>
		///			<columnItem id="9" name="Produit" idDetailLevelMatching="10"/>
		///			<columnItem id="6" name="Version" idDetailLevelMatching="6"/>
		///			<columnItem id="4" name="Centre d'interet" idDetailLevelMatching="4"/>
		///			<columnItem id="5" name="Régie" idDetailLevelMatching="5"/>
		///			<columnItem id="11" name="Format" idDetailLevelMatching="20"/>
		///			<columnItem id="12" name="Surface"/>
		///			<columnItem id="13" name="Couleur"/>
		///			<columnItem id="36" name="Position"/>
		///			<columnItem id="14" name="Prix"/>
		///			<columnItem id="15" name="Descriptif"/>		
		///		</detailColumn>		
		///	</example>
		/// <exception cref="XmlException">Thrown when the XmlTextReader read an invalid attribute for column</exception>
		/// <exception cref="System.Exception">Thrown when is impossible to load the GenericColumn XML file</exception>
		public static Hashtable Load(IDataSource source){
			Hashtable list=new Hashtable();			
			ArrayList columnIds=null;		
			XmlTextReader reader=null;
			GenericColumnItemInformation genericColumnItemInformation =null;
			Int64 id=0;
			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "detailColumn" :
								if(id!=0){
									list.Add(id,columnIds);
								}
								id=0;
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0)){
									id=Int64.Parse(reader.GetAttribute("id"));
									columnIds=new ArrayList();
								}
								else{
									throw(new XmlException("Invalide Attribute for vehicle"));
								}
								break;							
							case "columnItem":
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0)){
									genericColumnItemInformation = GenericColumnItemsInformation.Get(Int64.Parse(reader.GetAttribute("id")));
									if ((reader.GetAttribute("notInExcelExport")!=null && reader.GetAttribute("notInExcelExport").Length>0))
										genericColumnItemInformation.NotInExcelExport = bool.Parse(reader.GetAttribute("notInExcelExport"));//For excel export
                                    if ((reader.GetAttribute("visible") != null && reader.GetAttribute("visible").Length > 0))
                                        genericColumnItemInformation.Visible = bool.Parse(reader.GetAttribute("visible"));
									if ((reader.GetAttribute("idDetailLevelMatching")!=null && reader.GetAttribute("idDetailLevelMatching").Length>0))
										genericColumnItemInformation.IdDetailLevelMatching = int.Parse(reader.GetAttribute("idDetailLevelMatching"));
									columnIds.Add(genericColumnItemInformation);
								}
								else{
									throw(new XmlException("Invalide Attribute for defaultColumn"));
								}
								break;
							
						}
					}
				}
				if(id!=0 && columnIds!=null && columnIds.Count>0)list.Add(id,columnIds);
				source.Close();
				return(list);
			}
			catch(System.Exception err){
				source.Close();
				throw(new Exceptions.GenericColumnsInformationXLException("Impossible to load the GenericColumn XML file",err));
			}
		}
	}
}
