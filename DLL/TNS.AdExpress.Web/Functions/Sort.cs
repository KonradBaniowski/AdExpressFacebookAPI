#region Informations
// Auteur:
// Création:
// Modification:
#endregion

using System;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Ensemble des fonctions de tri pour AdExpress 3.0
	/// </summary>
	public class Sort{	
		/// <summary>
		/// Fonction de tri d'un tableau à 2 dimension
		/// </summary>
		/// <param name="tab">tableau à trier</param>
		/// <param name="column_index">colonne à trier</param>
		/// <param name="sortOrder">ordre de tri : croissant=true , décroissant=false</param>
		/// <param name="itemType">type de valuers triées : nombres ou chaine de caractères</param>
		/// <param name="beginningIndex">index de début du tri</param>
		/// <param name="endIndex">index de fin du tri</param>
		/// <returns>index des valeurs triés</returns>
		public static int[] Sort2D_Array(object[,] tab, Int64 column_index,bool sortOrder,string itemType,int beginningIndex,int endIndex){		
			int[] indexArr=null;						
			#region  TRI des index du tableau
			switch(itemType){
				case SortedItems.Euro :
				case SortedItems.Evolution :	
				case SortedItems.SOV :	
				case SortedItems.SOV_Cumul :	
				case SortedItems.Variation :	
						double[] doubleArr=  new double[tab.GetLongLength(0)];
						for(int j=0;j<tab.GetLongLength(0);j++){
							doubleArr[j]=double.Parse(tab[j,column_index].ToString());							
						}
						if(doubleArr!=null)indexArr = TNS.FrameWork.Tools.Sort.Tri.Tri_double(beginningIndex,endIndex,doubleArr,sortOrder);
					break;
				case SortedItems.Rank :
					int[] intArr=  new int[tab.GetLongLength(0)];
					for(int j=0;j<tab.Length;j++){
						intArr[j]=int.Parse(tab[j,column_index].ToString());						
					}
					if(intArr!=null)indexArr = TNS.FrameWork.Tools.Sort.Tri.Tri_int(beginningIndex,endIndex,intArr,sortOrder);
					break;
				default : 				
					throw new Exception("Sort2D_Array(object[,] tab, Int64 column_index,string sortOrder,SortedItems.Type itemType)-->Impossible de déterminer le type d'élément à trier.");

			}
			
			return indexArr;
			#endregion
		}


	}
}
