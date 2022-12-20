using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SavedataManager.Utils
{
    public static class UserInput
    {
        public static bool ReadLineYN()
        {
            string? str = Console.ReadLine();
            while (str == null || new List<string> { "Y", "y", "Yes", "yes", "N", "n", "No", "no" }.Contains(str))
            {
                Console.Write($"Input incorrect, please re-enter(Y/N): ");
                str = Console.ReadLine();
            }
            return str.ToLower()[0] == 'y';
        }
    }
}
