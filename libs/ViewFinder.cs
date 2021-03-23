using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;

namespace JPMorrow.Revit.Views {
    
    public static class ViewFinder {

        public static IEnumerable<View> GetViewsThatContainElement(ModelInfo info, ElementId id) 
        {

            var types = new List<Type> { typeof(ViewPlan), typeof(ViewSection) };
            ElementMulticlassFilter filter = new ElementMulticlassFilter(types);

            var coll = new FilteredElementCollector(info.DOC);
            var views = coll.WherePasses(filter).Cast<View>().Where(v => !v.IsTemplate);

            return (from v in views

                    let idList = new FilteredElementCollector(info.DOC, v.Id)
                        .WhereElementIsNotElementType().ToElementIds()

                    where !idList.Contains(id)
                    select v);
        }
    }
}