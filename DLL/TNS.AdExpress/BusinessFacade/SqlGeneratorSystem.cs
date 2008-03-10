#region Information
// Auteur G. Facon, B. Masson
// Date de cr�ation: 12/04/2005
// Date de modification: 12/04/2005

#endregion

using System;
using AdexpressExceptions = TNS.AdExpress.Exceptions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.BusinessFacade{
	/// <summary>
	/// Description r�sum�e de SqlGenerator.
	/// </summary>
	public class SqlGeneratorSystem{

		#region Droits

		#region Supports
		/// <summary>
		/// G�n�re les droits clients M�dia.
		/// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature support.
		/// </summary>
		/// <param name="customerLogin">Droit du client</param>
		/// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL g�n�r�</returns>
		public static string GetMediaRight(TNS.AdExpress.Right customerLogin,string tablePrefixe,bool beginByAnd){
			string sql="";
			bool premier=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(customerLogin[CustomerRightConstante.type.vehicleAccess].Length>0){
				if(beginByAnd) sql+=" and";
				sql+=" (("+tablePrefixe+".id_vehicle in ("+customerLogin[CustomerRightConstante.type.vehicleAccess]+") ";
				premier=false;
			}
			// Category
			if(customerLogin[CustomerRightConstante.type.categoryAccess].Length>0){
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" "+tablePrefixe+".id_category in ("+customerLogin[CustomerRightConstante.type.categoryAccess]+") ";
				premier=false;
			}
			// Media
			if(customerLogin[CustomerRightConstante.type.mediaAccess].Length>0){
				if(!premier) sql+=" or";
				else{
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" "+tablePrefixe+".id_media in ("+customerLogin[CustomerRightConstante.type.mediaAccess]+") ";
				premier=false;
			}
			if(!premier) sql+=" )";

			// Droits en exclusion
			// Vehicle
			if(customerLogin[CustomerRightConstante.type.vehicleException].Length>0){
				if(!premier) sql+=" and";
				else{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+tablePrefixe+".id_vehicle not in ("+customerLogin[CustomerRightConstante.type.vehicleException]+") ";
				premier=false;
			}
			// Category
			if(customerLogin[CustomerRightConstante.type.categoryException].Length>0){
				if(!premier) sql+=" and";
				else{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+tablePrefixe+".id_category not in ("+customerLogin[CustomerRightConstante.type.categoryException]+") ";
				premier=false;
			}
			// Media
			if(customerLogin[CustomerRightConstante.type.mediaException].Length>0){
				if(!premier) sql+=" and";
				else{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+tablePrefixe+".id_media not in ("+customerLogin[CustomerRightConstante.type.mediaException]+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			return(sql);
		}

		#endregion

		#region Produits

		/// <summary>
		/// G�n�re les droits clients Produit.
		/// Cette fonction est � utiliser si une m�me table contient tous les identifiants de la nomenclature produit.
		/// </summary>		
		/// <param name="tablePrefixe">Pr�fixe de la table qui contient les donn�es</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <param name="customerLogin">Droit client</param>
		/// <returns>Code SQL g�n�r�</returns>
		public static string GetProductRight(TNS.AdExpress.Right customerLogin,string tablePrefixe,bool beginByAnd){
			return(GetProductRight(customerLogin,tablePrefixe,tablePrefixe,tablePrefixe,tablePrefixe,beginByAnd));
		}


		/// <summary>
		/// G�n�re les droits clients Produit.
		/// Cette fonction est � utiliser si la nomenclature est � plat dans la requ�te.
		/// Les noms des tables sont:
		///    - sector: sc
		///    - subsector: ss
		///    - group_:gr
		///    - segment: sg
		/// Les pr�fixes sont d�finis dans TNS.AdExpress.Constantes.DB.Tables
		/// </summary>		
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <param name="customerLogin">Droit client</param>
		/// <returns>Code SQL g�n�r�</returns>
        public static string GetProductRight(TNS.AdExpress.Right customerLogin,bool beginByAnd) {
			return(GetProductRight(customerLogin,DBConstantes.Tables.SECTOR_PREFIXE,DBConstantes.Tables.SUBSECTOR_PREFIXE,DBConstantes.Tables.GROUP_PREFIXE,DBConstantes.Tables.SEGMENT_PREFIXE,beginByAnd));
		}
		

		/// <summary>
		/// G�n�re les droits clients Produit.
		/// </summary>		
		/// <param name="sectorPrefixe">Pr�fixe de la table sector</param>
		/// <param name="subsectorPrefixe">Pr�fixe de la table subsector</param>
		/// <param name="groupPrefixe">Pr�fixe de la table group_</param>
		/// <param name="segmentPrefixe">Pr�fixe de la table segment</param>
		/// <param name="customerLogin">Droit client</param>
		/// <param name="beginByAnd">True si le bloc doit commencer par un AND, false sinon</param>
		/// <returns>Code SQL g�n�r�</returns>
		public static string GetProductRight(TNS.AdExpress.Right customerLogin,string sectorPrefixe,string subsectorPrefixe,string groupPrefixe,string segmentPrefixe,bool beginByAnd){
			string sql="";
			bool premier=true;
			// Sector (Famille)
			if(customerLogin[CustomerRightConstante.type.sectorAccess].Length>0){
				if(beginByAnd) sql+=" and";
				sql+=" (("+sectorPrefixe+".id_sector in ("+customerLogin[CustomerRightConstante.type.sectorAccess]+") ";
				premier=false;
			}
			// SubSector (Classe)
			if(customerLogin[CustomerRightConstante.type.subSectorAccess].Length>0){
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" "+subsectorPrefixe+".id_subsector in ("+customerLogin[CustomerRightConstante.type.subSectorAccess]+") ";
				premier=false;
			}
			// Group (Groupe)
			if(customerLogin[CustomerRightConstante.type.groupAccess].Length>0){
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" "+groupPrefixe+".id_group_ in ("+customerLogin[CustomerRightConstante.type.groupAccess]+") ";
				premier=false;
			}
			// Segment (Vari�t�)
			if(customerLogin[CustomerRightConstante.type.segmentAccess].Length>0){
				if(!premier) sql+=" or";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" "+segmentPrefixe+".id_segment in ("+customerLogin[CustomerRightConstante.type.segmentAccess]+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			// Droits en exclusion
			// Sector (Famille)
			if(customerLogin[CustomerRightConstante.type.sectorException].Length>0){
				if(!premier) sql+=" and";
				else{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+sectorPrefixe+".id_sector not in ("+customerLogin[CustomerRightConstante.type.sectorException]+") ";
				premier=false;
			}
			// SubSector (Classe)
			if(customerLogin[CustomerRightConstante.type.subSectorException].Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+subsectorPrefixe+".id_subsector not in ("+customerLogin[CustomerRightConstante.type.subSectorException]+") ";
				premier=false;
			}
			// Group (Groupe)
			if(customerLogin[CustomerRightConstante.type.groupException].Length>0){
				if(!premier) sql+=" and";
				else{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+groupPrefixe+".id_group_  not in ("+customerLogin[CustomerRightConstante.type.groupException]+") ";
				premier=false;
			}
			// Segment (Vari�t�)
			if(customerLogin[CustomerRightConstante.type.segmentException].Length>0){
				if(!premier) sql+=" and";
				else {
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+segmentPrefixe+".id_segment  not in ("+customerLogin[CustomerRightConstante.type.segmentException]+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			return(sql);
		}


		#endregion

		#endregion

		#region Description Base de donn�es

		#region Media

		#region Tables
		/// <summary>
		/// D�termine la table � utiliser en fonction du vehicle
		/// </summary>
		/// <param name="vehicleName">Vehicle</param>
		/// <returns>Nom de la table</returns>
		public static string GetVehicleTableNameForDetail(DBClassificationConstantes.Vehicles.names vehicleName){
			switch(vehicleName){
				case DBClassificationConstantes.Vehicles.names.press:
					return(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_PRESS);
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					return(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_PRESS_INTER);
				case DBClassificationConstantes.Vehicles.names.radio:
					return(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_RADIO);
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return(DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_TV);
				default:
					throw(new AdexpressExceptions.SqlGeneratorSystemException("Impossible de d�terminer la table Vehicle � utiliser"));
			}
		}
		#endregion

		#endregion

		#endregion

	}
}
