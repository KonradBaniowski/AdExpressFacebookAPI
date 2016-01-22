using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TNS.Ares.Exceptions;

namespace TNS.Ares {
    /// <summary>
    /// Usefull tools
    /// </summary>
    public class Functions {
        private static Random r = new Random(unchecked((int)DateTime.Now.Ticks));

        private static char[] characters = new char[46] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_', '[', ']', '(', ')', '{', '}', '=', '!' };

        #region Random String
        /// <summary>
        /// Generate a string with random size and random caracters authorized in a file name
        /// </summary>
        /// <param name="maxL">Minimal length of the string</param>
        /// <param name="minL">Maximal Length of the string (0 or less for no limit)</param>
        /// <returns>Random String</returns>
        public static string GetRandomString(int minL, int maxL) {
            int max;
            r = new Random(unchecked((int)DateTime.Now.Ticks));
            if (maxL <= 0)
                max = GetRandomIntegerAbove(minL);
            else
                max = GetRandomInteger(minL, maxL);
            StringBuilder str = new StringBuilder(max);
            for (int i = 0; i < max; i++) {
                str.Append(characters[GetRandomInteger(0, characters.Length - 1)]);
            }
            return str.ToString();
        }

        /// <summary>
        /// Determine a random integer between i and j included
        /// </summary>
        /// <param name="min">Minimal value</param>
        /// <param name="max">Maximal value (0 or less for no limit)</param>
        /// <returns>Random integer</returns>
        public static int GetRandomInteger(int min, int max) {
            return r.Next(Math.Min(min, max), Math.Max(min, max));
        }

        /// <summary>
        /// Determine a random integer equal or superior to min
        /// </summary>
        /// <param name="min">Minimal value</param>
        /// <returns>Random number</returns>
        public static int GetRandomIntegerAbove(int min) {
            return (r.Next() + Math.Max(0, min));
        }

        /// <summary>
        /// Determine a random integer equal or inferior to max
        /// </summary>
        /// <param name="max">Maximal value</param>
        /// <returns>Random number</returns>
        public static int GetRandomIntegerUnder(int max) {
            return (r.Next(Math.Max(0, max)));
        }
        #endregion

        #region CleanWorkDirectory
        /// <summary>
        /// Suppress a directory and its content
        /// </summary>
        /// <param name="path">Directory path to delete</param>
        public static void CleanWorkDirectory(string path) {
            try {
                Directory.Delete(path, true);
            }
            catch (System.Exception e) {
                throw (new FunctionsException("Unable to clean the working directory " + path + ".", e));
            }
        }
        #endregion

    }
}
