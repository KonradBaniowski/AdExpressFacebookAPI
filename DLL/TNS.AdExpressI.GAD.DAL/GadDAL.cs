using System;
using System.Data;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.GAD.DAL.Exceptions;
using TNS.FrameWork.DB.Common;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.GAD.DAL
{
    public abstract class GadDAL : IGadDAL
    {
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;

        /// <summary>
        /// ID adresse
        /// </summary>
        protected string _idAddress;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idAddress">id adresss</param>
        public GadDAL(WebSession session, string idAddress)
        {
            _session = session;
            _idAddress = idAddress;
        }

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="session">source</param>
        /// <param name="idAddress">id adresss</param>
        public GadDAL(IDataSource source, string idAddress)
        {
            _session = new WebSession {Source = source};
            _idAddress = idAddress;
        }

        /// <summary>
        /// Get GAD data
        /// </summary>
        /// <returns>gad data</returns>
        public virtual DataSet GetData()
        {

            #region Construction de la requête
            var sql = new StringBuilder();
            sql.Append("select distinct adv.siret_number,ad.company, ad.street, ad.street2, ad.code_postal, ad.town,");
            sql.Append(" ct.telephone, ct.fax, ct.email ");
            // intégration Doc Marketing 
            sql.AppendFormat(", gd.id_gad");
            sql.AppendFormat(", (select doc_marketing_key from {0}.DOC_MARKETING_KEY where date_media_num=to_number(to_char(sysdate, 'yyyyMMdd'))) as docKey", WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.webnav01).Label);
            //fin modif
            string adexSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;
            sql.AppendFormat(" from {0}.address ad, {0}.contact ct ", adexSchema);

            //G Ragneau - intégration Doc Marketing - 22/01
            sql.AppendFormat(",{0}.GAD gd", adexSchema);
            //fin modif

            sql.AppendFormat(",{0}.ADVERTISER adv", adexSchema);

            sql.AppendFormat(" where ad.id_address={0}", _idAddress);
            sql.Append(" and gd.id_advertiser=adv.id_advertiser ");
            sql.Append(" and ad.id_address=ct.id_address ");
            sql.AppendFormat(" and ad.activation<{0}" , DBConstantes.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and ct.activation<{0}" , DBConstantes.ActivationValues.UNACTIVATED);
            //G Ragneau - intégration Doc Marketing - 22/01
            sql.AppendFormat(" and gd.activation < {0} ", DBConstantes.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and gd.id_address(+)=ad.id_address ");
            //fin modif
            #endregion

            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (Exception err)
            {
                throw (new GadDALException("Impossible to load GAD  data " + sql, err));
            }
            #endregion

        }
    }
}
