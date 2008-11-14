#region Informations
// Author: D. Mussuma, Y. Rkaina
// Creation date: 24/04/2006
// Modification date:
#endregion

using System;
using System.Collections;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Level;
using ConstantesDB = TNS.AdExpress.Constantes.DB;
using System.Collections.Generic;

namespace TNS.AdExpress.Domain.XmlLoader{

	///<summary>
	/// This class is used to load columns description
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class GenericColumnItemsInformationXL {
		
		/// <summary>
		/// Columns description load
		/// </summary>
		/// <example> This example shows the contents of the XML file
		///		<genericColumn>
		///			<genericColumnItems>
		///				<column id="1" name="Media" webTextId="363" dbId="id_vehicle" dbLabel="vehicle" dbTable="vehicle" dbTablePrefixe="vh"/>
		///				<column id="2" name="Categorie" webTextId="1382" dbId="id_category" dbLabel="category" dbTable="category" dbTablePrefixe="ct"/>
		///				<column id="3" name="Support" webTextId="971" dbId="id_media" dbLabel="media" dbTable="media" dbTablePrefixe="md"/>
		///				<column id="4" name="Centre d'interet" webTextId="1887" dbId="id_interest_center" dbLabel="interest_center" dbTable="interest_center" dbTablePrefixe="ic"/>
		///				<column id="5" name="Régie" webTextId="1383" dbId="id_media_seller" dbLabel="media_seller" dbTable="media_seller" dbTablePrefixe="ms"/>
		///				<column id="6" name="Version" webTextId="1888" dbId="id_slogan" dbIdAlias="id_slogan" convertNullDbId="true" dbLabel="id_slogan" dbLabelAlias="slogan" convertNullDbLabel="true"/>
		///				<column id="7" name="Annonceur" webTextId="857" dbId="id_advertiser" dbLabel="advertiser" dbTable="advertiser" dbTablePrefixe="ad"/>
		///				<column id="8" name="Groupe" webTextId="1110" dbId="id_group_" dbLabel="group_" dbTable="group_" dbTablePrefixe="gr"/>
		///				<column id="9" name="Produit" webTextId="858" dbId="id_product" dbLabel="product" dbTable="product" dbTablePrefixe="pr"/>
		///				<column id="10" name="Spot" webTextId="869"  dbLabel="associated_file"  />
		///				<column id="11" name="Format" webTextId="1420" dbId="id_format" dbLabel="format" dbTable="format" dbTablePrefixe="fo"/>
		///				<column id="12" name="Surface" webTextId="469"  dbLabel="area_page" />
		///				<column id="13" name="Couleur" webTextId="1438" dbId="id_color" dbLabel="color" dbTable="color" dbTablePrefixe="cl"/>
		///				<column id="14" name="Prix" webTextId="868" dbLabel="expenditure_euro" />
		///				<column id="15" name="Descriptif" webTextId="1769" dbId="id_location" dbLabel="location" dbTable="location" dbTablePrefixe="lo"/>
		///				<column id="16" name="Top de diffusion radio" webTextId="860" dbLabel="id_top_diffusion" />
		///				<column id="17" name="Top de diffusion télévision" webTextId="860" dbLabel="top_diffusion" />
		///				<column id="18" name="Durée" webTextId="861"  dbLabel="duration" />
		///				<column id="19" name="Position radio" webTextId="862"  dbLabel="rank" />
		///				<column id="20" name="Position télévision" webTextId="862"  dbLabel="id_rank" />
		///				<column id="21" name="Durée écran" webTextId="863"  dbLabel="duration_commercial_break" />
		///				<column id="22" name="Nombre Spots écran télévision" webTextId="864"  dbLabel="number_message_commercial_brea" />
		///				<column id="23" name="Nombre Spots écran radio" webTextId="864"  dbLabel="number_spot_com_break" />
		///				<column id="24" name="Position hap" webTextId="865"  dbLabel="rank_wap" />
		///				<column id="25" name="Durée écran hap" webTextId="866"  dbLabel="duration_com_break_wap" />
		///				<column id="26" name="Nombre spots hap" webTextId="867"  dbLabel="number_spot_com_break_wap" />
		///				<column id="27" name="Code écran" webTextId="495"  dbLabel="id_commercial_break" />
		///				<column id="28" name="Nombre de panneaux" webTextId="1604"  dbLabel="number_board" />
		///				<column id="29" name="Format du panneau" webTextId="1420"  dbLabel="type_board" />
		///				<column id="30" name="Type de réseau" webTextId="1609"  dbLabel="type_sale" />
		///				<column id="31" name="Réseau afficheur" webTextId="1611"  dbLabel="poster_network" />
		///				<column id="32" name="Agglomeration" webTextId="1660"  dbLabel="agglomeration" />		
		///			</genericColumnItems>
		///		</genericColumn>
		/// </example>
		/// <param name="source">Data Source</param>
		/// <returns>Hashtable contains Columns description</returns>
		/// <exception cref="XmlException">Thrown when the XmlTextReader read an invalid attribute for column</exception>
		/// <exception cref="System.Exception">Thrown when is impossible to load the GenericColumn XML file</exception>
		public static Dictionary<Int64, GenericColumnItemInformation> Load(IDataSource source){
			
			#region variables
			Dictionary<Int64, GenericColumnItemInformation> list = new Dictionary<Int64, GenericColumnItemInformation>();
			XmlTextReader reader=null;
			Int64 id=0;
			Int64 oldId=0;
			string name=null;
			Int64 webTextId=0;
			string dbId=null;
			string dbLabel=null;
			string dbLabelAlias=null;
			string dbIdAlias=null;
			string dbTable=null;
			string dbTablePrefixe=null;
            string cellType = null;
            string strFormat = null;
			string dbRelatedTablePrefixe=null;
			bool convertNullDbId=false;
			bool convertNullDbLabel=false;
			GenericColumnItemInformation genericColumnItemInformation = null;
			ArrayList dbFieldConstraintList = null;
			ArrayList dbJoinConstraintList = null;
			ArrayList dbTableConstraintList = null;
			ArrayList dbOrderConstraintList=null,dbGroupbyConstraintList=null;
			string dbFieldConstraint=null;
			string dbJoinConstraint=null;
			string dbTableConstraint=null;
			string dbOrderConstraint=null;
			string dbGroupbyConstraint=null;
			string sqlOperation=null;
            bool isSum = false;
            bool isMin = false;
            bool isCountDistinct = false;
            bool useLanguage = true;
			#endregion

			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){

							//Column element
							case "column":
					
								id=0;
								name=null;
								webTextId=0;
								dbId=null;
								dbLabel=null;
								dbLabelAlias=null;
								dbIdAlias=null;
								dbTable=null;
								dbTablePrefixe=null;
                                cellType=null;
                                strFormat = null;
								convertNullDbId=false;
								convertNullDbLabel=false;
                                isSum = false;
                                useLanguage = true;
                                isMin = false;
                                isCountDistinct = false;
								
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0) && 
									(reader.GetAttribute("name")!=null && reader.GetAttribute("name").Length>0) &&
									(reader.GetAttribute("webTextId")!=null && reader.GetAttribute("webTextId").Length>0) 
                                    //&& ((reader.GetAttribute("dbId")!=null && reader.GetAttribute("dbId").Length>0) || (reader.GetAttribute("dbLabel")!=null && reader.GetAttribute("dbLabel").Length>0) )
									){
									id=Int64.Parse(reader.GetAttribute("id"));
									name=reader.GetAttribute("name");
									webTextId=Int64.Parse(reader.GetAttribute("webTextId"));
									dbId=reader.GetAttribute("dbId");
									dbLabel=reader.GetAttribute("dbLabel");
									dbLabelAlias=reader.GetAttribute("dbLabelAlias");
									dbIdAlias=reader.GetAttribute("dbIdAlias");
									dbTable=reader.GetAttribute("dbTable");
									dbTablePrefixe=reader.GetAttribute("dbTablePrefixe");

                                    if (reader.GetAttribute("CellType") != null && reader.GetAttribute("CellType").Length > 0) cellType = reader.GetAttribute("CellType");
                                    if (reader.GetAttribute("format") != null && reader.GetAttribute("format").Length > 0) strFormat = reader.GetAttribute("format");
									if(reader.GetAttribute("convertNullDbId")!=null && reader.GetAttribute("convertNullDbId").Length>0)convertNullDbId=bool.Parse(reader.GetAttribute("convertNullDbId"));
									if(reader.GetAttribute("convertNullDbLabel")!=null && reader.GetAttribute("convertNullDbLabel").Length>0)convertNullDbLabel=bool.Parse(reader.GetAttribute("convertNullDbLabel"));
									if(reader.GetAttribute("sqlOperation")!=null && reader.GetAttribute("sqlOperation").Length>0) {										
										sqlOperation = reader.GetAttribute("sqlOperation");
									}
									if (reader.GetAttribute("dbRelatedTablePrefixe")!=null && reader.GetAttribute("dbRelatedTablePrefixe").Length>0)dbRelatedTablePrefixe = reader.GetAttribute("dbRelatedTablePrefixe");
									
									if(oldId!=id && genericColumnItemInformation!=null ){
										//Add constraint : database field
										if(dbFieldConstraintList!=null && dbFieldConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_FIELD_CONTRAINT_TYPE))
											genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_FIELD_CONTRAINT_TYPE,dbFieldConstraintList);
										//Add  constraint : database join
										if(dbJoinConstraintList!=null && dbJoinConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_JOIN_CONTRAINT_TYPE))
											genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_JOIN_CONTRAINT_TYPE,dbJoinConstraintList);
										//Add constraint : database table
										if(dbTableConstraintList!=null && dbTableConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_TABLE_CONTRAINT_TYPE))				
											genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_TABLE_CONTRAINT_TYPE,dbTableConstraintList);
										//Add constraint : order by clause
										if(dbOrderConstraintList!=null && dbOrderConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_ORDER_CONTRAINT_TYPE))
											genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_ORDER_CONTRAINT_TYPE,dbOrderConstraintList);
										//Add constraint : group by clause
										if(dbGroupbyConstraintList!=null && dbGroupbyConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.GROUP_BY_CONTRAINT_TYPE))
											genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.GROUP_BY_CONTRAINT_TYPE,dbGroupbyConstraintList);
									}

									if((sqlOperation!=null && sqlOperation.Length>0) && (dbRelatedTablePrefixe!=null && dbRelatedTablePrefixe.Length>0))
                                        genericColumnItemInformation = new GenericColumnItemInformation(id, name, webTextId, dbId, dbIdAlias, convertNullDbId, dbLabel, dbLabelAlias, convertNullDbLabel, dbTable, dbTablePrefixe, cellType, strFormat, dbRelatedTablePrefixe, sqlOperation);
									else
                                        genericColumnItemInformation = new GenericColumnItemInformation(id, name, webTextId, dbId, dbIdAlias, convertNullDbId, dbLabel, dbLabelAlias, convertNullDbLabel, dbTable, dbTablePrefixe, cellType, strFormat);
                                    if (reader.GetAttribute("isSum") != null && reader.GetAttribute("isSum").Length > 0) genericColumnItemInformation.IsSum = bool.Parse(reader.GetAttribute("isSum"));
                                    if (reader.GetAttribute("isCountDistinct") != null && reader.GetAttribute("isCountDistinct").Length > 0) genericColumnItemInformation.IsCountDistinct = bool.Parse(reader.GetAttribute("isCountDistinct"));
                                    if (reader.GetAttribute("isMax") != null && reader.GetAttribute("isMax").Length > 0) genericColumnItemInformation.IsMax = bool.Parse(reader.GetAttribute("isMax"));
                                    if (reader.GetAttribute("isMin") != null && reader.GetAttribute("isMin").Length > 0) genericColumnItemInformation.IsMin = bool.Parse(reader.GetAttribute("isMin"));
                                    if (reader.GetAttribute("useLanguage") != null && reader.GetAttribute("useLanguage").Length > 0) genericColumnItemInformation.UseLanguageRule = bool.Parse(reader.GetAttribute("useLanguage"));
									list.Add(id,genericColumnItemInformation);

									
									
								}
								else{
									throw(new XmlException("Invalide Attribute for column"));
								}
								oldId=id;
								sqlOperation=null;
								dbRelatedTablePrefixe = null;
								dbFieldConstraintList = null;
								dbJoinConstraintList = null;
								dbTableConstraintList = null;
								dbOrderConstraintList = null;
								dbGroupbyConstraintList = null;
								break;

							//Element : Database field constraint
							case "dbFieldConstraint" :								
								if(genericColumnItemInformation!=null){									
									if ((reader.GetAttribute("dbField")!=null && reader.GetAttribute("dbField").Length>0)) {
										if(dbFieldConstraintList==null) dbFieldConstraintList = new ArrayList();
										if ((reader.GetAttribute("dbTablePrefixe")!=null && reader.GetAttribute("dbTablePrefixe").Length>0))dbFieldConstraint=reader.GetAttribute("dbTablePrefixe")+".";
										 dbFieldConstraint+=reader.GetAttribute("dbField").Trim();
										dbFieldConstraintList.Add(dbFieldConstraint);										
									}																	
								}
								break;

							//Element : database join constraint
							case "dbJoinConstraint" :
								if(genericColumnItemInformation!=null){									
									if (reader.GetAttribute("dbField")!=null && reader.GetAttribute("dbField").Length>0 
										&& (( reader.GetAttribute("dbRelatedField")!=null && reader.GetAttribute("dbRelatedField").Length>0) || ( reader.GetAttribute("defaultLanguage")!=null && reader.GetAttribute("defaultLanguage").Length>0))
										&& reader.GetAttribute("sqlOperation")!=null && reader.GetAttribute("sqlOperation").Length>0
										) {
										if(dbJoinConstraintList==null) dbJoinConstraintList = new ArrayList();
										if ((reader.GetAttribute("dbTablePrefixe")!=null && reader.GetAttribute("dbTablePrefixe").Length>0))
											dbJoinConstraint=reader.GetAttribute("dbTablePrefixe")+"."+reader.GetAttribute("dbField");
										else dbJoinConstraint=reader.GetAttribute("dbField");
										if(reader.GetAttribute("sqlOperation").Equals("leftOuterJoin")){
											if(reader.GetAttribute("dbField").Equals("activation"))
											dbJoinConstraint+= "(+)<";
											else dbJoinConstraint+= "(+)=";
										}
										else if(reader.GetAttribute("sqlOperation").Equals("rightOuterJoin")){
											if(reader.GetAttribute("dbField").Equals("activation"))
												dbJoinConstraint+= "<(+)";
											else dbJoinConstraint+= "=(+)";
										}
										else{
											dbJoinConstraint+= "=";
										}
										if ((reader.GetAttribute("dbRelatedTablePrefixe")!=null && reader.GetAttribute("dbRelatedTablePrefixe").Length>0))
											dbJoinConstraint+=reader.GetAttribute("dbRelatedTablePrefixe")+".";
										if ((reader.GetAttribute("dbRelatedField")!=null && reader.GetAttribute("dbRelatedField").Length>0))
										dbJoinConstraint+=reader.GetAttribute("dbRelatedField");
										else if ((reader.GetAttribute("defaultLanguage")!=null && reader.GetAttribute("defaultLanguage").Length>0))
												 dbJoinConstraint+=reader.GetAttribute("defaultLanguage");
										dbJoinConstraintList.Add(dbJoinConstraint);										
									}									
								}
								break;
							//Element : database table constraint
							case "dbTableConstraint" :								
								if(genericColumnItemInformation!=null){									
									if ((reader.GetAttribute("dbTable")!=null && reader.GetAttribute("dbTable").Length>0)) {										
										if(dbTableConstraintList==null) dbTableConstraintList = new ArrayList();
										if ((reader.GetAttribute("dbSchema")!=null && reader.GetAttribute("dbSchema").Length>0))
											dbTableConstraint=reader.GetAttribute("dbSchema")+"."+reader.GetAttribute("dbTable");
										else dbTableConstraint=reader.GetAttribute("dbTable");
										if ((reader.GetAttribute("dbTablePrefixe")!=null && reader.GetAttribute("dbTablePrefixe").Length>0))dbTableConstraint+="  "+reader.GetAttribute("dbTablePrefixe");										
										dbTableConstraintList.Add(dbTableConstraint);										
									}																	
								}
								break;
							//Element : order by clause constraint
							case "dbOrderConstraint" :								
								if(genericColumnItemInformation!=null){	
									if ((reader.GetAttribute("dbTablePrefixe")!=null && reader.GetAttribute("dbTablePrefixe").Length>0))
                                        dbOrderConstraint=dbTableConstraint = reader.GetAttribute("dbTablePrefixe") + ".";										
									if ((reader.GetAttribute("dbField")!=null && reader.GetAttribute("dbField").Length>0)) {
										if(dbOrderConstraintList==null) dbOrderConstraintList = new ArrayList();
										dbOrderConstraint+=reader.GetAttribute("dbField");										
										dbOrderConstraintList.Add(dbOrderConstraint);										
									}																	
								}
								break;
							//Element : group by clause constraint
							case "dbGroupbyConstraint" :								
								if(genericColumnItemInformation!=null){	
									if ((reader.GetAttribute("dbTablePrefixe")!=null && reader.GetAttribute("dbTablePrefixe").Length>0))
										dbGroupbyConstraint=reader.GetAttribute("dbTablePrefixe")+".";										
									if ((reader.GetAttribute("dbField")!=null && reader.GetAttribute("dbField").Length>0)) {
										if(dbGroupbyConstraintList==null) dbGroupbyConstraintList = new ArrayList();
										dbGroupbyConstraint+=reader.GetAttribute("dbField");										
										dbGroupbyConstraintList.Add(dbGroupbyConstraint);										
									}																	
								}
								break;
						}
					}
				}
				if(oldId!=0 && genericColumnItemInformation!=null ){
					if(dbFieldConstraintList!=null && dbFieldConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_FIELD_CONTRAINT_TYPE))
						genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_FIELD_CONTRAINT_TYPE,dbFieldConstraintList);
					if(dbJoinConstraintList!=null && dbJoinConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_JOIN_CONTRAINT_TYPE))
						genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_JOIN_CONTRAINT_TYPE,dbJoinConstraintList);
					if(dbTableConstraintList!=null && dbTableConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_TABLE_CONTRAINT_TYPE))				
						genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_TABLE_CONTRAINT_TYPE,dbTableConstraintList);
					if(dbOrderConstraintList!=null && dbOrderConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.DB_ORDER_CONTRAINT_TYPE))
						genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.DB_ORDER_CONTRAINT_TYPE,dbOrderConstraintList);
					if(dbGroupbyConstraintList!=null && dbGroupbyConstraintList.Count>0 && genericColumnItemInformation.Constraints!=null && !genericColumnItemInformation.Constraints.ContainsKey(ConstantesDB.Constraints.GROUP_BY_CONTRAINT_TYPE))
						genericColumnItemInformation.Constraints.Add(ConstantesDB.Constraints.GROUP_BY_CONTRAINT_TYPE,dbGroupbyConstraintList);
				}
				source.Close();
				return(list);
			}
			catch(System.Exception err){
				source.Close();
				throw(new Exceptions.GenericColumnItemsInformationXLException("Impossible to load the GenericColumn XML file",err));
			}

		}
	}
}
