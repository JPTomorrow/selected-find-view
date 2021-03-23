using System.Windows.Input;
using System.Windows;
using JPMorrow.Revit.Documents;
using System.Linq;
using JPMorrow.Views.RelayCmd;
using om = System.Collections.ObjectModel;
using JPMorrow.RevitElementSearch;
using JPMorrow.Revit.BICs;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Views;
using JPMorrow.Tools.Diagnostics;
using System.Diagnostics;

namespace JPMorrow.UI.ViewModels
{
    // observable collection aliases
    using ObsView = om.ObservableCollection<ParentViewModel.ViewPresenter>;

    public partial class ParentViewModel : Presenter
    {
        private static ModelInfo Info { get; set; }

        // observable collections
        public ObsView ViewItems { get; set; } = new ObsView();

        public ICommand MasterCloseCmd => new RelayCommand<Window>(MasterClose);
        public ICommand ViewSelectionChangedCmd => new RelayCommand<Window>(ViewSelectionChanged);

        public List<ElementId> SelectedIds { get; set; } = new List<ElementId>();

        public ParentViewModel(ModelInfo info)
        {
            //revit documents and pre converted elements
            Info = info;

            // set selected elements
            SelectedIds = Info.SEL.GetElementIds().ToList();

            // populate the list of views
            if(!SelectedIds.Any()) {
                debugger.show(
                    header:"Selected Find Views", 
                    err:"Please select some elements before running this addin.");
                return;
            }

            var views = new List<Autodesk.Revit.DB.View>();
            foreach(var id in SelectedIds) 
            {
                views.AddRange(ViewFinder.GetViewsThatContainElement(Info, id));
            }

            ViewItems.Clear();
            views.ForEach(x => ViewItems.Add(new ViewPresenter(x, Info)));
            Update("ViewItems");
        }
    }
}
