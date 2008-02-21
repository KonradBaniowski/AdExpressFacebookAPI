#region Informations
// Auteur: G. Facon
// Date de cr�ation: 14/06/2006
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Drawing;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Liste d'�l�ment d�crivant un niveau de la nomenclature 
	/// </summary>
	public class ClassificationDescriptionCollection: CollectionBase{

		#region Constructeurs


		///<summary>
		/// Constructeur � partir d'un tableau d'�l�ment d�crivant un niveau de la nomenclature
		/// </summary>
		///  <param name="list">tableau d'�l�ment d�crivant un niveau de la nomenclature</param>
		///  <author>G. Facon</author>
		public ClassificationDescriptionCollection(ClassificationDescription[] list):base() {
			foreach (ClassificationDescription s in list) {
				this.Add((ClassificationDescription)s);
			}
		}

		///<summary>
		/// Constructeur � partir d'une liste d'�l�ment d�crivant un niveau de la nomenclature
		/// </summary>
		///  <param name="list">tableau d'�l�ment d�crivant un niveau de la nomenclature</param>
		///  <author>G. Facon</author>
		public ClassificationDescriptionCollection(ArrayList list):base() {
			for (int i = 0; i<list.Count; i++) {
				this.Add((ClassificationDescription)list[i]);
			}
		}

		///<summary>
		/// Constructeur de base
		/// </summary>
		///  <author>G. Facon</author>
		public ClassificationDescriptionCollection():base() {
		}
		#endregion

		#region Accesseurs


		///<summary>
		/// Indexeur
		/// </summary>
		///  <author>G. Facon</author>
		public ClassificationDescription this[int index] {
			get {return( (ClassificationDescription) List[index] );}
			set {List[index] = value;}
		}
		#endregion

		#region M�thodes


		///<summary>
		/// Retourn l'index de l'objet dans la liste des �l�ments d�crivant un niveau de la nomenclature
		/// </summary>
		///  <param name="value">Objet</param>
		///  <returns>index de l'objet dans la liste</returns>
		///  <author>G. Facon</author>
		public int IndexOf(ClassificationDescription value) {
			return( List.IndexOf(value) );
		}
		///<summary>
		/// Convertit la liste en list
		/// </summary>
		///  <returns>List convertie</returns>
		///  <author>G. Facon</author>
		public ClassificationDescription[] ToStringArray() {
			ClassificationDescription[] retArray = new ClassificationDescription[this.Count];
			for (int i = 0; i<this.Count; i++) {
				retArray[i] = this[i];
			}
			return(retArray);
		}
		///<summary>
		/// Convertit la liste en ArrayList
		/// </summary>
		///  <returns>List convertie</returns>
		///  <author>G. Facon</author>
		public ArrayList ToArrayList() {
			ArrayList retArray = new ArrayList();
			for (int i = 0; i<this.Count; i++) {
					retArray.Add(this[i]);
			}
			return(retArray);
		}
		///<summary>
		/// Ajoute un �l�ment
		/// </summary>
		///  <param name="classificationDescription">El�ment � ajouter</param>
		///  <returns>index o� se trouve l"�l�ment ajout�</returns>
		///  <author>G. Facon</author>
		public int Add(ClassificationDescription classificationDescription) {
			return( List.Add(classificationDescription));
		}
		///<summary>
		/// Ins�re un �l�ments
		/// </summary>
		///  <param name="index">Index o� doit �tre ajout� l'�l�ment</param>
		///  <param name="classificationDescription">El�ment � ajouter</param>
		///  <author>G. Facon</author>
		public void Insert(int index, ClassificationDescription classificationDescription) {
			List.Insert(index, classificationDescription);
		}
		///<summary>
		/// Suppression de l'�l�ment
		/// </summary>
		///  <param name="value">El�ment � supprimer</param>
		///  <author>G. Facon</author>
		public void Remove(ClassificationDescription value) {
			List.Remove(value);
		}
		#endregion
	}
}
