#region Informations
/*
 * 
 * Auteur: G. Ragneau
 * Creation : 23/09/2006
 * Modification :
 *		Author - Date - Descriptoin
 */
#endregion

using System;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Insertions.Exceptions{

    public class DescriptorException:BaseException{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DescriptorException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public DescriptorException(string message):base(message){
		
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception Source</param>
        public DescriptorException(string message, Exception innerException)
            : base(message, innerException)
        {
		
		}


		#endregion

    }
}
