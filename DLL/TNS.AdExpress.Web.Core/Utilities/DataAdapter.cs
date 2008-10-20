#region Informations
/*
 * Author : G Ragneau
 * Created on : 15/10/2008
 * History:
 *      Date - Author - Description
 * 
 * 
 * 
 * 
 * 
 * 
 * */
#endregion

using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace TNS.AdExpress.Web.Core.Utilities
{
    /// <summary>
    /// Data Adapter
    /// </summary>
    public class DataAdapter
    {
        StringBuilder str;
        public DataTable GroupBy(DataTable dt, List<string> orderFields, List<string> groupFields, string lineIdField, List<string> forceSumFields)
        {

            #region Check params
            if (dt == null || dt.Rows.Count < 2)
            {
                return dt;
            }
            string orderBy = (orderFields.Count>0)?string.Format("{0} asc", string.Join(" asc,", orderFields.ToArray())):string.Empty;
            //string groupBy = string.Format("{0} asc, {1} asc", string.Join(" asc,", groupFields.ToArray()), lineIdField);
            string groupBy = string.Format("{0} asc", string.Join(" asc,", groupFields.ToArray()));
            #endregion

            #region Order input
            DataView v = dt.DefaultView;
            v.Sort = groupBy;
            dt = v.ToTable();
            #endregion

            #region Group Data

            #region Sums rules
            //determine columns to sum
            List<string> sumFields = new List<string>();
            List<bool> forceSum = new List<bool>();
            List<Sum> sumDelegates = new List<Sum>();
            bool bForceSum = false;
            foreach (DataColumn c in dt.Columns)
            {
                bForceSum = forceSumFields!= null && forceSumFields.Contains(c.ColumnName);
                if (!groupFields.Contains(c.ColumnName) && (bForceSum || lineIdField != c.ColumnName || lineIdField == null || lineIdField.Length < 1))
                {
                    switch (Type.GetTypeCode(c.DataType))
                    {
                        case TypeCode.Decimal:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumDecimal));
                            break;
                        case TypeCode.Double:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumDouble));
                            break;
                        case TypeCode.Int16:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumInt16));
                            break;
                        case TypeCode.Int32:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumInt32));
                            break;
                        case TypeCode.Int64:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumInt64));
                            break;
                        case TypeCode.Single:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumSingle));
                            break;
                        case TypeCode.UInt16:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumUInt16));
                            break;
                        case TypeCode.UInt32:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumUInt32));
                            break;
                        case TypeCode.UInt64:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumUInt64));
                            break;
                        case TypeCode.String:
                            forceSum.Add(bForceSum);
                            sumFields.Add(c.ColumnName);
                            sumDelegates.Add(new Sum(SumString));
                            break;
                        default:
                            break;
                    }
                }

            }
            #endregion

            //loop over the data
            DataTable output = dt.Clone();
            DataRow oldRecord = null;
            bool newRow = false;
            string oldIdLine = string.Empty;
            string cIdLine = string.Empty;
            int sumNb = sumFields.Count;
            foreach (DataRow cRecord in dt.Rows)
            {

                newRow = false;
                //cIdLine = cRecord[lineIdField].ToString();
                foreach (string s in groupFields)
                {
                    if (oldRecord == null || !oldRecord[s].Equals(cRecord[s]))
                    {
                        newRow = true;
                        break;
                    }
                }
                if (newRow)
                {
                    if (oldRecord != null)
                    {
                        oldRecord["VERSIONNB"] = str.ToString();
                    }
                    str = new StringBuilder(50000);
                    oldRecord = output.Rows.Add(cRecord.ItemArray);
                    str.Append(cRecord["VERSIONNB"]);
                }
                else
                {
                    for(int i = 0; i < sumNb; i++)
                    {
                        if (forceSum[i])// || oldIdLine != cIdLine)
                        {
                            sumDelegates[i](oldRecord, sumFields[i], cRecord[sumFields[i]]);
                        }
                    }
                }
                oldIdLine = cIdLine;
            }
            #endregion


            #region Order output
            if (orderBy.Length > 0)
            {
                v = output.DefaultView;
                v.Sort = orderBy;
                output = v.ToTable();
            }
            #endregion
            return output;

        }

        #region Sum Methods
        private delegate void Sum(DataRow row, string colName, object value);
        private void SumDecimal(DataRow row, string colName, object value)
        {
            row[colName] = (Decimal)row[colName] + (Decimal)value;
        }
        private void SumDouble(DataRow row, string colName, object value)
        {
            row[colName] = (Double)row[colName] + (Double)value;
        }
        private void SumInt16(DataRow row, string colName, object value)
        {
            row[colName] = (Int16)row[colName] + (Int16)value;
        }
        private void SumInt32(DataRow row, string colName, object value)
        {
            row[colName] = (Int32)row[colName] + (Int32)value;
        }
        private void SumInt64(DataRow row, string colName, object value)
        {
            row[colName] = (Int64)row[colName] + (Int64)value;
        }
        private void SumSingle(DataRow row, string colName, object value)
        {
            row[colName] = (Single)row[colName] + (Single)value;
        }
        private void SumUInt16(DataRow row, string colName, object value)
        {
            row[colName] = (UInt16)row[colName] + (UInt16)value;
        }
        private void SumUInt32(DataRow row, string colName, object value)
        {
            row[colName] = (UInt32)row[colName] + (UInt32)value;
        }
        private void SumUInt64(DataRow row, string colName, object value)
        {
            row[colName] = (UInt64)row[colName] + (UInt64)value;
        }
        private void SumString(DataRow row, string colName, object value)
        {
            str.AppendFormat(",{0}", value);
            //row[colName] = string.Format("{0},{1}", row[colName],value);
        }
        #endregion

    }

}
