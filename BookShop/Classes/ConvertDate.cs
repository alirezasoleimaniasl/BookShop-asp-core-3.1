using MD.PersianDateTime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShop.Classes
{
    public class ConvertDate:IConvertDate
    {
        public DateTime ConvertShamsiToMiladi(string Date)
        {
            PersianDateTime persianDateTime = PersianDateTime.Parse(Date);
            return persianDateTime.ToDateTime();
        }

        public string ConvertMiladiToShamsi(DateTime Date)
        {
            PersianDateTime persianDateTime = new PersianDateTime(Date);
            return persianDateTime.ToString("yyyy/MM/dd");
        }

        public string ConvertMiladiToShamsi(DateTime Date, string format)
        {
            PersianDateTime persianDateTime = new PersianDateTime(Date);
            return persianDateTime.ToString(format);
        }

        public string ShamsiDateTimeNow()
        {
            PersianDateTime persianDateTime = new PersianDateTime(DateTime.Now);
            return persianDateTime.ToString("dddd d MMMM yyyy ");
        }
    }
}
