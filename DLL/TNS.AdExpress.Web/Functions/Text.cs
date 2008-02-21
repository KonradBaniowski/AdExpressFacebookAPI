#region Information
// Auteur : Benjamin Masson
// cr�� le : 23/12/2004
// modifi� le : 23/12/2004
//		G. Facon		12/08/2005	Suppression de la m�thode ToFormatHTML
#endregion

using System;
using System.Text.RegularExpressions;

namespace TNS.AdExpress.Web.Functions{
	/// <summary>
	/// Fonction sur les chaines de caract�res
	/// </summary>
	public class Text{
		
		#region Remplace les caract�res accentu�s
		/// <summary>
		/// Replace accuented caracters by the non accentued one
		/// </summary>
		/// <param name="text">String to process</param>
		/// <returns>String without accent</returns>
		public static string SuppressAccent(string text){
			string[,] characters = new string[,] { {
													 "�","A"},{"�","a"}, {
													 "�","A"},{"�","a"}, {
													 "�","A"},{"�","a"}, {
													 "�","C"},{"�","c"}, {
													 "�","E"},{"�","e"}, {
													 "�","E"},{"�","e"}, {
													 "�","E"},{"�","e"}, {
													 "�","I"},{"�","i"}, {
													 "�","O"},{"�","o"}, {
													 "�","O"},{"�","o"}, {
													 "�","U"},{"�","u"}, {
													 "�","U"},{"�","u"}, { 
													 "�","euro;"}};
			int i;
			for(i=0;i<characters.GetLength(0);i++){
				text=text.Replace(characters[i,0],characters[i,1]);
			}
			return(text);
		}
		#endregion
		
	}
}
