#region Informations
// Auteur: G. Facon
// Date de création: 14/06/2004
// Date de modification: 15/06/2004
#endregion

using System;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Description résumée de VehicleDetailDataAccess.
	/// </summary>
	public class VehicleDetailDataAccess{

		/// <summary>
		/// Ne fait rien, ne pas utiliser
		/// </summary>
		/// <param name="vehicle">Vehicle à traiter</param>
		/// <param name="idMedia">Identifiant du support</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="webSession">Session du client</param>
		public static DataSet GetData(DBClassificationConstantes.Vehicles.names vehicle,Int64 idMedia,int dateBegin,int dateEnd,WebSession webSession){
			switch(vehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					break;
				case DBClassificationConstantes.Vehicles.names.radio:
					break;
				case DBClassificationConstantes.Vehicles.names.tv:
					break;
				default:
					throw(new VehicleDetailDataAccessException("Le vehicle sélectionné n'est pas géré"));
			}
			return(null);
		}

		/// <summary>
		/// Ne fait rien, ne pas utiliser
		/// </summary>
		/// <param name="idMedia">Identifiant du média</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>NULL</returns>
		private string EndRequest(Int64 idMedia,int dateBegin,int dateEnd,WebSession webSession){
			return(null);
		}

		/// <summary>
		/// Ne fait rien, ne pas utiliser
		/// </summary>
		/// <param name="idMedia">Identifiant du média</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="webSession">Session du client</param>
		/// <returns>Null</returns>
		private DataSet GetPressData(Int64 idMedia,int dateBegin,int dateEnd,WebSession webSession){
			string list="";
			string idLanguageString=webSession.SiteLanguage.ToString();
			#region Construction de la requête
			bool premier=true;
			string sql="select sc.sector,gr.group_,hc.holding_company,ad.advertiser,pr.product,";
			sql+=" wp.date_media_num, wp.visual,wp.media_paging,";
			sql+=" fr.format,wp.area_page,wp.area_color_page,wp.expenditure_euro";
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".sector sc";
			sql+="      "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".group_ gr";
			sql+="      "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".holding_company hc";
			sql+="      "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".advertiser ad";
			sql+="      "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr";
			sql+="      "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_PRESS+" wp";
			sql+=" Where";
			// Langues
			sql+=" sc.id_language="+idLanguageString;
			sql+=" and gr.id_language="+idLanguageString;
			sql+=" and hc.id_language="+idLanguageString;
			sql+=" and ad.id_language="+idLanguageString;
			sql+=" and pr.id_language="+idLanguageString;
			// Activation 
			sql+=" sc.id_language="+idLanguageString;
			sql+=" and gr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and hc.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and ad.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			sql+=" and pr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			// Jointures
			sql+=" and sc.id_sector=wp.id_sector";
			sql+=" and gr.id_group_=wp.id_group_";
			sql+=" and hc.id_holding_company=wp.id_holding_company";
			sql+=" and ad.id_advertiser=wp.id_advertiser";
			sql+=" and pr.id_product=wp.id_product";
			// Date
			sql+=" and date_num_media >="+dateBegin.ToString();
			sql+=" and date_num_media <="+dateEnd.ToString();
			
			// Gestion des sélections et des droits

			#region Nomenclature Produit (droits)
			premier=true;
			//Droits en accès
			sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,"wp",true);
			#endregion

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 

			#region Sélection
			// Sélection en accès
			premier=true;
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyAccess);
			if(list.Length>0){
				sql+=" and ((wp.id_holding_company in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_advertiser in ("+list+") ";
				premier=false;
			}
			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
			if(list.Length>0){
				if(!premier) sql+=" or";
				else sql+=" and ((";
				sql+=" wp.id_product in ("+list+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			
			// Sélection en Exception
			// HoldingCompany
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.holdingCompanyException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_holding_company not in ("+list+") ";
				premier=false;
			}
			// Advertiser
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_advertiser not in ("+list+") ";
				premier=false;
			}
			// Product
			list=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productException);
			if(list.Length>0){
				if(premier) sql+=" and (";
				else sql+=" and";
				sql+=" wp.id_product not in ("+list+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			#endregion

			#endregion

			#region Nomenclature Media (droits et sélection)

			#region Droits
			sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,"wp",true);
			#endregion

			#region Sélection
			sql+=" and ((wp.id_media="+idMedia.ToString()+")) ";
			#endregion

			#endregion
	
	
			#endregion
			return(null);
		}
	}
}
