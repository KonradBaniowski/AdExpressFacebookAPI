using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace KMI.AdExpress.AdVolumeChecker.Domain.DataLoader {
    /// <summary>
    /// This class is used to load files containing information concerning Unencrypted Slots
    /// </summary>
    public class UnencryptedSlotsDL {

        /// <summary>
        /// Load Unencrypted Slots
        /// </summary>
        /// <param name="unencryptedSlotsFilePath"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static Dictionary<string, List<UnencryptedSlot>> Load(string unencryptedSlotsFilePath, DateTime startDate, DateTime endDate) {

            Dictionary<string, List<UnencryptedSlot>> unencryptedSlots = new Dictionary<string, List<UnencryptedSlot>>();

            while (startDate <= endDate) {

                if (!File.Exists(unencryptedSlotsFilePath + "//" + startDate.ToString("yyyyMMdd") + ".csv"))
                    return null;

                startDate = startDate.AddDays(1);
            }

            startDate = startDate.AddDays(-7);

            while (startDate <= endDate) {

                var reader = new StreamReader(File.OpenRead(unencryptedSlotsFilePath + "//" + startDate.ToString("yyyyMMdd") + ".csv"));
                DateTime startSlot;
                DateTime endSlot;
                int index = 0;

                unencryptedSlots.Add(DateTimeFormatInfo.CurrentInfo.GetDayName(startDate.DayOfWeek).ToString().ToUpper(), new List<UnencryptedSlot>());

                while (!reader.EndOfStream) {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    if(values[0] != null && values[0].Length > 0)
                        startSlot = GetDateTime(values[0]);
                    else
                        throw new Exception("Error in unencrypted slot file : " + startDate.ToString("yyyyMMdd"));

                    if (values[1] != null && values[1].Length > 0)
                        endSlot = GetDateTime(values[1]);
                    else
                        throw new Exception("Error in unencrypted slot file : " + startDate.ToString("yyyyMMdd"));

                    unencryptedSlots[DateTimeFormatInfo.CurrentInfo.GetDayName(startDate.DayOfWeek).ToString().ToUpper()].Add(new UnencryptedSlot(index, startSlot, endSlot));
                    index ++;
                }

                startDate = startDate.AddDays(1);
            }

            return unencryptedSlots;
        }

        private static DateTime GetDateTime(string date) {

            int year, month, day, hour, min, second;

            year = int.Parse(date.Substring(6,4));
            month = int.Parse(date.Substring(3,2));
            day = int.Parse(date.Substring(0,2));
            hour = int.Parse(date.Substring(11,2));
            min = int.Parse(date.Substring(14,2));
            second = int.Parse(date.Substring(17,2));

            return new DateTime(year, month, day, hour, min, second);

        }

    }
}
