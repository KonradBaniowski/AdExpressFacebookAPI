using System.Data;

namespace TNS.AdExpressI.GAD.DAL
{
    public interface IGadDAL
    {
        /// <summary>
        /// Obtient les données du gad
        /// </summary>
        /// <returns>Données</returns>
        DataSet GetData();
    }
}