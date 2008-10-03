#region Informations
/*
 * 
 * Auteur: G. Ragneau
 * Creation : 23/09/2008
 * Modification :
 *		Author - Date - Descriptoin
 */
#endregion

using System;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Insertions.DAL.Exceptions{

    public class InsertionsDALException:BaseException{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public InsertionsDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public InsertionsDALException(string message):base(message){
		
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception Source</param>
        public InsertionsDALException(string message, Exception innerException)
            : base(message, innerException)
        {
		
		}


		#endregion

    }
}
