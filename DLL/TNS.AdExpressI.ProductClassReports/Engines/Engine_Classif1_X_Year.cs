using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using CstWeb = TNS.AdExpress.Constantes.Web;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstPersonalized = TNS.AdExpress.Constantes.Web.AdvertiserPersonalisation.Type;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.Classification.Universe;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date;
using System.Reflection;
//using FctWeb = TNS.AdExpress.Web.

namespace TNS.AdExpressI.ProductClassReports.Engines
{
    /// <summary>
    /// Implement an engine to build a report presented as Classif1 X Year
    /// </summary>
    public class Engine_Classif1_X_Year : Engine
    {

        #region Attributes
        #endregion

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public Engine_Classif1_X_Year(WebSession session, int result) : base(session, result) { }
        #endregion

        #region Abstract methods implementation
        /// <summary>
        /// Compute data got from the DAL layer before to design the report
        /// Build a table of two types based on the user parameters in session:
        /// Type 1 : (media) X (N [PDM, N-1, EVOL])
        /// Type 2 : (produit) X (N [PDV, N-1, EVOL])
        /// Steps:
        ///		- Check data
        ///		- Build constraints:
        ///			- reference and competitors params
        ///			- index of first column with numerical data
        ///			- indexes table with (column index in dstatable, line of the level in the result table, level id) for each classification level
        /// </summary>
        /// <param name="data">DAL data</param>
        /// <returns>data computed from DAL result</returns>
        protected override object[,] ComputeData(DataSet dsData)
        {

            DataTable dtData = dsData.Tables[0];

			if (dtData.Rows.Count<=0) return null;

			object[,] data;

			int nbLine = 0;
			int currentLine;
            CstDBClassif.Vehicles.names vehicle = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);

			#region Constantes

