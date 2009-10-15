#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification:
#endregion

#region Namespaces
using System;
using System.Collections;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.AdExpress.Bastet.DataAccess;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using Cst=TNS.AdExpress.Bastet.Constantes;
#endregion

namespace TNS.AdExpress.Bastet.Rules{
	/// <summary>
	/// Rules des indicateurs
	/// </summary>
	public class IndicatorsRules{

		/// <summary>
		/// Méthodes pour le traitement des données des indicateurs
		/// </summary>
		/// <param name="dt">DataTable contenant les données de la requête</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>Organisation des données dans un tableau à 2 dimensions tab[,]</returns>
		internal static object[,] GetRules(DataTable dt, DateTime dateBegin, DateTime dateEnd){

			#region Variables
			bool newMedia=false;
			Int64 oldIdVehicle=0;
			Int64 oldIdCategory=0;
			Int64 oldIdMedia=0;
			Int64 oldIdDiffusion=0;
			Int64 currentLineIndex=0;
			int nbline;
			int currentDate=0;
			int oldCurrentDate=0;
			int k = 0;
			#endregion

			#region Année
//			//MAJ GR : Colonnes totaux par année si nécessaire
//			//FIRST_PERIOD_INDEX a remplacé FrameWorkResultConstantes.MediaPlan.FIRST_PEDIOD_INDEX dans toute la fonction
//			Hashtable YEARS_INDEX = new Hashtable();
//			currentDate = int.Parse(dateBegin.Substring(0,4));
//			oldCurrentDate = int.Parse(dateEnd.Substring(0,4));
//			//int FIRST_PERIOD_INDEX = FrameWorkConstantes.Results.MediaPlanAlert.FIRST_PEDIOD_INDEX;
//			int FIRST_PERIOD_INDEX = FIRST_PEDIOD_COlUMN_INDEX;
//			if (currentDate!=oldCurrentDate) {
//				for(k=currentDate; k<=oldCurrentDate; k++) {
//					YEARS_INDEX.Add(k,FIRST_PERIOD_INDEX);
//					FIRST_PERIOD_INDEX++;
//				}
//			}
//			currentDate = 0;
//			oldCurrentDate = 0;
//			//fin MAJ
//
//			//string tmp="";
			#endregion

			#region Compte le nombre de vehicle et category (pour taille du tableau)
			int nbVehicle=0;
			int nbCategory=0;
			foreach(DataRow currentRow in dt.Rows){
				if(oldIdVehicle!=(Int64)currentRow["id_vehicle"]){
					nbVehicle++;
					oldIdVehicle=(Int64)currentRow["id_vehicle"];
				}
				if(oldIdCategory!=(Int64)currentRow["id_category"]){
					nbCategory++;
					oldIdCategory=(Int64)currentRow["id_category"];
				}
			}
			// Il n'y a pas de données
			if(nbVehicle==0)return(new object[0,0]);

			oldIdVehicle=0;
			oldIdCategory=0;
			#endregion

			#region Création du tableau des jours
			ArrayList periodItemsList=new ArrayList();
			Hashtable periodColumnIndexList=new Hashtable();
			
			DateTime currentDateTime = new DateTime(dateBegin.Year,dateBegin.Month,dateBegin.Day);
			DateTime endDate = new DateTime(dateEnd.Year,dateEnd.Month,dateEnd.Day);
			int periodColumnIndex=7;
			
			while(currentDateTime<=endDate){
				periodColumnIndexList.Add(currentDateTime,periodColumnIndex++);
				currentDateTime = currentDateTime.AddDays(1);
			}
			#endregion

			#region Déclaration des tableaux
			// Nombre de colonne
			int nbCol=periodColumnIndex;//periodItemsList.Count+FIRST_PEDIOD_COlUMN_INDEX+YEARS_INDEX.Count;
			// Tableau de résultat
			object[,] tab=new object[dt.Rows.Count+2+nbVehicle+nbCategory,nbCol];
			// Tableau des indexes des categories
			Int64[] tabCategoryIndex=new Int64[nbCategory+1];
			// Tableau des indexes des vehicles
			Int64[] tabVehicleIndex=new Int64[nbVehicle+1];
			#endregion

			#region Libellé des colonnes
			foreach(DateTime currentColumnIndex in periodColumnIndexList.Keys){
				tab[0,(int)periodColumnIndexList[currentColumnIndex]]=currentColumnIndex;
			}
			#endregion

