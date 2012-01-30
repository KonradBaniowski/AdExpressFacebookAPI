#region Information
//  Author : D. Mussuma
//  Creation  date: 15/03/2010
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Domain.CampaignTypes
{
    /// <summary>
    /// Campaign Type description
    /// </summary>
    public class CampaignTypeInformation {

        #region Variables
        /// <summary>
        /// Unit Id
        /// </summary>
        private CustomerSessions.CampaignType _id;
        /// <summary>
        /// Text Id
        /// </summary>
        private int _webTextId;
      
        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Campaign Type Id</param>     
        /// <param name="webTextId">Text Id</param>
        /// <param name="databaseId">Data base Id</param>     
        public CampaignTypeInformation(string id, int webTextId)
        {
            if (id == null || id.Length == 0) throw (new ArgumentException("Invalid paramter Campaign Type id"));                   
            _webTextId=webTextId;
          
            try {
                _id = (CustomerSessions.CampaignType)Enum.Parse(typeof(CustomerSessions.CampaignType), id, true);
            }
            catch(System.Exception err) {
                throw (new ArgumentException("Invalid id parameter", err));
            }
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get  Campaign Type Id
        /// </summary>
        public CustomerSessions.CampaignType Id {
            get{return(_id);}
        }

        /// <summary>
        /// Get Web Text Id
        /// </summary>
        public int WebTextId {
            get { return (_webTextId); }
        }

      
     
        #endregion

     
    }
}
