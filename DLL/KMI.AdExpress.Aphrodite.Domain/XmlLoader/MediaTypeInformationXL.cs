using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DbConstantes=TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.Aphrodite.Domain;


namespace KMI.AdExpress.Aphrodite.Domain.XmlLoaders {
	/// <summary>
    ///  Load Media Type informations descriped in XML file
	/// </summary>
    public class MediaTypeInformationXL {
		
		#region Chargement de l'objet vehicle à partir du fichier Xml
		/// <summary>
		/// Load Media Type informations
		/// </summary>
		/// <param name="source">DataSource</param>
        public static Dictionary<DbConstantes.Vehicles.names,MediaTypeInformation> Load(IDataSource source) {
			
			#region Variables
            Dictionary<DbConstantes.Vehicles.names,MediaTypeInformation> vehicleList=new Dictionary<TNS.AdExpress.Constantes.Classification.DB.Vehicles.names,MediaTypeInformation>();
            DbConstantes.Vehicles.names vehicleId=DbConstantes.Vehicles.names.plurimedia;
			#endregion

			XmlTextReader Reader=(XmlTextReader)source.GetSource();
			try{
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){					
						switch(Reader.LocalName){
							case "vehicle":
                                if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("databaseId")!=null) {
                                    vehicleId=(DbConstantes.Vehicles.names)Enum.Parse(typeof(DbConstantes.Vehicles.names),Reader.GetAttribute("id"),true);
                                    vehicleList.Add(vehicleId,new MediaTypeInformation());
                                    vehicleList[vehicleId].DatabaseId=Int64.Parse(Reader.GetAttribute("databaseId"));
                                    vehicleList[vehicleId].VehicleId=vehicleId;
								}
                                else{
                                    throw(new XmlException("Vehicle description is invalid"));
                                }
								break;
							case "media":
								if(Reader.GetAttribute("listId")!=null)
                                    vehicleList[vehicleId].MediaListId=Int64.Parse(Reader.GetAttribute("listId"));
								break;
							case "dataTable":
								if(Reader.GetAttribute("name")!=null)
									vehicleList[vehicleId].DataTable=Reader.GetAttribute("name");
								break;
                            case "tableTendenciesMonth":
								if(Reader.GetAttribute("name")!=null)
                                    vehicleList[vehicleId].MonthTrendsTable=Reader.GetAttribute("name");
								break;
                            case "totalTableTendenciesMonth":
								if(Reader.GetAttribute("name")!=null)
                                    vehicleList[vehicleId].TotalMonthTrendsTable=Reader.GetAttribute("name");
								break;
                            case "tableTendenciesWeek":
                                if(Reader.GetAttribute("name")!=null)
                                    vehicleList[vehicleId].WeekTrendsTable=Reader.GetAttribute("name");
                                break;
                            case "totalTableTendenciesWeek":
                                if(Reader.GetAttribute("name")!=null)
                                    vehicleList[vehicleId].TotalWeekTrendsTable=Reader.GetAttribute("name");
                                break;

							case "type":
								if(Reader.GetAttribute("currentYear")!=null)
                                    vehicleList[vehicleId].ListCurrentUnit.Add(Reader.GetAttribute("currentYear"));
								if(Reader.GetAttribute("previousYear")!=null)
                                    vehicleList[vehicleId].ListPreviousUnit.Add(Reader.GetAttribute("previousYear"));
								if(Reader.GetAttribute("dataYear")!=null)
                                    vehicleList[vehicleId].ListDataUnit.Add(Reader.GetAttribute("dataYear"));

								break;
						    }
					    }
                    }
                }
				catch(System.Exception et){
					throw (new XmlException("Impossible to load vehicle description file"+et.Message));
				}		
                
            source.Close();
			return(vehicleList);
		}
		#endregion

	}
}
