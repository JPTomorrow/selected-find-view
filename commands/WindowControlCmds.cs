using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using JPMorrow.Revit.Views;
using JPMorrow.RevitElementSearch;
using JPMorrow.Tools.Diagnostics;
using JPMorrow.UI.Views;
using MoreLinq;

namespace JPMorrow.UI.ViewModels
{
	public partial class ParentViewModel
    {
        /// <summary>
        /// prompt for save and exit
        /// </summary>
        public void MasterClose(Window window)
        {
            try {
                window.Close();
            }
            catch(Exception ex) {
                debugger.show(err:ex.ToString());
            }
        }

        public void ViewSelectionChanged(Window window) {
            
            try {
                var selected = ViewItems.Where(x => x.IsSelected).ToList();
                if(!selected.Any()) return;

                var view = selected.First().Value;

                Info.UIDOC.ActiveView = view;
                Info.UIDOC.ShowElements(SelectedIds);
                Info.SEL.SetElementIds(SelectedIds);
            }
            catch(Exception ex) {
                debugger.show(header:"View Selection Changed", err:ex.ToString());
            }
        }
	}
}