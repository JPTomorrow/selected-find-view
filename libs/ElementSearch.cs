
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;
using JPMorrow.Revit.BICs;
using System.Linq;
using JPMorrow.Tools.Diagnostics;

namespace JPMorrow.RevitElementSearch {

    /// <summary>
    /// Search through Revit Elements given a search term and a category to which the element belongs
    /// </summary>
    public static class ElementSearch {

        public static BuiltInCategory[] SearchCategories { get; } = new BuiltInCategory[] { 
            BuiltInCategory.OST_Conduit,
            BuiltInCategory.OST_ConduitFitting,
            BuiltInCategory.OST_GenericModel,
            BuiltInCategory.OST_ElectricalFixtures,
            BuiltInCategory.OST_ElectricalEquipment,
            BuiltInCategory.OST_FlexPipeCurves,
        };

        public static IEnumerable<ElementQuery> QueryElements(
            ModelInfo info, string bic_display_name, string parameter_name, string search_term) 
        {
            var qs = new List<ElementQuery>();

            // convert bic name to bic
            var bic = BICutil.GetCategoryByDisplayName(bic_display_name);
            var ids = GetElementsWithParameterValue(info, bic, parameter_name, search_term);

            foreach(var id in ids) {
                var query = ElementQuery.GenerateQuery(info, id, parameter_name, search_term);
                if(query == null) continue;
                qs.Add(query);
            }

            return qs;
        }

        private static IEnumerable<ElementId> GetElementsWithParameterValue(
            ModelInfo info, BuiltInCategory bic, string parameter_name, string search_term) 
        {
            FilteredElementCollector coll = new FilteredElementCollector(info.DOC);

            bool has_value(Element x) {
                var p = x.LookupParameter(parameter_name);

                if(p == null || !p.HasValue) return false;
                string str = p.AsString();
                double? dbl = p.AsDouble();
                int? i = p.AsInteger();
                string val_str = p.AsValueString();

                if(!string.IsNullOrWhiteSpace(str) && str.Contains(search_term)) return true;
                if(!string.IsNullOrWhiteSpace(val_str) && val_str.Contains(search_term)) return true;
                if(dbl != null && dbl.HasValue && dbl.Value.ToString().Equals(search_term)) return true;
                if(i != null && i.HasValue && i.Value.ToString().Equals(search_term)) return true;
                return false;
            }

            var ids = coll.OfCategory(bic).Where(x => has_value(x)).Select(x => x.Id);
            return ids;
        }

        public static IEnumerable<Parameter> GetParametersForQuery(ModelInfo info, ElementQuery q) {
            var el = info.DOC.GetElement(q.CollectedElement);
            return el.GetOrderedParameters();
        }

        public static IEnumerable<string[]> GetParameterNamesAndValues(IEnumerable<Parameter> parameters) {

            var param_strs = new List<string[]>();

            foreach(var p in parameters) {
                if(p == null || !p.HasValue) continue;

                string str = p.AsString();
                double? dbl = p.AsDouble();
                int? i = p.AsInteger();
                string val_str = p.AsValueString();
                string[] add = null;

                bool t(params ParameterType[] x) => x.Any(y => y == p.Definition.ParameterType);

                if(t(ParameterType.Text) && !string.IsNullOrWhiteSpace(str)) 
                    add = new[] { p.Definition.Name, p.AsString() };
                else if(!string.IsNullOrWhiteSpace(val_str)) 
                    add = new[] { p.Definition.Name, p.AsValueString() };
                else if(t(ParameterType.Number) && dbl != null && dbl.HasValue) 
                    add = new[] { p.Definition.Name, p.AsDouble().ToString() };
                else if(t(ParameterType.Integer) && i != null && i.HasValue) 
                    add = new[] { p.Definition.Name, p.AsInteger().ToString() };
                else
                    add = new[] { p.Definition.Name, "UNSET" };

                if(add != null) param_strs.Add(add);
            }

            return param_strs;
        }
    }

    public class ElementQuery {

        public ElementId CollectedElement { get; private set; }
        public string ParameterDisplayValue { get; private set; }

        public string Id { get => CollectedElement.IntegerValue.ToString(); }
        public string GetName(Document doc) => doc.GetElement(CollectedElement).Name;

        public override string ToString() {
            return string.Format("{0} : {1}", Id, ParameterDisplayValue);
        }


        private ElementQuery(ElementId id, string param_val) {
            CollectedElement = id;
            ParameterDisplayValue = param_val;
        }

        public static ElementQuery GenerateQuery(ModelInfo info, ElementId id, string parameter_name, string search_term) {

            var el = info.DOC.GetElement(id);
            var p = el.LookupParameter(parameter_name);

            if(p == null || !p.HasValue) return null;

            string str = p.AsString();
            double? dbl = p.AsDouble();
            int? i = p.AsInteger();
            string val_str = p.AsValueString();

            ElementQuery q = null;
            if(!string.IsNullOrWhiteSpace(str) && str.Contains(search_term)) 
                q = new ElementQuery(id, string.Format("{0}: {1}", p.Definition.Name, p.AsString()));

            if(!string.IsNullOrWhiteSpace(val_str) && val_str.Contains(search_term)) 
                q = new ElementQuery(id, string.Format("{0}: {1}", p.Definition.Name, p.AsValueString()));

            if(dbl != null && dbl.HasValue && dbl.Value.ToString().Equals(search_term))
                q = new ElementQuery(id, string.Format("{0}: {1}", p.Definition.Name, p.AsDouble()));

            if(i != null && i.HasValue && i.Value.ToString().Equals(search_term))
                q = new ElementQuery(id, string.Format("{0}: {1}", p.Definition.Name, p.AsInteger()));

            return q;
        }
    }
}