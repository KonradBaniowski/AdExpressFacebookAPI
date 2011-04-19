using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using KMI.P3.Domain.Translation;
using KMI.P3.Domain.Web;
using KMI.P3.Domain.Exceptions;

namespace KMI.P3.Domain.XmlLoader
{
    public class CustomerTypesXL
        {

        /// <summary>
        /// CustomerType file path description
        /// </summary>
        public struct CustomerType{
            /// <summary>
            /// CustomerType Id
            /// </summary>
            public Int64 Id;
            /// <summary>
            /// HTML file path
            /// </summary>
            public  string FilePath;
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="id">CustomerType Id</param>
            /// <param name="filePath">HTML file path</param>
            public CustomerType(Int64 id,string filePath){
                Id=id;FilePath=filePath;
            }
        }




        #region Web Languages
        /// <summary>
        /// Chargement de la langue par defaut de Cyberpub
        /// </summary>
        /// <param name="dataSource">Source de données</param>
        /// <returns>Langue par defaut de l'application</returns>
        /// <summary>
        internal static Dictionary<Int64, CustomerType> LoadCustomerTypes(IDataSource dataSource)
        {

            #region Variables
            Dictionary<Int64, CustomerType> customerTypes = new Dictionary<Int64, CustomerType>();
            XmlTextReader reader = null;
            Int64 id=0;
            string filePath = "";
           
                    
            #endregion

            try
            {
                reader = (XmlTextReader)dataSource.GetSource();
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        id = 0;
                        filePath = "";                   
                        switch (reader.LocalName)
                        {
                            case "CustomerType":
                                if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
                                id = int.Parse(reader.GetAttribute("id"));                             
                                if (reader.GetAttribute("filePath") != null) filePath = reader.GetAttribute("filePath");
                                customerTypes.Add(id, new CustomerType(id, filePath));
                                break;
                          
                           
                        }
                    }
                }
            }
            #region Error Management
            catch (System.Exception err)
            {

                #region Close the file
                if (dataSource.GetSource() != null) dataSource.Close();
                #endregion

                throw (new XmlException(" Error : ", err));
            }
            #endregion

            dataSource.Close();
            return (customerTypes);
        }
        #endregion
    }
}
