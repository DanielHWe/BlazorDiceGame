using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiceGame.Helper
{
    public class DiceUnicode
    {
        private static char[] _unicode = new char[] { '\u2680', '\u2681', '\u2682', '\u2683', '\u2684', '\u2685' };

        public static char GetUnicode(int diceResult)
        {
            return _unicode[diceResult - 1];
        }
    }
}
