#region Informations
// Author: G. Facon
// Creation date: 27/03/2006
// Modification date:
#endregion

using System;
using System.Collections;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
using CoreExceptions=TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Core.DataAccess{


	///<summary>
	/// Load detail level descriptions
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class DetailLevelItemsInformationDataAccess {
		/// <summary>
		/// Load detail level descriptions
		/// </summary>
		/// <example>This example shows the contents of the XML file
		///		<Detaillevels>
		///			<level id="1" name="Media" webTextId="363" dbId="id_media" dbName="media" dbTable="media" dbTablePrefixe="md"/>
		///			<level id="2" name="Categorie" webTextId="1382" dbId="id_category" dbName="category" dbTable="category" dbTablePrefixe="ct"/>
		///			<level id="3" name="Support" webTextId="971" dbId="id_advertiser" dbName="advertiser" dbTable="advertiser" dbTablePrefixe="ad"/>
		///			<level id="4" name="Centre d'interet" webTextId="1887" dbId="id_interest_center" dbName="interest_center" dbTable="interest_center" dbTablePrefixe="ic"/>
		///			<level id="5" name="Régie" webTextId="1383" dbId="id_media_seller" dbName="media_seller" dbTable="media_seller" dbTablePrefixe="ms"/>
		///			<level id="6" name="Accroche" webTextId="1888" dbId="id_slogan" dbName="id_slogan" dbNameAlias="slogan"/>
		///			<level id="7" name="Groupe de société" webTextId="1589" dbId="id_holding_company" dbName="holding_company" dbTable="holding_company" dbTablePrefixe="hc"/>
		///			<level id="8" name="Annonceur" webTextId="857" dbId="id_advertiser" dbName="advertiser" dbTable="advertiser" dbTablePrefixe="ad"/>
		///			<level id="9" name="Marque" webTextId="1889" dbId="id_brand" dbName="brand" dbTable="brand" dbTablePrefixe="br"/>
		///			<level id="10" name="Produit" webTextId="858" dbId="id_product" dbName="product" dbTable="product" dbTablePrefixe="pr"/>
		///			<level id="11" name="Famille" webTextId="1103" dbId="id_sector" dbName="sector" dbTable="sector" dbTablePrefixe="sc"/>
		///			<level id="12" name="Classe" webTextId="552" dbId="id_subsector" dbName="subsector" dbTable="subsector" dbTablePrefixe="ssc"/>
		///			<level id="13" name="Groupe" webTextId="1110" dbId="id_group_" dbName="group_" dbTable="group_" dbTablePrefixe="gr"/>
		///			<level id="14" name="Variété" webTextId="592" dbId="id_segment" dbName="segment" dbTable="segment" dbTablePrefixe="sg"/>
		///			<level id="15" name="Groupe d'agence" webTextId="1890" dbId="id_group_media_agency" dbName="group_media_agency" dbTable="group_media_agency" dbTablePrefixe="gma" toTrack="true"/>
		///			<level id="16" name="Agence" webTextId="1891" dbId="id_media_agency" dbName="media_agency" dbTable="media_agency" dbTablePrefixe="ma" toTrack="true"/>
		///		</Detaillevels>
		/// </example>
		/// <param name="source">Data Source</param>
		/// <returns>Detail level item descriptions (Hashtable)</returns>
		/// <exception cref="XmlException">Thrown when the XmlTextReader read an invalid attribute for level</exception>
		/// <exception cref="System.Exception">Thrown when is impossible to load the GenericDetailLevel XML file</exception>
		public static Hashtable Load(IDataSource source){
			Hashtable list=new Hashtable();
			XmlTextReader reader=null;
			int id=0;
			string name=null;
			Int64 webTextId=0;
			string dbId=null;
			string dbLabel=null;
			string dbLabelAlias=null;
			string dbIdAlias=null;
			string dbTable=null;
			string dbTablePrefixe=null;
			bool toTrack=false;
			bool convertNullDbId=false;
			bool convertNullDbLabel=false;
			bool dbExternalJoin = false;

			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "level":
								id=0;
								name=null;
								webTextId=0;
								dbId=null;
								dbLabel=null;
								dbLabelAlias=null;
								dbIdAlias=null;
								dbTable=null;
								dbTablePrefixe=null;
								toTrack=false;
								convertNullDbId=false;
								convertNullDbLabel=false;
								dbExternalJoin=false;
								if ((reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0) && 
									(reader.GetAttribute("name")!=null && reader.GetAttribute("name").Length>0) &&
									(reader.GetAttribute("webTextId")!=null && reader.GetAttribute("webTextId").Length>0) &&
									(reader.GetAttribute("dbId")!=null && reader.GetAttribute("dbId").Length>0)){
									id=int.Parse(reader.GetAttribute("id"));
									name=reader.GetAttribute("name");
									webTextId=Int64.Parse(reader.GetAttribute("webTextId"));
									dbId=reader.GetAttribute("dbId");
									dbLabel=reader.GetAttribute("dbLabel");
									dbLabelAlias=reader.GetAttribute("dbLabelAlias");
									dbIdAlias=reader.GetAttribute("dbIdAlias");
									dbTable=reader.GetAttribute("dbTable");
									dbTablePrefixe=reader.GetAttribute("dbTablePrefixe");
									if(reader.GetAttribute("toTrack")!=null && reader.GetAttribute("toTrack").Length>0)toTrack=bool.Parse(reader.GetAttribute("toTrack"));
									if(reader.GetAttribute("convertNullDbId")!=null && reader.GetAttribute("convertNullDbId").Length>0)convertNullDbId=bool.Parse(reader.GetAttribute("convertNullDbId"));
									if(reader.GetAttribute("convertNullDbLabel")!=null && reader.GetAttribute("convertNullDbLabel").Length>0)convertNullDbLabel=bool.Parse(reader.GetAttribute("convertNullDbLabel"));
									if(reader.GetAttribute("dbExternalJoin")!=null && reader.GetAttribute("dbExternalJoin").Length>0)dbExternalJoin=bool.Parse(reader.GetAttribute("dbExternalJoin"));
									list.Add(id,new DetailLevelItemInformation(id,name,webTextId,dbId,dbIdAlias,convertNullDbId,dbLabel,dbLabelAlias,convertNullDbLabel,dbTable,dbTablePrefixe,toTrack,dbExternalJoin));
								}
								else{
									throw(new XmlException("Invalid Attribute for level"));
								}
								break;
						}
					}
				}
				source.Close();
				return(list);
			}
			catch(System.Exception err){
				source.Close();
				throw(new CoreExceptions.DetailLevelItemsInformationDataAccessException("Impossible to load the GenericDetailLevel XML file",err));
			}

		}
	}
}