using System.Data;

namespace TNS.AdExpressI.GAD.DAL
{
    public interface IGadDAL
    {
        /// <summary>
        /// Obtient les donn�es du gad
        /// </summary>
        /// <returns>Donn�es</returns>
        DataSet GetData();
    }
}