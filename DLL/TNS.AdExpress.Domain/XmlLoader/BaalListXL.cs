using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.DB.Common;
using BaalLevels=TNS.Baal.ExtractList.Constantes;
using TNS.FrameWork.Exceptions;


namespace TNS.AdExpress.Domain.XmlLoader {
    /// <summary>
    /// Load list from baal
    /// </summary>
    public class BaalListXL<T> where T:IBaalItemsList {


        /// <summary>
        /// Load baal list
        /// </summary>
        /// <param name="source">DataSource</param>
        public static void Load(IDataSource source) {
            T itemsList;
            XmlTextReader reader;
            List<BaalLevels.Levels> levelsList=new List<BaalLevels.Levels>();
            string[] levelsString;
            try {
                source.Open();
                reader=(XmlTextReader)source.GetSource();
                while(reader.Read()) {
                    if(reader.NodeType==XmlNodeType.Element) {
                        if(reader.LocalName==ListContener<T>.BaalXmlNodeName) {
                            levelsList.Clear();
                            if(reader.GetAttribute("baalId")==null || reader.GetAttribute("baalId").Length==0) throw (new InvalidXmlValueException("Invalid baalId parameter"));
                            if(reader.GetAttribute("levels")==null || reader.GetAttribute("levels").Length==0) throw (new InvalidXmlValueException("Invalid levels parameter"));
                            levelsString=reader.GetAttribute("levels").Split(',');
                            foreach(string currentLevel in levelsString){
                                levelsList.Add((BaalLevels.Levels)Enum.Parse(typeof(BaalLevels.Levels),currentLevel,true));
                            }
                            itemsList=Activator.CreateInstance<T>();
                            itemsList.InitFromBaal(int.Parse(reader.GetAttribute("baalId")),levelsList);
                            ListContener<T>.Add(int.Parse(reader.GetAttribute("baalId")),itemsList);
                        }

                    }
                }
            }
            #region Error Management
            catch(System.Exception err) {

                #region Close the file
                if(source.GetSource()!=null) source.Close();
                #endregion

                throw (new Exception(" Error : ",err));
            }
            #endregion

            source.Close();
        }
    }
}
