using System;
using System.Collections.Generic;
using System.Text;
using P3.Localization;
using System.Globalization;
using KMI.P3.Domain.Web;

namespace KMI.P3.Domain.Translation
{
    public class GestionWeb
    {
        /// <summary>
        /// Texte à extraire.
        /// Si la function retourne ! c'est que le code langue n'est pas correct
        /// Si la function retourne ? c'est que le code du mots n'existe pas
        /// </summary>
        /// <param name="code">Code du mot à extraire</param>
        /// <param name="langue">Langue souhaitée</param>
        /// <returns></returns>
        public static string GetWebWord(Int64 code, IFormatProvider fp)
        {
            try
            {
                CultureInfo cInfo = null;
                if (fp is CultureInfo)
                {
                    cInfo = (CultureInfo)fp;
                }
                Global.CurrentCultureInfo = cInfo;
                return Global.GetValue("w" + code.ToString(), cInfo).ToString();
                //return _list[langue].GetWebWord(code);

            }
            catch (System.Exception)
            {
                // le code langue n'est pas correcte
                return ("!");
            }
        }

        /// <summary>
        /// Texte à extraire.
        /// Si la function retourne ! c'est que le code langue n'est pas correct
        /// Si la function retourne ? c'est que le code du mots n'existe pas
        /// </summary>
        /// <param name="code">Code du mot à extraire</param>
        /// <param name="langue">Langue souhaitée</param>
        /// <returns></returns>
        public static string GetWebWord(Int64 code, int langue)
        {
            try
            {
                Global.CurrentCultureInfo = WebApplicationParameters.AllowedLanguages[langue].CultureInfo;
                return Global.GetValue("w" + code.ToString(), WebApplicationParameters.AllowedLanguages[langue].CultureInfo).ToString();
                //return _list[langue].GetWebWord(code);

            }
            catch (System.Exception)
            {
                // le code langue n'est pas correcte
                return ("!");
            }
        }
        /// <summary>
        /// Texte à extraire.
        /// Si la function retourne ! c'est que le code langue n'est pas correct
        /// Si la function retourne ? c'est que le code du mots n'existe pas
        /// </summary>
        /// <param name="code">Code du mot à extraire</param>
        /// <param name="langue">Langue souhaitée</param>
        /// <returns></returns>
        public static string GetWebWord(string mnemonic, int langue)
        {
            try
            {
                return Global.GetValue(mnemonic, WebApplicationParameters.AllowedLanguages[langue].CultureInfo).ToString();
            }
            catch (System.Exception)
            {
                // le code langue n'est pas correcte
                return ("!");
            }
        }
    }
}
