using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using JPMorrow.Revit.Documents;
using JPMorrow.RevitElementSearch;
using JPMorrow.Tools.Text;

namespace JPMorrow.UI.ViewModels
{
    public partial class ParentViewModel
    {
        /// <summary>
        /// View List Binding
        /// </summary>
        public class ViewPresenter : Presenter
        {
            public View Value;
            public ViewPresenter(View value, ModelInfo info)
            {
                Value = value;
                RefreshDisplay(info);
            }

            public void RefreshDisplay(ModelInfo info)
            {
                Name = Value.Name;
                Type = TextTransform.AddSpacesBetweenCaps(Enum.GetName(typeof(ViewType), Value.ViewType));
            }

            private string n;
            public string Name {get => n;
            set {
                n = value;
                Update("Name");
            }}

            private string id;
            public string Type {get => id;
            set {
                id = value;
                Update("ID");
            }}

            //Item Selection Bindings
            private bool _isSelected;
            public bool IsSelected { get => _isSelected;
                set {
                    _isSelected = value;
                    Update("Run_Items");
            }}
        }
    }

    /// <summary>
    /// Default Presenter: Just Presents a string value as a listbox item,
    /// can replace with an object for more complex listbox databindings
    /// </summary>
    public class ItemPresenter : Presenter
    {
        private readonly string _value;
        public ItemPresenter(string value) => _value = value;
    }

    #region Inherited Classes
    public abstract class Presenter : INotifyPropertyChanged
    {
         public event PropertyChangedEventHandler PropertyChanged;

         public void Update(string val) => RaisePropertyChanged(val);

         protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
         {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
         }
    }
    #endregion
}
