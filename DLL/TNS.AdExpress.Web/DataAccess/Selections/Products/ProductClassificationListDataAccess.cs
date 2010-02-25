#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 26/08/2004 
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
using TablesDBConstantes=TNS.AdExpress.Constantes.DB.Tables;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Exceptions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.DataAccess.Selections;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
namespace TNS.AdExpress.Web.DataAccess.Selections.Products{
	/// <summary>
	/// Description r�sum�e de ProductClassificationList.
	/// </summary>
	public class ProductClassificationListDataAccess{		

		#region liste de Famille en fonction des droits du client
		/// <summary>
		///Donne une liste de Famille en fonction des droits du client. 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Liste des familles autoris�es au client.</returns>
		public static DataSet SectorList(WebSession webSession){
				return SectorList(webSession,"");
		}

		/// <summary>
		///Donne une liste de Famille en fonction des droits du client. 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idSector">identifiant(s) de(s) famille(s)</param>
		/// <returns>Liste des familles autoris�es au client.</returns>
		public static DataSet SectorList(WebSession webSession,string idSector){
            
            TNS.AdExpress.Domain.DataBaseDescription.View oView = WebApplicationParameters.DataBaseDescription.GetView(ViewIds.allProduct);
			
            #region Construction de la requ�te
			string sql="";
            sql += " select distinct " + oView.Prefix + ".id_sector," + oView.Prefix + ".sector";
            sql += " from " + oView.Sql + webSession.DataLanguage.ToString() + " "+ oView.Prefix;
          
			sql+=" Where 0=0 ";
										
			if(CheckedText.IsStringEmpty(idSector)){
                sql += " and " + oView.Prefix + ".id_sector in (" + idSector + ") ";	
			}
			#region Nomenclature Produit (droits)
			//Droits en acc�s
            sql += SQLGenerator.getClassificationCustomerProductRight(webSession, oView.Prefix, oView.Prefix, oView.Prefix, oView.Prefix, true);
			#endregion
			
			// Tri
            sql += " order by " + oView.Prefix + ".sector";

			#endregion

			#region Execution de la requ�te
			try{
				return(webSession.Source.Fill(sql));
			}
			catch(System.Exception err){
				throw (new ProductClassificationListDataAccessException("Impossible d'obtenir une liste de Famille en fonction des droits du client",err));
			}
			#endregion

		}
		#endregion	

		#region Retourne la liste des id_sector � partir d'un treeNode
		/// <summary>
		/// Retourne la liste des id_sector en acc�s � partir d'un treeNode
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

			#region Construction de la requ�te
			
			sectorAccess=webSession.GetSelection(tree,CustomerRightConstante.type.sectorAccess);
			sectorException=webSession.GetSelection(tree,CustomerRightConstante.type.sectorException);
			groupAccess=webSession.GetSelection(tree,CustomerRightConstante.type.groupAccess);
			groupException=webSession.GetSelection(tree,CustomerRightConstante.type.groupException);
			segmentAccess=webSession.GetSelection(tree,CustomerRightConstante.type.segmentAccess);
			segmentException=webSession.GetSelection(tree,CustomerRightConstante.type.segmentException);

			//Requ�te SQL
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

			#region Execution de la requ�te
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
				throw (new ProductClassificationListDataAccessException("Impossible d'obtenir la liste des id_sector en acc�s � partir d'un treeNode",err));
			}
			#endregion			

			return (listSectorForRecap);
		}

		/// <summary>
		/// Retourne la liste des id_sector en acc�s � partir d'un treeNode
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


			#region Construction de la requ�te
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

			//Requ�te SQL
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

			#region Execution de la requ�te
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
				throw (new ProductClassificationListDataAccessException("Impossible d'obtenir la liste des id_sector en acc�s � partir d'un treeNode", err));
			}
			#endregion

			return (listSectorForRecap);
		}
		#endregion

		#region Retourne la liste des id_sector s�lectionn�s et leur noms
		/// <summary>
		/// retourne la liste des familles s�lectionn�es
		/// </summary>
		/// <param name="webSession"> session client</param>
		/// <returns>Liste des familles s�lectionn�es</returns>
		public static DataSet GetListSectorSelected(WebSession webSession){
			//string listSector =webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);									
			string listSector = webSession.PrincipalProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.SECTOR);									
			return ProductClassificationListDataAccess.SectorList(webSession,listSector);			
		}
		

		#endregion

	}
}
