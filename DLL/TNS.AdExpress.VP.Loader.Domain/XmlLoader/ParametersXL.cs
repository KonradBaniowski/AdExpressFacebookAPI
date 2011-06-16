#region Information
//  Author : Y Rkaina && D. Mussuma
//  Creation  date: 15/07/2009
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Constantes.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using System.Xml.XPath;
using System.IO;


namespace TNS.AdExpress.VP.Loader.Domain.XmlLoader {
    public class ParametersXL {

        /// <summary>
        /// Common Application Data file description
        /// </summary>
        /// <param name="pathFileSave">source</param>
        /// <returns>layers list</returns>
        public static Parameters Load(IDataSource source) {
            
            try {
                string visualPromotionPathOut;
                string visualConditionPathOut;

                source.Open();
                XPathDocument xPathDoc = new XPathDocument((XmlTextReader)source.GetSource());
                XPathNavigator xPathNav = xPathDoc.CreateNavigator();

                string xPathExp = "//parameters/parameter[@visualConditionPathOut]";
                XPathNodeIterator node = xPathNav.Select(xPathNav.Compile(xPathExp));
                if (node.MoveNext() && !string.IsNullOrEmpty(node.Current.GetAttribute("visualConditionPathOut", "")))
                    visualConditionPathOut = node.Current.GetAttribute("visualConditionPathOut", "");
                else
                    throw new Exception("Impossible to load visualConditionPathOut");

                xPathExp = "//parameters/parameter[@visualPromotionPathOut]";
                node = xPathNav.Select(xPathNav.Compile(xPathExp));
                if (node.MoveNext() && !string.IsNullOrEmpty(node.Current.GetAttribute("visualPromotionPathOut", "")))
                    visualPromotionPathOut = node.Current.GetAttribute("visualPromotionPathOut", "");
                else
                    throw new Exception("Impossible to load visualPromotionPathOut");


                return new Parameters(visualPromotionPathOut, visualConditionPathOut);
            }
            catch (System.Exception err) {
                throw (new Exception(" Error : ", err));
            }
            finally{
                if(source != null)source.Close();
            }
        }       
    }
}
