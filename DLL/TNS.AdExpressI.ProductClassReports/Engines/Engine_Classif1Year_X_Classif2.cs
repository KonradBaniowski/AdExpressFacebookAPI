using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using System.Data;

using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctUtilities = TNS.AdExpress.Web.Functions;

using TNS.Classification.Universe;
using TNS.AdExpressI.ProductClassReports.Exceptions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpressI.ProductClassReports.Engines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Year X Classif2
    /// </summary>
    public class Engine_Classif1Year_X_Classif2 : Engine
    {

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public Engine_Classif1Year_X_Classif2(WebSession session, int result) : base(session, result) { }
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
			object[,] data = null;
			int i,j,k,l,currentLine;
			string yearN ,yearN1;

			//Personalized concept
			List<long> referenceIDS = null;
            List<long> competitorIDS = null;
			if(dtData.Columns.Contains("id_advertiser")){
				_isPersonalized=1;
				if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].Contains(0)) {
					referenceIDS = _session.SecondaryProductUniverses[0].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);					
				}
				if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].Contains(0)) {
                    competitorIDS = _session.SecondaryProductUniverses[1].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
				}
			}
            if (referenceIDS == null) referenceIDS = new List<long>();
            if (competitorIDS == null) competitorIDS = new List<long>();

            int NB_LINE = 0;
			int NB_OPTION = 0;
			int nbYearData;
			int FIRST_DATA_INDEX;
			int FIRST_MEDIA_DATA_INDEX;
			int FIRST_RESULT_LINE_INDEX = 0;

			int[,] PRODUCT_DATA_INDEXES;
			int[,] MEDIA_DATA_INDEXES;

			int FIRST_MEDIA_RESULT_INDEX;
			int FIRST_PRODUCT_RESULT_INDEX = 0;

			string TOTAL = "PLURIMEDIA";

			string[] mainHeaders;
			StringBuilder headers = new StringBuilder(750);
			#endregion

			#region Compute index of first numerical column
			for(i=0; i<dtData.Columns.Count; i=i+2){
				if (dtData.Columns[i].ColumnName.IndexOf("ID_M")<0 && dtData.Columns[i].ColumnName.IndexOf("ID_P")<0){
					break;
				}
			}
			FIRST_DATA_INDEX=i;
			//delete useless lines
			CleanDataTable(dtData, FIRST_DATA_INDEX);
			#endregion

			#region Indexes of product classif
			//Edit list of colums			
			if (_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.brand ||
				_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.group ||
				_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.advertiser)
			{
				PRODUCT_DATA_INDEXES = new int[1, 3]{{0,0,-1}};
			}
			else{
				PRODUCT_DATA_INDEXES = new int[2, 3]{{0,0,-1},{2,0,-1}};
			}
			#endregion

			#region Compute number of lines in result table
			if (!_session.ComparativeStudy) NB_LINE = 2;
			else{
				NB_LINE = 3;
				if (_session.Evolution){
					NB_OPTION++;
					NB_LINE++;
				}
			}
			if (_session.PDM){
				NB_OPTION += (_session.ComparativeStudy)?2:1;
				NB_LINE += (_session.ComparativeStudy)?2:1;
			}
			if (_session.PDV){
				NB_OPTION += (_session.ComparativeStudy)?2:1;
				NB_LINE += (_session.ComparativeStudy)?2:1;
			}
			foreach(DataRow currentRow in dtData.Rows){
				for(i=0; i<=PRODUCT_DATA_INDEXES.GetUpperBound(0); i++){
					if(PRODUCT_DATA_INDEXES[i,2]!=int.Parse(currentRow[PRODUCT_DATA_INDEXES[i,0]].ToString())){
						if (!_session.ComparativeStudy) NB_LINE+=2+NB_OPTION;
						else NB_LINE+=3+NB_OPTION;
						PRODUCT_DATA_INDEXES[i,2]=int.Parse(currentRow[PRODUCT_DATA_INDEXES[i,0]].ToString());
						for(j=i+1; j<=PRODUCT_DATA_INDEXES.GetUpperBound(0); j++){
							PRODUCT_DATA_INDEXES[j,2]=-1;
						}
					}
				}
			}
			#endregion

			#region Media classif managment

			#region Media classif data sorting
			string sortStr = "";
			switch(_session.PreformatedMediaDetail){
				case CstFormat.PreformatedMediaDetails.vehicle:
					sortStr = "ID_M1";
					break;
				case CstFormat.PreformatedMediaDetails.vehicleCategory:
				case CstFormat.PreformatedMediaDetails.vehicleMedia:
					sortStr = "ID_M1,ID_M2";
					break;
				case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
					sortStr = "ID_M1,ID_M2,ID_M3";
					break;
				default:
					throw new ProductClassReportsException("Detail format " + _session.PreformatedMediaDetail.ToString() + " unvalid.");
			}
			DataRow[] medias = dsData.Tables[0].Select("", sortStr);
			#endregion

			#region Init media classif constants
			Dictionary<long, int[]> MEDIA_RESULT_INDEXES = new Dictionary<long, int[]>();

			//Index of first media column
			FIRST_MEDIA_RESULT_INDEX = FIRST_PRODUCT_RESULT_INDEX+PRODUCT_DATA_INDEXES.GetLength(0);

			//Index of first media id column
			FIRST_MEDIA_DATA_INDEX = dtData.Columns["ID_M1"].Ordinal;

			//Dynamic tool to spot columns with numerical data about media
			MEDIA_DATA_INDEXES = new int[(FIRST_DATA_INDEX-FIRST_MEDIA_DATA_INDEX)/2, 3];
			//Init
			for(i=0; i<MEDIA_DATA_INDEXES.GetLength(0); i++){
				//Column index
				MEDIA_DATA_INDEXES[i,0]=FIRST_MEDIA_DATA_INDEX + 2*i;
				//old value
				MEDIA_DATA_INDEXES[i,1]=-1;
				//Index in result table
				MEDIA_DATA_INDEXES[i,2]=0;
			}
			#endregion

			#region Build list of media classif columns and list of headers in result table
			int numColumn;
			//Pluri or not ? Yes ==> total pluri to do, else no
			//Column total : plurimedia ou field "m1"
			if (VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) != CstDBClassif.Vehicles.names.plurimedia){
				numColumn = FIRST_MEDIA_RESULT_INDEX;
			}
			else{
				headers.Append(TOTAL);
				numColumn = FIRST_MEDIA_RESULT_INDEX + 1;
                MEDIA_RESULT_INDEXES.Add(0, new int[1] { FIRST_MEDIA_RESULT_INDEX });
			}
			int[] tmpIndexes = null;
			foreach(DataRow currentRow in medias){
				//Build list of headers and media columns indexing
				for(i=0; i<MEDIA_DATA_INDEXES.GetLength(0); i++){
                    int z = Convert.ToInt32(currentRow[MEDIA_DATA_INDEXES[i,0]]);
					if (z != MEDIA_DATA_INDEXES[i,1]){
						//Save current level
						MEDIA_DATA_INDEXES[i,1] = z;
						MEDIA_DATA_INDEXES[i,2] = numColumn;
						if (i==MEDIA_DATA_INDEXES.GetUpperBound(0)){
							//lowest level must not be added
							//detail pluri/media ==> add level media
							//detail pluri / media / category ==> add category level

							//build list of columns to affect for current media level
							tmpIndexes = new int[i+1];
							for(j=0; j<=i; j++){
								tmpIndexes[j] = MEDIA_DATA_INDEXES[j,2];
							}
							MEDIA_RESULT_INDEXES.Add(Convert.ToInt64(currentRow[MEDIA_DATA_INDEXES[MEDIA_DATA_INDEXES.GetUpperBound(0),0]]), tmpIndexes);
							//Add column header
							headers.AppendFormat("{0}{1}", ((headers.Length<=0)?"":","), currentRow[MEDIA_DATA_INDEXES[i,0]+1]);
						}
						else{
							//Add Column header
							headers.AppendFormat("{0}{1}{2}", (headers.Length>0?"|":""), currentRow[MEDIA_DATA_INDEXES[i,0]+1], (headers.Length>0?("," + GestionWeb.GetWebWord(1102,_session.SiteLanguage)):""));
						}
						numColumn++;
					}
				}
			}
			//A this time, we have:
			//List of headers joined by commas if detail stop before support
			//or as following MEDIA|CAT1,SUP1,SUP2|CAT3,SUP3,SUP5,SUP4,SUP8
			//For each lowest media level, index of affected columns (its and totals)
			#endregion

			#endregion

			#region Init result table : instanciation and headers
			//Instanciation
				//size : 
                //  number of data lines + number of header lines (2 max) 
                //  and number of product columns + number of media columns + nb of media totals
				//number of media totals = 0 if media=pluri
                //  or mono detailled by vehicle. else based on principa headers numbers
			mainHeaders = headers.ToString().Split('|');
			int offset = (_session.PreformatedMediaDetail==CstFormat.PreformatedMediaDetails.vehicle)?0:mainHeaders.Length;
			data = new object[NB_LINE + Math.Min(mainHeaders.Length,2),PRODUCT_DATA_INDEXES.GetLength(0)+MEDIA_RESULT_INDEXES.Count+offset+_isPersonalized];
			//Fill headers
			if (mainHeaders.Length<2){
				//one line of headers <== no support detail
				currentLine = 0;
			}
			else{
				//two lines <== support detail
				currentLine = 1;
			}
			//First header above the first data column
			i=FIRST_MEDIA_RESULT_INDEX;
			foreach(string str in mainHeaders){
				string[] lowerHeaders = str.Split(',');
				data[0,i] = lowerHeaders[0];
				//if only one header in main chain or in sub chain,
				//that means we have the chain  TOTO|TITI|TATA so only one line of header and we go next cell
				if (lowerHeaders.Length<2||mainHeaders.Length<2)i++;
				for(j=1; j<lowerHeaders.Length; j++){
					data[currentLine,i] = lowerHeaders[j];
					i++;
				}
			}
			#endregion

			#region Fill table without evolution, PDM or PDV

			#region Reinit "oldLevelId" in PRODUCT_DATA_INDEXES
			for(i=0; i<=PRODUCT_DATA_INDEXES.GetUpperBound(0); i++){
				PRODUCT_DATA_INDEXES[i,2]=-1;
			}
			#endregion

			#region Fill table with data
			yearN= FctUtilities.Dates.getPeriodLabel(_session,CstPeriod.Type.currentYear);
			yearN1=FctUtilities.Dates.getPeriodLabel(_session,CstPeriod.Type.previousYear);
			//	2 if study on one year
			//	3 if study on two years
			if (!_session.ComparativeStudy)nbYearData=2;
			else nbYearData = 3;

			//total line
			currentLine++;
			FIRST_RESULT_LINE_INDEX = currentLine;
			for (j=currentLine; j< currentLine+ nbYearData+NB_OPTION ; j++){
				for(i=0; i < FIRST_MEDIA_RESULT_INDEX; i++)data[j, i] = null;
				for(i=FIRST_MEDIA_RESULT_INDEX; i< data.GetLength(1)-_isPersonalized; i++)data[j, i]=0.0;
				if (_isPersonalized>0){
					data[j,data.GetUpperBound(1)] = CstWeb.AdvertiserPersonalisation.Type.none;
				}
			}
			data[FIRST_RESULT_LINE_INDEX,0] = "TOTAL";
			data[FIRST_RESULT_LINE_INDEX+1,0] = yearN;
			k=2;
			if(nbYearData>2){
				data[FIRST_RESULT_LINE_INDEX+2,0]=yearN1;
				k++;
				if (_session.Evolution){
					data[FIRST_RESULT_LINE_INDEX+3, 0] = GestionWeb.GetWebWord(1168, _session.SiteLanguage);
					k++;
				}
			}

			if(_session.PDM){
				data[FIRST_RESULT_LINE_INDEX+k, 0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
				k++;
				if (nbYearData>2) {
                    data[FIRST_RESULT_LINE_INDEX + k, 0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
					k++;
				}
			}
			
			if(_session.PDV){
				data[FIRST_RESULT_LINE_INDEX+k, 0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
				k++;
				if (nbYearData>2) {
					data[FIRST_RESULT_LINE_INDEX+k, 0] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166, _session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
				}
			}

			
            bool isPluri = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstDBClassif.Vehicles.names.plurimedia;
			foreach(DataRow currentRow in dtData.Rows){
				
				#region Données qualitatives
				for(i=0; i<=PRODUCT_DATA_INDEXES.GetUpperBound(0); i++){
					
					//For each level of classification (product classif only)
					if(PRODUCT_DATA_INDEXES[i,2]!=Convert.ToInt32(currentRow[PRODUCT_DATA_INDEXES[i,0]])){
						
						//inc current line
						currentLine += nbYearData+NB_OPTION;

						//init cells
						for(k = 0; k<nbYearData+NB_OPTION; k++){
							//For each line of current block
							//Set to null all columns of higher levels in each necessar lines
							for(j=0; j<i; j++){data[currentLine+k, j]=null;}
							//Set to null all lower levels and 0 as data
							for(j=i+1; j<FIRST_MEDIA_RESULT_INDEX; j++) data[currentLine+k, j]=null;
							for(j=FIRST_MEDIA_RESULT_INDEX; j<data.GetLength(1)-_isPersonalized; j++) data[currentLine+k, j]=0.0;
							if (_isPersonalized>0) data[currentLine+k, data.GetUpperBound(1)] = CstWeb.AdvertiserPersonalisation.Type.none;
						}

						//Set current line
						data[currentLine, i] = currentRow[PRODUCT_DATA_INDEXES[i,0]+1].ToString();
						data[currentLine+1, i] = yearN;
						//Advertiser usual, reference or competitor
						//si on visualise les elmt perso et
						//		si on considère un niveau de détail avec advertiser en niveau pere
						//		ou si on est en detail gp/Annon ou Gp/Ref et que c le niveau le plus bas
						//			si l'annonceur considéré est reference ou concurrents...
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
                             * */
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.sectorAdvertiser
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.subSectorAdvertiser
							)
							&& i == PRODUCT_DATA_INDEXES.GetUpperBound(0)))
							){
                            long idAdv = Convert.ToInt64(currentRow["id_advertiser"]);
							if (referenceIDS.Contains(idAdv)){
								data[currentLine,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.reference;
								data[currentLine+1,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.reference;
							}
							else if (competitorIDS.Contains(idAdv)){
								data[currentLine,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.competitor;
								data[currentLine+1,data.GetLength(1)-1] = CstWeb.AdvertiserPersonalisation.Type.competitor;
							}

						}

						//Year N-2?
						k=2;
						if (nbYearData>2){ 
							data[currentLine+2, i] = yearN1;
							data[currentLine+2, data.GetLength(1)-1] = data[currentLine,data.GetLength(1)-1];
							k++;
							if (_session.Evolution){
								data[currentLine+3, i] = GestionWeb.GetWebWord(1168, _session.SiteLanguage);
								k++;
							}
						}
						if(_session.PDV){
							data[currentLine+k, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
							k++;
							if (nbYearData>2)
							{
								data[currentLine+k, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(1166,_session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
								k++;
							}
						}
						if(_session.PDM){
							data[currentLine+k, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord( 806,_session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN);
							k++;
							if (nbYearData>2)
							{
								data[currentLine+k, i] = string.Format("{0}{1}{2}", GestionWeb.GetWebWord(806,_session.SiteLanguage), GestionWeb.GetWebWord(1187, _session.SiteLanguage), yearN1);
								k++;
							}
						}

						//Save current level
						PRODUCT_DATA_INDEXES[i,2]=Convert.ToInt32(currentRow[PRODUCT_DATA_INDEXES[i,0]]);
						PRODUCT_DATA_INDEXES[i,1]=currentLine;
						//Init lower level
						for(j=i+1; j<=PRODUCT_DATA_INDEXES.GetUpperBound(0); j++){
							PRODUCT_DATA_INDEXES[j,2]=-1;
						}
					}
				}
				#endregion

				#region year N
                long z = Convert.ToInt64(currentRow[FIRST_DATA_INDEX-2]);
				for(i=0; i<PRODUCT_DATA_INDEXES.GetLength(0);i++){
					for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
						//year N
						data[PRODUCT_DATA_INDEXES[i,1]+1, MEDIA_RESULT_INDEXES[z][j]] = 
							Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1]+1, MEDIA_RESULT_INDEXES[z][j]])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]);

						//total multi year
						data[PRODUCT_DATA_INDEXES[i,1], MEDIA_RESULT_INDEXES[z][j]] = 
							Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1], MEDIA_RESULT_INDEXES[z][j]])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]);
					}
					//Plurimedia? yes ==> compute totals of pluri columns
					if (isPluri){
						//total year
						data[PRODUCT_DATA_INDEXES[i,1]+1, FIRST_MEDIA_RESULT_INDEX] = 
							Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1]+1, FIRST_MEDIA_RESULT_INDEX])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]);

						//total multi year
						data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX] = 
							Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]);
					}
				}
				//Total global
				for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
					//on year
					data[FIRST_RESULT_LINE_INDEX+1, MEDIA_RESULT_INDEXES[z][j]] = 
						Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX+1, MEDIA_RESULT_INDEXES[z][j]])
						+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]);

					//multi year
					data[FIRST_RESULT_LINE_INDEX, MEDIA_RESULT_INDEXES[z][j]] = 
						Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX, MEDIA_RESULT_INDEXES[z][j]])
						+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]); 

				}
				//Plurimedia? yes ==> compute totals of pluri
				if (isPluri){
					//total year
					data[FIRST_RESULT_LINE_INDEX+1, FIRST_MEDIA_RESULT_INDEX] = 
						Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX+1, FIRST_MEDIA_RESULT_INDEX])
						+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]);

					data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX] = 
						Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX])
						+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX]);
				}
				#endregion

				#region Year N-1 if comparative study
				if (nbYearData>2){
					//year N : set cell (produit X media) and horizontal totals
					for(i=0; i<PRODUCT_DATA_INDEXES.GetLength(0);i++){
						for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
							//year N
							data[PRODUCT_DATA_INDEXES[i,1]+2, MEDIA_RESULT_INDEXES[z][j]] = 
								Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1]+2, MEDIA_RESULT_INDEXES[z][j]])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);

							//total multi year
							data[PRODUCT_DATA_INDEXES[i,1], MEDIA_RESULT_INDEXES[z][j]] = 
								Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1], MEDIA_RESULT_INDEXES[z][j]])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);

						}
						//Plurimedia? yes ==> compute pluri totals
						if (isPluri){
							//total year
							data[PRODUCT_DATA_INDEXES[i,1]+2, FIRST_MEDIA_RESULT_INDEX] = 
								Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1]+2, FIRST_MEDIA_RESULT_INDEX])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);

							//total multi year
							data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX] = 
								Convert.ToDouble(data[PRODUCT_DATA_INDEXES[i,1], FIRST_MEDIA_RESULT_INDEX])
								+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);
						}
					}
					//Totaux global
					//Plurimedia? yes ==> compute totals and pluri column
					if (isPluri){
						data[FIRST_RESULT_LINE_INDEX+2, FIRST_MEDIA_RESULT_INDEX] = 
							Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX+2, FIRST_MEDIA_RESULT_INDEX])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);

						data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX] = 
							Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX, FIRST_MEDIA_RESULT_INDEX])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);

					}

					//medias columns
					for(j=0; j<MEDIA_DATA_INDEXES.GetLength(0); j++){
						//on year
						data[FIRST_RESULT_LINE_INDEX+2, MEDIA_RESULT_INDEXES[z][j]] = 
							Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX+2, MEDIA_RESULT_INDEXES[z][j]])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);

						//multi year
						data[FIRST_RESULT_LINE_INDEX, MEDIA_RESULT_INDEXES[z][j]] = 
							Convert.ToDouble(data[FIRST_RESULT_LINE_INDEX, MEDIA_RESULT_INDEXES[z][j]])
							+ Convert.ToDouble(currentRow[FIRST_DATA_INDEX+1]);
					}

				}
				#endregion

			}
			#endregion

			#endregion

			#region Compute evolutions, PDM and PDV

			#region Init "totals" of intermadir levels (i.e =/ of lowest level which is not a total reference)
			//Product_Index will contains on a line i, the totals of classif level higehr to level i
			//ligne 0 : totaux généraux pour le calcul des PDM du niveau 0
			double[,] TOTAL_INDEXES_N = new double[FIRST_MEDIA_RESULT_INDEX,data.GetLength(1)-FIRST_MEDIA_RESULT_INDEX-_isPersonalized];
			double[,] TOTAL_INDEXES_N_1 = new double[FIRST_MEDIA_RESULT_INDEX,data.GetLength(1)-FIRST_MEDIA_RESULT_INDEX-_isPersonalized];

			for (i=0; i < TOTAL_INDEXES_N.GetLength(0); i++)
			{
				for(j=0; j < TOTAL_INDEXES_N.GetLength(1); j++){
					TOTAL_INDEXES_N[i,j] = 0.0;
					if (_session.ComparativeStudy)
						TOTAL_INDEXES_N_1[i,j] = 0.0;
				}
			}
			#endregion

			#region Computings
			currentLine = FIRST_RESULT_LINE_INDEX;

			for(i=FIRST_RESULT_LINE_INDEX; i<=data.GetUpperBound(0); i++){

				//Extract level info
				for(j=0; j<FIRST_MEDIA_RESULT_INDEX; j++){
					if (data[i,j]!=null) break;
				}

				//Edit info evol, PDM, PDV of each column
				for(k=FIRST_MEDIA_RESULT_INDEX; k < data.GetLength(1)-_isPersonalized; k++){

					//if line total, affect in j, i.e 0
					//else, if si line != total and level != lowest level, afect in j+1, i.e level+1
					if(i==FIRST_RESULT_LINE_INDEX)
					{
						TOTAL_INDEXES_N[j,k-FIRST_MEDIA_RESULT_INDEX] = Convert.ToDouble(data[i+1,k]);
						if (_session.ComparativeStudy)
							TOTAL_INDEXES_N_1[j,k-FIRST_MEDIA_RESULT_INDEX] = Convert.ToDouble(data[i+2,k]);
					}
					else if(j<FIRST_MEDIA_RESULT_INDEX-1)
					{
						TOTAL_INDEXES_N[j+1,k-FIRST_MEDIA_RESULT_INDEX] = Convert.ToDouble(data[i+1,k]);
						if (_session.ComparativeStudy)
							TOTAL_INDEXES_N_1[j+1,k-FIRST_MEDIA_RESULT_INDEX] = Convert.ToDouble(data[i+2,k]);
					}

					currentLine = i + nbYearData;

					//compute evol
					if (_session.ComparativeStudy && _session.Evolution){
						if (_isPersonalized>0) data[currentLine, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
						data[currentLine,k] = 100 * ( Convert.ToDouble(data[i+1,k])
							- Convert.ToDouble(data[i+2,k]))
							/ Convert.ToDouble(data[i+2,k]);
						currentLine++;
					}

					//compute PDV
					if (_session.PDV){
						if (_isPersonalized>0) {
							data[currentLine, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
							if(_session.ComparativeStudy)
								data[currentLine+1, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
						}
						if(i!=FIRST_RESULT_LINE_INDEX){
							data[currentLine,k] = 100 * Convert.ToDouble(data[i+1,k]) 
								/ TOTAL_INDEXES_N[j,k-FIRST_MEDIA_RESULT_INDEX];
							if (_session.ComparativeStudy)
								data[currentLine+1,k] = 100 * Convert.ToDouble(data[i+2,k]) 
									/ TOTAL_INDEXES_N_1[j,k-FIRST_MEDIA_RESULT_INDEX];
						}
						else{
							data[currentLine,k] = 100;
							if (_session.ComparativeStudy)
								data[currentLine+1,k] = 100;
						}
						currentLine+=(_session.ComparativeStudy)?2:1;
					}

					//compute PDM
					if (_session.PDM){
						if (_isPersonalized>0) {
							data[currentLine, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
							if (_session.ComparativeStudy)
								data[currentLine+1, data.GetUpperBound(1)] = data[i,data.GetUpperBound(1)];
						}
						if(k != FIRST_MEDIA_RESULT_INDEX) {
							if (FIRST_RESULT_LINE_INDEX<2) {
								//PDV par rapport au total général
								if (Convert.ToDouble(data[i+1,FIRST_MEDIA_RESULT_INDEX]) != 0) {
									data[currentLine,k] = 100 * Convert.ToDouble(data[i+1,k])
										/ Convert.ToDouble(data[i+1,FIRST_MEDIA_RESULT_INDEX]);
								}
								else {
									data[currentLine,k] = null;
								}
								if (_session.ComparativeStudy) {
									if (Convert.ToDouble(data[i+2,FIRST_MEDIA_RESULT_INDEX]) != 0) {
										data[currentLine+1,k] = 100 * Convert.ToDouble(data[i+2,k])
											/ Convert.ToDouble(data[i+2,FIRST_MEDIA_RESULT_INDEX]);
									}
									else {
										data[currentLine+1,k] = null;
									}
								}

							}
							else {
								//pdv relative to a subtotal to search
								//find total if current column = sub total
                                // else fin "sub total"=GestionWeb.GetWebWord(1102,_session.SiteLanguage))
								if (data[FIRST_RESULT_LINE_INDEX-1,k].ToString() == GestionWeb.GetWebWord(1102,_session.SiteLanguage)) {
									l = FIRST_MEDIA_RESULT_INDEX;
								}
								else {
									for(l=k-1; l>=FIRST_MEDIA_RESULT_INDEX; l--) {
										if (! (data[FIRST_RESULT_LINE_INDEX-1, l].ToString() != GestionWeb.GetWebWord(1102,_session.SiteLanguage))) {
											break;
										}
									}
								}
								if(Convert.ToDouble(data[i+1,l])!=0)
									data[currentLine,k] = 100 * Convert.ToDouble(data[i+1,k])
										/ Convert.ToDouble(data[i+1,l]);
								else
									data[currentLine,k] = null;
								if (_session.ComparativeStudy) {
									if(Convert.ToDouble(data[i+2,l])!=0)
										data[currentLine+1,k] = 100 * Convert.ToDouble(data[i+2,k])
											/ Convert.ToDouble(data[i+2,l]);
									else
										data[currentLine+1,k] = null;
								}

							}
						}
						else {
							data[currentLine,k] = 100;
							if (_session.ComparativeStudy)
								data[currentLine+1,k] = 100;
						}
					}
				}

				i += nbYearData + NB_OPTION - 1;

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
			//compteurs			
			int i,j;

			bool display = true;
			// Total line ?
			bool totalLine=false;

			//Perso concept? last column = advertiser ==> _isPersonalized=las tcolumn
			if(! (_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.group
				|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupSegment
                /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                **/
                || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.sector
                || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.subSector
				)
				){
				_isPersonalized=1;
			}

			//str stuff
			int beginningIndex;
			int lastMainHeaderIndex;
			string tmpHtml = "";

			//variables extracted from session data
			int NB_OPTION = 0;
			int NB_YEAR = 1;
			if(_session.ComparativeStudy){
				NB_YEAR++;
				if (_session.Evolution) NB_OPTION++;
			}
			if (_session.PDM)
			{
				NB_OPTION += NB_YEAR;
			}
			if (_session.PDV)
			{
				NB_OPTION += NB_YEAR;
			}

			//Indexe of lines
			int FIRST_DATA_LINE=0;
			do{
				FIRST_DATA_LINE++;
			}
			while(FIRST_DATA_LINE < data.GetLength(0) && data[FIRST_DATA_LINE,0]==null);

			//index of columns
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
			string Light_L0_CSS = "";
			string L1_CSS = "";
			string L1_COMPET_CSS = "";
			string L1_REF_CSS = "";
			string Light_L1_CSS = "";
			string Light_L1_COMPET_CSS = "";
			string Light_L1_REF_CSS = "";
			string L2_CSS = "";
			string L2_COMPET_CSS = "";
			string L2_REF_CSS = "";
			string Light_L2_CSS = "";
			string Light_L2_COMPET_CSS = "";
			string Light_L2_REF_CSS = "";

			if(!_excel){ //Css str
				HEADER_CSS = "astd0";
				L0_CSS = "asl0";
				Light_L0_CSS = "asl0";
			
				L1_CSS = "asl3";
				L1_COMPET_CSS = "asl3c";
				L1_REF_CSS = "asl3r";

				Light_L1_CSS = "asl3b";
				Light_L1_COMPET_CSS = "asl3bc";
				Light_L1_REF_CSS = "asl3br";

				L2_CSS = (L1_DATA_INDEX>=0)?"asl5":"asl3";
				L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5c":"asl3c";
				L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5r":"asl3r";

				Light_L2_CSS = (L1_DATA_INDEX>=0)?"asl5b":"asl3b";
				Light_L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5bc":"asl3bc";
				Light_L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5br":"asl3br";
			}
			else{ //Css _excel
				HEADER_CSS = "astd0x";
				L0_CSS = "asl0";
				Light_L0_CSS = "asl0x";
			
				L1_CSS = "asl3x";
				L1_COMPET_CSS = "asl3cx";
				L1_REF_CSS = "asl3rx";

				Light_L1_CSS = "asl3bx";
				Light_L1_COMPET_CSS = "asl3bcx";
				Light_L1_REF_CSS = "asl3brx";

				L2_CSS = (L1_DATA_INDEX>=0)?"asl5x":"asl3x";
				L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5cx":"asl3cx";
				L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5rx":"asl3rx";

				Light_L2_CSS = (L1_DATA_INDEX>=0)?"asl5bx":"asl3bx";
				Light_L2_COMPET_CSS = (L1_DATA_INDEX>=0)?"asl5bcx":"asl3bcx";
				Light_L2_REF_CSS = (L1_DATA_INDEX>=0)?"asl5brx":"asl3brx";
			}

			string lineCssClass = "";
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

			#endregion

			#region Table begin
			str.Append("<table cellPadding=0 cellSpacing=1px border=0>");
			#endregion

			#region table headers
			beginningIndex = str.Length;
			if (FIRST_DATA_LINE>1){
				//two header lines
				tmpHtml = string.Format("{0}<tr class={1}>", tmpHtml, HEADER_CSS);
                str.AppendFormat("<tr class={0}>", HEADER_CSS);
				tmpHtml = string.Format("{0}<td rowSpan=2>{1}</td>", tmpHtml, GestionWeb.GetWebWord(1164, _session.SiteLanguage));
				lastMainHeaderIndex = FIRST_DATA_INDEX;
				for(i=FIRST_DATA_INDEX; i < data.GetLength(1)-_isPersonalized; i++){
					if (data[1,i]!=null)
						str.AppendFormat("<td nowrap>{0}</td>", data[1,i]);
					if(data[0,i]!=null && i!=FIRST_DATA_INDEX){
						tmpHtml = string.Format("{0}<td", tmpHtml);
						if(lastMainHeaderIndex == FIRST_DATA_INDEX) tmpHtml = string.Format("{0} rowSpan=2", tmpHtml);
						tmpHtml = string.Format("{0} colSpan={1}>{2}</td>", tmpHtml, (i-lastMainHeaderIndex), data[0,lastMainHeaderIndex]);
						lastMainHeaderIndex = i;
					}
				}
				str.Append("</tr>");
				tmpHtml = string.Format("{0}<td colSpan={1}>{2}</td></tr>", tmpHtml, (i-lastMainHeaderIndex), data[0,lastMainHeaderIndex]);
				str.Insert(beginningIndex,tmpHtml);
			}
			else{
				str.AppendFormat("<tr class={0}>", HEADER_CSS);
				str.AppendFormat("<td>{0}</td>", GestionWeb.GetWebWord(1164, _session.SiteLanguage));
				for(i=FIRST_DATA_INDEX; i < data.GetLength(1)-_isPersonalized; i++){
					str.AppendFormat("<td>{0}</td>", data[0,i]);
				}
				str.Append("</tr>");
			}
			#endregion

			#region Corps du tableau
			//totale lines
			lineCssClass=L0_CSS;
			// Add 1 to avoid first line display
			for(i=FIRST_DATA_LINE+1; i <= FIRST_DATA_LINE+NB_YEAR+NB_OPTION; i++){

                str.AppendFormat("<tr class={0}><td", lineCssClass);
				if(i<1+FIRST_DATA_LINE){
					str.Append(" align=left");
				}
				str.AppendFormat(" nowrap>{0}</td>", data[i,0]);
				
				for(j=FIRST_DATA_INDEX; j < data.GetLength(1)-_isPersonalized; j++){
					if (data[i,j] != null)
                        AppendNumericData(_session, str, Convert.ToDouble(data[i, j]), IsMultiYearLine(i,FIRST_DATA_LINE,NB_YEAR,NB_OPTION), data[i,0].ToString(), false, fp);
					else
						str.Append("<td>-</td>");

				}
				str.Append("</tr>");
				lineCssClass=Light_L0_CSS;
			}

			//next lines
			string align ="";
			int labelIndex = 0;
			StringBuilder bufferHtml = new StringBuilder(5000);
			StringBuilder output = str;
			bool newLine = false;
			for(i = FIRST_DATA_LINE+NB_YEAR+NB_OPTION+1; i < data.GetLength(0); i++){
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
							if (IsMultiYearLine(i,FIRST_DATA_LINE,NB_YEAR,NB_OPTION))
								bufferHtml.Length = 0;
							output = bufferHtml;
							display = true;
						}
						if (display){
							if (!IsMultiYearLine(i,FIRST_DATA_LINE,NB_YEAR,NB_OPTION)){
								totalLine=false;
								lineCssClass = Light_L1_CSS;
								if (_isPersonalized>0){
									if ((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
										lineCssClass=Light_L1_COMPET_CSS;
									else if (((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
										lineCssClass=Light_L1_REF_CSS;
								}
								align = "";
							}
							else{
								align = " align=left ";
								totalLine=true;
								lineCssClass = L1_CSS;
								if (_isPersonalized>0){
									if ((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
										lineCssClass=L1_COMPET_CSS;
									else if (((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
										lineCssClass=L1_REF_CSS;
								}
							}
							newLine = true;
                            output.AppendFormat("<tr class={0}><td {1} nowrap>{2}</td>", lineCssClass, align,  data[i, j]);
							
							labelIndex = j;
							j = FIRST_DATA_INDEX-1;
						}
					}
					else if(j==L2_DATA_INDEX){
						if (_isPersonalized<1)
							display = true;
						else if (_session.PersonalizedElementsOnly && (CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.none)
							display = false;
						else
						{
							display = true;
						}
						if (display && data[i,j]!=null){
							str.Append(bufferHtml.ToString());
							bufferHtml.Length = 0;
							output = str;
							if (!IsMultiYearLine(i,FIRST_DATA_LINE,NB_YEAR,NB_OPTION))
							{
								totalLine=false;
								align = "";
								lineCssClass = Light_L2_CSS;
								if (_isPersonalized>0){
									if ((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
										lineCssClass=Light_L2_COMPET_CSS;
									else if (((CstWeb.AdvertiserPersonalisation.Type)data[i,data.GetLength(1)-1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
										lineCssClass=Light_L2_REF_CSS;
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
                            labelIndex = j;
							j = FIRST_DATA_INDEX-1;
						}
					}
					else {
						if (display){
							if(data[i,j]!=null){
                                AppendNumericData(_session, output, Convert.ToDouble(data[i, j]), IsMultiYearLine(i,FIRST_DATA_LINE,NB_YEAR,NB_OPTION),data[i,labelIndex].ToString(),totalLine, fp);
	
							}
							else if (j >= FIRST_DATA_INDEX)
								output.Append("<td>-</td>");
						}

					}
				}
				if (newLine)
					output.Append("</tr>");
			}
			#endregion

			#region fermeture des balise du tableau
			str.Append("</table>");
			#endregion
			
		}
        #endregion

    }

}
