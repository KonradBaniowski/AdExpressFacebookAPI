using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;

using CstClassif = TNS.AdExpress.Constantes.Classification;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstWeb = TNS.AdExpress.Constantes.Web;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Translation;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;

namespace TNS.AdExpressI.ProductClassReports.Engines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Classif2 X Period (monthly or yearly)
    /// </summary>
    public abstract class Engine_Classif1Classif2_X_Periods : Engine
    {

        #region Attributes
        /// <summary>
        /// Determine if the table must be computed with a monthly or a yearly detail
        /// </summary>
        protected bool _monthlyExtended;
        #endregion

        #region Accessors
        /// <summary>
        /// Get / Set the period breakdown (monthly or yearly)
        /// </summary>
        public bool MonthlyExtended
        {
            get
            {
                return _monthlyExtended;
            }
            set
            {
                _monthlyExtended = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public Engine_Classif1Classif2_X_Periods(WebSession session, int result) : base(session, result) { }
        #endregion

        #region Abstract methods implementation
        /// <summary>
        /// Compute data got from the DAL layer before to design the report
        /// </summary>
        /// <param name="data">DAL data</param>
        /// <returns>data computed from DAL result</returns>
        protected override object[,] ComputeData(DataSet dsData)
        {
		
			#region Variables
			object[,] data = null;

			DataTable dtData = dsData.Tables[0];
            if (dtData.Rows.Count <= 0) return null;

			#region Indexes relatives to classifications

			//First numerical data
			//Indexes of each classification
			int DATA_FIRST_PRODUCT_INDEX = -1;
			int DATA_FIRST_MEDIA_INDEX = -1;
			int DATA_FIRST_DATA_COLUMN = -1;
            for (int i = 0; i < dtData.Columns.Count; i = i + 2)
            {

                if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0 || dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0)
                {

                    if (dtData.Columns[i].ColumnName.IndexOf("ID_M") >= 0 && DATA_FIRST_MEDIA_INDEX < 0)
                    {
                        DATA_FIRST_MEDIA_INDEX = i;
                    }
                    else if (dtData.Columns[i].ColumnName.IndexOf("ID_P") >= 0 && DATA_FIRST_PRODUCT_INDEX < 0)
                    {
                        DATA_FIRST_PRODUCT_INDEX = i;
                    }
                }
                else
                {
                    DATA_FIRST_DATA_COLUMN = i;
                    break;
                }
            }

			// Delete useless lines
			if(dtData.Columns.Contains("id_advertiser")){
				_isPersonalized = 1;
			}
			CleanDataTable(dtData, DATA_FIRST_DATA_COLUMN);
			

			//Identify classification hierarchy
			int DATA_MAIN_CLASSIF_INDEX;
			int DATA_SECOND_CLASSIF_INDEX;
            CstClassif.Branch.type MAIN_CLASSIF_TYPE;
			CstClassif.Branch.type SECOND_CLASSIF_TYPE;
			if (DATA_FIRST_MEDIA_INDEX < DATA_FIRST_PRODUCT_INDEX){
				//Main classification  = media
				DATA_MAIN_CLASSIF_INDEX = DATA_FIRST_MEDIA_INDEX;
				MAIN_CLASSIF_TYPE = CstClassif.Branch.type.media;
				//Secondary classification = produit
				DATA_SECOND_CLASSIF_INDEX = DATA_FIRST_PRODUCT_INDEX;
				SECOND_CLASSIF_TYPE = CstClassif.Branch.type.product;
			}
			else{
                //Main classification = produit
				DATA_MAIN_CLASSIF_INDEX = DATA_FIRST_PRODUCT_INDEX;
				MAIN_CLASSIF_TYPE = CstClassif.Branch.type.product;
                //Secondary classification = media
				DATA_SECOND_CLASSIF_INDEX = DATA_FIRST_MEDIA_INDEX;
				SECOND_CLASSIF_TYPE = CstClassif.Branch.type.media;
			}

			//indexes and totals of main classif
			int[,] MAIN_CLASSIF_INDEXES = new int[DATA_SECOND_CLASSIF_INDEX/2,3];
			//Year N totals
			double[] MAIN_CLASSIF_TOTALS_N = new double[DATA_SECOND_CLASSIF_INDEX/2];
			//Year N-1 totals
			double[] MAIN_CLASSIF_TOTALS_N_1 = new double[DATA_SECOND_CLASSIF_INDEX/2];
			//Global totals of secondary classif for each level of main classif
			//(example : total of Gp > annonceurs for each level vehicle if type 4 table)
			ExtendedHashTable[] SCD_TOTALS_N = new ExtendedHashTable[MAIN_CLASSIF_INDEXES.GetLength(0)];
			ExtendedHashTable[] SCD_TOTALS_N_1 = new ExtendedHashTable[MAIN_CLASSIF_INDEXES.GetLength(0)];
			for(int i = 0; i < MAIN_CLASSIF_INDEXES.GetLength(0); i++)
			{
				//Index in data table from DAL layer
				MAIN_CLASSIF_INDEXES[i,0] = 2*i;
				//Key of current index
				MAIN_CLASSIF_INDEXES[i,1] = -1;
				//Bonus field
				MAIN_CLASSIF_INDEXES[i,2] = 0;
				//Year N total
				MAIN_CLASSIF_TOTALS_N[i] = 0.0;
				//Year N-1 total
				MAIN_CLASSIF_TOTALS_N_1[i] = 0.0;
			}

			//Indexes and totals of second classif
			int[,] SECONDARY_CLASSIF_INDEXES = new int[(DATA_FIRST_DATA_COLUMN-DATA_SECOND_CLASSIF_INDEX)/2,3];
			//Year N total
			double[] SECONDARY_CLASSIF_TOTALS_N = new double[(DATA_FIRST_DATA_COLUMN-DATA_SECOND_CLASSIF_INDEX)/2];
			//Year N-1 total
			double[] SECONDARY_CLASSIF_TOTALS_N_1 = new double[(DATA_FIRST_DATA_COLUMN-DATA_SECOND_CLASSIF_INDEX)/2];
			for(int i = 0; i < SECONDARY_CLASSIF_INDEXES.GetLength(0); i++)
			{
				//Index in data table
				SECONDARY_CLASSIF_INDEXES[i,0] = DATA_SECOND_CLASSIF_INDEX + 2*i;
				//Bonus field
				SECONDARY_CLASSIF_INDEXES[i,2] = 0;
				//Total
				SECONDARY_CLASSIF_TOTALS_N[i] = 0.0;
				SECONDARY_CLASSIF_TOTALS_N_1[i] = 0.0;
			}

			#endregion

			#region Indexes of columns with numerical data in result table
            //Indexes of year N, PDM, PDV, Year N-1, Evolution, Monthly data (_monthlyExtended), Type of classif and type of advertiser in result table
			//Year N
			int RESULT_N_INDEX = MAIN_CLASSIF_INDEXES.GetLength(0) + SECONDARY_CLASSIF_INDEXES.GetLength(0) - 1;
			
			//PDV
			int RESULT_PDV_INDEX = -10;
			if (_session.PDV) RESULT_PDV_INDEX = MAIN_CLASSIF_INDEXES.GetLength(0)+SECONDARY_CLASSIF_INDEXES.GetLength(0);
			//PDM
			int RESULT_PDM_INDEX = -10;
			if(_session.PDM){
				if (RESULT_PDV_INDEX>0)
					RESULT_PDM_INDEX = RESULT_PDV_INDEX + 1;
				else
					RESULT_PDM_INDEX = MAIN_CLASSIF_INDEXES.GetLength(0)+SECONDARY_CLASSIF_INDEXES.GetLength(0);
			}
			//Year N-1
			int RESULT_N_1_INDEX = -10;
			int NB_YEAR = 1;
			if (_session.ComparativeStudy){
				RESULT_N_1_INDEX = Math.Max(RESULT_PDM_INDEX, Math.Max(RESULT_PDV_INDEX,RESULT_N_INDEX)) + 1;
				NB_YEAR = 2;
			}
			//PDV N-1
			int RESULT_PDV_N_1_INDEX = -10;
			if (_session.ComparativeStudy && _session.PDV)
			{
				RESULT_PDV_N_1_INDEX = RESULT_N_1_INDEX + 1;
			}
			
			//PDM N-1
			int RESULT_PDM_N_1_INDEX = -10;
			if (_session.ComparativeStudy && _session.PDM)
			{
				RESULT_PDM_N_1_INDEX = Math.Max(RESULT_N_1_INDEX, RESULT_PDV_N_1_INDEX) + 1;
			}

			//Evol
			int RESULT_EVOL_INDEX = -10;
			if (_session.Evolution && _session.ComparativeStudy){
				RESULT_EVOL_INDEX = Math.Max(RESULT_PDM_N_1_INDEX,
					Math.Max(RESULT_N_1_INDEX, RESULT_PDV_N_1_INDEX)) + 1;
			}
			
			//Month
			int RESULT_MONTHS = -10;
			int RESULT_LAST_MONTHS = -10;

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string absolutePeriodEnd = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);
			
            if (_monthlyExtended) 
			{
				RESULT_MONTHS = Math.Max(RESULT_PDV_N_1_INDEX, 
					Math.Max(RESULT_PDM_N_1_INDEX, 
					Math.Max(RESULT_PDM_INDEX, 
					Math.Max(RESULT_PDV_INDEX,
					Math.Max(RESULT_N_INDEX, 
					Math.Max(RESULT_EVOL_INDEX, RESULT_N_1_INDEX)
					))))) + 1;
				RESULT_LAST_MONTHS = RESULT_MONTHS + (int.Parse(absolutePeriodEnd.Substring(4,2))-int.Parse(_session.PeriodBeginningDate.Substring(4,2)));
			}


			//Type of classif
			int RESULT_CLASSIF_TYPE_INDEX = Math.Max(RESULT_LAST_MONTHS, 
				Math.Max(RESULT_PDV_N_1_INDEX, 
				Math.Max(RESULT_PDM_N_1_INDEX, 
				Math.Max(RESULT_PDM_INDEX, 
				Math.Max(RESULT_PDV_INDEX,
				Math.Max(RESULT_N_INDEX, 
				Math.Max(RESULT_EVOL_INDEX, RESULT_N_1_INDEX)
				)))))) + 1;
			//Type of advertiser
			int RESULT_ADVERTISER_INDEX = RESULT_CLASSIF_TYPE_INDEX + 1;
			string tempString = "";
			List<long> referenceIDS = null;
            List<long> competitorIDS = null;
			if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].Contains(0)) {
				referenceIDS = _session.SecondaryProductUniverses[0].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
			}
			if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].Contains(0)) {
				competitorIDS = _session.SecondaryProductUniverses[1].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
            }
            if (referenceIDS == null)
                referenceIDS = new List<long>();
            if (competitorIDS == null)
                competitorIDS = new List<long>();
			#endregion
			
			#region Init result table
			int NB_LINE = 0;

			foreach(DataRow currentRow in dtData.Rows){
				
				//Foreach level of main classif
				for ( int i = 0; i < MAIN_CLASSIF_INDEXES.GetLength(0); i++){

					if(MAIN_CLASSIF_INDEXES[i,1] != int.Parse(currentRow[MAIN_CLASSIF_INDEXES[i,0]].ToString())){
						//Init lower levels
						for(int b = i+1; b < MAIN_CLASSIF_INDEXES.GetLength(0); b++)
						{
							MAIN_CLASSIF_INDEXES[b,1] = -1;
						}
						//Init secondary classif levels
						for(int b = 0; b < SECONDARY_CLASSIF_INDEXES.GetLength(0); b++)
						{
							SECONDARY_CLASSIF_INDEXES[b,1] = -1;
						}
						MAIN_CLASSIF_INDEXES[i,1] = Convert.ToInt32(currentRow[MAIN_CLASSIF_INDEXES[i,0]]);
						NB_LINE++;

						for ( int j = i+1; j < MAIN_CLASSIF_INDEXES.GetLength(1); j++){
							MAIN_CLASSIF_INDEXES[i,1] = -1 ;
						}

						//If lowest level
						if (i == MAIN_CLASSIF_INDEXES.GetUpperBound(0)){
							
							for (int j = 0; j < SECONDARY_CLASSIF_INDEXES.GetLength(0); j++){
								//for each level of second classif
								if (SECONDARY_CLASSIF_INDEXES[j,1] != Convert.ToInt32(currentRow[SECONDARY_CLASSIF_INDEXES[j,0]])){
									//Init lower levels
									for(int b = j+1; b < SECONDARY_CLASSIF_INDEXES.GetLength(0); b++)
									{
										SECONDARY_CLASSIF_INDEXES[b,1] = -1;
									}

									for(int k = 0; k < MAIN_CLASSIF_INDEXES.GetLength(0); k++){
										//each level of second classif will be present on a level of main classif
										NB_LINE++;
									}
								}
							}

						}
					}
				
				}

			}
			data = new object[NB_LINE+1, RESULT_ADVERTISER_INDEX+1];

			#endregion

			#endregion

			DataSet workDataSet;
			string conditions = "";
			string scndConditions = "";
			string sort = "";
			int currentLine = 0;
			int constComparativeStudy=0;
			string[] Keys;

			#region Construction du tableau
            for (int i = 0; i < MAIN_CLASSIF_INDEXES.GetLength(0); i++)
            {
                //clé de l'index courant
                MAIN_CLASSIF_INDEXES[i, 1] = -1;
            }

			foreach(DataRow currentRow in dtData.Rows){
				//For each row of from DAL layer

				for (int i = 0; i < MAIN_CLASSIF_INDEXES.GetLength(0); i++){
					//For each level of classif
                    int c = Convert.ToInt32(currentRow[MAIN_CLASSIF_INDEXES[i,0]]);
					if (MAIN_CLASSIF_INDEXES[i,1] != c){
						//if current level different from previous

						//Id of record
                        MAIN_CLASSIF_INDEXES[i, 1] = c;

						#region Numerical data of main classif
						//Insert current record
						//Label
						data[currentLine,i] = currentRow[MAIN_CLASSIF_INDEXES[i,0]+1].ToString();
						//Classif
						data[currentLine, RESULT_CLASSIF_TYPE_INDEX] = MAIN_CLASSIF_TYPE;
						//Advertiser
                        if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.product && dtData.Columns.Contains("id_advertiser")
                            && (_session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.advertiser.ToString())
                            || ((_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupAdvertiser
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupProduct
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.brand
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupBrand
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentAdvertiser
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentBrand
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.product
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentProduct
                            /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                             * */
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.sectorAdvertiser
                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.subSectorAdvertiser
                            )
                            && i == MAIN_CLASSIF_INDEXES.GetUpperBound(0)))
                            )
                        {
                            long idAdv = Convert.ToInt64(currentRow["id_advertiser"]);
                            if (referenceIDS.Contains(idAdv))
                                data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.reference;
                            else if (competitorIDS.Contains(idAdv))
                                data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.competitor;
                            else
                                data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.none;
                        }
                        else
                            data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.none;
						#endregion

						workDataSet = new DataSet();
						//Build condition and sort clauses to get line smatching ids of current and higher levels
						conditions = "";
						for (int j = 0; j <= i ; j++){
							conditions = string.Format("{0}{1}{2}={3} ", conditions, ((j > 0) ? " AND " : ""), dtData.Columns[MAIN_CLASSIF_INDEXES[j,0]].ColumnName, MAIN_CLASSIF_INDEXES[j,1]);
						}

						//Sort on second classif levels
						sort = "";
						for (int j = 0; j < SECONDARY_CLASSIF_INDEXES.GetLength(0) ; j++){
							sort = string.Format("{0}{1}{2},{3}", sort, ( (j > 0) ? " ," : ""), dtData.Columns[SECONDARY_CLASSIF_INDEXES[j,0]+1].ColumnName, dtData.Columns[SECONDARY_CLASSIF_INDEXES[j,0]].ColumnName);
						}


						//Get lines matching conditions
						workDataSet.Merge(dtData.Select(conditions, sort));

						#region Data of main classif
						//Insret numerical data of current line in table result
						//Year N
						data[currentLine, RESULT_N_INDEX] = MAIN_CLASSIF_TOTALS_N[i] 
                            = Convert.ToDouble(dtData.Compute( string.Format("sum({0})", dtData.Columns[DATA_FIRST_DATA_COLUMN].ColumnName),conditions));


                        //Year N-1
						if(RESULT_N_1_INDEX > 0)
							data[currentLine, RESULT_N_1_INDEX] = MAIN_CLASSIF_TOTALS_N_1[i]
                                = Convert.ToDouble(dtData.Compute( string.Format("sum({0})", dtData.Columns[DATA_FIRST_DATA_COLUMN+1].ColumnName),conditions));

						//PDM Year N
						if (RESULT_PDM_INDEX > -1){
							if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media){
								data[currentLine, RESULT_PDM_INDEX] = (i!=0)?
									100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / MAIN_CLASSIF_TOTALS_N[i-1]
									:
									100.0;
								}
							else{
								data[currentLine, RESULT_PDM_INDEX] = 100.0;
							}
						}
						//PDM Year N-1
						if (RESULT_PDM_N_1_INDEX>-1)
						{
							if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media)
							{
								data[currentLine, RESULT_PDM_N_1_INDEX] = (i!=0)?
                                    100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / MAIN_CLASSIF_TOTALS_N_1[i-1]
									:
									100.0;
							}
							else
							{
								data[currentLine, RESULT_PDM_N_1_INDEX] = 100.0;
							}
						}

						//PDV
						if (RESULT_PDV_INDEX>-1)
						{
							if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media){
								data[currentLine, RESULT_PDV_INDEX] = 100.0;
							}
							else{
								data[currentLine, RESULT_PDV_INDEX] = (i!=0)?
                                    100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / MAIN_CLASSIF_TOTALS_N[i-1]
									:
									100.0;
							}
						}
						//PDV Year N-1
						if (RESULT_PDV_N_1_INDEX>-1)
						{
							if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media)
							{
								data[currentLine, RESULT_PDV_N_1_INDEX] = 100.0;
							}
							else
							{
								data[currentLine, RESULT_PDV_N_1_INDEX] = (i!=0)?
                                    100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / MAIN_CLASSIF_TOTALS_N_1[i-1]
									:
									100.0;
							}
						}
						//Evolution
						if (RESULT_EVOL_INDEX>0)
                            data[currentLine, RESULT_EVOL_INDEX] = 100 * (Convert.ToDouble(data[currentLine, RESULT_N_INDEX])
                                - Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]))
                                / Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]);

						//Monthes of year N
						if (_monthlyExtended)
						{
							for (int j = 0; j <= (RESULT_LAST_MONTHS - RESULT_MONTHS); j++)
							{
                                data[currentLine, RESULT_MONTHS + j] = Convert.ToDouble(dtData.Compute( string.Format("sum({0})", dtData.Columns[DATA_FIRST_DATA_COLUMN+NB_YEAR+j].ColumnName),conditions));
							}
						}


						//Inc line in result table
						currentLine++;

						#endregion

						#region Nomenclature secondaire
						//Init second classif levels
 						for(int j = 0; j < SECONDARY_CLASSIF_INDEXES.GetLength(0); j++){
							SECONDARY_CLASSIF_INDEXES[j,1] = -1;
						}


						SCD_TOTALS_N[i] = new ExtendedHashTable();
						if (RESULT_N_1_INDEX>0)
							SCD_TOTALS_N_1[i] = new ExtendedHashTable();


						foreach(DataRow secondaryRow in workDataSet.Tables[0].Rows)
						{
							//for each lines matching current id and higher levels

							scndConditions = conditions;

                            for (int j = 0; j < SECONDARY_CLASSIF_INDEXES.GetLength(0); j++)
                            {
                                //For each level of second classif
                                int c2 = Convert.ToInt32(secondaryRow[SECONDARY_CLASSIF_INDEXES[j, 0]]);
                                if (SECONDARY_CLASSIF_INDEXES[j, 1] != c2)
                                {
                                    //If current id different from previous

                                    for (int b = j + 1; b < SECONDARY_CLASSIF_INDEXES.GetLength(0); b++)
                                    {
                                        SECONDARY_CLASSIF_INDEXES[b, 1] = -1;
                                    }

                                    SECONDARY_CLASSIF_INDEXES[j, 1] = c2;

                                    for (int k = 0; k <= j; k++)
                                    {
                                        scndConditions = string.Format("{0} AND {1}={2}", scndConditions, dtData.Columns[SECONDARY_CLASSIF_INDEXES[k, 0]].ColumnName, SECONDARY_CLASSIF_INDEXES[k, 1]);
                                    }

                                    //Insert in table result
                                    //label
                                    data[currentLine, i + j] = secondaryRow[SECONDARY_CLASSIF_INDEXES[j, 0] + 1].ToString();

                                    //classif
                                    data[currentLine, RESULT_CLASSIF_TYPE_INDEX] = SECOND_CLASSIF_TYPE;

                                    //year N
                                    data[currentLine, RESULT_N_INDEX] = Convert.ToDouble(workDataSet.Tables[0].Compute(string.Format("sum({0})", dtData.Columns[DATA_FIRST_DATA_COLUMN].ColumnName), scndConditions));

                                    //current total N
                                    SECONDARY_CLASSIF_TOTALS_N[j] = Convert.ToDouble(data[currentLine, RESULT_N_INDEX]);

                                    //totals in main classif
                                    Keys = new string[j + 1];
                                    for (int k = 0; k <= j; k++)
                                    {
                                        Keys[k] = secondaryRow[SECONDARY_CLASSIF_INDEXES[k, 0]].ToString();
                                    }
                                    SCD_TOTALS_N[i].Add(Convert.ToDouble(data[currentLine, RESULT_N_INDEX]), Keys);

                                    //year N-1
                                    if (RESULT_N_1_INDEX > 0)
                                    {
                                        data[currentLine, RESULT_N_1_INDEX] = Convert.ToDouble(workDataSet.Tables[0].Compute(string.Format("sum({0})", dtData.Columns[DATA_FIRST_DATA_COLUMN + 1].ColumnName), scndConditions));

                                        //current N-1 total
                                        SECONDARY_CLASSIF_TOTALS_N_1[j] = Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]);
                                        //total in main classif
                                        SCD_TOTALS_N_1[i].Add(Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]), Keys);
                                    }

                                    //PDM year N
                                    if (RESULT_PDM_INDEX > -1)
                                    {
                                        if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media)
                                        {
                                            data[currentLine, RESULT_PDM_INDEX] = (i != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / SCD_TOTALS_N[i - 1].GetValue(Keys)
                                                :
                                                100.0;
                                        }
                                        else
                                        {
                                            data[currentLine, RESULT_PDM_INDEX] = (j != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / SECONDARY_CLASSIF_TOTALS_N[j - 1]
                                                :
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / MAIN_CLASSIF_TOTALS_N[i]
                                                ;
                                        }
                                    }
                                    //PDM year N-1
                                    if (RESULT_PDM_N_1_INDEX > -1)
                                    {
                                        if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media)
                                        {
                                            data[currentLine, RESULT_PDM_N_1_INDEX] = (i != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / SCD_TOTALS_N_1[i - 1].GetValue(Keys)
                                                :
                                                100.0;
                                        }
                                        else
                                        {
                                            data[currentLine, RESULT_PDM_N_1_INDEX] = (j != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / SECONDARY_CLASSIF_TOTALS_N_1[j - 1]
                                                :
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / MAIN_CLASSIF_TOTALS_N_1[i]
                                                ;
                                        }
                                    }


                                    //PDV
                                    if (RESULT_PDV_INDEX > -1)
                                    {
                                        if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media)
                                        {
                                            data[currentLine, RESULT_PDV_INDEX] = (j != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / SECONDARY_CLASSIF_TOTALS_N[j - 1]
                                                :
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / MAIN_CLASSIF_TOTALS_N[i]
                                                ;
                                        }
                                        else
                                        {
                                            data[currentLine, RESULT_PDV_INDEX] = (i != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_INDEX]) / SCD_TOTALS_N[i - 1].GetValue(Keys)
                                                :
                                                100.0;
                                        }
                                    }
                                    //PDV N-1
                                    if (RESULT_PDV_N_1_INDEX > -1)
                                    {
                                        if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media)
                                        {
                                            data[currentLine, RESULT_PDV_N_1_INDEX] = (j != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / SECONDARY_CLASSIF_TOTALS_N_1[j - 1]
                                                :
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / MAIN_CLASSIF_TOTALS_N_1[i]
                                                ;
                                        }
                                        else
                                        {
                                            data[currentLine, RESULT_PDV_N_1_INDEX] = (i != 0) ?
                                                100 * Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]) / SCD_TOTALS_N_1[i - 1].GetValue(Keys)
                                                :
                                                100.0;
                                        }
                                    }

                                    //Evolution
                                    if (RESULT_EVOL_INDEX > 0)
                                        data[currentLine, RESULT_EVOL_INDEX] = 100 *
                                            (Convert.ToDouble(data[currentLine, RESULT_N_INDEX])
                                            - Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]))
                                            / Convert.ToDouble(data[currentLine, RESULT_N_1_INDEX]);

                                    //Moonthes of year N
                                    if (_monthlyExtended)
                                    {
                                        for (int k = 0; k <= (RESULT_LAST_MONTHS - RESULT_MONTHS); k++)
                                        {
                                            //!!!!!!!!!!!
                                            if (_session.ComparativeStudy)
                                                constComparativeStudy = 2;
                                            else constComparativeStudy = 1;

                                            data[currentLine, RESULT_MONTHS + k] = Convert.ToDouble(workDataSet.Tables[0].Compute(string.Format("sum({0})", dtData.Columns[DATA_FIRST_DATA_COLUMN + constComparativeStudy + k].ColumnName), scndConditions));

                                        }
                                    }
                                    //advertiser
                                    if (SECOND_CLASSIF_TYPE == CstClassif.Branch.type.product && workDataSet.Tables[0].Columns.Contains("id_advertiser")
                                        && (_session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.advertiser.ToString())
                                        || ((_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupAdvertiser
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupProduct
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.brand
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupBrand
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentAdvertiser
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentBrand
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentProduct
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.product
                                        /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                                        * */
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.sectorAdvertiser
                                        || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.subSectorAdvertiser
                                        )
                                        && j == SECONDARY_CLASSIF_INDEXES.GetUpperBound(0)))
                                        )
                                    {
                                        int idAdv = Convert.ToInt32(secondaryRow["id_advertiser"]);
                                        if (referenceIDS.Contains(idAdv))
                                            data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.reference;
                                        else if (competitorIDS.Contains(idAdv))
                                            data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.competitor;
                                        else
                                            data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.none;
                                    }
                                    else
                                        data[currentLine, RESULT_ADVERTISER_INDEX] = CstWeb.AdvertiserPersonalisation.Type.none;

                                    //Inc line in result table
                                    currentLine++;
                                }

                            }

						}

						#endregion

					}

				}
			}
			data[currentLine,0] = new TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd();
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

			//Column indexes
			//Year N
			int N_COLUMN = 0;
			do
			{
				N_COLUMN++;
			}
			while(N_COLUMN < data.GetLength(1) && data[0,N_COLUMN]==null);

			//PDV
			int PDV_COLUMN = -1;
			if (_session.PDV)
				PDV_COLUMN = N_COLUMN + 1;

			//PDM
			int PDM_COLUMN = -1;
			if (_session.PDM)
				PDM_COLUMN = Math.Max(PDV_COLUMN, N_COLUMN) + 1;

			//N-1
			int N_1_COLUMN = -1;
			int EVOL_COLUMN = -1;
			int PDM_N_1_COLUMN = -1;
			int PDV_N_1_COLUMN = -1;
			if (_session.ComparativeStudy)
			{
				N_1_COLUMN = Math.Max(PDV_COLUMN, Math.Max(PDM_COLUMN, N_COLUMN)) + 1;
				//PDV N-1
				if(_session.PDV)
					PDV_N_1_COLUMN = N_1_COLUMN + 1;
				//PDM N-1
				if (_session.PDM)
					PDM_N_1_COLUMN = Math.Max(PDV_N_1_COLUMN, N_1_COLUMN) + 1;
				//Evol
				if (_session.Evolution)
					EVOL_COLUMN = Math.Max(N_1_COLUMN,Math.Max(PDV_N_1_COLUMN, PDM_N_1_COLUMN)) + 1;
			}

			//Monthes of N
			int MONTH_COLUMN = -1;
			int LAST_MONTH_COLUMN = -2;

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string absolutePeriodEnd = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

            if (_monthlyExtended)
			{
				MONTH_COLUMN = Math.Max(EVOL_COLUMN,
					Math.Max(PDV_N_1_COLUMN,
					Math.Max(PDM_N_1_COLUMN,
					Math.Max(N_1_COLUMN,
					Math.Max(PDV_COLUMN,
					Math.Max(PDM_COLUMN, N_COLUMN)
					))))) + 1;
                LAST_MONTH_COLUMN = MONTH_COLUMN + Convert.ToInt32(absolutePeriodEnd.Substring(4, 2)) - Convert.ToInt32(_session.PeriodBeginningDate.Substring(4, 2));
			}

			//Type of advertiser
			int ADVERTISER_COLUMN = data.GetLength(1)-1;

			//Classif of the line
			int CLASSIF_TYPE_COLUMN = ADVERTISER_COLUMN - 1;


			int MEDIA_LEVEL_NUMBER;
			switch(_session.PreformatedMediaDetail)
			{
				case CstFormat.PreformatedMediaDetails.vehicle:
					MEDIA_LEVEL_NUMBER = 1;
					break;
                case CstFormat.PreformatedMediaDetails.vehicleCategory:
                case CstFormat.PreformatedMediaDetails.vehicleMedia:
					MEDIA_LEVEL_NUMBER = 2;
					break;
				default:
					MEDIA_LEVEL_NUMBER = 3;
					break;
			}

			int PRODUCT_LEVEL_NUMBER;
			switch(_session.PreformatedProductDetail)
			{
				case CstFormat.PreformatedProductDetails.advertiser:
				case CstFormat.PreformatedProductDetails.brand:
				case CstFormat.PreformatedProductDetails.group:
				case CstFormat.PreformatedProductDetails.product:
                /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                * */
                case CstFormat.PreformatedProductDetails.sector:
                case CstFormat.PreformatedProductDetails.subSector:
					PRODUCT_LEVEL_NUMBER = 1;
					break;
				default:
					PRODUCT_LEVEL_NUMBER = 2;
					break;
			}

			//Classifications and CSS
			CstClassif.Branch.type MAIN_CLASSIF_TYPE;

			int MAIN_LEVEL_L1 = 0;
			int MAIN_LEVEL_L2 = 1;
			int MAIN_LEVEL_L3 = 2;
			int SCD_LEVEL_L1 = 0;
			int SCD_LEVEL_L2 = 1;
			int SCD_LEVEL_L3 = 2;

			string MAIN_L1_CSS="";
			string MAIN_L2_CSS="";
			string MAIN_L3_CSS="";
			string SCD_L1_CSS="";
			string SCD_L2_CSS="";
			string SCD_L3_CSS="";

			string MAIN_L1_REF_CSS="";
			string MAIN_L2_REF_CSS="";
			string MAIN_L3_REF_CSS="";
			string SCD_L1_REF_CSS="";
			string SCD_L2_REF_CSS="";
			string SCD_L3_REF_CSS="";

			string MAIN_L1_CON_CSS="";
			string MAIN_L2_CON_CSS="";
			string MAIN_L3_CON_CSS="";
			string SCD_L1_CON_CSS="";
			string SCD_L2_CON_CSS="";
			string SCD_L3_CON_CSS="";

			string HEADER_CSS = "astd0";
			if (_session.PreformatedTable != CstFormat.PreformatedTables.mediaProduct_X_Year 
				&& _session.PreformatedTable != CstFormat.PreformatedTables.mediaProduct_X_YearMensual )
			{
				if(!_excel)
				{ //css str
					//report produit =+> support
					MAIN_CLASSIF_TYPE = CstClassif.Branch.type.product;
					MAIN_L1_CSS = "asl0";
					MAIN_L1_REF_CSS = "asl0r";
					MAIN_L1_CON_CSS = "asl0c";
					MAIN_L2_CSS = "asl3";
					MAIN_L2_REF_CSS = "asl3r";
					MAIN_L2_CON_CSS = "asl3c";
					MAIN_L3_CSS = "asl3b";
					MAIN_L3_CON_CSS = "asl3bc";
					MAIN_L3_REF_CSS = "asl3br";
					SCD_L1_CSS = "asl2";
					SCD_L2_CSS = "asl5";
					SCD_L3_CSS = "asl5b";
					MAIN_LEVEL_L1 = 0;
					MAIN_LEVEL_L2 = 1;
					MAIN_LEVEL_L3 = (PRODUCT_LEVEL_NUMBER>1)?2:-1;
					SCD_LEVEL_L1 = 0;
					//lower level if pluri or secified by detail
					SCD_LEVEL_L2 = (MEDIA_LEVEL_NUMBER>1)?1:-1;
					SCD_LEVEL_L3 = (MEDIA_LEVEL_NUMBER>2)?2:-1;;
				}
				else
				{ // css _excel
					HEADER_CSS = "astd0x";
                    //report produit =+> support
					MAIN_CLASSIF_TYPE = CstClassif.Branch.type.product;
					MAIN_L1_CSS = "asl0";
					MAIN_L1_REF_CSS = "asl0rx";
					MAIN_L1_CON_CSS = "asl0cx";
					MAIN_L2_CSS = "asl3x";
					MAIN_L2_REF_CSS = "asl3rx";
					MAIN_L2_CON_CSS = "asl3cx";
					MAIN_L3_CSS = "asl3bx";
					MAIN_L3_CON_CSS = "asl3bcx";
					MAIN_L3_REF_CSS = "asl3brx";
					SCD_L1_CSS = "asl2x";
					SCD_L2_CSS = "asl5x";
					SCD_L3_CSS = "asl5bx";
					MAIN_LEVEL_L1 = 0;
					MAIN_LEVEL_L2 = 1;
					MAIN_LEVEL_L3 = (PRODUCT_LEVEL_NUMBER>1)?2:-1;
					SCD_LEVEL_L1 = 0;
                    //lower level if pluri or secified by detail
                    SCD_LEVEL_L2 = (MEDIA_LEVEL_NUMBER > 1) ? 1 : -1;
					SCD_LEVEL_L3 = (MEDIA_LEVEL_NUMBER>2)?2:-1;;
				}
			}
			else
			{
				if (!_excel)
				{ //css str
					//report support =+> produit
					MAIN_CLASSIF_TYPE = CstClassif.Branch.type.media;
					MAIN_L1_CSS = "asl0";
					MAIN_L1_REF_CSS = "asl0r";
					MAIN_L1_CON_CSS = "asl0c";
                    MAIN_L2_CSS = (VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimedia
                        || VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimediaExtended)
						?"asl2":"asl5";
					MAIN_L3_CSS = "asl5b";
					SCD_L1_CSS = "asl3";
					SCD_L1_REF_CSS = "asl3r";
					SCD_L1_CON_CSS = "asl3c";
					SCD_L2_REF_CSS = "asl3br";
					SCD_L2_CON_CSS = "asl3bc";
					SCD_L2_CSS = "asl3b";
					SCD_L3_CSS = "asl5";
					SCD_L3_REF_CSS = "asl5r";
					SCD_L3_CON_CSS = "asl5b";
					MAIN_LEVEL_L1 = 0;
					MAIN_LEVEL_L2 = (MEDIA_LEVEL_NUMBER>1 ||
                        VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimedia
                        || VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimediaExtended

                        ) ?1:-1;
					MAIN_LEVEL_L3 = (MEDIA_LEVEL_NUMBER>2)?2:-1;
					SCD_LEVEL_L1 = 0;
					SCD_LEVEL_L2 = (PRODUCT_LEVEL_NUMBER>1)?1:-1;
					SCD_LEVEL_L3 = -1;
				}
				else
				{ //css _excel
					HEADER_CSS = "astd0x";
					//report support =+> produit
					MAIN_CLASSIF_TYPE = CstClassif.Branch.type.media;
					MAIN_L1_CSS = "asl0";
					MAIN_L1_REF_CSS = "asl0rx";
					MAIN_L1_CON_CSS = "asl0cx";
                    MAIN_L2_CSS = (VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimedia
                        || VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimediaExtended
                        )
						?"asl2x":"asl5x";
					MAIN_L3_CSS = "asl5bx";
					SCD_L1_CSS = "asl3x";
					SCD_L1_REF_CSS = "asl3rx";
					SCD_L1_CON_CSS = "asl3cx";
					SCD_L2_REF_CSS = "asl3brx";
					SCD_L2_CON_CSS = "asl3bcx";
					SCD_L2_CSS = "asl3bx";
					SCD_L3_CSS = "asl5x";
					SCD_L3_REF_CSS = "asl5rx";
					SCD_L3_CON_CSS = "asl5bx";
					MAIN_LEVEL_L1 = 0;
					MAIN_LEVEL_L2 = (MEDIA_LEVEL_NUMBER>1 ||
                        VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimedia
                        || VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstClassif.DB.Vehicles.names.plurimediaExtended

                        ) ?1:-1;
					MAIN_LEVEL_L3 = (MEDIA_LEVEL_NUMBER>2)?2:-1;
					SCD_LEVEL_L1 = 0;
					SCD_LEVEL_L2 = (PRODUCT_LEVEL_NUMBER>1)?1:-1;
					SCD_LEVEL_L3 = -1;
				}
			}

			#endregion

			#region Table begin
			str.Append("<table>");
			#endregion

			#region Headers
            str.AppendFormat("<tr class={0}>", HEADER_CSS);
			if (_session.PreformatedTable != CstFormat.PreformatedTables.media_X_Year)
                str.AppendFormat("<td colspan=2>{0}</td>", GestionWeb.GetWebWord(1164, _session.SiteLanguage));
			else
                str.AppendFormat("<td>{0}</td>", GestionWeb.GetWebWord(1357, _session.SiteLanguage));
            str.AppendFormat("<td>{0}</td>", _session.PeriodBeginningDate.Substring(0, 4));
			//PDV
			if (_session.PDV)
                str.AppendFormat("<td>{0} {1}</td>", GestionWeb.GetWebWord(1166, _session.SiteLanguage), _session.PeriodBeginningDate.Substring(0, 4));
			//PDM
			if (_session.PDM)
                str.AppendFormat("<td>{0} {1}</td>", GestionWeb.GetWebWord(806, _session.SiteLanguage), _session.PeriodBeginningDate.Substring(0, 4));
			//N-1
			if (_session.ComparativeStudy)
			{
                str.AppendFormat("<td>{0}</td>", Convert.ToInt32(_session.PeriodBeginningDate.Substring(0, 4)) - 1);
				//PDV N-1
				if (_session.PDV)
                    str.AppendFormat("<td>{0} {1}</td>", GestionWeb.GetWebWord(1166, _session.SiteLanguage), int.Parse(_session.PeriodBeginningDate.Substring(0, 4)) - 1);
				//PDM N-1
				if (_session.PDM)
                    str.AppendFormat("<td>" + GestionWeb.GetWebWord(806, _session.SiteLanguage) + " " + (int.Parse(_session.PeriodBeginningDate.Substring(0, 4)) - 1) + "</td>");
				//Evol
				if (_session.Evolution)
                    str.AppendFormat("<td>" + GestionWeb.GetWebWord(1168, _session.SiteLanguage) + "</td>");
			}
			//Months
			if(_monthlyExtended)
			{
				for( int j = 0; j <= (int.Parse(absolutePeriodEnd.Substring(4,2)) - int.Parse(_session.PeriodBeginningDate.Substring(4,2))); j++)
				{
					CultureInfo cInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].Localization);
					str.AppendFormat("<td>{0}</td>", MonthString.GetCharacters(FctUtilities.Dates.GetPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType).AddMonths(j).Month, cInfo, 0));
					//str.AppendFormat("<td>{0}</td>", FctUtilities.Dates.getPeriodBeginningDate(_session.PeriodBeginningDate, _session.PeriodType).AddMonths(j).ToString("MMMM"));
				}
			}
			str.Append("</tr>");
			#endregion

			#region Append data
			string offset = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
			string mainOffset = "&nbsp;&nbsp;&nbsp;";
			string LINE_CSS;
			double tmpData = 0.0;
			int father_level = 0;
			bool displayFather = true;
			bool dysplayChild = true;

			StringBuilder outputHtml = str;
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;


			bool newLine = false;

			//Variables used to display personalised elements
			//bufferHtml is used to redirect code output to a temporary buffer
			StringBuilder bufferHtml = new StringBuilder(50000);
			//Temporary container for the header of the second classif
			StringBuilder scndHeaderHtml = new StringBuilder(50000);
			bool addScndData = false;

            int i = 0;
			while( i < data.GetLength(0))
			{

				newLine = false;

				if(data[i,0]!= null)
				{
					if(data[i,0].GetType() == typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd))
						break;
				}

				if (MAIN_CLASSIF_TYPE == (CstClassif.Branch.type)data[i, CLASSIF_TYPE_COLUMN])
				{

					displayFather = true;
	
					for(int j = 0; j < data.GetLength(1); j++)
					{

						if (data[i,j] != null)
						{
							if (j == MAIN_LEVEL_L1) 
							{
								LINE_CSS = MAIN_L1_CSS;
								newLine = true;
                                outputHtml.AppendFormat("<tr class={0} style=\"FONT-WEIGHT: bold\">", LINE_CSS);
                                outputHtml.AppendFormat("<td colspan=2 align=left nowrap>{0}</td>", data[i, j]);
								father_level = j;
								j = N_COLUMN-1;
							}
							else if(j == MAIN_LEVEL_L2) 
							{
								if ( CstWeb.AdvertiserPersonalisation.Type.none != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
								{
									if ( CstWeb.AdvertiserPersonalisation.Type.competitor != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
										LINE_CSS = MAIN_L2_REF_CSS;
									else
										LINE_CSS = MAIN_L2_CON_CSS;
								}
								else
								{
									LINE_CSS = MAIN_L2_CSS;
									if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.product && _session.PersonalizedElementsOnly)
									{
										if (_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupBrand ||
											_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupProduct ||
											_session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.groupAdvertiser
											|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentAdvertiser
											//toto
											|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.product
											|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentBrand
											|| _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.segmentProduct
                                            /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                                             **/
                                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.sectorAdvertiser
                                            || _session.PreformatedProductDetail == CstFormat.PreformatedProductDetails.subSectorAdvertiser
                                            )
										{
											bufferHtml.Length = 0;
											outputHtml = bufferHtml;
										}
										//reference or competitor
										if (_session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.advertiser.ToString())
											||_session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.brand.ToString())
											)
											displayFather = false;
									}
									else if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media && _session.PersonalizedElementsOnly
										&& _session.PreformatedProductDetail != CstFormat.PreformatedProductDetails.groupSegment)
									{
										bufferHtml.Length = 0;
										outputHtml = bufferHtml;
									}
								}
								if (displayFather)
								{
									newLine = true;
									outputHtml.AppendFormat("<tr class={0} style=\"FONT-WEIGHT: bold\">", LINE_CSS);
                                    outputHtml.AppendFormat("<td colspan=2 align=left nowrap>{0}</td>", data[i, j]);
									father_level = j;
									j = N_COLUMN-1;
								}
							}
							else if(j == MAIN_LEVEL_L3) 
							{
								//display
								if (displayFather)
								{
									if ( CstWeb.AdvertiserPersonalisation.Type.none != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
									{
										outputHtml = str;
										str.Append(bufferHtml.ToString());
										bufferHtml.Length = 0;
										if ( CstWeb.AdvertiserPersonalisation.Type.competitor != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
											LINE_CSS = MAIN_L3_REF_CSS;
										else
											LINE_CSS = MAIN_L3_CON_CSS;
									}
									else
									{
										LINE_CSS = MAIN_L3_CSS;
										//reference or competitor
										if (MAIN_CLASSIF_TYPE == CstClassif.Branch.type.product && _session.PersonalizedElementsOnly
											&& _session.PreformatedProductDetail != CstFormat.PreformatedProductDetails.groupSegment)
											displayFather = false;
									}
									if (displayFather)
									{
										newLine = true;
                                        outputHtml.AppendFormat("<tr class={0} style=\"FONT-WEIGHT: bold\">", LINE_CSS);
                                        outputHtml.AppendFormat("<td colspan=2 align=left nowrap>{0}{1}</td>", mainOffset, data[i, j]);
										father_level = j;
										j = N_COLUMN-1;
									}
								}
							}
							else if ((j == N_COLUMN || j == N_1_COLUMN || (j >= MONTH_COLUMN && j <= LAST_MONTH_COLUMN) ) && displayFather) 
							{
								//year
								tmpData = Convert.ToDouble(data[i,j]);
								outputHtml.AppendFormat("<td nowrap>{0}</td>", (!double.IsNaN(tmpData) && tmpData!=0) ? FctUtilities.Units.ConvertUnitValueToString(tmpData,_session.Unit, fp) : "");
							}
							else if ( (j == PDV_COLUMN || j == PDM_COLUMN || j== PDM_N_1_COLUMN || j == PDV_N_1_COLUMN)  && displayFather ) 
							{
								//pourcentage
								tmpData = Convert.ToDouble(data[i,j]);
                                outputHtml.AppendFormat("<td nowrap>{0}</td>", (tmpData == 0 || Double.IsNaN(tmpData) || Double.IsInfinity(tmpData)) ? "" : (FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %"));
							}
							else if ( j == EVOL_COLUMN && displayFather) 
							{
								if (!_excel)
								{
									//evol
									tmpData = Convert.ToDouble(data[i,j]);
									if (tmpData>0) //rise
                                        outputHtml.AppendFormat("<td nowrap>{0}<img src=/I/g.gif></td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
									else if(tmpData<0) //slide
                                        outputHtml.AppendFormat("<td nowrap>{0}<img src=/I/r.gif></td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
									else if (!Double.IsNaN(tmpData)) // 0 exactly
                                        outputHtml.AppendFormat("<td nowrap>0 %<img src=/I/o.gif></td>");
									else
										outputHtml.Append("<td></td>");
								}
								else if (_excel)
								{
									//evol
									tmpData = double.Parse(data[i,j].ToString());
									if (tmpData>0) //rise
										outputHtml.AppendFormat("<td nowrap>{0}</td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
									else if(tmpData<0) //slide
                                        outputHtml.AppendFormat("<td nowrap>{0}</td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
									else if (!Double.IsNaN(tmpData))
                                        outputHtml.AppendFormat("<td nowrap> 0 %</td>");
									else
										outputHtml.Append("<td></td>");
								}
							}
						}
					}
					if (newLine)
						outputHtml.Append("</tr>");
				}
				else
				{
					if (displayFather)
					{
						StringBuilder dataOutput = outputHtml;
						dysplayChild = true;
						addScndData = false;
						for(int j = 0; j < data.GetLength(1); j++)
						{

							if (data[i,j] != null)
							{

								if (j-father_level == SCD_LEVEL_L1) 
								{
									scndHeaderHtml.Length = 0;
									if ( CstWeb.AdvertiserPersonalisation.Type.none != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
									{
										dataOutput = outputHtml = str;
										outputHtml.Append(bufferHtml.ToString());
										bufferHtml.Length = 0;
										if ( CstWeb.AdvertiserPersonalisation.Type.competitor != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
											LINE_CSS = (father_level!=0)?SCD_L1_REF_CSS:MAIN_L1_REF_CSS;
										else
											LINE_CSS = (father_level!=0)?SCD_L1_CON_CSS:MAIN_L1_CON_CSS;
									}
									else
									{
										LINE_CSS = (father_level!=0)?SCD_L1_CSS:MAIN_L1_CSS;
										if (_session.PersonalizedElementsOnly && MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media
											&& _session.PreformatedProductDetail != CstFormat.PreformatedProductDetails.groupSegment)
										{
											dysplayChild=false;
											newLine = true;
                                            scndHeaderHtml.AppendFormat("<tr Font-Italic=True class={0} style=\"FONT-WEIGHT: normal\">", LINE_CSS);
                                            scndHeaderHtml.AppendFormat("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>{0}{1}</td>", offset, data[i, j]);
											addScndData = true;
											dataOutput = scndHeaderHtml;
											j = N_COLUMN-1;
										}
									}

									if (dysplayChild)
									{
										newLine = true;
                                        outputHtml.AppendFormat("<tr Font-Italic=True class={0} style=\"FONT-WEIGHT: normal\">", LINE_CSS);
                                        outputHtml.AppendFormat("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>{0}{1}</td>", offset, data[i, j]);
										j = N_COLUMN-1;
									}
								}
								else if(j-father_level == SCD_LEVEL_L2) 
								{

									if ( CstWeb.AdvertiserPersonalisation.Type.none != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
									{
										dataOutput = outputHtml = str;
										outputHtml.Append(bufferHtml.ToString());
										bufferHtml.Length = 0;
										if ( CstWeb.AdvertiserPersonalisation.Type.competitor != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
											LINE_CSS = (father_level!=0)?SCD_L2_REF_CSS:MAIN_L1_REF_CSS;
										else
											LINE_CSS = (father_level!=0)?SCD_L2_CON_CSS:MAIN_L1_CON_CSS;
									}
									else
									{
										LINE_CSS = (father_level!=0)?SCD_L2_CSS:MAIN_L1_CSS;
										if (_session.PersonalizedElementsOnly && MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media
											&& _session.PreformatedProductDetail != CstFormat.PreformatedProductDetails.groupSegment)
											dysplayChild=false;
									}
								
									if (dysplayChild)
									{
										newLine = true;
										outputHtml.Append(scndHeaderHtml.ToString());///////////////////
										scndHeaderHtml.Length = 0;////////////
                                        outputHtml.AppendFormat("<tr class={0} style=\"FONT-WEIGHT: normal\">", LINE_CSS);
                                        outputHtml.AppendFormat("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>{0}{0}{1}</td>", offset, data[i, j]);
										j = N_COLUMN-1;
									}
								}
								else if(j-father_level == SCD_LEVEL_L3) 
								{

									if ( CstWeb.AdvertiserPersonalisation.Type.none != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
									{
										if ( CstWeb.AdvertiserPersonalisation.Type.competitor != (CstWeb.AdvertiserPersonalisation.Type)data[i,ADVERTISER_COLUMN])
											LINE_CSS = (father_level!=0)?SCD_L3_REF_CSS:MAIN_L1_REF_CSS;
										else
											LINE_CSS = (father_level!=0)?SCD_L3_CON_CSS:MAIN_L1_CON_CSS;
									}
									else
									{
										LINE_CSS = (father_level!=0)?SCD_L3_CSS:MAIN_L1_CSS;
										if (_session.PersonalizedElementsOnly && MAIN_CLASSIF_TYPE == CstClassif.Branch.type.media)
											dysplayChild=false;
									}
									if (dysplayChild)
									{
										newLine = true;
										outputHtml.Append(scndHeaderHtml.ToString());///////////////////
										scndHeaderHtml.Length = 0;/////////////
                                        outputHtml.AppendFormat("<tr Font-Italic=True class={0} style=\"FONT-WEIGHT: normal\">", LINE_CSS);
                                        outputHtml.AppendFormat("<td class=\"whiteBackGround\"><img src=/I/p.gif width=10></td><td align=left nowrap>{0}{0}{0}{1}</td>", offset, data[i, j]);
										j = N_COLUMN-1;
									}
								}
								else if ((j == N_COLUMN || j == N_1_COLUMN || (j >= MONTH_COLUMN && j <= LAST_MONTH_COLUMN) ) && (dysplayChild || addScndData)) 
								{
									//year
									tmpData = double.Parse(data[i,j].ToString());
                                    dataOutput.AppendFormat("<td nowrap>{0}</td>", ((!double.IsNaN(tmpData) && tmpData != 0) ? FctUtilities.Units.ConvertUnitValueToString(tmpData, _session.Unit, fp) : ""));
								}
								else if ((j == PDV_COLUMN || j == PDM_COLUMN || j== PDM_N_1_COLUMN || j == PDV_N_1_COLUMN)  && displayFather && (dysplayChild || addScndData)) 
								{
									//percentage
									tmpData = double.Parse(data[i,j].ToString());
									dataOutput.AppendFormat("<td nowrap>{0}</td>", ( (tmpData==0||Double.IsNaN(tmpData)||Double.IsInfinity(tmpData)) ? "" : FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData,_session.Unit,true, fp) + " %" ));
								}
								else if (j == EVOL_COLUMN && (dysplayChild || addScndData)) 
								{
									if (!_excel)
									{
										//evol
										tmpData = double.Parse(data[i,j].ToString());
										if (tmpData>0) //rise
                                            dataOutput.AppendFormat("<td nowrap>{0}<img src=/I/g.gif></td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
										else if(tmpData<0) //slide
                                            dataOutput.AppendFormat("<td nowrap>{0}<img src=/I/r.gif></td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
										else if (!Double.IsNaN(tmpData)) // 0 exactly
											dataOutput.Append("<td nowrap>0 %<img src=/I/o.gif></td>");
										else
											dataOutput.Append("<td></td>");
									}
									else if (_excel)
									{
										//evol
										tmpData = double.Parse(data[i,j].ToString());
										if (tmpData>0) //rise
                                            dataOutput.AppendFormat("<td nowrap>{0}</td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
										else if(tmpData<0) //slide
                                            dataOutput.AppendFormat("<td nowrap>{0}</td>", ((!Double.IsInfinity(tmpData)) ? FctUtilities.Units.ConvertUnitValueAndPdmToString(tmpData, _session.Unit, true, fp) + " %" : ""));
										else if (!Double.IsNaN(tmpData)) // 0 exactly
											dataOutput.Append("<td nowrap> 0 %</td>");
										else
											dataOutput.Append("<td></td>");
									}
								}
							}

						}
						if (newLine)
							dataOutput.Append("</tr>");
					}
				}

				i++;
			}
			#endregion

			#region Fermeture tableau
			str.Append("</table>");
			#endregion
		
		}
        #endregion
    }
}
