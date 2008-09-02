#region Informations
// Author: K. Shehzad
// Date of creation: 02/08/2005 
// Date of modification:
#endregion
using System;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Translation;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using SQLFunctions=TNS.AdExpress.Web.DataAccess;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Web.Rules.Results.APPM
{
	/// <summary>
	/// This class is used to format the data for constructing PDV Plan table and charts.
	/// </summary>
	public class PDVPlanRules
	{
		#region Rules for Table
		/// <summary>
		/// Formats the date for APPM PDV Plan Table 
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <param name="graphics">Boolean that indicates whether the rules are to be constructed for graphics or table</param>
		/// <returns>Formatted table for the PDV Analysis table</returns>		
		public static DataTable GetData(WebSession webSession,IDataSource dataSource,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget,bool graphics)
		{
			#region  variables
			DataTable refUnivData=null;			
			DataTable conUniversData=null;
			DataTable groupData=null;
			DataTable resultPDVPlan=null;
			DataRow resultRow=null;
			string idGroups=string.Empty;
			string oldGroup=string.Empty;
			Int64 refEuros=0;
			Int64 refInsertions=0;
			double refPages=0;
			double refGRP=0;
			Int64 compGroupEuros=0;
			Int64 compGroupInsertions=0;
			double compGroupPages=0;
			double compGroupGRP=0;
			double compGroupGRPBaseTarget=0;
			double refGRPBaseTarget=0;
			string nameRefUnivers=string.Empty;
			
			#endregion

			try{
			
					#region Get Data for Reference univers
				//charging the reference univers
				//webSession.CurrentUniversAdvertiser=(TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;
				
				//getting the reference univers data
				refUnivData=TNS.AdExpress.Web.DataAccess.Results.APPM.PDVPlanDataAccess.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,false).Tables[0];
				
				
				
				if(	refUnivData!=null && refUnivData.Rows.Count>0)
				{
					//Getting data from the reference univers
					foreach(DataRow dr in refUnivData.Rows){
						//Base target Values
						if(Convert.ToInt64(dr["id_target"])==baseTarget){
                            refGRPBaseTarget += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);
						}
						//Supplementary target values
						else{
                            refEuros += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()]);
                            refPages += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);
                            refInsertions += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()]);
                            refGRP += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);
						}
					}
					
					#endregion

					#region Get Data for Competitor Univers or Groups
					//if competitor univers is selected
					if(webSession.PrincipalProductUniverses.Count>1){
						//charging the competitor univers
						//webSession.CurrentUniversAdvertiser=(TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[2]).TreeCompetitorAdvertiser;
						conUniversData=TNS.AdExpress.Web.DataAccess.Results.APPM.PDVPlanDataAccess.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,true).Tables[0];
												

						if(conUniversData.Rows.Count>0)
						{
							foreach(DataRow dr in conUniversData.Rows){
								//Base target Values
								if(Convert.ToInt64(dr["id_target"])==baseTarget){
                                    compGroupGRPBaseTarget += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);
								}
								//Supplementary target values
								else{
                                    compGroupEuros += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()]);
                                    compGroupPages += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);
                                    compGroupInsertions += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()]);
                                    compGroupGRP += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);
								}
							}
						}
