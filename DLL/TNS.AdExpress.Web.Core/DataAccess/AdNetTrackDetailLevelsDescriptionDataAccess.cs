#region Informations
// Author: G. Facon
// Creation date: 21/03/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.IO;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core.DataAccess{
	/// <summary>
	/// Load AdNetTrack detail levels description for AdNetTrack
	/// </summary>
	public class AdNetTrackDetailLevelsDescriptionDataAccess{

		#region Parcours du fichier XML
		/// <summary>
		/// This static method is used to initialize the detail levels description for AdNetTrack
		/// </summary>
		/// <param name="source">Data Source to load informations</param>
		/// <example> This example shows the contents of the XML file<br>
		/// <code>
		/// 	&lt;AdNetTrackDetailLevel&gt;
		///		&lt;defaultAdNetTrackDetailLevels&gt;
		///			&lt;defaultAdNetTrackDetailLevel id="1" name="media\catégorie" /&gt;
		///			&lt;defaultAdNetTrackDetailLevel id="2" name="media\catégorie\support" /&gt;
		///				&lt;defaultAdNetTrackDetailLevel id="4" name="media\centre d'interet\support" /&gt;
		///				&lt;defaultAdNetTrackDetailLevel id="6" name="media\regie\support" /&gt;
		///				&lt;defaultAdNetTrackDetailLevel id="8" name="media\version\support" /&gt;
		///			&lt;/defaultAdNetTrackDetailLevels&gt;
		///			&lt;allowedAdNetTrackLevelItems&gt;
		///				&lt;allowedAdNetTrackLevelItem id="1" name="Media"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="2" name="Categorie"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="3" name="Support"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="4" name="Centre d'interet"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="5" name="Régie"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="8" name="Annonceur"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="9" name="Marque"/&gt;	
		///				&lt;allowedAdNetTrackLevelItem id="10" name="Produit"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="11" name="Famille"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="12" name="Classe"/&gt;
		///				&lt;allowedAdNetTrackLevelItem id="13" name="Groupe"/&gt;
		///			&lt;/allowedAdNetTrackLevelItems&gt;
		///		&lt;/AdNetTrackDetailLevel&gt;
		///		</code>
		/// </example> 
		/// <exception cref="System.Exception">Thrown when is impossible to load the data source file</exception>
		public static void Load(IDataSource source){   

			XmlTextReader reader;

			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "defaultAdNetTrackDetailLevel":
								if(reader.GetAttribute("id")!=null){
									AdNetTrackDetailLevelsDescription.DefaultAdNetTrackDetailLevels.Add(DetailLevelsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								}
								break;
							case "allowedAdNetTrackLevelItem":
								if(reader.GetAttribute("id")!=null){
									AdNetTrackDetailLevelsDescription.AllowedAdNetTrackLevelItems.Add(DetailLevelItemsInformation.Get(int.Parse(reader.GetAttribute("id"))));
								}
								break;
						}
					}
				}
			}
			catch(System.Exception err){
				source.Close();
				throw(new Core.Exceptions.AdNetTrackDetailLevelsDescriptionDataAccessException("Impossible to load AdNetTrack Levels file",err));
			}
			source.Close();
		}
		#endregion

	}
}