			#region Construction du tableau de résultat
			foreach(DataRow currentRow in dt.Rows) {
				// Nouveau Media
				if(oldIdVehicle!=(Int64)currentRow["id_vehicle"]){
					currentLineIndex++;
					tab[currentLineIndex,Cst.VEHICLE_COLUMN_INDEX]=currentRow["vehicle"].ToString();
					oldIdVehicle=(Int64)currentRow["id_vehicle"];

					tab[currentLineIndex,Cst.CATEGORY_COLUMN_INDEX]=null;
					tab[currentLineIndex,Cst.MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,Cst.ID_MEDIA_COLUMN_INDEX]=null;
			
					tab[currentLineIndex,Cst.ID_VEHICLE_COLUMN_INDEX]=(Int64)currentRow["id_vehicle"];
					tab[currentLineIndex,Cst.ID_CATEGORY_COUMN_INDEX]=(Int64)currentRow["id_category"];
					tab[currentLineIndex,Cst.ID_MEDIA_COLUMN_INDEX]=(Int64)currentRow["id_media"];
				}
				// Nouvelle Categorie
				if(oldIdCategory!=(Int64)currentRow["id_category"]){
					currentLineIndex++;
					tab[currentLineIndex,Cst.CATEGORY_COLUMN_INDEX]=currentRow["category"].ToString();
					oldIdCategory=(Int64)currentRow["id_category"];
					
					tab[currentLineIndex,Cst.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,Cst.MEDIA_COLUMN_INDEX]=null;
					tab[currentLineIndex,Cst.ID_MEDIA_COLUMN_INDEX]=null;

					tab[currentLineIndex,Cst.ID_VEHICLE_COLUMN_INDEX]=(Int64)currentRow["id_vehicle"];
					tab[currentLineIndex,Cst.ID_CATEGORY_COUMN_INDEX]=(Int64)currentRow["id_category"];
					tab[currentLineIndex,Cst.ID_MEDIA_COLUMN_INDEX]=(Int64)currentRow["id_media"];;
				}
				// Nouveau Support
				if(oldIdMedia!=(Int64)currentRow["id_media"]){
					currentLineIndex++;
					tab[currentLineIndex,Cst.MEDIA_COLUMN_INDEX]=currentRow["media"].ToString();
					oldIdMedia=(Int64)currentRow["id_media"];

					tab[currentLineIndex,Cst.VEHICLE_COLUMN_INDEX]=null;
					tab[currentLineIndex,Cst.CATEGORY_COLUMN_INDEX]=null;
					tab[currentLineIndex,Cst.ID_DIFFUSION_COLUMN_INDEX]=null;

					tab[currentLineIndex,Cst.ID_VEHICLE_COLUMN_INDEX]=currentRow["id_vehicle"].ToString();
					tab[currentLineIndex,Cst.ID_CATEGORY_COUMN_INDEX]=currentRow["id_category"].ToString();
					tab[currentLineIndex,Cst.ID_MEDIA_COLUMN_INDEX]=currentRow["id_media"].ToString();
					tab[currentLineIndex,Cst.ID_DIFFUSION_COLUMN_INDEX]=currentRow["id_diffusion"].ToString();
					newMedia=true;
					oldIdDiffusion=-1;
					if(currentRow["id_diffusion"]!=System.DBNull.Value)
						oldIdDiffusion=(Int64)currentRow["id_diffusion"];
				}
				// Nouvelle diffusion
				if(!newMedia && currentRow["id_diffusion"]!=System.DBNull.Value && oldIdDiffusion!=(Int64)currentRow["id_diffusion"]){
					currentLineIndex++;
					tab[currentLineIndex,Cst.MEDIA_COLUMN_INDEX]=currentRow["media"].ToString();//"->";
					tab[currentLineIndex,Cst.ID_DIFFUSION_COLUMN_INDEX]=currentRow["id_diffusion"].ToString();
					oldIdDiffusion=(Int64)currentRow["id_diffusion"];
				}
				newMedia=false;
				try{
					tab[currentLineIndex,(int)periodColumnIndexList[TNS.FrameWork.Date.DateString.YYYYMMDDToDateTime(currentRow["date_media_num"].ToString())]]=Int64.Parse(currentRow["nb_line"].ToString());
				}
				catch(System.Exception err){
					string msg=err.Message;
				}
			}
			#endregion
			
			#region Ecriture de fin de tableau
			nbCol=tab.GetLength(1);
			nbline=tab.GetLength(0);
			if(currentLineIndex+1<nbline)
				tab[currentLineIndex+1,0]=new TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd();
			#endregion

			dt.Dispose();

			#region Debug: Voir le tableau
//			string HTML1="<html><table border=1><tr>";
//			for(int z=0;z<=currentLineIndex;z++){
//				for(int r=0;r<nbCol;r++){
//					if(tab[z,r]!=null)HTML1+="<td>"+tab[z,r].ToString()+"</td>";
//					else HTML1+="<td>&nbsp;</td>";
//				}
//				HTML1+="</tr><tr>";
//			}
//			HTML1+="</tr></table></html>";
			#endregion

			return(tab);
		}

	}
}
