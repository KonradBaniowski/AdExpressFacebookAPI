#region Informations
// Auteur: G. Facon
// Date de création: 14/06/2006
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.Drawing;

namespace TNS.AdExpress.Web.Controls.Results{
	/// <summary>
	/// Liste d'élément décrivant un niveau de la nomenclature 
	/// </summary>
	public class ClassificationDescriptionCollection: CollectionBase{

		#region Constructeurs


		///<summary>
		/// Constructeur à partir d'un tableau d'élément décrivant un niveau de la nomenclature
		/// </summary>
		///  <param name="list">tableau d'élément décrivant un niveau de la nomenclature</param>
		///  <author>G. Facon</author>
		public ClassificationDescriptionCollection(ClassificationDescription[] list):base() {
			foreach (ClassificationDescription s in list) {
				this.Add((ClassificationDescription)s);
			}
		}

		///<summary>
		/// Constructeur à partir d'une liste d'élément décrivant un niveau de la nomenclature
		/// </summary>
		///  <param name="list">tableau d'élément décrivant un niveau de la nomenclature</param>
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

		#region Méthodes


		///<summary>
		/// Retourn l'index de l'objet dans la liste des éléments décrivant un niveau de la nomenclature
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
		/// Ajoute un élément
		/// </summary>
		///  <param name="classificationDescription">Elément à ajouter</param>
		///  <returns>index où se trouve l"élément ajouté</returns>
		///  <author>G. Facon</author>
		public int Add(ClassificationDescription classificationDescription) {
			return( List.Add(classificationDescription));
		}
		///<summary>
		/// Insère un éléments
		/// </summary>
		///  <param name="index">Index où doit être ajouté l'élément</param>
		///  <param name="classificationDescription">Elément à ajouter</param>
		///  <author>G. Facon</author>
		public void Insert(int index, ClassificationDescription classificationDescription) {
			List.Insert(index, classificationDescription);
		}
		///<summary>
		/// Suppression de l'élément
		/// </summary>
		///  <param name="value">Elément à supprimer</param>
		///  <author>G. Facon</author>
		public void Remove(ClassificationDescription value) {
			List.Remove(value);
		}
		#endregion
	}
}
