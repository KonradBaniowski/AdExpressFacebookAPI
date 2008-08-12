#region Information
// Auteur : Benjamin Masson
// créé le : 23/12/2004
// modifié le : 23/12/2004
//		G. Facon		12/08/2005	Suppression de la méthode ToFormatHTML
#endregion

using System;
using System.Text.RegularExpressions;

namespace TNS.AdExpress.Web.Core.Utilities{
	/// <summary>
	/// Work on string
	/// </summary>
	public class Text{
		
		#region Replace accents
		/// <summary>
		/// Replace accuented caracters by the non accentued one
		/// </summary>
		/// <param name="text">String to process</param>
		/// <returns>String without accent</returns>
		public static string SuppressAccent(string text){
			string[,] characters = new string[,] { {
													 "Á","A"},{"á","a"}, {
													 "Â","A"},{"â","a"}, {
													 "À","A"},{"à","a"}, {
													 "Ç","C"},{"ç","c"}, {
													 "É","E"},{"é","e"}, {
													 "Ê","E"},{"ê","e"}, {
													 "È","E"},{"è","e"}, {
													 "Î","I"},{"î","i"}, {
													 "Ô","O"},{"ô","o"}, {
													 "Ö","O"},{"ö","o"}, {
													 "Û","U"},{"û","u"}, {
													 "Ü","U"},{"ü","u"}, { 
													 "€","euro;"}};
			int i;
			for(i=0;i<characters.GetLength(0);i++){
				text=text.Replace(characters[i,0],characters[i,1]);
			}
			return(text);
		}
		#endregion
		
	}
}
