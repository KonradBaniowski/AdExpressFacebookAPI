#region Information
/*	Author			G. Facon
*	Creation		29/11/2006
*	Modifications:	date - author - comment
*					
*/
#endregion

using System;
using System.Text;
using System.Collections;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Web.Core.Result{
	/// <summary>
	/// Header de la colonne Cr�ation
	/// </summary>
	public class HeaderCreative:HeaderBase{

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public HeaderCreative():base(){		
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">Identifiant de la colonne</param>
		public HeaderCreative(Int64 id):base(id) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		public HeaderCreative(string label):base(label){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		/// <param name="id">Identifiant de la colonne</param>
		public HeaderCreative(string label,Int64 id):base(label,id){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		/// <param name="newGroup">Sp�cifie si l'ent�te fait partie d'un nouveau groupe d'ent�tes</param>
		public HeaderCreative(string label, bool newGroup):base(label,newGroup){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		/// <param name="newGroup">Sp�cifie si l'ent�te fait partie d'un nouveau groupe d'ent�tes</param>
		/// <param name="id">Identifiant de la colonne</param>
		public HeaderCreative(string label, bool newGroup,Int64 id):base(label,newGroup,id){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		/// <param name="sortable">Sp�cifie si la colonne peut �tre tri�e ou non</param>
		public HeaderCreative(bool sortable, string label):base(sortable,label) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		/// <param name="sortable">Sp�cifie si la colonne peut �tre tri�e ou non</param>
		/// <param name="id">Identifiant de la colonne</param>
		public HeaderCreative(bool sortable, string label,Int64 id):base(sortable,label,id) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		/// <param name="newGroup">Sp�cifie si l'ent�te fait partie d'un nouveau groupe d'ent�tes</param>
		/// <param name="sortable">Sp�cifie si la colonne peut �tre tri�e ou non</param>
		public HeaderCreative(string label, bool sortable, bool newGroup):base(label,sortable,newGroup){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="label">Texte de l'ent�te</param>
		/// <param name="newGroup">Sp�cifie si l'ent�te fait partie d'un nouveau groupe d'ent�tes</param>
		/// <param name="sortable">Sp�cifie si la colonne peut �tre tri�e ou non</param>
		/// <param name="id">Identifiant de la colonne</param>
		public HeaderCreative(string label, bool sortable, bool newGroup,Int64 id):base(label,sortable,newGroup,id){
		}
		#endregion

		#region Impl�mentation des m�thodes Cell
		/// <summary>
		/// Rendu de la cellule
		/// </summary>
		/// <param name="output">Sortie HTML</param>
		/// <param name="first">Indique si la cellule est la premi�re d'une ligne ou non</param>
		/// <param name="lineStart">LineStart de l'ent�te</param>
		/// <param name="nodes">Queue de nodes</param>
		/// <param name="rowSpan">Nombre de lignes que l'ent�te doit occuper</param>
		public override void RenderExcel(StringBuilder output, ref bool first, Queue nodes, int rowSpan, LineStart lineStart){
			return;
		}
		/// <summary>
		/// Rendu de la cellule
		/// </summary>
		/// <param name="output">Sortie HTML</param>
		/// <param name="first">Indique si la cellule est la premi�re d'une ligne ou non</param>
		/// <param name="lineStart">LineStart de l'ent�te</param>
		/// <param name="nodes">Queue de nodes</param>
		/// <param name="rowSpan">Nombre de lignes que l'ent�te doit occuper</param>
		/// <param name="iLevelColumn">Index de la colonne contenant les description de niveau</param>
		/// <param name="tLevelLabels">Libell�s des ent�tes de niveaux dans leur ordre hi�rarchique</param>
		public override void RenderRowExcel(StringBuilder output, ref bool first, Queue nodes, int rowSpan, LineStart lineStart, int iLevelColumn, string[] tLevelLabels) {
			return;
		}

		#endregion
	}
}
