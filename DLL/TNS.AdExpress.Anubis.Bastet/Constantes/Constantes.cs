using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.AdExpress.Anubis.Bastet.Constantes {

    /// <summary>
    /// All tab to show in excel documents
    /// </summary>
    public enum excelTabs
    {
        //Rappel des param�tres
        customerSelelection = 0,

        //IP Adresse par client
        iPAdresseByClient = 1,

        //Top type clients qui se connectent le plus
        topTypeConnectedCustomer = 2,

        //Top connections par type de clients et par jour nomm�
        topTypeConnectedCustomerByDay = 3,

        //Top connections par type de clients et par mois
        topTypeConnectedCustomerByMonth = 4,

        //Top clients qui se connectent le plus
        topConnectedCustomer = 5,

        //Top connections clients par mois
        topConnectedCustomerByMonth = 6,

        //Top connections clients par jour nomm�
        topConnectedCustomerByDay = 7,

        //Top soci�t�s qui se connectent le plus
        topConnectedCompany = 8,

        //Top soci�t�s qui se connectent le plus par mois
        topConnectedCompanyByMonth = 9,

        //Top soci�t�s qui se connectent le plus par jour nomm�
        topConnectedCompanyByDay = 10,

        //Dur�e moyenne des connections par clients
        connexionDurationAverage = 11,

        //Top des modules et des groupes de modules 
        topUsedModules = 12,

        //Top m�dias utilis�s
        topVehicle = 13,

        //Top m�dias utilis�s par module
        topVehicleByModule = 14,

        //Top utilisation du fichier GAD etAGM
        topFileUsing = 15,

        //Top export de fichier
        topExport = 16,

        //Top des options utilis�es
        topUsedTab = 17,

        //Top des unit�s utilis�es
        topUsedUnits = 18,

        //Top des p�riode utilis�es
        topUsedPeriod = 19,

        //Top clients utilisant le plus les requ�tes sauvegard�es
        topUsingSavedSession = 20,
        /// <summary>
        /// Top connections Clients, IP, time slot
        /// </summary>
        topConnectedByIpTimeSlot = 21
    }
	public class Images {
		/// <summary>
		/// Logo TNS
		/// </summary>
		public const string LOGO_TNS = @"Images\logoTNSMedia.gif";
		/// <summary>
		/// Logo BASTET
		/// </summary>
		public const string LOGO_BASTET = @"Images\Bastet.gif";
	}
}
