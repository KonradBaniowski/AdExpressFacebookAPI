using System;
using KMI.PromoPSA.Web.Exceptions;

namespace KMI.PromoPSA.Web.Functions
{
   public class Utils
    {
       public static void SendErrorMail(string message, Exception e)
       {
           var cwe = new CustomerWebException(message, e.StackTrace);
           cwe.SendMail();

       }
    }
}
