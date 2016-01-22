#region Information
//  Author : D. Mussuma
//  Creation  date: 15/03/2010
//  Modifications:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.CampaignTypes
{

    /// <summary>
    /// Campaign Types descriptions
    /// </summary>
    public class CampaignTypesInformation {

        #region variables
		///<summary>
        /// Campaign Types description list
		/// </summary>
        private static Dictionary<CustomerSessions.CampaignType, CampaignTypeInformation> _list = new Dictionary<CustomerSessions.CampaignType, CampaignTypeInformation>();
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
        static CampaignTypesInformation()
        {
		}
		#endregion

		#region Accesseurs
        /// <summary>
        /// Get Campaign Types description list
        /// </summary>
        public static Dictionary<CustomerSessions.CampaignType, CampaignTypeInformation> List
        {
            get { return _list; }
        }
		#endregion

		#region Méthodes publiques
		/// <summary>
        /// Get CampaignType informations
		/// </summary>
        public static CampaignTypeInformation Get(CustomerSessions.CampaignType id)
        {
			try{
				return(_list[id]);
			}
			catch(System.Exception err){
                throw (new ArgumentException("impossible to reteive the requested CampaignType", err));
			}
		}

        /// Get CampaignType informations
        /// </summary>
        public static bool ContainsKey(CustomerSessions.CampaignType id)
        {
            try
            {
                return (_list.ContainsKey(id));
            }
            catch (System.Exception)
            {
                return false;
            }
        }

		/// <summary>
		/// Initialisation de la liste à partir du fichier XML
		/// </summary>
		/// <param name="source">Source de données</param>
		public static void Init(IDataSource source){
            _list.Clear();
            List<CampaignTypeInformation> campaignTypes = CampaignTypesDescriptionXL.Load(source);
            try{
                foreach (CampaignTypeInformation currentCampaignType in campaignTypes)
                {
                    _list.Add(currentCampaignType.Id, currentCampaignType);   
                }
            }
            catch(System.Exception err){
                throw (new UnitException("Impossible the CampaignType list", err));
            }
		}
		#endregion
    }
}
