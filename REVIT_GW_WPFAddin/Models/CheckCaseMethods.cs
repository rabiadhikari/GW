using System;

namespace REVIT_GW_WPFAddin.Models
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