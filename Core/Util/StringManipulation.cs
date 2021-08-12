using System;

namespace Glass.Core.Util {
    static class StringManipulation {

        public static string ToUpperFirstLetter(string wordToUpper) {
            char[] stringBuffer = wordToUpper.ToCharArray();
            stringBuffer[0] = Char.ToUpper(stringBuffer[0]);

            return new String(stringBuffer);
        }

    }
}
