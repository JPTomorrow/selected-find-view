
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JPMorrow.Tools.Text;

namespace JPMorrow.Revit.BICs {

    public static class BICutil {

        /// <summary>
        /// Get a display friendly name for a BuiltInCategory
        /// </summary>
        /// <param name="bic">BuiltInCategory to convert</param>
        /// <returns></returns>
        public static string GetDisplayName(BuiltInCategory bic) {
            var name = Enum.GetName(typeof(BuiltInCategory), bic);
            name = TextTransform.AddSpacesBetweenCaps(name.Replace("OST_", ""));
            return name;
        }

        /// <summary>
        /// Get a BuiltinCategory given its display friendly name
        /// </summary>
        /// <param name="display_name">Display name to convert</param>
        /// <returns></returns>
        public static BuiltInCategory GetCategoryByDisplayName(string display_name) {
            var bic_name = "OST_" + display_name.Replace(" ", "");
            return (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), bic_name);
        }

        /// <summary>
        /// Get a list of display friendly names for a list of BuiltInCategories
        /// </summary>
        /// <param name="bics">BuiltInCategories to convert</param>
        /// <returns></returns>
        public static IEnumerable<string> GetCategoryNames(IEnumerable<BuiltInCategory> bics) {

            var ret_strs = new List<string>();
            foreach(var b in bics) {
                ret_strs.Add(GetDisplayName(b));
            }

            return ret_strs;
        } 
    }
}