            //Advertiser filter ? last column == advertiser ==> _isPersonalized = last column
            List<long> referenceIDS = new List<long>();
            List<long> competitorIDS = new List<long>();
            if (dtData.Columns.Contains("id_advertiser"))
            {
                _isPersonalized = 1;
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0) && _session.SecondaryProductUniverses[0].Contains(0))
                {
                    referenceIDS = _session.SecondaryProductUniverses[0].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
                }
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1) && _session.SecondaryProductUniverses[1].Contains(0))
                {
                    competitorIDS = _session.SecondaryProductUniverses[1].GetGroup(0).Get(TNSClassificationLevels.ADVERTISER);
                }
            }
            else {
                _isPersonalized = 0;
            }

			//Compute first data column
			int FIRST_DATA_INDEX = 0;
			for(int i = 0; i <  dtData.Columns.Count - _isPersonalized; i = i+2){
				if (dtData.Columns[i].ColumnName.IndexOf("ID_M")<0 && dtData.Columns[i].ColumnName.IndexOf("ID_P")<0){
                    FIRST_DATA_INDEX = i;
					break;
				}
			}

			// delete useless lines
			this.CleanDataTable(dtData, FIRST_DATA_INDEX);

			//Edit list of classification columns(media, produit, groupe, category...) an init values
			int[,] CLASSIF_INDEXES = new int[FIRST_DATA_INDEX/2, 3];
			for(int i = 0; i < FIRST_DATA_INDEX; i += 2){
				CLASSIF_INDEXES[i/2, 0] = i;
				CLASSIF_INDEXES[i/2, 1] = 0;
				CLASSIF_INDEXES[i/2, 2] = -1;
			}

			//Option number
			int NB_OPTION = 0;
			bool percentage = false;
			bool evolution = false;
			if (_session.PDM && _session.PreformatedTable == CstFormat.PreformatedTables.media_X_Year){
				NB_OPTION++;
				percentage = true;
			}

			if (_session.PDV && _session.PreformatedTable == CstFormat.PreformatedTables.product_X_Year){
				NB_OPTION++;
				percentage = true;
			}
		

			if (_session.ComparativeStudy){
				if (_session.Evolution)
				{
					NB_OPTION++;
					evolution = true;
				}
				if (percentage)
					NB_OPTION++;
			}

			#endregion

			#region Compute max number of lines and init data
			foreach(DataRow currentRow in dtData.Rows){

				for( int i = 0; i <= CLASSIF_INDEXES.GetUpperBound(0); i++){

                    int c = Convert.ToInt32( currentRow[CLASSIF_INDEXES[i,0]]);
					if( CLASSIF_INDEXES[i,2] != c){
						nbLine++;
						CLASSIF_INDEXES[i,2] = c;
						for(int j = i+1; j <= CLASSIF_INDEXES.GetUpperBound(0); j++){
							CLASSIF_INDEXES[j,2]=-1;
						}
					}
				}
			}
			//if productXyear or plurimedia ==> add total line
            if (_session.PreformatedTable == CstFormat.PreformatedTables.product_X_Year
                || vehicle == CstDBClassif.Vehicles.names.plurimedia
                  || vehicle == CstDBClassif.Vehicles.names.plurimediaExtended
               )
            {
                nbLine++;
                currentLine = 0;
			    data = new object[nbLine, dtData.Columns.Count-CLASSIF_INDEXES.GetLength(0) + NB_OPTION];
                data[0, 0] = "TOTAL";
                for (int i = 0; i < (dtData.Columns.Count + NB_OPTION - FIRST_DATA_INDEX) - _isPersonalized; i++)
                {
                    data[0, CLASSIF_INDEXES.GetLength(0) + i] = 0.0;
                }

            }
            else
            {
                currentLine = -1;
			    data = new object[nbLine, dtData.Columns.Count-CLASSIF_INDEXES.GetLength(0) + NB_OPTION];
            }
			#endregion

			#region Construction du tableau
			//Init "oldIdLevel"
			for(int i = 0; i <= CLASSIF_INDEXES.GetUpperBound(0); i++){
				CLASSIF_INDEXES[i,2] = -1;
			}


			int offset = (percentage) ? 2 : 1;

			foreach(DataRow currentRow in dtData.Rows){

				for(int i = 0; i <= CLASSIF_INDEXES.GetUpperBound(0); i++){
				//For eache level of classification (either product or media)
                    int c = Convert.ToInt32(currentRow[CLASSIF_INDEXES[i, 0]]);
					if(CLASSIF_INDEXES[i,2] != c){
						
						//level different from previous == new lines
						currentLine++;
						
						//Set all columns for higher levels to null
						for(int j = 0; j < i; j++) 
                            data[currentLine, j]=null;
						//Init totals
						for(int j = 0; j < (dtData.Columns.Count+NB_OPTION-FIRST_DATA_INDEX-_isPersonalized); j++)
                            data[currentLine, CLASSIF_INDEXES.GetLength(0)+j] = 0.0;
						//Set to null all lower levels
						for(int j = i+1; j <= (CLASSIF_INDEXES.GetUpperBound(0)-2); j++) 
                            data[currentLine, j]=null;
						if (_isPersonalized > 0) 
                            data[currentLine, data.GetLength(1)-1] = CstPersonalized.none;
						//Set up current line
						data[currentLine, i] = currentRow[CLASSIF_INDEXES[i,0]+1].ToString();

						//Check advertiser is reference, competitor or none
						if(_isPersonalized>0 &&
                            (_session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.advertiser.ToString())
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
                            && i == CLASSIF_INDEXES.GetUpperBound(0)))
                            )
                        {
                            long idAdvertiser = Convert.ToInt64(currentRow["id_advertiser"]);
							if (referenceIDS.Contains(idAdvertiser))
								data[currentLine,data.GetLength(1)-1] = CstPersonalized.reference;
							else if (competitorIDS.Contains(idAdvertiser))
								data[currentLine,data.GetLength(1)-1] = CstPersonalized.competitor;
							else
								data[currentLine,data.GetLength(1)-1] = CstPersonalized.none;
						}
						
						//Save current level
						CLASSIF_INDEXES[i,2] = c;
						CLASSIF_INDEXES[i,1] = currentLine;
						
						//Init lower levels
						for(int j = i+1; j <= CLASSIF_INDEXES.GetUpperBound(0); j++)
                            CLASSIF_INDEXES[j,2] = -1;
					}
				}
				//Numerical data of the current line
				for(int i = 0; i < (dtData.Columns.Count-FIRST_DATA_INDEX-_isPersonalized); i++){
					data[currentLine, CLASSIF_INDEXES.GetLength(0)+offset*i] = Convert.ToDouble(currentRow[FIRST_DATA_INDEX+i]);
				}
				//COmpute totals
				for(int i = CLASSIF_INDEXES.GetUpperBound(0)-1; i>=0; i--){
					for(int j = 0; j < (dtData.Columns.Count-FIRST_DATA_INDEX-_isPersonalized); j++){
						data[CLASSIF_INDEXES[i,1], CLASSIF_INDEXES.GetLength(0)+offset*j] = Convert.ToDouble(data[CLASSIF_INDEXES[i,1], CLASSIF_INDEXES.GetLength(0)+offset*j]) + Convert.ToDouble(data[currentLine, CLASSIF_INDEXES.GetLength(0)+offset*j].ToString());
					}				
				}
				if (data[0,0].ToString() == "TOTAL"){
					for(int j = 0; j < (dtData.Columns.Count-FIRST_DATA_INDEX-_isPersonalized); j++){
                        data[0, CLASSIF_INDEXES.GetLength(0) + offset * j] = Convert.ToDouble(data[0, CLASSIF_INDEXES.GetLength(0) + offset * j]) + Convert.ToDouble(data[currentLine, CLASSIF_INDEXES.GetLength(0) + offset * j].ToString());
					}
				}
			}
			#endregion

			#region Calculs
			if(evolution || percentage){

				#region Init totals of intermediary levels (i.e. from lowest which is not a reference total)
				int FIRST_RESULT_COLUMN = CLASSIF_INDEXES.GetLength(0);
				//Product_Index contiendra sur une ligne i les totaux du niveau de nomenclature supérieur au niveau i
				//ligne 0 : totaux généraux pour le calcul des PDM/PDV du niveau 0
				double[] TOTAL_INDEXES_N = null;
				double[] TOTAL_INDEXES_N_1 = null;
				if(percentage){
					TOTAL_INDEXES_N = new double[CLASSIF_INDEXES.GetLength(0)];
					TOTAL_INDEXES_N_1 = new double[CLASSIF_INDEXES.GetLength(0)];
					for (int i = 0; i < TOTAL_INDEXES_N.GetLength(0); i++)
					{
						TOTAL_INDEXES_N[i] = TOTAL_INDEXES_N_1[i] = 0.0;
					}
				}
				#endregion

				#region Parcours du tableau

			    bool monoMedia = _session.PreformatedTable == CstFormat.PreformatedTables.media_X_Year
			                     && (vehicle != CstDBClassif.Vehicles.names.plurimedia 
                                 && vehicle != CstDBClassif.Vehicles.names.plurimediaExtended
                                );
			    //pluri et présentation nomenclature media

			    for(int i = 0; i < data.GetLength(0); i++){

					//Extract level info
                    int lIndex = 0;
					for(int j = 0; j < FIRST_RESULT_COLUMN; j++){
                        if (data[i, j] != null)
                        {
                            lIndex = j;
                            break;
                        }
					}
 
					if(percentage){
						//if current line = total, affect in j ie 0
						//else if line != total and line != lowest level, affect in j+1, i.e level+1
						if(i==0 && !monoMedia)
						{
                            TOTAL_INDEXES_N[lIndex] = Convert.ToDouble(data[i, FIRST_RESULT_COLUMN]);
							if (_session.ComparativeStudy)
                                TOTAL_INDEXES_N_1[lIndex] = Convert.ToDouble(data[i, FIRST_RESULT_COLUMN + 2]);
						}
                        else if (lIndex < FIRST_RESULT_COLUMN - 1)
						{
                            TOTAL_INDEXES_N[lIndex + 1] = Convert.ToDouble(data[i, FIRST_RESULT_COLUMN]);
							if (_session.ComparativeStudy)
                                TOTAL_INDEXES_N_1[lIndex + 1] = Convert.ToDouble(data[i, FIRST_RESULT_COLUMN + 2]);
						}
						//calcul pdv ou pdm
						if(i!=0)
						{
                            double tmp = Convert.ToDouble(TOTAL_INDEXES_N[lIndex]);
                            if (tmp != 0)
                                data[i, FIRST_RESULT_COLUMN + 1] = 100 * Convert.ToDouble(data[i, FIRST_RESULT_COLUMN]) / tmp;
							else
								data[i,FIRST_RESULT_COLUMN+1] = null;
                            if (_session.ComparativeStudy)
                            {
                                tmp = Convert.ToDouble(TOTAL_INDEXES_N_1[lIndex]);
                                if (tmp != 0)
                                    data[i, FIRST_RESULT_COLUMN + 3] = 100 * Convert.ToDouble(data[i, FIRST_RESULT_COLUMN + 2]) / tmp;
                                else
                                    data[i, FIRST_RESULT_COLUMN + 3] = null;
                            }
						}
						else{
							data[i,FIRST_RESULT_COLUMN+1] = 100;
							if (_session.ComparativeStudy)
								data[i,FIRST_RESULT_COLUMN+3] = 100;
						}					
					}
					//compute evolution
					if (evolution){
                        double tmp = Convert.ToDouble(data[i, FIRST_RESULT_COLUMN + ((percentage) ? 2 : 1)]);
                        data[i, FIRST_RESULT_COLUMN + 1 + NB_OPTION] = 100 * (Convert.ToDouble(data[i, FIRST_RESULT_COLUMN])
                            - tmp)
							/ tmp;
						currentLine++;
					}

				}

				#endregion

			}
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
            CstFormat.PreformatedTables tableType = (CstFormat.PreformatedTables)_tableType;

			//Column indexes
			int FIRST_DATA_INDEX=0;
			do{
				FIRST_DATA_INDEX++;
			}
			while(FIRST_DATA_INDEX < data.GetLength(1) && data[0,FIRST_DATA_INDEX]==null);

			bool evolution = _session.Evolution && _session.ComparativeStudy;
			bool percentage = (_session.PDM && tableType==CstFormat.PreformatedTables.media_X_Year)
				|| (_session.PDV && tableType==CstFormat.PreformatedTables.product_X_Year);

			int L1_DATA_INDEX = FIRST_DATA_INDEX-3;
			int L2_DATA_INDEX = FIRST_DATA_INDEX-2;
			int L3_DATA_INDEX = FIRST_DATA_INDEX-1;

			//style Css
			string HEADER_CSS		= "";
			string L0_CSS			= "";
			string L1_CSS			= "";
			string L2_CSS			= "";
			string L2_COMPET_CSS	= "";
			string L2_REF_CSS		= "";
			string L3_CSS			= "";
			string L3_COMPET_CSS	= "";
			string L3_REF_CSS		= "";

			if (!_excel){ //Css str
				HEADER_CSS = "astd0";
				L0_CSS = "asl0";
				L1_CSS = "asl3";
				L2_CSS = FIRST_DATA_INDEX<3?"asl3":"asl4";
				L2_COMPET_CSS = "asl4c";
				L2_REF_CSS = "asl4r";
				L3_CSS = FIRST_DATA_INDEX<2?"asl4":"asl5"; //données
				L3_COMPET_CSS = FIRST_DATA_INDEX<2?"asl4c":"asl5bc"; //données
				L3_REF_CSS = FIRST_DATA_INDEX<2?"asl4r":"asl5br"; //données
				if(tableType.ToString().StartsWith("media")){
					L2_CSS = FIRST_DATA_INDEX<4?"asl3":"asl4";
					L3_CSS = FIRST_DATA_INDEX<3?"asl4":"asl5"; //données
				}
			}
			else{ //Css Excel
				HEADER_CSS="astd0x";
				L0_CSS = "asl0";
				L1_CSS = "asl3x";
				L2_CSS = FIRST_DATA_INDEX<3?"asl3x":"asl4x";
				L2_COMPET_CSS = "asl4cx";
				L2_REF_CSS = "asl4rx";
				L3_CSS = FIRST_DATA_INDEX<2?"asl4x":"asl5x"; //données
				L3_COMPET_CSS = FIRST_DATA_INDEX<2?"asl4cx":"asl5bcx"; //données
				L3_REF_CSS = FIRST_DATA_INDEX<2?"asl4rx":"asl5brx"; //données
				if(tableType.ToString().StartsWith("media")){
					L2_CSS = FIRST_DATA_INDEX<4?"asl3x":"asl4x";
					L3_CSS = FIRST_DATA_INDEX<3?"asl4x":"asl5x"; //données
				}
			}

			string lineCssClass = "";
            IFormatProvider fp = WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].CultureInfo;

			#endregion

			#region ouverture du tableau
			str.Append("<table cellPadding=0 cellSpacing=1px border=0>");
			#endregion

			#region entêtes tableau
			str.Append("<tr class=" + HEADER_CSS + ">");
			if (tableType != CstFormat.PreformatedTables.media_X_Year) 
				str.Append("<td>" + GestionWeb.GetWebWord(1164, _session.SiteLanguage) + "</td>");
			else
				str.Append("<td>" + GestionWeb.GetWebWord(1357, _session.SiteLanguage) + "</td>");
            CoreLayer coreLayer = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.date];
            IDate dateBLL = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + coreLayer.AssemblyName, coreLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

            str.Append("<td>" + dateBLL.GetPeriodLabel(_session, CstWeb.CustomerSessions.Period.Type.currentYear) + "</td>"); 
			//PDM
			if (_session.PDM && tableType==CstFormat.PreformatedTables.media_X_Year)
				str.Append("<td>" + GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + _session.PeriodBeginningDate.Substring(0,4) + "</td>");
			//PDV
			if (_session.PDV && tableType==CstFormat.PreformatedTables.product_X_Year)
				str.Append("<td>" + GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + _session.PeriodBeginningDate.Substring(0,4) + "</td>");
			//N-1
			if (_session.ComparativeStudy){
				str.Append("<td>" + dateBLL.GetPeriodLabel(_session,CstWeb.CustomerSessions.Period.Type.previousYear) + "</td>"); 
				//PDV N-1
				if (_session.PDV && tableType==CstFormat.PreformatedTables.product_X_Year)
					str.Append("<td>" + GestionWeb.GetWebWord(1166, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + (int.Parse(_session.PeriodBeginningDate.Substring(0,4))-1) + "</td>");
				//PDM N-1
				if (_session.PDM && tableType==CstFormat.PreformatedTables.media_X_Year)
					str.Append("<td>" + GestionWeb.GetWebWord(806, _session.SiteLanguage) + GestionWeb.GetWebWord(1187, _session.SiteLanguage) + (int.Parse(_session.PeriodBeginningDate.Substring(0,4))-1) + "</td>");
				//Evol
				if (_session.Evolution)
					str.Append("<td>" + GestionWeb.GetWebWord(1168, _session.SiteLanguage) + "</td>");
			}
			str.Append("</tr>");
			#endregion

			#region Corps du tableau
			//ligne totale
			str.Append("<tr class=" + (lineCssClass=L0_CSS) + "><td align=left nowrap>" + data[0,0] + "</td>");
			for (int j = FIRST_DATA_INDEX; j < data.GetLength(1)-_isPersonalized; j++){
				if(data[0,j]!=null)
					AppendNumericData(_session, str,double.Parse(data[0,j].ToString()), evolution, percentage,
						data.GetUpperBound(1)-_isPersonalized, j, FIRST_DATA_INDEX, _excel, fp);
				else
					str.Append("<td></td>");
			}
			str.Append("</tr>");
			//lignes suivantes
			StringBuilder headersstr = new StringBuilder(300);
			for(int i = 1; i < data.GetLength(0); i++){
				for(int j = 0; j < data.GetLength(1)-_isPersonalized; j++){

					if(j==L1_DATA_INDEX){
						if (data[i,j]!=null){
							str.Append("<tr class=" + (lineCssClass=L1_CSS) + "><td align=left nowrap>" + data[i,j] + "</td>");
							j = FIRST_DATA_INDEX-1;
						}
					}
					else if(j==L2_DATA_INDEX){
						if (_isPersonalized<1)
							display = true;
						else if (_session.PreformatedProductDetail.ToString().StartsWith(CstFormat.PreformatedProductDetails.advertiser.ToString())
							&& _session.PersonalizedElementsOnly
                            && (CstWeb.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == CstWeb.AdvertiserPersonalisation.Type.none) 
							display = false;
							else
								display = true;
						if (display && data[i,j]!=null){
							lineCssClass=L2_CSS;
							if (_isPersonalized>0){
                                if ((CstWeb.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == CstWeb.AdvertiserPersonalisation.Type.competitor)
									lineCssClass=L2_COMPET_CSS;
                                else if (((CstWeb.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
									lineCssClass=L2_REF_CSS;
							}
							headersstr.Length = 0;
							headersstr.Append("<tr class=" + lineCssClass + "><td align=left nowrap>" + data[i,j] + "</td>");
							j = FIRST_DATA_INDEX-1;
						}
					}
					else if(j==L3_DATA_INDEX){
						if (_isPersonalized<1)
						{
							display = true;
						}
                        else if (_session.PersonalizedElementsOnly && (CstWeb.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1] == CstWeb.AdvertiserPersonalisation.Type.none) 
							display = false;
						else
							display = true;
						if (display && data[i,j]!=null){
							str.Append(headersstr.ToString());
							headersstr.Length = 0;
							lineCssClass=L3_CSS;
							if (_isPersonalized>0){
                                if (((CstWeb.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == CstWeb.AdvertiserPersonalisation.Type.competitor)
									lineCssClass=L3_COMPET_CSS;
                                else if (((CstWeb.AdvertiserPersonalisation.Type)data[i, data.GetLength(1) - 1]) == CstWeb.AdvertiserPersonalisation.Type.reference)
									lineCssClass=L3_REF_CSS;
							}
							str.Append("<tr class=" + lineCssClass + "><td align=left nowrap>" + data[i,j] + "</td>");
							j = FIRST_DATA_INDEX-1;
						}
					}
					else {
						if (display){
							if(data[i,j]!=null)
							{
								if(headersstr.Length<1)
								{
									AppendNumericData(_session, str, double.Parse(data[i,j].ToString()), evolution, 
										percentage,data.GetUpperBound(1)-_isPersonalized, j, FIRST_DATA_INDEX, _excel, fp);
								}
								else
								{
									AppendNumericData(_session, headersstr, double.Parse(data[i,j].ToString()), evolution, 
										percentage,data.GetUpperBound(1)-_isPersonalized, j, FIRST_DATA_INDEX, _excel, fp);
								}
							}
							else
							{
								if(headersstr.Length<1)
								{
									str.Append("<td></td>"); //<td>- </td>
								}
								else
								{
									headersstr.Append("<td></td>"); //<td>- </td>
								}
							}
						}
					}
				}
				str.Append("</tr>");
			}
			#endregion

			#region fermeture des balise du tableau
			str.Append("</table>");
			#endregion
			
		}
        #endregion

    }
}
