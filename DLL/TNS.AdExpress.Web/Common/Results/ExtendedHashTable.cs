#region Information
// Auteur: G Ragneau
// Créé le:
// Modifiée le:
//	G. Facon	11/08/2005	Nom des variables
#endregion


using System;
using System.Collections;

using TNS.AdExpress.Web.Core.Exceptions;

namespace TNS.AdExpress.Web.Common.Results{
	/// <summary>
    /// ExtendedHashTable permet d'imbriquer plusieurs niveaux de HashTable
	/// </summary>
	public class ExtendedHashTable:Hashtable{

		#region Variables
		/// <summary>
		/// Valeur du niveau courant
		/// </summary>
		private double _currentValue = -1;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit Valeur du niveau courant
		/// </summary>
		public double Value{
			get{return _currentValue;}
			set{_currentValue = value;}
		}
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ExtendedHashTable():base(){
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		public ExtendedHashTable(double Value):base() {
			_currentValue = Value;
		}
		#endregion

		#region Add
		/// <summary>
		/// Permet d'ajouter une node au niveau 0 avec la clé Key
		/// </summary>
		/// <param name="Value">Valeur</param>
		/// <param name="Key">Clé</param>
		public void Add(double Value, string Key){
			if(this.ContainsKey(Key)) throw new ExtendedHashTableException("Clé " + Key + " dupliquée");

			try{
				this.Add(Key, new ExtendedHashTable(Value));
			}
			catch{
				 throw new ExtendedHashTableException("Erreur lors de l'ajout de la node " + Key);
			}
		}

		/// <summary>
		/// Permet d'ajouter une node au niveau 1, sous la clé ParentKey avec la clé childKey
		/// </summary>
		/// <param name="Value">Valeur</param>
		/// <param name="ParentKey">Clé Parente</param>
		/// <param name="ChildKey">Clé</param>
		public void Add(double Value, string ParentKey, string ChildKey){
			if (!this.ContainsKey(ParentKey)) throw new ExtendedHashTableException("Clé " + ParentKey + " absente");
			if (((ExtendedHashTable)this[ParentKey]).Contains(ChildKey)) throw new ExtendedHashTableException("Clé " + ChildKey + " dupliquée");

			try{
				((ExtendedHashTable)this[ParentKey]).Add(ChildKey,new ExtendedHashTable(Value));
			}
			catch{
				throw new ExtendedHashTableException("Erreur lors de l'ajout de la node " + ParentKey + " / " + ChildKey);
			}

		}

		/// <summary>
		/// Permet d'ajouter une node de valeur Value au niveau Keys.Length, sous les clés Keys[]
		/// </summary>
		/// <param name="Value">Valeur</param>
		/// <param name="Keys">Tableau des clés</param>
		public void Add(double Value, string[] Keys){
			if(Keys.Length <= 0)   throw new ExtendedHashTableException("Clés absentes");
			switch(Keys.Length){
				case 1:
					this.Add(Value, Keys[0]);
					break;
				case 2:
					this.Add(Value, Keys[0], Keys[1]);
					break;
				default:
					if (!this.Contains(Keys[0])) throw new ExtendedHashTableException("Clé " + Keys[0] + " absente");
					string[] tmp = new string[Keys.Length-1];
					for(int i = 1; i < Keys.Length; i++) tmp[i-1] = Keys[i];
					((ExtendedHashTable)this[Keys[0]]).Add(Value, tmp);
					break;
			}
		}
		#endregion

		#region Get
		/// <summary>
		/// Permet d'obtenir la valeur d'une node Key
		/// </summary>
		///<param name="Key">Clé</param>
		/// <returns>Valeur</returns>
		public double GetValue(string Key){
			
			if (!this.ContainsKey(Key)) throw new ExtendedHashTableException("Aucune clé " + Key + " trouvée.");

			return ((ExtendedHashTable)this[Key]).Value;

		}

		/// <summary>
		/// Permet d'obtenir la valeur de la node enfants childKey dans la node ParentKey
		/// </summary>
		///<param name="ParentKey">Clé parente</param>
		///<param name="ChildKey">Clé fille</param>
		/// <returns>Valeur</returns>
		public double GetValue(string ParentKey, string ChildKey){
			
			if (!this.ContainsKey(ParentKey)) throw new ExtendedHashTableException("Aucune clé " + ParentKey + " parent trouvée.");
			if (!((ExtendedHashTable)this[ParentKey]).Contains(ChildKey)) throw new ExtendedHashTableException("Aucune clé " + ChildKey + " enfant trouvée.");

			return ((ExtendedHashTable)((ExtendedHashTable)this[ParentKey])[ChildKey]).Value;

		}

		/// <summary>
		/// Permet d'obtenir la valeur d'un node situé au bout du chemn composé par les clés Keys
		/// </summary>
		///<param name="Keys">Liste des clés</param>
		/// <returns></returns>
		public double GetValue(string[] Keys){
			if(Keys.Length <= 0)   throw new ExtendedHashTableException("Clés absentes");
			switch(Keys.Length){
				case 1:
					return this.GetValue(Keys[0]);					
				case 2:
					return this.GetValue(Keys[0], Keys[1]);					
				default:
					if (!this.Contains(Keys[0])) throw new ExtendedHashTableException("Clé " + Keys[0] + " non trouvée");
					string[] tmp = new string[Keys.Length-1];
					for(int i = 1; i < Keys.Length; i++) tmp[i-1] = Keys[i];
					return ((ExtendedHashTable)this[Keys[0]]).GetValue(tmp);					
			}
		}
		#endregion

	}
}
