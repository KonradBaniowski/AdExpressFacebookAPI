#region Information
// Author: D. Mussuma
// Creation date: 08/08/2008
// Modification date:
#endregion
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.FrameWork.Results;

using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;

using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpressI.Portofolio.Engines {
	/// <summary>
	/// Compute portofolio synthesis' results
	/// </summary>
	public class SynthesisEngine : Engine{

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicle">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
			: base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
		}

		#endregion
		
		#region Public methods

		#region Abstract methods implementation
		/// <summary>
		/// Get Result Table for portofolio synthesis
		/// </summary>
		/// <returns>ResultTable</returns>
		protected override ResultTable ComputeResultTable() {

			#region Constantes
			/// <summary>
			/// Header column index
			/// </summary>
			const int HEADER_COLUMN_INDEX = 0;
			/// <summary>
			/// First column index
			/// </summary>
			const int FIRST_COLUMN_INDEX = 1;
			/// <summary>
			/// Second column index
			/// </summary>
			const int SECOND_COLUMN_INDEX = 2;
			#endregion

			#region Variables
			string investment = "";
			string firstDate = "";
			string lastDate = "";
			string support = "";
			string periodicity = "";
			string category = "";
			string regie = "";
			string interestCenter = "";
			string pageNumber = "", adNumber = null;
			string ojd = "";
			string nbrSpot = "";
			string nbrEcran = "";
			decimal averageDurationEcran = 0;
			decimal nbrSpotByEcran = 0;
			string totalDuration = "";
			string numberBoard = "";
			string volume = "";
			ResultTable resultTable = null;
			LineType lineType = LineType.level1;
			string typeReseauStr = string.Empty;
			bool isAlertModule = _webSession.CustomerPeriodSelected.Is4M; //(_webSession.CurrentModule == WebCst.Module.Name.ALERTE_PORTEFEUILLE);
			string durationlabel = (isAlertModule) ? "duration" : "duree";
			DataTable dtPage = null;
			#endregion
			
			#region Accès aux tables
			if (_module.CountryDataAccessLayer == null) throw (new NullReferenceException("DAL layer is null for the portofolio result"));
			object[] parameters = new object[5];
			parameters[0] = _webSession;
			parameters[1] = _vehicleInformation;
			parameters[2] = _idMedia;
			parameters[3] = _periodBeginning;
			parameters[4] = _periodEnd;
			IPortofolioDAL portofolioDAL = (IPortofolioDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + _module.CountryDataAccessLayer.AssemblyName, _module.CountryDataAccessLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null, null);

			DataSet dsInvestment = portofolioDAL.GetSynthisData(PortofolioSynthesis.INVESTMENT_DATA);
			DataTable dtInvestment = dsInvestment.Tables[0];

			DataTable dtInsertionNumber = null;
			if (isAlertModule && _vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing) {
				DataSet dsInsertionNumber = portofolioDAL.GetSynthisData(PortofolioSynthesis.INSERTION_NUMBER_DATA);
				dtInsertionNumber = dsInsertionNumber.Tables[0];
			}
			DataSet dsCategory = portofolioDAL.GetSynthisData(PortofolioSynthesis.CATEGORY_MEDIA_SELLER_DATA);
			DataTable dtCategory = dsCategory.Tables[0];

			if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing) {
				DataSet dsPage = portofolioDAL.GetSynthisData(PortofolioSynthesis.NUMBER_PAGE_DATA);
				 dtPage = dsPage.Tables[0];
			}

			DataTable dtTypeSale = null;
			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor) {
				DataSet dsTypeSale = portofolioDAL.GetSynthisData(PortofolioSynthesis.TYPE_SALE_NUMBER_DATA);
				dtTypeSale = dsTypeSale.Tables[0];
			}

			object[] tab = portofolioDAL.GetNumber(PortofolioSynthesis.NUMBER_PRODUCT_ADVERTISER_DATA);
			object[] tabEncart = null;

			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
				tabEncart = portofolioDAL.GetNumber(PortofolioSynthesis.NUMBER_INSET_DATA);
			}
			#endregion

			#region For each table's row
			foreach (DataRow row in dtInvestment.Rows) {
				investment = row["investment"].ToString();
				if (isAlertModule) {
					firstDate = row["first_date"].ToString();
					lastDate = row["last_date"].ToString();

					if (dtInvestment.Columns.Contains("number_board")) {
						numberBoard = row["number_board"].ToString();
					}
				}
				if (dtInvestment.Columns.Contains(durationlabel)) {
					totalDuration = row[durationlabel].ToString();
				}
				if (dtInvestment.Columns.Contains("insertion")) nbrSpot = row["insertion"].ToString();
				//Volume
				if (dtInvestment.Columns.Contains("volume")) {
					if (row["volume"].ToString().Length > 0) {
						volume = Convert.ToString(Math.Round(decimal.Parse(row["volume"].ToString())));
						volume = WebFunctions.Units.ConvertUnitValueAndPdmToString(volume, WebCst.CustomerSessions.Unit.volume, false);
					}
					else volume = "0";
				}
				//Nb ad pages
				if (dtInvestment.Columns.Contains("page") && row["page"] != System.DBNull.Value)
					adNumber = row["page"].ToString();
			}
			//nombre d'insertions
			if (isAlertModule && dtInsertionNumber != null && !dtInsertionNumber.Equals(System.DBNull.Value) && dtInsertionNumber.Rows.Count > 0) {
				nbrSpot = dtInsertionNumber.Rows[0]["insertion"].ToString();
			}

			foreach (DataRow row in dtCategory.Rows) {
				support = row["support"].ToString();
				category = row["category"].ToString();
				regie = row["media_seller"].ToString();
				interestCenter = row["interest_center"].ToString();
				if (dtCategory.Columns.Contains("periodicity"))
					periodicity = row["periodicity"].ToString();
				if (dtCategory.Columns.Contains("ojd"))
					ojd = row["ojd"].ToString();
			}

			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
				foreach (DataRow row in dtPage.Rows) {
					pageNumber = row["page"].ToString();
				}
				if (pageNumber.Length == 0)
					pageNumber = "0";

				if (isAlertModule & tabEncart != null && tabEncart[0] != null)
					adNumber = tabEncart[0].ToString(); //Nb ad pages
			}

			if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
				&& isAlertModule) {
				DataSet dsEcran = portofolioDAL.GetEcranData();
				DataTable dtEcran = dsEcran.Tables[0];

				foreach (DataRow row in dtEcran.Rows) {
					nbrEcran = row["nbre_ecran"].ToString();
					if (row["nbre_ecran"] != System.DBNull.Value) {
						averageDurationEcran = decimal.Parse(row["ecran_duration"].ToString()) / decimal.Parse(row["nbre_ecran"].ToString());
						nbrSpotByEcran = decimal.Parse(row["nbre_spot"].ToString()) / decimal.Parse(row["nbre_ecran"].ToString());
					}
				}
			}
			#endregion

			#region Period
			DateTime dtFirstDate = DateTime.Today;
			DateTime dtLastDate = DateTime.Today;
			if (isAlertModule) {
				if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor) {
					if (firstDate.Length > 0) {
						dtFirstDate = Convert.ToDateTime(firstDate);
						dtFirstDate = dtFirstDate.Date;
					}
					if (lastDate.Length > 0) {
						dtLastDate = Convert.ToDateTime(lastDate);
						dtLastDate = dtLastDate.Date;
					}
				}
				else {
					if (firstDate.Length > 0) {
						dtFirstDate = new DateTime(int.Parse(firstDate.Substring(0, 4)), int.Parse(firstDate.Substring(4, 2)), int.Parse(firstDate.Substring(6, 2)));
					}

					if (lastDate.Length > 0) {
						dtLastDate = new DateTime(int.Parse(lastDate.Substring(0, 4)), int.Parse(lastDate.Substring(4, 2)), int.Parse(lastDate.Substring(6, 2)));
					}
				}
			}
			else {
				dtFirstDate = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
				dtLastDate = WebFunctions.Dates.getPeriodEndDate(_periodEnd, _webSession.PeriodType);
			}
			#endregion

			#region nbLines init
			long nbLines = 0;
			long lineIndex = 0;
			switch (_vehicleInformation.Id) {
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
					nbLines = 9;
					if (adNumber != null && adNumber.Length > 0) nbLines = nbLines + 2;
					if (isAlertModule && tabEncart != null && tabEncart[1] != null && ((string)tabEncart[1]).Length > 0) nbLines++;
					if (isAlertModule && tabEncart != null && tabEncart[2] != null && ((string)tabEncart[2]).Length > 0) nbLines++;
					if (investment != null && investment.Length > 0) nbLines++;
					if (isAlertModule) nbLines = nbLines + 2; // nbLines = 16;
					break;
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
					nbLines = 10;
					if (isAlertModule) nbLines = nbLines + 5;
					if (investment != null && investment.Length > 0) nbLines++; // nbLines = 16;
					break;
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
					nbLines = 9;
					if (dtTypeSale != null && dtTypeSale.Rows.Count > 0) {
						nbLines++;//number board
						if (isAlertModule) nbLines++;
					}
					if (investment != null && investment.Length > 0) nbLines++;//nbLines = 12;
					break;
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
					nbLines = 7;
					if (_webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_VOLUME_MARKETING_DIRECT) && !isAlertModule) nbLines++;
					if (investment != null && investment.Length > 0) nbLines++;
					if (isAlertModule) nbLines = nbLines + 2; //nbLines = 11;
					break;
				case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
					nbLines = 8;
					if (investment != null && investment.Length > 0) nbLines++;
					break;
				default:
					throw (new PortofolioException("Vehicle unknown"));
			}
			#endregion

			#region headers
			Headers headers = new Headers();
			TNS.FrameWork.WebResultUI.Header header = new TNS.FrameWork.WebResultUI.Header(support.ToString(), HEADER_COLUMN_INDEX, "SynthesisH1");
			header.Add(new TNS.FrameWork.WebResultUI.Header("", FIRST_COLUMN_INDEX, "SynthesisH2"));
			header.Add(new TNS.FrameWork.WebResultUI.Header("", SECOND_COLUMN_INDEX, "SynthesisH2"));
			headers.Root.Add(header);
			resultTable = new ResultTable(nbLines, headers);
			#endregion

			#region Building resultTable
			// Date begin and date end for outdooor
			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor) {
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1607, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(dtFirstDate.Date.ToString("dd/MM/yyyy"));

				ChangeLineType(ref lineType);

				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1608, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(dtLastDate.Date.ToString("dd/MM/yyyy"));
			}
			// Period selected
			else {
				if (firstDate.Length > 0 || !isAlertModule) {
					lineIndex = resultTable.AddNewLine(lineType);
					if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
					|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) && isAlertModule)
						resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1381, _webSession.SiteLanguage));
					else resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1541, _webSession.SiteLanguage));
					if ((firstDate == lastDate && isAlertModule) || (dtLastDate.CompareTo(dtFirstDate) == 0 && !isAlertModule)) {
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(dtFirstDate.Date.ToString("dd/MM/yyyy"));
					}
					else {
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(896, _webSession.SiteLanguage) + " " + dtFirstDate.Date.ToString("dd/MM/yyyy") + " " + GestionWeb.GetWebWord(1730, _webSession.SiteLanguage) + " " + dtLastDate.Date.ToString("dd/MM/yyyy"));
					}
				}
			}

			ChangeLineType(ref lineType);

			// Périodicité
			if (dtCategory.Columns.Contains("periodicity")) {
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1450, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(periodicity);
				ChangeLineType(ref lineType);
			}
			// Categorie
			lineIndex = resultTable.AddNewLine(lineType);
			resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1416, _webSession.SiteLanguage));
			resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(category);
			ChangeLineType(ref lineType);

			// Régie
			if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing) {
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1417, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(regie);
				ChangeLineType(ref lineType);
			}

			//  Volume for Marketing Direct
			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.directMarketing &&
				_webSession.CustomerLogin.CustormerFlagAccess(DBCst.Flags.ID_VOLUME_MARKETING_DIRECT) && !isAlertModule) {
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(2216, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellVolume(double.Parse(volume));
				ChangeLineType(ref lineType);
			}

			// Centre d'intérêt
			if (_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.directMarketing) {
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1411, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(interestCenter);
				ChangeLineType(ref lineType);
			}

			//number board et type de reseau ,cas de l'Affichage
			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.outdoor && dtTypeSale != null && dtTypeSale.Rows.Count > 0) {
				//number board
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1604, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = (isAlertModule) ? new CellNumber(double.Parse(numberBoard)) : new CellNumber(double.Parse(nbrSpot));
				ChangeLineType(ref lineType);

				//Type sale
				if (isAlertModule) {
					int count = 0;
					lineIndex = resultTable.AddNewLine(lineType);
					resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1609, _webSession.SiteLanguage));
					if (dtTypeSale.Rows.Count == 0) typeReseauStr = "&nbsp;";
					else {
						foreach (DataRow row in dtTypeSale.Rows) {
							if (count > 0) {
								typeReseauStr += "<BR>";
							}
							typeReseauStr += SQLGenerator.SaleTypeOutdoor(row["type_sale"].ToString(), _webSession.SiteLanguage);
							count++;
						}
					}
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(typeReseauStr);
					ChangeLineType(ref lineType);
				}
			}
			// Cas de la presse
			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.press
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.internationalPress) {
				// Nombre de page
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1385, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(pageNumber));
				ChangeLineType(ref lineType);

				if (adNumber != null && adNumber.Length > 0) {
					// Nombre de page pub		
					lineIndex = resultTable.AddNewLine(lineType);
					resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1386, _webSession.SiteLanguage));
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(adNumber));
					ChangeLineType(ref lineType);

					// Ratio
					if (pageNumber != null && pageNumber.Length > 0) {
						lineIndex = resultTable.AddNewLine(lineType);
						resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage));
						//resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(((decimal.Parse(adNumber) / decimal.Parse(pageNumber) * 100)/1000).ToString("0.###")+" %");
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPercent(((double.Parse(adNumber) / double.Parse(pageNumber) * 100) / (double)1000));
					}
					else {
						lineIndex = resultTable.AddNewLine(lineType);
						resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1387, _webSession.SiteLanguage));
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel("&nbsp;&nbsp;&nbsp;&nbsp;");
					}
					ChangeLineType(ref lineType);
				}
				if (isAlertModule) {
					if (tabEncart != null && ((string)tabEncart[1]).Length > 0) {
						// Nombre de page de pub hors encarts
						if (((string)tabEncart[1]).Length == 0)
							tabEncart[1] = "0";
						lineIndex = resultTable.AddNewLine(lineType);
						resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1388, _webSession.SiteLanguage));
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(tabEncart[1].ToString()));
						ChangeLineType(ref lineType);
					}
					if (tabEncart != null && ((string)tabEncart[2]).Length > 0) {
						// Nombre de page de pub encarts
						if (((string)tabEncart[2]).Length == 0) {
							tabEncart[2] = "0";
						}
						lineIndex = resultTable.AddNewLine(lineType);
						resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1389, _webSession.SiteLanguage));
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellPage(double.Parse(tabEncart[2].ToString()));
						ChangeLineType(ref lineType);
					}
				}
			}

			// Cas tv, radio
			if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others) {

				//Nombre de spot
				if (nbrSpot.Length == 0) {
					nbrSpot = "0";
				}
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1404, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(nbrSpot));
				ChangeLineType(ref lineType);

				if (isAlertModule) {
					// Nombre d'ecran
					if (nbrEcran.Length == 0) {
						nbrEcran = "0";
					}
					lineIndex = resultTable.AddNewLine(lineType);
					resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1412, _webSession.SiteLanguage));
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(nbrEcran));
					ChangeLineType(ref lineType);
				}
				if (totalDuration.Length == 0) {
					totalDuration = "0";
				}
				// total durée
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1413, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellDuration(double.Parse(totalDuration));
				ChangeLineType(ref lineType);
			}

			// Total investissements
			if (investment != null && investment.Length > 0) {
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1390, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellEuro(double.Parse(investment));
				ChangeLineType(ref lineType);
			}

			//Nombre de produits
			lineIndex = resultTable.AddNewLine(lineType);
			resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1393, _webSession.SiteLanguage));
			resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(tab[0].ToString()));
			ChangeLineType(ref lineType);

			if ((_vehicleInformation.Id != DBClassificationConstantes.Vehicles.names.outdoor) && isAlertModule) {
				//Nombre de nouveaux produits dans la pige
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1394, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(tab[2].ToString()));
				ChangeLineType(ref lineType);

				//Nombre de nouveaux produits dans le support
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1395, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellNumber(double.Parse(tab[1].ToString()));
				ChangeLineType(ref lineType);
			}
			//Nombre d'annonceurs
			lineIndex = resultTable.AddNewLine(lineType);
			resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1396, _webSession.SiteLanguage));
			resultTable[lineIndex, SECOND_COLUMN_INDEX] = (isAlertModule) ? new CellNumber(double.Parse(tab[3].ToString())) : new CellNumber(double.Parse(tab[1].ToString()));
			ChangeLineType(ref lineType);

			// Cas tv, presse
			if ((_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.radio
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
				|| _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.others)
				&& isAlertModule) {

				// Durée moyenne d'un écran
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1414, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellDuration(Convert.ToDouble(((long)averageDurationEcran).ToString()));
				ChangeLineType(ref lineType);

				// Nombre moyen de spots par écran
				lineIndex = resultTable.AddNewLine(lineType);
				resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1415, _webSession.SiteLanguage));
				resultTable[lineIndex, SECOND_COLUMN_INDEX] = new CellLabel(nbrSpotByEcran.ToString("0.00"));

			}
			#endregion

			return resultTable;

		}
		/// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override string BuildHtmlResult() {
			throw new PortofolioException("The method or operation is not implemented.");
		}
		#endregion

		

		#endregion

		#region Protected methods

		#region Change line type
		/// <summary>
		/// Change line type
		/// </summary>
		/// <param name="lineType">Line Type</param>
		/// <returns>Line type</returns>
		protected virtual void ChangeLineType(ref LineType lineType) {

			if (lineType == LineType.level1)
				lineType = LineType.level2;
			else
				lineType = LineType.level1;

		}
		#endregion		

		#endregion
	}
}
