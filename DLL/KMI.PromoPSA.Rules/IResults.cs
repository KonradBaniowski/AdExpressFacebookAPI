using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Rules
{
    public interface IResults
    {
         Int64 GetPSALoginId(string login, string password);
        bool CanAccessToPSA(string login, string password);
    }
}
