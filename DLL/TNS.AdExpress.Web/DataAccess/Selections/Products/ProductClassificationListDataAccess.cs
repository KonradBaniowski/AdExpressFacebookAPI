#region Informations
// Auteur: G. Facon 
// Date de création: 26/08/2004 
// Date de modification: 26/08/2004 
// 25/11/2005 Par B.Masson > webSession.Source
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Rules.Customer;
using TablesDBConstantes=TNS.AdExpress.Constantes.DB.Tables;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.DataAccess.Selections;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// Description résumée de ProductClassificationList.
	/// </summary>
	public class ProductClassificationListDataAccess{		

		#region liste de Famille en fonction des droits du client
		/// <summary>
		///Donne une liste de Famille en fonction des droits du client. 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Liste des familles autorisées au client.</returns>
		public static DataSet SectorList(WebSession webSession){
				return SectorList(webSession,"");
		}

		/// <summary>
		///Donne une liste de Famille en fonction des droits du client. 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idSector">identifiant(s) de(s) famille(s)</param>
		/// <returns>Liste des familles autorisées au client.</returns>
		public static DataSet SectorList(WebSession webSession,string idSector){

			#region Construction de la requête
			string sql="";
			sql+=" select distinct "+TablesDBConstantes.SECTOR_PREFIXE+".id_sector,"+TablesDBConstantes.SECTOR_PREFIXE+".sector";
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".sector "+TablesDBConstantes.SECTOR_PREFIXE;
			sql+=" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".subsector "+TablesDBConstantes.SUBSECTOR_PREFIXE;
			sql+=" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".group_ "+TablesDBConstantes.GROUP_PREFIXE;
			sql+=" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".segment "+TablesDBConstantes.SEGMENT_PREFIXE;
			sql+=" Where";
			// Jointure
			sql+=" "+TablesDBConstantes.SECTOR_PREFIXE+".id_sector="+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_sector ";
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_subsector="+TablesDBConstantes.GROUP_PREFIXE+".id_subsector ";
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".id_group_="+TablesDBConstantes.SEGMENT_PREFIXE+".id_group_ ";			
			// Langue
			sql+=" and "+TablesDBConstantes.SECTOR_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();		
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();	
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();	
			sql+=" and "+TablesDBConstantes.SEGMENT_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();	
			// Activation
			sql+=" and "+TablesDBConstantes.SECTOR_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and "+TablesDBConstantes.SUBSECTOR_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;						
			sql+=" and "+TablesDBConstantes.GROUP_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;						
			sql+=" and "+TablesDBConstantes.SEGMENT_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;						
			
			if(CheckedText.IsStringEmpty(idSector)){
				sql+=" and "+TablesDBConstantes.SECTOR_PREFIXE+".id_sector in ("+idSector+") ";	
			}
			#region Nomenclature Produit (droits)
			//Droits en accès
			sql+=SQLGenerator.getClassificationCustomerProductRight(webSession,true);
			#endregion
			
			// Tri
			sql+=" order by "+TablesDBConstantes.SECTOR_PREFIXE+".sector";

			#endregion

			#region Execution de la requête
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw (new ProductClassificationListDataAccessException("Impossible d'obtenir une liste de Famille en fonction des droits du client",err));
			}
			#endregion

		}
		#endregion	

		#region Retourne la liste des id_sector à partir d'un treeNode
		/// <summary>
		/// Retourne la liste des id_sector en accès à partir d'un treeNode
		/// </summary>
		/// <param name="tree">arbre</param>
		/// <param name="webSession">Session web</param>
		/// <returns></returns>
		public static string ListSectorInTreeNode(WebSession webSession,TreeNode tree){

			#region Variables
			string sql="";
			bool premier=true;
			string sectorAccess="";
			string sectorException="";
			string groupAccess="";
			string groupException="";
			string segmentAccess="";
			string segmentException="";
			string listSectorForRecap="";
			#endregion

			#region Construction de la requête
			
			sectorAccess=webSession.GetSelection(tree,CustomerRightConstante.type.sectorAccess);
			sectorException=webSession.GetSelection(tree,CustomerRightConstante.type.sectorException);
			groupAccess=webSession.GetSelection(tree,CustomerRightConstante.type.groupAccess);
			groupException=webSession.GetSelection(tree,CustomerRightConstante.type.groupException);
			segmentAccess=webSession.GetSelection(tree,CustomerRightConstante.type.segmentAccess);
			segmentException=webSession.GetSelection(tree,CustomerRightConstante.type.segmentException);

			//Requête SQL
			sql="Select distinct su.id_sector from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".subsector su,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".group_  gp,"+DBConstantes.Schema.ADEXPRESS_SCHEMA+".segment sg ";
			sql+=" where";
			// Langue
			sql+=" su.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and gp.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql+=" and sg.id_language="+TNS.AdExpress.Constantes.DB.Language.FRENCH;

			// Activation
			sql+=" and su.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;	
			sql+=" and gp.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;	
			sql+=" and sg.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;	

			// Jointure
			sql+=" and su.id_subsector=gp.id_subsector";
			sql+=" and gp.id_group_=sg.id_group_";
						
			
			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// sector
			if(sectorAccess.Length>0){
				if(beginByAnd) sql+=" and";
				sql+=" ((su.id_sector in ("+sectorAccess+") ";
				premier=false;
			}
			// group
			if(groupAccess.Length>0){
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" gp.id_group_ in ("+groupAccess+") ";
				premier=false;
			}
			// segment
			if(segmentAccess.Length>0) {
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" sg.id_segment in ("+segmentAccess+") ";
				premier=false;
			}
			if(!premier) sql+=" )";

			// Droits en exclusion
			// sector
			if(sectorException.Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" su.id_sector not in ("+sectorException+") ";
				premier=false;
			}
			// group
			if(groupException.Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" gp.id_group_ not in ("+groupException+") ";
				premier=false;
			}
			// segment
			if(segmentException.Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" sg.id_segment not in ("+segmentException+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			#endregion

			#region Execution de la requête
			try{
				DataSet ds = webSession.Source.Fill(sql);
				if(ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
					foreach(DataRow current in ds.Tables[0].Rows){
						listSectorForRecap += current[0].ToString() +",";
					}
					if(listSectorForRecap.Length>0){
						listSectorForRecap=listSectorForRecap.Substring(0,listSectorForRecap.Length-1);
					}
				}
			}
			catch(System.Exception err){
				throw (new ProductClassificationListDataAccessException("Impossible d'obtenir la liste des id_sector en accès à partir d'un treeNode",err));
			}
			#endregion			

			return (listSectorForRecap);
		}

		/// <summary>
		/// Retourne la liste des id_sector en accès à partir d'un treeNode
		/// </summary>
		/// <param name="tree">arbre</param>
		/// <param name="webSession">Session web</param>
		/// <returns></returns>
		public static string ListSectorInUniverse(WebSession webSession, AdExpressUniverse adExpressUniverse) {

			#region Variables
			string sql = "";
			bool premier = true;
			string sectorAccess = "";
			string sectorException = "";
			string groupAccess = "";
			string groupException = "";
			string segmentAccess = "";
			string segmentException = "";
			string listSectorForRecap = "";
			List<NomenclatureElementsGroup> groups = null;
			#endregion


			#region Construction de la requête
			if (adExpressUniverse != null && adExpressUniverse.Count() > 0) {
				groups = adExpressUniverse.GetExludes();
				if (groups != null && groups.Count>0 ) {
					sectorException = groups[0].GetAsString(TNSClassificationLevels.SECTOR);
					groupException = groups[0].GetAsString(TNSClassificationLevels.GROUP_);
					segmentException = groups[0].GetAsString(TNSClassificationLevels.SEGMENT);
				}
				groups = adExpressUniverse.GetIncludes();
				if (groups != null && groups.Count > 0) {
					sectorAccess = groups[0].GetAsString(TNSClassificationLevels.SECTOR);
					groupAccess = groups[0].GetAsString(TNSClassificationLevels.GROUP_);
					segmentAccess = groups[0].GetAsString(TNSClassificationLevels.SEGMENT);
				}
			}

			//Requête SQL
			sql = "Select distinct su.id_sector from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".subsector su," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".group_  gp," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".segment sg ";
			sql += " where";
			// Langue
			sql += " su.id_language=" + TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql += " and gp.id_language=" + TNS.AdExpress.Constantes.DB.Language.FRENCH;
			sql += " and sg.id_language=" + TNS.AdExpress.Constantes.DB.Language.FRENCH;

			// Activation
			sql += " and su.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql += " and gp.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql += " and sg.activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;

			// Jointure
			sql += " and su.id_subsector=gp.id_subsector";
			sql += " and gp.id_group_=sg.id_group_";


			premier = true;
			bool beginByAnd = true;
			// le bloc doit il commencer par AND
			// sector
			if (sectorAccess!=null && sectorAccess.Length > 0) {
				if (beginByAnd) sql += " and";
				sql += " ((su.id_sector in (" + sectorAccess + ") ";
				premier = false;
			}
			// group
			if (groupAccess !=null && groupAccess.Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " gp.id_group_ in (" + groupAccess + ") ";
				premier = false;
			}
			// segment
			if (segmentAccess!=null && segmentAccess.Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " sg.id_segment in (" + segmentAccess + ") ";
				premier = false;
			}
			if (!premier) sql += " )";

			// Droits en exclusion
			// sector
			if (sectorException != null && sectorException.Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " su.id_sector not in (" + sectorException + ") ";
				premier = false;
			}
			// group
			if (groupException != null && groupException.Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " gp.id_group_ not in (" + groupException + ") ";
				premier = false;
			}
			// segment
			if (segmentException !=null && segmentException.Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " sg.id_segment not in (" + segmentException + ") ";
				premier = false;
			}
			if (!premier) sql += " )";
			#endregion

			#region Execution de la requête
			try {
				DataSet ds = webSession.Source.Fill(sql);
				if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) {
					foreach (DataRow current in ds.Tables[0].Rows) {
						listSectorForRecap += current[0].ToString() + ",";
					}
					if (listSectorForRecap.Length > 0) {
						listSectorForRecap = listSectorForRecap.Substring(0, listSectorForRecap.Length - 1);
					}
				}
			}
			catch (System.Exception err) {
				throw (new ProductClassificationListDataAccessException("Impossible d'obtenir la liste des id_sector en accès à partir d'un treeNode", err));
			}
			#endregion

			return (listSectorForRecap);
		}
		#endregion

		#region Retourne la liste des id_sector sélectionnés et leur noms
		/// <summary>
		/// retourne la liste des familles sélectionnées
		/// </summary>
		/// <param name="webSession"> session client</param>
		/// <returns>Liste des familles sélectionnées</returns>
		public static DataSet GetListSectorSelected(WebSession webSession){
			//string listSector =webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);									
			string listSector = webSession.PrincipalProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.SECTOR);									
			return ProductClassificationListDataAccess.SectorList(webSession,listSector);			
		}
		

		#endregion

	}
}
