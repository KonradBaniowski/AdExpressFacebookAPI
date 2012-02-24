#region Informations
// Auteur: A.DADOUCH
// Date de création: 22/08/2005
#endregion

using System;
using System.Data;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.DataAccess.Results;
using FrameWorkDate=TNS.FrameWork.Date;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;
using AdExpressWebRules=TNS.AdExpress.Web.Rules;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using WebCommon = TNS.AdExpress.Web.Common;
using TNS.AdExpress.Domain.Translation;


namespace TNS.AdExpress.Web.Rules.Results {
	/// <summary>
	/// Traitement des listes de fichiers pour Fichiers resultats
	/// </summary>
	public class FilesItemRules{


			#region Traitement
			/// <summary>
			/// Obtient le tableau de résultat Fichiers resultats
			/// </summary>
			/// <param name="typeList">Liste des types possibles pour les fichiers résultats</param>
			/// <param name="webSession">Session</param>
			/// <param name="dataSource">DataSource pour la creation de DataSet</param>
			/// <returns>Tableau de résultat</returns>
			internal static ArrayList GetData(ArrayList typeList, WebSession webSession,IDataSource dataSource){

				#region Variables
				DataTable dt = null;
				int nbItemsInTabData=0;
				string[,] tabData=null;
			//	string pathDirectory="";
				string linkFile="";
				string name="";
				string linkTemp="";
				string typeListString="";
				DataRow[] rows;
				ArrayList list=new ArrayList();
								
				#endregion

				try{

					#region Configuration
					linkFile = WebConstantes.FilesResults.LINK_FILE;
					#endregion

					#region Liste des fichiers
                    foreach(int type in typeList){
							typeListString+=type.ToString()+",";
					}
					typeListString=typeListString.Substring(0,typeListString.Length-1);

					dt=FilesItemDataAccess.GetData(webSession,dataSource,typeListString).Tables[0];
					
					if(dt.Rows.Count<1) return(null);
					
					foreach(int type in typeList){
						rows = dt.Select("id_pdf_result_type="+type);
						if(rows!=null && rows.Length>0){
							tabData=new string[2,rows.Length];
							nbItemsInTabData=0;
							foreach(DataRow current in rows)
							{
								name = current["PDF_USER_FILENAME"].ToString();
								switch((TNS.AdExpress.Anubis.Constantes.Result.type)type)
								{
									case TNS.AdExpress.Anubis.Constantes.Result.type.appmInsertionDetail :
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.pachet:
										linkTemp =linkFile+current["ID_LOGIN"].ToString()+"/"+current["pdf_name"].ToString()+".txt";break;
									case TNS.AdExpress.Anubis.Constantes.Result.type.amset :
									case TNS.AdExpress.Anubis.Constantes.Result.type.appmExcel :
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.tefnout:
										linkTemp =linkFile+current["ID_LOGIN"].ToString()+"/"+current["pdf_name"].ToString()+".xls";break;																									
									case TNS.AdExpress.Anubis.Constantes.Result.type.appm :	
									case TNS.AdExpress.Anubis.Constantes.Result.type.hotep :
									case TNS.AdExpress.Anubis.Constantes.Result.type.miysis :
									case TNS.AdExpress.Anubis.Constantes.Result.type.mnevis :
									case TNS.AdExpress.Anubis.Constantes.Result.type.shou:
									case TNS.AdExpress.Anubis.Constantes.Result.type.aton:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.selket:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.thoueris:
										linkTemp =linkFile+current["ID_LOGIN"].ToString()+"/"+current["pdf_name"].ToString()+".pdf";break;
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.dedoum:
                                        linkTemp = linkFile + current["ID_LOGIN"].ToString() + "/" + current["pdf_name"].ToString() + ".zip"; break;


								}
								//Colonne 0 : Nom du fichier - Colonne 1 : Chemin du fichier
								tabData.SetValue(name,0,nbItemsInTabData);
								tabData.SetValue(linkTemp,1,nbItemsInTabData);
								//haveInserItem=true;
								nbItemsInTabData++;
							}
							list.Add(new WebCommon.Results.FilesItem(getTypeName((TNS.AdExpress.Anubis.Constantes.Result.type)type,webSession),tabData,(TNS.AdExpress.Anubis.Constantes.Result.type)type));
						}
					}

					//Tri des fichiers par ordre alphabétique
					//Array.Sort(files,Comparer.Default);
					#endregion
			
				}
				catch(System.Exception e){
					throw (new WebExceptions.FilesItemRulesException("Erreur dans la récupération des données",e));
				}
				//if(!haveInserItem) throw (new WebExceptions.FilesItemRulesException("Aucuns éléments correspondent aux critères"));
				return (list);
			}
			#endregion

			#region Méthodes internes
			/// <summary>
			/// Fonction qui teste la présence d'un caractère spécifié dans une chaine
			/// </summary>
			/// <param name="name">Chaine à tester</param>
			private static bool VerifCharacter(string name){
				if(name==null)return(false);
				if(name.Length<1)return(false);
				if(name.IndexOf("_")<0)return(false);
				return(true);
			}

			private static string getTypeName(TNS.AdExpress.Anubis.Constantes.Result.type type,WebSession webSession){
				
				switch((TNS.AdExpress.Anubis.Constantes.Result.type)type){
					case(TNS.AdExpress.Anubis.Constantes.Result.type.appm):return(GestionWeb.GetWebWord(1981,webSession.SiteLanguage));
					case(TNS.AdExpress.Anubis.Constantes.Result.type.appmInsertionDetail):return(GestionWeb.GetWebWord(1919,webSession.SiteLanguage));
					case(TNS.AdExpress.Anubis.Constantes.Result.type.appmExcel):return(GestionWeb.GetWebWord(1940,webSession.SiteLanguage));
					case(TNS.AdExpress.Anubis.Constantes.Result.type.hotep):return(GestionWeb.GetWebWord(1982,webSession.SiteLanguage));
					case(TNS.AdExpress.Anubis.Constantes.Result.type.miysis):return(GestionWeb.GetWebWord(2008,webSession.SiteLanguage));
					case TNS.AdExpress.Anubis.Constantes.Result.type.shou :return(GestionWeb.GetWebWord(2118,webSession.SiteLanguage));
					case(TNS.AdExpress.Anubis.Constantes.Result.type.mnevis):return(GestionWeb.GetWebWord(2009,webSession.SiteLanguage).Replace("'"," "));
					case(TNS.AdExpress.Anubis.Constantes.Result.type.amset):return(GestionWeb.GetWebWord(2119,webSession.SiteLanguage));
					case(TNS.AdExpress.Anubis.Constantes.Result.type.aton):return(GestionWeb.GetWebWord(2120,webSession.SiteLanguage));
                    case (TNS.AdExpress.Anubis.Constantes.Result.type.tefnout): return (GestionWeb.GetWebWord(2892, webSession.SiteLanguage));
                    case (TNS.AdExpress.Anubis.Constantes.Result.type.selket): return (GestionWeb.GetWebWord(2895, webSession.SiteLanguage));
                    case (TNS.AdExpress.Anubis.Constantes.Result.type.thoueris): return (GestionWeb.GetWebWord(2896, webSession.SiteLanguage));
                    case (TNS.AdExpress.Anubis.Constantes.Result.type.dedoum): return (GestionWeb.GetWebWord(2937, webSession.SiteLanguage));
                    case (TNS.AdExpress.Anubis.Constantes.Result.type.pachet): return (GestionWeb.GetWebWord(2942, webSession.SiteLanguage));
					default:return("");	
				}	
			}

			#endregion

		}
	}
