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
using System.Collections.Generic;
using TNS.AdExpress.Domain.Web;


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
        public static void Load(IDataSource source, Dictionary<Int64, List<GenericColumnItemInformation>> columnsSets, Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>> columnsVisibility, Dictionary<Int64, List<GenericColumnItemInformation>> columnsSetKeys, Dictionary<Int64, Dictionary<GenericColumnItemInformation.Columns, bool>> columnsFilter)
        {
			List<GenericColumnItemInformation> columnIds = null;
            List<GenericColumnItemInformation> keys = null;
            Dictionary<GenericColumnItemInformation.Columns, bool> visibility = null;
            Dictionary<GenericColumnItemInformation.Columns, bool> filter = null;
            XmlTextReader reader=null;
			GenericColumnItemInformation genericColumnItemInformation =null;
			Int64 id=0;
            bool bVisible = true;
            bool bFilter = false;
			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "detailColumn" :
								if(id!=0){
                                    columnsSets.Add(id, columnIds);
                                    columnsVisibility.Add(id, visibility);
                                    columnsSetKeys.Add(id, keys);
                                    columnsFilter.Add(id, filter);
                                }
								id=0;
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0)){
									id=Int64.Parse(reader.GetAttribute("id"));
									columnIds = new List<GenericColumnItemInformation>();
                                    visibility = new Dictionary<GenericColumnItemInformation.Columns, bool>();
                                    filter = new Dictionary<GenericColumnItemInformation.Columns, bool>();
                                    keys = new List<GenericColumnItemInformation>();
								}
								else{
									throw(new XmlException("Invalide Attribute for vehicle"));
								}
								break;							
							case "columnItem":
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0)){
									genericColumnItemInformation = WebApplicationParameters.GenericColumnItemsInformation.Get(Int64.Parse(reader.GetAttribute("id")));
									if ((reader.GetAttribute("notInExcelExport")!=null && reader.GetAttribute("notInExcelExport").Length>0))
										genericColumnItemInformation.NotInExcelExport = bool.Parse(reader.GetAttribute("notInExcelExport"));//For excel export
                                    if ((reader.GetAttribute("visible") != null && reader.GetAttribute("visible").Length > 0)) {
                                        genericColumnItemInformation.Visible = bVisible = bool.Parse(reader.GetAttribute("visible"));
                                    }
                                    else {
                                        bVisible = true;
                                    }
                                    if ((reader.GetAttribute("filter") != null && reader.GetAttribute("filter").Length > 0)) {
                                        bFilter = bool.Parse(reader.GetAttribute("filter"));
                                    }
                                    else {
                                        bFilter = false;
                                    }
									if ((reader.GetAttribute("idDetailLevelMatching")!=null && reader.GetAttribute("idDetailLevelMatching").Length>0))
										genericColumnItemInformation.IdDetailLevelMatching = int.Parse(reader.GetAttribute("idDetailLevelMatching"));
                                    if ((reader.GetAttribute("isKey") != null && reader.GetAttribute("isKey").Length > 0)) {
                                        if (bool.Parse(reader.GetAttribute("isKey"))) {
                                            keys.Add(genericColumnItemInformation);
                                        }
                                    }
									columnIds.Add(genericColumnItemInformation);
                                    visibility.Add(genericColumnItemInformation.Id, bVisible);
                                    filter.Add(genericColumnItemInformation.Id, bFilter);
								}
								else{
									throw(new XmlException("Invalide Attribute for defaultColumn"));
								}
								break;
							
						}
					}
				}
                if (id != 0 && columnIds != null && columnIds.Count > 0) {
                    columnsSets.Add(id, columnIds);
                    columnsVisibility.Add(id, visibility);
                    columnsFilter.Add(id, filter);
                    columnsSetKeys.Add(id, keys);
                }
				source.Close();
			}
			catch(System.Exception err){
				source.Close();
				throw(new Exceptions.GenericColumnsInformationXLException("Impossible to load the GenericColumn XML file",err));
			}
		}
	}
}
