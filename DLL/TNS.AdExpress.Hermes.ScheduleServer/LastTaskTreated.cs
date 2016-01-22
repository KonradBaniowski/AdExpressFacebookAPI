#region Informations
// Auteur : B.Masson
// Date de cr�ation : 15/02/2007
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;

namespace TNS.AdExpress.Hermes.ScheduleServer{
	/// <summary>
	/// Classe pour la gestion des r�gles trait�es dans le fichier
	/// </summary>
	public class LastTaskTreated:Hashtable{

		#region Variables
		/// <summary>
		/// Chemin du fichier contenant la liste des r�gles trait�es
		/// </summary>
		private string _filePath=String.Empty;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="filePath"></param>
		public LastTaskTreated(string filePath):base(){
			_filePath = filePath;
			if(!File.Exists(filePath)){
				StreamWriter sr = File.CreateText(filePath);
				sr.Close();
			}
			// Chargement des r�gles du fichier
			Load();
		}
		#endregion

		#region Accesseur
		/// <summary>
		/// Obtient ou d�fini une t�che trait�e
		/// </summary>
		public override object this[object key]{
			set{
				if(ContainsKey(key))
					Remove(key);
				Add(key,value);
				Save();
			}
		}
		#endregion

		#region M�thodes priv�es
		/// <summary>
		/// M�thode pour le chargement des r�gles
		/// </summary>
		private void Load(){

			#region Variables
			StreamReader sr = new StreamReader(_filePath);
			string line;
			string[] temp;
			#endregion

			this.Clear(); // Vide la hashtable
			while ((line = sr.ReadLine()) != null){ // Lecture du fichier txt
				temp = line.Split(';');
				this.Add(Int64.Parse(temp[0].ToString()),temp[1]); // Insere dans la hashtable
			}
			sr.Close();
		}

		/// <summary>
		/// M�thode pour la sauvegarde des r�gles
		/// </summary>
		private void Save(){

			#region Variables
			StreamWriter sw = new StreamWriter(_filePath);
			#endregion

			foreach(object current in this.Keys){
				sw.WriteLine(current.ToString()+";"+this[current].ToString()); // Insere dans le fichier
			}
			sw.Close();
		}
		#endregion

	}
}
