using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AdVolumeChecker.Search.ViewModel {
    /// <summary>
    /// Base class for all ViewModel classes displayed by TreeViewItems.  
    /// This acts as an adapter between a raw data object and a TreeViewItem.
    /// </summary>
    public class TreeViewItemViewModel : INotifyPropertyChanged {
        
        #region Data
        /// <summary>
        /// Children
        /// </summary>
        readonly ObservableCollection<TreeViewItemViewModel> _children;
        /// <summary>
        /// Parent
        /// </summary>
        readonly TreeViewItemViewModel _parent;
        /// <summary>
        /// Indicates if an item is expanded
        /// </summary>
        bool _isExpanded;
        /// <summary>
        /// Indicates if an item is selected
        /// </summary>
        bool _isSelected;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">Parent</param>
        protected TreeViewItemViewModel(TreeViewItemViewModel parent) {
            
            _parent = parent;
            _children = new ObservableCollection<TreeViewItemViewModel>();

        }
        #endregion

        #region Presentation Members

        #region Children

        /// <summary>
        /// Returns the logical child items of this object.
        /// </summary>
        public ObservableCollection<TreeViewItemViewModel> Children {
            get { return _children; }
        }

        #endregion // Children

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded {
            get { return _isExpanded; }
            set {
                if (value != _isExpanded) {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (value != _isSelected) {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region LoadChildren

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        protected virtual void LoadChildren() {
        }

        #endregion // LoadChildren

        #region Parent

        public TreeViewItemViewModel Parent {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
