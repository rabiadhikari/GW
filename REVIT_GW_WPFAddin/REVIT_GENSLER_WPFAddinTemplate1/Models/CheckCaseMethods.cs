using System;

namespace REVIT_GENSLER_WPFAddinTemplate1.Models
{
    public class CheckCaseMethods
    {
        public static bool CaseIsUpper(String input)
        {
            if (input.ToUpper() == input)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}