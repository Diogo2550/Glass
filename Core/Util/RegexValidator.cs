using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Glass.Core.Util {
    static class RegexValidator {

        public static bool IsNumber(string value) {
            return Regex.IsMatch(value, "^\\d+$");
        }

    }
}
