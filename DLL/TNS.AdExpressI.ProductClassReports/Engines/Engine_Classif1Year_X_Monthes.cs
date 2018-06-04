#region Information
/*
 * Author : G Ragneau
 * Created on : 24/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.Date;
namespace TNS.AdExpressI.ProductClassReports.Engines
{
    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Year X Monthes
    /// </summary>
    public class Engine_Classif1Year_X_Monthes : Engine
    {

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public Engine_Classif1Year_X_Monthes(WebSession session, int result) : base(session, result) { }
        #endregion

        #region Abstract methods implementation
        /// <summary>
        /// Compute data got from the DAL layer before to design the report
        /// </summary>
        /// <param name="data">DAL data</param>
        /// <returns>data computed from DAL result</returns>
        protected override object[,] ComputeData(DataSet dsData)
        {

			DataTable dtData = dsData.Tables[0];

			if (dtData.Rows.Count<=0) return null;


			#region Variables
			object[,] data;
            bool isPluri = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstDBClassif.Vehicles.names.plurimedia
                ;

			int i,j,k;

			int FIRST_DATA_INDEX;
			int[,] CLASSIF_INDEXES;
			int NB_LINE;
			int NB_OPTION = 0;
			int NB_YEAR = 2;
			string YEAR_N = "";
			string YEAR_N1 = "";
			int currentLine;
			bool media = false;
			bool mediaN1 = false;
			bool product = false;
			bool productN1 = false;
			bool evolution = false;

			#endregion

			#region Constantes

			//Perso Concept? last column = advertiser ==> _isPersonalized=last column
			List<long> referenceIDS = null;
            List<long> competitorIDS = null;
			if(dtData.Columns.Contains("id_advertiser")){
                _isPersonalized = 1;
				if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].Contains(0)) {
                    referenceIDS = _session.SecondaryProductUniverses[0].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
				}
				if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].Contains(0)) {
					competitorIDS = _session.SecondaryProductUniverses[1].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
				}
			}
            if (referenceIDS == null)
                referenceIDS = new List<long>();
            if (competitorIDS == null)
                competitorIDS = new List<long>();

			//Comparative study?
			if (!_session.ComparativeStudy){
				NB_YEAR--;
			}
			else if (_session.Evolution){
				evolution = true;
				NB_OPTION++;
			}

			//nombre d'options PDM, PDV, Evol
			if (_session.PDM && _session.PreformatedTable.ToString().StartsWith("media")){
				media = true;
				if (NB_YEAR>1)
					mediaN1 = true;
				NB_OPTION += NB_YEAR;
			}

			if (_session.PDV && _session.PreformatedTable.ToString().StartsWith("product")){
				product = true;
				if (NB_YEAR>1)
					productN1 = true;
				NB_OPTION += NB_YEAR;
			}


			//Compute index of first data column
			for(i=0; i<dtData.Columns.Count; i=i+2){
				if (dtData.Columns[i].ColumnName.IndexOf("ID_M")<0 && dtData.Columns[i].ColumnName.IndexOf("ID_P")<0){
					break;
				}
			}
			FIRST_DATA_INDEX=i;
			//Delete useless lines
			CleanDataTable(dtData,FIRST_DATA_INDEX);

			//Edit list of descriptive columns (media, produit, groupe, category...) et init of "nbLevel" and "oldLevelID"
			CLASSIF_INDEXES = new int[FIRST_DATA_INDEX/2, 3];
			for(i = 0; i<(FIRST_DATA_INDEX); i+=2){
				CLASSIF_INDEXES[i/2, 0]=i;
				CLASSIF_INDEXES[i/2, 1]=0;
				CLASSIF_INDEXES[i/2, 2]=-1;
			}
			#endregion

			#region Compute number of lines an dinit data

			if (_session.PreformatedTable == CstFormat.PreformatedTables.productYear_X_Cumul
				|| _session.PreformatedTable == CstFormat.PreformatedTables.productYear_X_Mensual
				|| isPluri)
			{	
				NB_LINE = 1+NB_YEAR+NB_OPTION;
			}
			else{
				NB_LINE = 0;
			}

			foreach(DataRow currentRow in dtData.Rows){
				for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
                    int z = Convert.ToInt32(currentRow[CLASSIF_INDEXES[i,0]]);
					if(CLASSIF_INDEXES[i,2] != z){
						NB_LINE+= 1+NB_YEAR+NB_OPTION;
						CLASSIF_INDEXES[i,2] = z;
						for(j=i+1; j<=CLASSIF_INDEXES.GetUpperBound(0); j++){
							CLASSIF_INDEXES[j,2]=-1;
						}
					}
				}
			}
			//Init data:
				//size: nb of lines...    nb of columns = number of columns in table / number of years + number of classification levels
			data = new object[NB_LINE,(dtData.Columns.Count - _isPersonalized - 2*CLASSIF_INDEXES.GetLength(0)) / NB_YEAR + CLASSIF_INDEXES.GetLength(0) + _isPersonalized];
			#endregion

			#region Construction du tableau
			//Init "oldIdLevel"
			for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
				CLASSIF_INDEXES[i,2]=-1;
			}

			YEAR_N = _session.PeriodBeginningDate.Substring(0,4);
			if (NB_YEAR>1)YEAR_N1 = (int.Parse(YEAR_N)-1).ToString();
			

			currentLine = -(1+NB_YEAR+NB_OPTION);

			//Total Line
			if (_session.PreformatedTable == CstFormat.PreformatedTables.productYear_X_Cumul
				|| _session.PreformatedTable == CstFormat.PreformatedTables.productYear_X_Mensual
				|| isPluri)
			{
				for(j=0; j<=NB_YEAR+NB_OPTION; j++){
					for(i=0; i < CLASSIF_INDEXES.GetLength(0); i++)data[j, i] = null;
					for(i=CLASSIF_INDEXES.GetLength(0); i < data.GetLength(1) - _isPersonalized; i++)data[j, i] = 0.0;
					if (_isPersonalized>0) data[j, data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.none;
				}
				data[0,0] = "TOTAL";
				data[1,0] = YEAR_N;
				if(NB_YEAR>1)data[2,0] = YEAR_N1;
				if(evolution)data[1+NB_YEAR,0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1168,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
				if(media)
				{
					if (mediaN1)
					{
						data[NB_YEAR+NB_OPTION-1,0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
						data[NB_YEAR+NB_OPTION,0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N1);
					}
					else
					{
                        data[NB_YEAR + NB_OPTION, 0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), YEAR_N);
					}
				}
				if(product)
				{
					if (productN1)
					{
						data[NB_YEAR+NB_OPTION-1,0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
						data[NB_YEAR+NB_OPTION,0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N1);
					}
					else
					{
						data[NB_YEAR+NB_OPTION,0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
					}
				}
				currentLine = 0;
			}

			
			foreach(DataRow currentRow in dtData.Rows){

				for(i=0; i<=CLASSIF_INDEXES.GetUpperBound(0); i++){
					//For each level of classification (produit or media)
                    int c = Convert.ToInt32(currentRow[CLASSIF_INDEXES[i,0]]);
					if(CLASSIF_INDEXES[i,2] != c){

						//level different from previous ==> new line
						currentLine += 1+NB_YEAR+NB_OPTION;
						
						//For each year, each option and multi year line
						for(k=0; k <= NB_YEAR+NB_OPTION; k++){
							//Set to null all columns for higher levels
							for(j=0; j<i; j++) data[currentLine, j]=null;
							//Init data cells
							for(j=CLASSIF_INDEXES.GetLength(0); j < data.GetLength(1)-_isPersonalized; j++)data[currentLine+k, j] = 0.0;
							//Set to null lower levels
							for(j=i+1; j<=(CLASSIF_INDEXES.GetUpperBound(0)-2); j++) data[currentLine+k, j]=null;
							if (_isPersonalized>0) data[currentLine+k, data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.none;
						}
						//Set current line
						data[currentLine, i] = currentRow[CLASSIF_INDEXES[i,0]+1].ToString();
						data[currentLine+1, i] = YEAR_N;
						if (YEAR_N1!="")data[currentLine+2, i] = YEAR_N1;
						if (_session.Evolution)data[currentLine+NB_YEAR+1, i] = GestionWeb.GetWebWord(1168,_session.SiteLanguage);
						if (media)
						{
							if (!mediaN1)
							{
								data[currentLine+NB_YEAR+NB_OPTION, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
							}
							else
							{
								data[currentLine+NB_YEAR+NB_OPTION-1, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
								data[currentLine+NB_YEAR+NB_OPTION, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N1);
							}
						}
						if (product)
						{
							if (!productN1)
							{
								data[currentLine+NB_YEAR+NB_OPTION, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
							}
							else
							{
								data[currentLine+NB_YEAR+NB_OPTION-1, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N);
								data[currentLine+NB_YEAR+NB_OPTION, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187,_session.SiteLanguage), YEAR_N1);
							}
						}
						//Advertiser
						if(_isPersonalized>0 && 
							( _session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.advertiser.ToString())
							|| (( _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupAdvertiser
							||_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupProduct
							||_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.brand
							||_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupBrand
							|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentAdvertiser
							|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentBrand
							|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentProduct
							|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.product
                            /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                             **/
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.sectorAdvertiser
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.subSectorAdvertiser
							)
							&& i == CLASSIF_INDEXES.GetUpperBound(0)))
							){
                                long idAdv = Convert.ToInt64(currentRow["id_advertiser"]);
							if (referenceIDS.Contains(idAdv)){
								data[currentLine,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.reference;
								data[currentLine+1,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.reference;
								if(NB_YEAR>1) data[currentLine+2,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.reference;
							}
							else if (competitorIDS.Contains(idAdv)){
								data[currentLine,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.competitor;
								data[currentLine+1,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.competitor;
								if(NB_YEAR>1) data[currentLine+2,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.competitor;
							}
						}

						//Save current level
						CLASSIF_INDEXES[i,2]=int.Parse(currentRow[CLASSIF_INDEXES[i,0]].ToString());
						CLASSIF_INDEXES[i,1]=currentLine;
						
						//Init lower levels
						for(j=i+1; j<=CLASSIF_INDEXES.GetUpperBound(0); j++)CLASSIF_INDEXES[j,2]=-1;
					}
				}

				//Numerical data of current line

				for(i=1; i <= NB_YEAR; i++){
				//For each year + multi year total
					//For each data column
					for(k = 0; k < ( dtData.Columns.Count - FIRST_DATA_INDEX - _isPersonalized ); k+=NB_YEAR){


						for(j=0; j<CLASSIF_INDEXES.GetLength(0);j++){
						//For each level of classification
							//line per year
							data[CLASSIF_INDEXES[j,1]+i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] = 
								Convert.ToDouble(data[CLASSIF_INDEXES[j,1]+i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)]);

							//multi year line
							data[CLASSIF_INDEXES[j,1], CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] =
                                Convert.ToDouble(data[CLASSIF_INDEXES[j, 1], CLASSIF_INDEXES.GetLength(0) + k / NB_YEAR])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)]);
						}

						//Total global if required : plurimedia or produit classification
						if (_session.PreformatedTable == CstFormat.PreformatedTables.productYear_X_Cumul
							|| _session.PreformatedTable == CstFormat.PreformatedTables.productYear_X_Mensual
							|| isPluri)
						{
							//line per year
							data[i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] = 
								Convert.ToDouble(data[i, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)]);

							//multi year line
							data[0, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR] = 
								Convert.ToDouble(data[0, CLASSIF_INDEXES.GetLength(0)+k/NB_YEAR])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+k+Math.Max(0,i-1)]);
						}
					}
				}
			}
			#endregion

			#region Calculs

			#region Init "totals" of intermedia levels  (i.e != lowest level)
			int FIRST_RESULT_COLUMN = CLASSIF_INDEXES.GetLength(0);
			//Product_Index will contains for a line i totals of higher level
			//line 0 : totals global for PDM/PDV computings of level 0
			double[,] TOTAL_INDEXES_N = new double[CLASSIF_INDEXES.GetLength(0),data.GetLength(1)-FIRST_RESULT_COLUMN-_isPersonalized];
			double[,] TOTAL_INDEXES_N_1 = new double[CLASSIF_INDEXES.GetLength(0),data.GetLength(1)-FIRST_RESULT_COLUMN-_isPersonalized];
			for (i=0; i < TOTAL_INDEXES_N.GetLength(0); i++)
			{
				for(j=0; j < TOTAL_INDEXES_N.GetLength(1); j++){
					TOTAL_INDEXES_N[i,j] = 0.0;
					if (productN1||mediaN1) TOTAL_INDEXES_N_1[i,j] = 0.0;
				}
			}
			#endregion

			#region Go threw table
			currentLine = 0;
			bool monoMedia = false;
			//pluri and media classif
			if ((_session.PreformatedTable == CstFormat.PreformatedTables.mediaYear_X_Cumul
				||_session.PreformatedTable == CstFormat.PreformatedTables.mediaYear_X_Mensual)
				&&!isPluri){
				monoMedia = true;
			}

			for(i=0; i<=data.GetUpperBound(0); i++){

				//Extract level info
				for(j=0; j<FIRST_RESULT_COLUMN; j++){
					if (data[i,j]!=null) break;
				}

				if (_isPersonalized>0){
					currentLine = i +  1 + NB_YEAR;
					//calcul evolution
					if (evolution){
						data[currentLine,data.GetLength(1)-1] = data[i,data.GetLength(1)-1];
						currentLine++;
					}

					//personnalise PDM || PDV
					if (media || product){
						data[currentLine,data.GetLength(1)-1] = data[i,data.GetLength(1)-1];
						if (mediaN1||productN1)data[currentLine+1,data.GetLength(1)-1] = data[i,data.GetLength(1)-1];
					}
				}


				//Edit info evol, PDM, PDV for each columns
				for(k=FIRST_RESULT_COLUMN; k < data.GetLength(1)-_isPersonalized; k++){
 
					//if current line = total, affect in j i.e 0
					//else, if line != total and level != lowest level, affect in j+1 i.e level+1
					if(i==0 && !monoMedia)
					{
                        TOTAL_INDEXES_N[j, k - FIRST_RESULT_COLUMN] = Convert.ToDouble(data[i + 1, k]);
						if(mediaN1||productN1)
                            TOTAL_INDEXES_N_1[j, k - FIRST_RESULT_COLUMN] = Convert.ToDouble(data[i + 2, k]);
					}
					else if(j<FIRST_RESULT_COLUMN-1)
					{
                        TOTAL_INDEXES_N[j + 1, k - FIRST_RESULT_COLUMN] = Convert.ToDouble(data[i + 1, k]);
						if(mediaN1||productN1)
                            TOTAL_INDEXES_N_1[j + 1, k - FIRST_RESULT_COLUMN] = Convert.ToDouble(data[i + 2, k]);
					}

					currentLine = i +  1 + NB_YEAR;

					//evolution computing
					if (evolution){
                        data[currentLine, k] = 100 * (Convert.ToDouble(data[i + 1, k])
                            - Convert.ToDouble(data[i + 2, k]))
                            / Convert.ToDouble(data[i + 2, k]);
						currentLine++;
					}

					//PDM || PDV computing
					if (media || product){
						if(i!=0){
                            if (Convert.ToDouble(TOTAL_INDEXES_N[j, k - FIRST_RESULT_COLUMN]) != 0)
                                data[currentLine, k] = 100 * Convert.ToDouble(data[i + 1, k]) 
									/ TOTAL_INDEXES_N[j,k-FIRST_RESULT_COLUMN];
							else
								data[currentLine,k] = null;
						}
						else{
							data[currentLine,k] = 100;
						}
						if (mediaN1||productN1)
						{
							if(i!=0)
							{
                                if (Convert.ToDouble(TOTAL_INDEXES_N_1[j, k - FIRST_RESULT_COLUMN]) != 0)
                                    data[currentLine + 1, k] = 100 * Convert.ToDouble(data[i + 2, k]) 
										/ TOTAL_INDEXES_N_1[j,k-FIRST_RESULT_COLUMN];
								else
									data[currentLine+1,k] = null;
							}
							else
							{
								data[currentLine+1,k] = 100;
							}
						}

					}

				}

				i += NB_YEAR + NB_OPTION ;

			}

			#endregion

			#endregion

			return data;
        }

        /// <summary>
        /// Design report
        /// </summary>
        /// <param name="str">Html code container</param>
        /// <param name="data">data to display</param>
        protected override void BuildHTML(StringBuilder str, object[,] data)
        {

			#region Variables

			bool display = true;
			// Total line ?
			bool totalLine=false;

			//Perco concept
			if(! (_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.group
				|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupSegment
                /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                **/
                || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.sector
                || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.subSector
				|| _session.PreformatedTable == CstFormat.PreformatedTables.mediaYear_X_Mensual
				|| _session.PreformatedTable == CstFormat.PreformatedTables.mediaYear_X_Cumul)
				){
				_isPersonalized=1;
			}

			//compteurs			
			int i,j;

			//Line indexes

			//Variables from user session
			int NB_OPTION = 0;
			int NB_YEAR = 1;
			if(_session.ComparativeStudy){
				NB_YEAR++;
				if (_session.Evolution){
					NB_OPTION++;
				}
			}

			//Options number (PDM, PDV, Evol)
			if (_session.PDM && _session.PreformatedTable.ToString().StartsWith("media"))
			{
				NB_OPTION += (_session.ComparativeStudy)?2:1;
			}

			if (_session.PDV && _session.PreformatedTable.ToString().StartsWith("product"))
			{
				NB_OPTION += (_session.ComparativeStudy)?2:1;
			}			

			//Column indexes
			int FIRST_DATA_INDEX = 0;
			do{
				FIRST_DATA_INDEX++;
			}
			while(FIRST_DATA_INDEX < data.GetLength(1) && data[0,FIRST_DATA_INDEX]==null);


			int L1_DATA_INDEX = FIRST_DATA_INDEX-2;
			int L2_DATA_INDEX = FIRST_DATA_INDEX-1;

			//style Css
			string HEADER_CSS = "";
			string L0_CSS = "";
			string LIGHT_L0_CSS = "";
			string L1_CSS = "";
			string L1_COMPET_CSS = "";
			string L1_REF_CSS = "";
			string LIGHT_L1_CSS = "";
			string LIGHT_L1_COMPET_CSS = "";
			string LIGHT_L1_REF_CSS = "";
			string L2_CSS = "";
			string L2_COMPET_CSS = "";
			string L2_REF_CSS = "";
			string LIGHT_L2_CSS = "";
			string LIGHT_L2_COMPET_CSS = "";
			string LIGHT_L2_REF_CSS = "";
			string lineCssClass = "";

			if(!_excel){ //Css str
				HEADER_CSS = "astd0";
				L0_CSS = "asl0";
				LIGHT_L0_CSS = "asl0";
			
				L1_CSS = "asl3";
				L1_COMPET_CSS = "asl3c";
				L1_REF_CSS = "asl3r";

				LIGHT_L1_CSS = "asl3b";
				LIGHT_L1_COMPET_CSS = "asl3bc";
				LIGHT_L1_REF_CSS = "asl3br";

				L2_CSS = (L1_DATA_INDEX>=0)?"asl5":"asl3";
				L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5c":"asl3c";
				L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5r":"asl3r";

				LIGHT_L2_CSS = (L1_DATA_INDEX>=0)?"asl5b":"asl3b";
				LIGHT_L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5bc":"asl3bc";
				LIGHT_L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5br":"asl3br";
				lineCssClass = "";

				if(_session.PreformatedTable.ToString().StartsWith("media")){
					L2_CSS = L1_DATA_INDEX>0?"asl5":"asl3";
					LIGHT_L2_CSS = L1_DATA_INDEX>0?"asl5b":"asl3b";
				}
			}
			else{ //Css _excel
				HEADER_CSS = "astd0x";
				L0_CSS = "asl0";
				LIGHT_L0_CSS = "asl0";
			
				L1_CSS = "asl3x";
				L1_COMPET_CSS = "asl3cx";
				L1_REF_CSS = "asl3rx";

				LIGHT_L1_CSS = "asl3bx";
				LIGHT_L1_COMPET_CSS = "asl3bcx";
				LIGHT_L1_REF_CSS = "asl3brx";

				L2_CSS = (L1_DATA_INDEX>=0)?"asl5x":"asl3x";
				L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5cx":"asl3cx";
				L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5rx":"asl3rx";

				LIGHT_L2_CSS = (L1_DATA_INDEX>=0)?"asl5bx":"asl3bx";
				LIGHT_L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5bcx":"asl3bcx";
				LIGHT_L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5brx":"asl3brx";
				lineCssClass = "";

				if(_session.PreformatedTable.ToString().StartsWith("media")){
					L2_CSS = L1_DATA_INDEX>0?"asl5x":"asl3x";
					LIGHT_L2_CSS = L1_DATA_INDEX>0?"asl5bx":"asl3bx";
				}
			}
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;
            #endregion

			#region Table begin
			str.Append("<table cellPadding=0 cellSpacing=1px border=0>");
			#endregion

			#region Table headers
			str.Append("<tr class=" + HEADER_CSS + ">");
			if (_session.PreformatedTable != CstFormat.PreformatedTables.mediaYear_X_Cumul
				&& _session.PreformatedTable != CstFormat.PreformatedTables.mediaYear_X_Mensual) 
				str.AppendFormat("<td>{0}</td>", GestionWeb.GetWebWord(1164, _session.SiteLanguage));
			else
				str.AppendFormat("<td>{0}</td>", GestionWeb.GetWebWord(1357, _session.SiteLanguage));
			j = FIRST_DATA_INDEX;
			if (_session.PreformatedTable.ToString().EndsWith("Mensual")){
				str.Append("<td>TOTAL</td>");
				j++;
			}
			for(i=j; i < data.GetLength(1)-_isPersonalized; i++){
				CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
				str.AppendFormat("<td>{0}</td>", MonthString.GetCharacters(FctUtilities.Dates.GetPeriodBeginningDate((int.Parse(_session.PeriodBeginningDate) + i - j).ToString(), _session.PeriodType).Month, cInfo, 0));
			}
			str.Append("</tr>");

			#endregion

			#region Tablme body
			//line totale
			lineCssClass = L0_CSS;
			for(i=1; i <= NB_YEAR+NB_OPTION; i++){
				str.AppendFormat("<tr class={0}><td", lineCssClass);
				if(i<1){
					str.Append(" align=left");
				}
				str.AppendFormat(" nowrap>{0}</td>", data[i,0]);
				for(j = FIRST_DATA_INDEX; j < data.GetLength(1)-_isPersonalized; j++){
					if (data[i,j]!=null){
                        AppendNumericData(_session, str, Convert.ToDouble(data[i, j]), IsMultiYearLine(i,0,NB_YEAR,NB_OPTION), data[i,0].ToString(), false,  fp);
					}
					else{
						str.Append("<td nowrap></td>");
					}
				}
				str.Append("</tr>");
				lineCssClass = LIGHT_L0_CSS;
			}
			//next lines
			string align = "";
			int labelIndex = 0;
			StringBuilder output = str;
			StringBuilder bufferHtml = new StringBuilder(5000);
			bool newLine = false;
			for(i = 1+NB_YEAR+NB_OPTION; i < data.GetLength(0); i++){
				newLine = false;
				for(j = 0; j < data.GetLength(1)-_isPersonalized; j++){

					if(j==L1_DATA_INDEX && data[i,j]!=null){
						if (_isPersonalized<1)
							display = true;
						else if (_session.PersonalizedElementsOnly 
							&& (CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.none
							&& _session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.advertiser.ToString())) 
							display = false;
						else
						{
							if (IsMultiYearLine(i,0,NB_YEAR,NB_OPTION))
								bufferHtml.Length = 0;
							output = bufferHtml;
							display = true;
						}

						if (display){
							labelIndex = j;
							if (!IsMultiYearLine(i,0,NB_YEAR,NB_OPTION)){
								totalLine=false;
								align = "";
								lineCssClass = LIGHT_L1_CSS;
								if (_isPersonalized>0){
									if ((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
										lineCssClass=LIGHT_L1_COMPET_CSS;
									else if (((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
										lineCssClass=LIGHT_L1_REF_CSS;
								}
							}
							else{
								totalLine=true;
								align = " align=left ";
								lineCssClass = L1_CSS;
								if (_isPersonalized>0){
									if ((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
										lineCssClass=L1_COMPET_CSS;
									else if (((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
										lineCssClass=L1_REF_CSS;
								}
							}
							newLine = true;
							output.AppendFormat("<tr class={0}><td {1} nowrap>{2}</td>", lineCssClass, align, data[i,j]);
							j = FIRST_DATA_INDEX-1;
						}
					}
					else if(j==L2_DATA_INDEX){
						if (_isPersonalized<1)
							display = true;
						else if (_session.PersonalizedElementsOnly && (CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.none) 
							display = false;
						else
							display = true;
						if (display && data[i,j]!=null){
							str.Append(bufferHtml.ToString());
							bufferHtml.Length = 0;
							output = str;
							labelIndex = j;
							if (!IsMultiYearLine(i,0,NB_YEAR,NB_OPTION)){
								totalLine=false;
								align = "";
								lineCssClass = LIGHT_L2_CSS;
								if (_isPersonalized>0){
									if ((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
										lineCssClass=LIGHT_L2_COMPET_CSS;
									else if (((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
										lineCssClass=LIGHT_L2_REF_CSS;
								}
							}
							else{
								totalLine=true;
								align = " align=left ";
								lineCssClass = L2_CSS;
								if (_isPersonalized>0){
									if ((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
										lineCssClass=L2_COMPET_CSS;
									else if (((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
										lineCssClass=L2_REF_CSS;
								}
							}
							newLine = true;
                            output.AppendFormat("<tr class={0}><td {1} nowrap>{2}</td>", lineCssClass, align, data[i, j]);
                            j = FIRST_DATA_INDEX - 1;
						}
					}
					else {
						if (display){
							if(data[i,j]!=null){
                                AppendNumericData(_session, output, Convert.ToDouble(data[i, j]), IsMultiYearLine(i,0,NB_YEAR,NB_OPTION),data[i,labelIndex].ToString(), totalLine, fp);
							}
							else if(j>L2_DATA_INDEX)
								output.Append("<td></td>"); //<td>- </td>
						}

					}
				}
				if (newLine)
					output.Append("</tr>");
			}
			#endregion

			#region End table
			str.Append("</table>");
			#endregion
			
        }
        #endregion

    }
}
