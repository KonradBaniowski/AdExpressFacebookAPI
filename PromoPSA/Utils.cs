using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Rules
{
   public class Utils
    {
       private static void SendErrorMail(string message, Exception e)
       {
           var cwe = new CustomerWebException(message, e.StackTrace);
           cwe.SendMail();

       }
    }
}
