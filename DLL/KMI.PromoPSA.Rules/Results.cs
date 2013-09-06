using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Rules
{
  public  class Results :IResults
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Results()
        {
        }

      #region GetEvaliantLoginId
      public Int64 GetPSALoginId(string login, string password)
      {

          return 0;
      }

      public bool CanAccessToPSA(string login, string password)
      {
          throw new NotImplementedException();
      }

      #endregion
    }
}
