using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KM.AdExpress.AlertPreRoll.Tools
{
    public class DateTimeHelpers
    {
        public static DateTime YYYYMMDDToDateTime(string date)
        {
            return (new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2))));
        }
    }
}
