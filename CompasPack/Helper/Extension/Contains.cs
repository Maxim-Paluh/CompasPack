using System;
using System.Collections.Generic;
using System.Linq;

namespace CompasPack.Service
{
    static class ContainsExtensions
    {
        public static bool Contains(this String str, String substring, StringComparison comp)
        {
            if (substring == null)
                throw new ArgumentNullException("substring",
                                             "substring cannot be null.");
            else if (!System.Enum.IsDefined(typeof(StringComparison), comp))
                throw new ArgumentException("comp is not a member of StringComparison",
                                         "comp");

            return str.IndexOf(substring, comp) >= 0;
        }
    }
}
