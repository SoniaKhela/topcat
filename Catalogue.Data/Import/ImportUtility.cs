using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Utilities.Text;

namespace Catalogue.Data.Import
{
    public class ImportUtility
    {
        public static DateTime ParseDate(string d)
        {
            return d.IsNotBlank() ? DateTime.Parse(d) : DateTime.Now;
        }
    }
}
