using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.DataAccess;
using CustomerCst = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Constantes.Customer.DB;
using TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.PigeLuxottica.DAL
{
    public class PigeLuxotticaDAL
    {
        
        protected string _idAdvertisers = "";

        protected string _idProducts = "";

        protected Int64 _idVehicle = -1;

        protected int _dataLanguage = -1;

        protected string _beginningDate = "";

        protected string _endDate = "";

        protected IDataSource _source = null;

        protected string _password = "";

        protected string _login ="";

        protected long _loginId = -1;

        protected TNS.AdExpress.Right _rights = null;
        public PigeLuxotticaDAL(IDataSource source ,string login,string password,int dataLanguage,string idAdvertisers,string idProducts, Int64 idVehicle,string beginningDate, string endDate)
        {
            if (idAdvertisers == null || idAdvertisers.Length == 0) throw new ArgumentException(" Invalid idAdvertisers parameter ");
            _idAdvertisers = idAdvertisers;
            if (idProducts == null || idProducts.Length == 0) throw new ArgumentException(" Invalid idProducts parameter ");
            _idProducts = idProducts;
            if (idVehicle < 1) throw new ArgumentException(" Invalid idVehicle parameter ");
            _idVehicle = idVehicle;
            if (beginningDate == null || beginningDate.Length == 0) throw new ArgumentException(" Invalid beginningDate parameter ");
            _beginningDate = beginningDate;
            if (endDate == null || endDate.Length == 0) throw new ArgumentException(" Invalid endDate parameter ");
            _endDate = endDate;
            if (login == null || login.Length == 0) throw new ArgumentException(" Invalid login parameter ");
            _login = login;
            if (password == null || password.Length == 0) throw new ArgumentException(" Invalid password parameter ");
            _password = password;
            if (dataLanguage < 1) throw new ArgumentException(" Invalid dataLanguage parameter ");
            _dataLanguage = dataLanguage;
            if (source == null) throw new ArgumentNullException("source parameter is null");
            _source = source;
            _rights = new Right("DMUSSUMA2", "SANDIE5", 33);
            _rights.SetRights();
        }

        public DataSet GetData()
        {
            StringBuilder sql = new StringBuilder(5000);

            //Select clause
            sql.Append(" select distinct  ct.id_category,ct.category,md.id_media,md.media,wp.date_media_num as date_num ");
            sql.Append(" ,wp.date_media_num as date_media_num, wp.id_advertisement,adexpr03.stragg(distinct visual) as visual ");
            sql.Append(" ,media_paging,ad.id_advertiser,ad.advertiser,gr.id_group_,gr.group_,pr.id_product,pr.product");
            sql.Append(" ,nvl(id_slogan,0) as id_slogan,nvl(id_slogan,0) as slogan,ic.id_interest_center,ic.interest_center");
            sql.Append(",ms.id_media_seller,ms.media_seller,fo.id_format,fo.format,area_page,cl.id_color,cl.color,rank_media");
            sql.Append(" ,expenditure_euro,adexpr03.stragg(distinct location) as location,wp.date_cover_num ");
            sql.Append(" ,appliMd.disponibility_visual,appliMd.activation,wp.id_advertisement ");

            //From Clause
            sql.Append(" from  adexpr03.data_press wp , adexpr03.category ct,adexpr03.media md ");
            sql.Append(" , adexpr03.advertiser ad,adexpr03.group_ gr,adexpr03.product pr,adexpr03.interest_center ic");
            sql.Append(" ,adexpr03.media_seller ms,adexpr03.format fo,adexpr03.color cl,adexpr03.location lo ");
            sql.Append(" , adexpr03.application_media  appliMd,adexpr03.data_location  dl ");

            //Where Clause
            sql.AppendFormat(" Where  wp.date_media_num >= {0} and wp.date_media_num <= {1} ", _beginningDate, _endDate);

            //Filtering by advertisers
            sql.AppendFormat(" and   wp.id_advertiser in ({0}) ", _idAdvertisers);

            //Filtering by products
            sql.AppendFormat(" and   wp.id_product in ({0}) ", _idProducts);

            //Filtering with media rights
            //sql.Append(GetCustomerMediaRight("wp", true));

            //Filtering with Press identifier            
            sql.AppendFormat(" and wp.id_vehicle={0}   and ad.id_advertiser=wp.id_advertiser and ad.id_language={1} ", _idVehicle, _dataLanguage);

            //Filtering with activation codes
            sql.AppendFormat("and ad.activation<{0} and gr.id_group_=wp.id_group_ and gr.id_language={1} ", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED, _dataLanguage);
            sql.AppendFormat(" and gr.activation<{0} and pr.id_product=wp.id_product and pr.id_language={1} ", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED, _dataLanguage);
            sql.AppendFormat(" and pr.activation<{0} and ic.id_interest_center=wp.id_interest_center ", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and ic.id_language={0} and ic.activation<{1} and ms.id_media_seller=wp.id_media_seller ", _dataLanguage, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and ms.id_language={0} and ms.activation<{1} and fo.id_format(+)=wp.id_format ", _dataLanguage, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and fo.id_language(+)={0} and fo.activation(+)<{1} and cl.id_color(+)=wp.id_color  ", _dataLanguage, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and cl.id_language(+)={0} and cl.activation(+)<{1} and lo.id_location(+)=dl.id_location  ", _dataLanguage, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and lo.id_language(+)={0} and lo.activation(+)<{1}   and ct.id_category=wp.id_category  ", _dataLanguage, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and ct.id_language={0} and ct.activation<{1} and md.id_media=wp.id_media and md.id_language={0}  ", _dataLanguage, TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.AppendFormat(" and md.activation<{0}   and appliMd.id_media(+)=wp.id_media ", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and appliMd.date_debut(+)=wp.date_media_num and appliMd.id_project(+)=1 ");
            sql.AppendFormat(" and appliMd.activation(+)<{0} and dl.id_advertisement(+)=wp.id_advertisement ", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
            sql.Append(" and dl.id_media(+)=wp.id_media and dl.date_media_num(+)=wp.date_media_num ");
            sql.AppendFormat(" and dl.activation(+)<{0} ", TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);

            // Group by
            sql.Append(" group by ad.id_advertiser,ad.advertiser,ct.id_category,ct.category,md.id_media ");
            sql.Append(" ,md.media,wp.date_media_num,wp.date_media_num, wp.id_advertisement,media_paging ");
            sql.Append(" ,gr.id_group_,gr.group_,pr.id_product,pr.product,id_slogan,id_slogan ");
            sql.Append(" ,ic.id_interest_center,ic.interest_center,ms.id_media_seller,ms.media_seller ");
            sql.Append(" ,fo.id_format,fo.format,area_page,cl.id_color,cl.color,rank_media,expenditure_euro ");
            sql.Append(" ,wp.date_cover_num, appliMd.disponibility_visual,appliMd.activation,wp.id_advertisement ");

            //Order by
            sql.Append(" order by  ad.advertiser,ad.id_advertiser,ct.category,ct.id_category,md.media ");
            sql.Append(" ,md.id_media,date_media_num,date_num, wp.id_advertisement,media_paging,gr.group_ ");
            sql.Append(" ,gr.id_group_,pr.product,pr.id_product,slogan,id_slogan,ic.interest_center ");
            sql.Append(" ,ic.id_interest_center,ms.media_seller,ms.id_media_seller,fo.format,fo.id_format ");
            sql.Append(" ,area_page,cl.color,cl.id_color,rank_media,expenditure_euro,wp.date_cover_num,wp.id_advertisement ");

            return _source.Fill(sql.ToString());
        }

        /// <summary>
        /// Get Customer Media Right
        /// </summary>
        /// <param name="tablePrefixe">Table Prefixe</param>
        /// <param name="beginByAnd">True if begin by and</param>
        /// <returns>String SQL </returns>
        protected string GetCustomerMediaRight(string tablePrefixe, bool beginByAnd)
        {

            string sql = "";
            bool premier = true;
            // le bloc doit il commencer par AND
            // Vehicle
            if (_rights[CustomerRightConstante.type.vehicleAccess].Length > 0)
            {
                if (beginByAnd) sql += " and";
                sql += " ((" + tablePrefixe + ".id_vehicle in (" + _rights[CustomerRightConstante.type.vehicleAccess] + ") ";
                premier = false;
            }
            // Category
            if (_rights[CustomerRightConstante.type.categoryAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + tablePrefixe + ".id_category in (" + _rights[CustomerRightConstante.type.categoryAccess] + ") ";
                premier = false;
            }
            // Media
            if (_rights[CustomerRightConstante.type.mediaAccess].Length > 0)
            {
                if (!premier) sql += " or";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " ((";
                }
                sql += " " + tablePrefixe + ".id_media in (" + _rights[CustomerRightConstante.type.mediaAccess] + ") ";
                premier = false;
            }
            if (!premier) sql += " )";

            // Droits en exclusion
            // Vehicle
            if ( _rights[CustomerRightConstante.type.vehicleException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + tablePrefixe + ".id_vehicle not in (" + _rights[CustomerRightConstante.type.vehicleException] + ") ";
                premier = false;
            }
            // Category
            if ( _rights[CustomerRightConstante.type.categoryException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + tablePrefixe + ".id_category not in (" + _rights[CustomerRightConstante.type.categoryException] + ") ";
                premier = false;
            }
            // Media
            if (_rights[CustomerRightConstante.type.mediaException].Length > 0)
            {
                if (!premier) sql += " and";
                else
                {
                    if (beginByAnd) sql += " and";
                    sql += " (";
                }
                sql += " " + tablePrefixe + ".id_media not in (" + _rights[CustomerRightConstante.type.mediaException] + ") ";
                premier = false;
            }
            if (!premier) sql += " )";
            return (sql);

        }

        //protected void GetRights()
        //{
        //    DataSet ds = null;
        //    string vh = "";
        //    _loginId = RightDAL.GetLoginId(_source, _login, _password);
        //    #region Media Template
        //    if (RightDAL.IsMediaTemplateExist(_source, _loginId))
        //    {
        //        ds = RightDAL.GetMediaTemplate(_source, _loginId);
        //        try
        //        {
        //            if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows != null)
        //            {
        //                foreach (DataRow row in ds.Tables[0].Rows)
        //                {
        //                    //Vehicle en accès
        //                    if ((Int64)row[2] == MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_NOT_EXCEPTION)
        //                    {
        //                        vh = row[0].ToString();
        //                        _rights.Add(CustomerCst.Right.type.vehicleAccess, vh.Split(','));
                                
        //                        // On ajoute AdNetTrack si Internet est présent
        //                    }
        //                    //Vehicle en exception
        //                    if ((Int64)row[2] == MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_EXCEPTION)
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.vehicleException, row[0].ToString().Split(','));
                                
        //                    }
        //                    //Category en accès
        //                    if ((Int64)row[2] == MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_NOT_EXCEPTION)
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.categoryAccess, row[0].ToString().Split(','));
                               
        //                    }
        //                    //Category en exception
        //                    if ((Int64)row[2] == MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_EXCEPTION)
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.categoryException, row[0].ToString().Split(','));
                               
        //                    }
        //                    //média en accès
        //                    if ((Int64)row[2] == MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_NOT_EXCEPTION)
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.mediaAccess, row[0].ToString().Split(','));
                               
        //                    }
        //                    //média en exception
        //                    if ((Int64)row[2] == MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_EXCEPTION)
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.mediaException, row[0].ToString().Split(','));
                               
        //                    }

        //                }
        //            }
        //        }
        //        catch (System.Exception err)
        //        {
        //            throw (new Exception("Impossible to load template media right", err));
        //        }
        //    }
        //    #endregion

        //    #region Customer media rights
        //   ds = RightDAL.GetMediaRights(_source, _loginId);
        //    try
        //    {
        //        if (ds != null && ds.Tables != null && ds.Tables[0] != null && ds.Tables[0].Rows != null)
        //        {
        //            foreach (DataRow row in ds.Tables[0].Rows)
        //            {
        //                //Vehicle en accès
        //                if ((Int64)row[2] == MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_NOT_EXCEPTION)
        //                {
        //                    if (!_rights.ContainsKey(CustomerCst.Right.type.vehicleAccess))
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.vehicleAccess, row[0].ToString().Split(','));
        //                    }
        //                    else _rights[CustomerCst.Right.type.vehicleAccess] = listValue((string[])_rights[CustomerCst.Right.type.vehicleAccess], row[0].ToString().Split(',')).Split(',');
                          
        //                }
        //                //Vehicle en exception
        //                if ((Int64)row[2] == MediaProductIdType.ID_VEHICLE_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_EXCEPTION)
        //                {
        //                    if (!_rights.ContainsKey(CustomerCst.Right.type.vehicleException))
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.vehicleException, row[0].ToString().Split(','));
        //                    }
        //                    else _rights[CustomerCst.Right.type.vehicleException] = listValue((string[])_rights[CustomerCst.Right.type.vehicleException], row[0].ToString().Split(',')).Split(',');
                           

        //                }
        //                //Category en accès
        //                if ((Int64)row[2] == MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_NOT_EXCEPTION)
        //                {
        //                    if (!_rights.ContainsKey(CustomerCst.Right.type.categoryAccess))
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.categoryAccess, row[0].ToString().Split(','));
        //                    }
        //                    else _rights[CustomerCst.Right.type.categoryAccess] = listValue((string[])_rights[CustomerCst.Right.type.categoryAccess], row[0].ToString().Split(',')).Split(',');
                            
        //                }
        //                //Category en exception
        //                if ((Int64)row[2] == MediaProductIdType.ID_CATEGORY_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_EXCEPTION)
        //                {
        //                    if (!_rights.ContainsKey(CustomerCst.Right.type.categoryException))
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.categoryException, row[0].ToString().Split(','));
        //                    }
        //                    else _rights[CustomerCst.Right.type.categoryException] = listValue((string[])_rights[CustomerCst.Right.type.categoryException], row[0].ToString().Split(',')).Split(',');
                           
        //                }
        //                //média en accès
        //                if ((Int64)row[2] == MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_NOT_EXCEPTION)
        //                {
        //                    if (!_rights.ContainsKey(CustomerCst.Right.type.mediaAccess))
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.mediaAccess, row[0].ToString().Split(','));
        //                    }
        //                    else _rights[CustomerCst.Right.type.mediaAccess] = listValue((string[])_rights[CustomerCst.Right.type.mediaAccess], row[0].ToString().Split(',')).Split(',');
                            

        //                }
        //                //média en exception
        //                if ((Int64)row[2] == MediaProductIdType.ID_MEDIA_TYPE && Convert.ToInt32(row[1]) == ExceptionValues.IS_EXCEPTION)
        //                {
        //                    if (!_rights.ContainsKey(CustomerCst.Right.type.mediaException))
        //                    {
        //                        _rights.Add(CustomerCst.Right.type.mediaException, row[0].ToString().Split(','));
        //                    }
        //                    else _rights[CustomerCst.Right.type.mediaException] = listValue((string[])_rights[CustomerCst.Right.type.mediaException], row[0].ToString().Split(',')).Split(',');
                           
        //                }
        //            }
        //        }
        //    }
        //    catch (System.Exception err)
        //    {
        //        throw (new Exception("Impossible to load media template right", err));
        //    }
        //    #endregion
        //}

        /// <summary>
        /// Retreive string (addition tab1 plus tab2)
        /// </summary>
        /// <param name="tab1">right string table</param>
        /// <param name="tab2">right string table</param>
        /// <returns>string</returns>
        protected string listValue(string[] tab1, string[] tab2)
        {

            string res = "";
            int i = 0;
            int j = 0;
            bool notExist = true;
            for (i = 0; i < tab1.Length; i++)
            {
                res += tab1[i] + ",";
            }
            for (i = 0; i < tab2.Length; i++)
            {
                notExist = true;
                for (j = 0; j < tab1.Length; j++)
                {
                    if (tab1[j] == tab2[i])
                    {
                        notExist = false;
                    }
                }
                if (notExist)
                {
                    res += tab2[i] + ",";
                }
            }
            return (res.Substring(0, res.Length - 1));
        }

    }
}
