using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Rules {
    /// <summary>
    /// Result Interface
    /// </summary>
    public interface IResults {
        /// <summary>
        /// Get PSA Login Id
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        Int64 GetPSALoginId(string login, string password);
        /// <summary>
        /// Can Access To PSA
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        bool CanAccessToPSA(string login, string password);
    }
}
