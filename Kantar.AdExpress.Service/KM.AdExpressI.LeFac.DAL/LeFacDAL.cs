﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace KM.AdExpressI.LeFac.DAL
{
    public abstract class LeFacDAL : ILeFacDAL
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
        public LeFacDAL(WebSession session, string idAddress)
        {
            _session = session;
            _idAddress = idAddress;
        }
        /// <summary>
        /// Get leFac data
        /// </summary>
        /// <returns>leFac data</returns>
        public virtual DataSet GetData()
        {

            #region Construction de la requête
            var sql = new StringBuilder();
            sql.Append("select distinct ad.company, ad.street, ad.street2, ad.code_postal, ad.town,");
            sql.Append(" ct.siren_number, ct.telephone, ct.fax, ct.email ");

            string adexSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;
            sql.AppendFormat(" from {0}.address ad, {0}.contact ct ", adexSchema);

            sql.AppendFormat(",{0}.FAC fa", adexSchema);

            sql.AppendFormat(",{0}.ADVERTISER adv", adexSchema);

            sql.AppendFormat(" where ad.id_address={0}", _idAddress);
            sql.Append(" and fa.id_advertiser=adv.id_advertiser ");
            sql.Append(" and ad.id_address=ct.id_address ");
            sql.AppendFormat(" and ad.activation<{0}", DBConstantes.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and ct.activation<{0}", DBConstantes.ActivationValues.UNACTIVATED);

            sql.AppendFormat(" and fa.activation < {0} ", DBConstantes.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and fa.id_address(+)=ad.id_address ");
            #endregion

            #region Execution de la requête
            try
            {
                return _session.Source.Fill(sql.ToString());
            }
            catch (Exception err)
            {
                throw new Exception("Impossible to load LeFac data " + sql, err);
            }
            #endregion

        }
    }
}