//						compGroupPages=Math.Round((compGroupPages/1000),3);
					
						//we do the sum of refernece and competitor univers
						compGroupEuros=compGroupEuros+refEuros;
						compGroupPages=compGroupPages+refPages;
						compGroupInsertions=compGroupInsertions+refInsertions;
						compGroupGRP=compGroupGRP+refGRP;
						compGroupGRPBaseTarget=compGroupGRPBaseTarget+refGRPBaseTarget;
					}
						//if competitor univers is not selected we use reference groups
					else{
						//The following function returns the Distinct group ids of the products being selected 
						DataTable groups=SQLFunctions.Functions.SelectDistinct("groups",refUnivData,"id_group_");
						foreach(DataRow dr in groups.Rows){
							idGroups+=dr["id_group_"].ToString()+",";
						}					
						if(!idGroups.Equals(""))
							idGroups=idGroups.Remove(idGroups.Length-1,1);
						//Getting data for the groups of products
						groupData=TNS.AdExpress.Web.DataAccess.Results.APPM.PDVPlanDataAccess.UniversgroupInvestment(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget,idGroups).Tables[0];
						if(groupData.Rows.Count>0){
							foreach(DataRow dr in groupData.Rows){
								//Base target Values
								if(Convert.ToInt64(dr["id_target"])==baseTarget){
                                    compGroupGRPBaseTarget += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);
								}
								//Supplementary target values
								else{
                                    compGroupEuros += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()]);
                                    compGroupPages += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);
                                    compGroupInsertions += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()]);
                                    compGroupGRP += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);
								}
							}							
						}
					}
					#endregion

					#region Resultant Table
					resultPDVPlan=new DataTable();
				
					#region colunms
					//Creating colunms for the new table
					resultPDVPlan.Columns.Add("products",System.Type.GetType("System.String"));
					resultPDVPlan.Columns.Add("euros",System.Type.GetType("System.String"));
					resultPDVPlan.Columns.Add("pages",System.Type.GetType("System.String"));
					resultPDVPlan.Columns.Add("insertions",System.Type.GetType("System.String"));
					resultPDVPlan.Columns.Add("GRP",System.Type.GetType("System.String"));
					resultPDVPlan.Columns.Add("GRPBaseTarget",System.Type.GetType("System.String"));
					#endregion
				
					#region Total
					//Results for the total row which is either the sum of competitor+reference univers or groups of products selected.
					resultRow=resultPDVPlan.NewRow();
					resultRow["products"]=GestionWeb.GetWebWord(1401,webSession.SiteLanguage);
					resultRow["euros"]=compGroupEuros;
					resultRow["pages"]=compGroupPages;
					resultRow["insertions"]=compGroupInsertions;
					resultRow["GRP"]=compGroupGRP;
					resultRow["GRPBaseTarget"]=compGroupGRPBaseTarget;
					resultPDVPlan.Rows.Add(resultRow);
					#endregion

					#region Reference Univers
					//Sum of Reference univers 
					resultRow=resultPDVPlan.NewRow();
					if (webSession.PrincipalProductUniverses.Count > 0)
						nameRefUnivers = webSession.PrincipalProductUniverses[0].Label;	
					//nameRefUnivers=(string)(((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[1]).NameCompetitorAdvertiser);
					if(nameRefUnivers.Length<=0)
						nameRefUnivers=GestionWeb.GetWebWord(1734,webSession.SiteLanguage);
					resultRow["products"]=nameRefUnivers;
					resultRow["euros"]=refEuros;
					resultRow["pages"]=refPages;
					resultRow["insertions"]=refInsertions;
					resultRow["GRP"]=refGRP;
					resultRow["GRPBaseTarget"]=refGRPBaseTarget;
					resultPDVPlan.Rows.Add(resultRow);
					#endregion

					#region PDV
					//PDV of the reference univers with respect to the total. We dont need it in case of graphics
					if(!graphics){
						resultRow=resultPDVPlan.NewRow();
						resultRow["products"]=GestionWeb.GetWebWord(1167,webSession.SiteLanguage);
						if(Convert.ToDouble(compGroupEuros)!=0)
							resultRow["euros"]=Math.Round(Convert.ToDouble(refEuros)/Convert.ToDouble(compGroupEuros)*100,2);
						if(Convert.ToDouble(compGroupPages)!=0)
							resultRow["pages"]=Math.Round(Convert.ToDouble(refPages)/Convert.ToDouble(compGroupPages)*100,2);
						if(Convert.ToDouble(compGroupInsertions)!=0)
							resultRow["insertions"]=Math.Round(Convert.ToDouble(refInsertions)/Convert.ToDouble(compGroupInsertions)*100,2);
						if(Convert.ToDouble(compGroupGRP)!=0)
							resultRow["GRP"]=Math.Round((refGRP/compGroupGRP)*100,2);
						if(Convert.ToDouble(compGroupGRPBaseTarget)!=0)
							resultRow["GRPBaseTarget"]=Math.Round((refGRPBaseTarget/compGroupGRPBaseTarget)*100,2);
						resultPDVPlan.Rows.Add(resultRow);
					}
					#endregion

					#region competitor univers products
					//The products of the competitor univers. We dont need it in case of graphics.
					//As we need to show GRP for both base target as well as supplementary target which are at different lines
					//in the conUniversData DataTable we need an algo which will add one line in the resultant table "resultPDVPlan"
					//by processing two lines from the "conUniversData".
					if(!graphics){
						bool newRow=true;
						if(webSession.PrincipalProductUniverses.Count>1 && conUniversData.Rows.Count>1){
							foreach(DataRow dr in conUniversData.Rows){						
								if(newRow){
									resultRow=resultPDVPlan.NewRow();
									newRow=false;
								}
								else{
									resultPDVPlan.Rows.Add(resultRow);
									newRow=true;
								}
								if(Convert.ToInt64(dr["id_target"])==baseTarget){
                                    resultRow["GRPBaseTarget"] = Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);
								}
								else{
									resultRow["products"]=dr["product"].ToString();
                                    resultRow["euros"] = Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()]);
                                    resultRow["pages"] = Convert.ToDecimal(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);
                                    resultRow["insertions"] = Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()]);
                                    resultRow["GRP"] = Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString()]);							
								}						
							}
						}
					}
					#endregion
				
					#endregion

				}

			}
			catch(System.Exception err){
				throw(new WebExceptions.PDVPlanRulesException("Error while formatting the data of PDV Plan in APPM ",err));
			}
			return resultPDVPlan;
		}
		#endregion

		#region Rules for Graphics
		/// <summary>
		/// Formats the data for APPM PDV Plan Charts 
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dataSource">dataSource for creating Datasets </param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="additionalTarget">additional target</param>
		/// <returns>Formatted table for the PDV Analysis Graph</returns>		
		public static DataTable GetGraphicsData(WebSession webSession,IDataSource dataSource,int dateBegin,int dateEnd,Int64 additionalTarget)
		{
			#region variables
			DataTable graphicsData=null;
			string advertisersSelected=string.Empty;
			string productsSelected=string.Empty;
			string brandsSelected=string.Empty;
			string expression=string.Empty; 
			//DataRow[] foundRows=null;
			DataTable graphicsTable=null;
			DataRow resultRow=null;
			string[] elements=null;
			string elementName=string.Empty;
			double euros=0;
			double pages=0;
			double insertions=0;
			double grp=0;
			bool dataExists=false;
			List<long> oldIdItems = null;
			#endregion

			#region setting current Univers
			//setting current univers to reference univers
			//webSession.CurrentUniversAdvertiser=(TreeNode)((TNS.AdExpress.Web.Core.Sessions.CompetitorAdvertiser)webSession.CompetitorUniversAdvertiser[1]).TreeCompetitorAdvertiser;
			#endregion

			#region get Data
			graphicsData=TNS.AdExpress.Web.DataAccess.Results.APPM.PDVPlanDataAccess.GetGraphicsData(webSession,dataSource,dateBegin,dateEnd,additionalTarget).Tables[0];
			#endregion
			
			try{
				#region construction of the graphics table
				if(graphicsData!=null && graphicsData.Rows.Count>0){

					#region Elements selected by client
					////Advertisers
					////advertisersSelected=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.advertiserAccess);
					//if(webSession.PrincipalProductUniverses.Count>0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER,AccessType.includes))
					//advertisersSelected= webSession.PrincipalProductUniverses[0].GetLevel(TNSClassificationLevels.ADVERTISER,AccessType.includes); 
					////Products
					////productsSelected=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.productAccess);
					//if(webSession.PrincipalProductUniverses.Count>0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT,AccessType.includes))
					//productsSelected = webSession.PrincipalProductUniverses[0].GetLevel(TNSClassificationLevels.PRODUCT,AccessType.includes); 
					////Brands
					////brandsSelected=webSession.GetSelection(webSession.CurrentUniversAdvertiser,CustomerRightConstante.type.brandAccess);
					//if(webSession.PrincipalProductUniverses.Count>0 && webSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND,AccessType.includes))
					//brandsSelected = webSession.PrincipalProductUniverses[0].GetLevel(TNSClassificationLevels.BRAND,AccessType.includes); 
					#endregion

					#region colunms for table
					//Creating the new table
					graphicsTable=new DataTable();
					graphicsTable.Columns.Add("elementType",System.Type.GetType("System.String"));
					graphicsTable.Columns.Add("elements",System.Type.GetType("System.String"));
					graphicsTable.Columns.Add("euros",System.Type.GetType("System.Double"));
					graphicsTable.Columns.Add("pages",System.Type.GetType("System.Double"));
					graphicsTable.Columns.Add("insertions",System.Type.GetType("System.Double"));
					graphicsTable.Columns.Add("GRP",System.Type.GetType("System.Double"));

					#endregion

					#region Advertisers
					//if the advertisers were selected
					if ((graphicsData != null && graphicsData.Rows.Count > 0 && graphicsData.Columns.Contains("id_advertiser"))) {
						//Ids of the advertisers are added in the elements array
						//elements=advertisersSelected.Split(',');
						//So for each advertiser we search it in the graphicsData table and calculates its values
						//foreach(string element in elements){
						 oldIdItems = new List<long>();

						foreach (DataRow dr in graphicsData.Rows) {
							
							if (!oldIdItems.Contains(Convert.ToInt64(dr["id_advertiser"].ToString()))) {
								elementName = dr["advertiser"].ToString();
                                euros = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString() + ")", "id_advertiser = " + dr["id_advertiser"].ToString() + "").ToString());
                                insertions = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString() + ")", "id_advertiser = " + dr["id_advertiser"].ToString() + "").ToString());
                                pages = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString() + ")", "id_advertiser = " + dr["id_advertiser"].ToString() + "").ToString());
                                grp = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString() + ")", "id_advertiser = " + dr["id_advertiser"].ToString() + "").ToString());

								//if the data exists for the advertiser we add it to the new table "graphicsTable"
								resultRow = graphicsTable.NewRow();
								resultRow["elementType"] = GestionWeb.GetWebWord(857, webSession.SiteLanguage);
								resultRow["elements"] = elementName;
								resultRow["euros"] = euros;
								resultRow["pages"] = (decimal)pages;
								resultRow["insertions"] = insertions;
								resultRow["GRP"] = grp;
								graphicsTable.Rows.Add(resultRow);

								//reinitializing the values
								elementName = "";
								euros = 0;
								pages = 0;
								insertions = 0;
								grp = 0;								

								oldIdItems.Add(Convert.ToInt64(dr["id_advertiser"].ToString()));
							}
						}
						
					}						

					//}
					#endregion				

					#region Brands
					//if the brands were selected
					if ((graphicsData != null && graphicsData.Rows.Count > 0 && graphicsData.Columns.Contains("id_brand"))) {
						//Ids of the brands are added in the elements array						
						//elements=brandsSelected.Split(',');
						//So for each brand we search it in the graphicsData table and calculates its values						

						oldIdItems = new List<long>();

						foreach (DataRow dr in graphicsData.Rows) {

							if (!oldIdItems.Contains(Convert.ToInt64(dr["id_brand"].ToString()))) {
								elementName = dr["brand"].ToString();
                                euros = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString() + ")", "id_brand = " + dr["id_brand"].ToString() + "").ToString());
                                insertions = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString() + ")", "id_brand = " + dr["id_brand"].ToString() + "").ToString());
                                pages = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString() + ")", "id_brand = " + dr["id_brand"].ToString() + "").ToString());
                                grp = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString() + ")", "id_brand = " + dr["id_brand"].ToString() + "").ToString());

								//if the data exists for the advertiser we add it to the new table "graphicsTable"
								resultRow = graphicsTable.NewRow();
								resultRow["elementType"] = GestionWeb.GetWebWord(1149, webSession.SiteLanguage);
								resultRow["elements"] = elementName;
								resultRow["euros"] = euros;
								resultRow["pages"] = (decimal)pages;
								resultRow["insertions"] = insertions;
								resultRow["GRP"] = grp;
								graphicsTable.Rows.Add(resultRow);

								//reinitializing the values
								elementName = "";
								euros = 0;
								pages = 0;
								insertions = 0;
								grp = 0;

								oldIdItems.Add(Convert.ToInt64(dr["id_brand"].ToString()));
							}
						}
					}
					

					#endregion

					#region Products
					//if the prodcuts were selected
					if ((graphicsData != null && graphicsData.Rows.Count > 0 && graphicsData.Columns.Contains("id_product"))) {//productsSelected.Length > 0 &&
						//Ids of the products are added in the elements array						
						//elements=productsSelected.Split(',');
						//So for each product we search it in the graphicsData table and calculates its values					
						oldIdItems = new List<long>();

						foreach (DataRow dr in graphicsData.Rows) {

							if (!oldIdItems.Contains(Convert.ToInt64(dr["id_product"].ToString()))) {
								elementName = dr["product"].ToString();
                                euros = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString() + ")", "id_product = " + dr["id_product"].ToString() + "").ToString());
                                insertions = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString() + ")", "id_product = " + dr["id_product"].ToString() + "").ToString());
                                pages = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString() + ")", "id_product = " + dr["id_product"].ToString() + "").ToString());
                                grp = Convert.ToDouble(graphicsData.Compute("Sum(" + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.grp].Id.ToString() + ")", "id_product = " + dr["id_product"].ToString() + "").ToString());

								//if the data exists for the advertiser we add it to the new table "graphicsTable"
								resultRow = graphicsTable.NewRow();
								resultRow["elementType"] = GestionWeb.GetWebWord(858, webSession.SiteLanguage);
								resultRow["elements"] = elementName;
								resultRow["euros"] = euros;
								resultRow["pages"] = (decimal)pages;
								resultRow["insertions"] = insertions;
								resultRow["GRP"] = grp;
								graphicsTable.Rows.Add(resultRow);

								//reinitializing the values
								elementName = "";
								euros = 0;
								pages = 0;
								insertions = 0;
								grp = 0;

								oldIdItems.Add(Convert.ToInt64(dr["id_product"].ToString()));
							}
						}

						//}
					}

					#endregion
					
				}
				#endregion
			}
			catch(System.Exception err){
				throw(new WebExceptions.PDVPlanRulesException("Error while formatting the data of PDV Plan Graph in APPM",err));
			}
			return graphicsTable;

		}
		#endregion

		#region Private Methods
		
		
		#endregion

	}
}
