using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using KMI.AdExpress.AdVolumeChecker.Domain.DataModel;
using cst = KMI.AdExpress.AdVolumeChecker.Domain.Constantes;

namespace AdVolumeChecker.Search.ViewModel {
    public class ClassificationViewModel {

        readonly ReadOnlyCollection<ClassificationLevelViewModel> _classificationLevels;
        
        string _searchText = String.Empty;
        readonly ICommand _searchCommand;

        public ClassificationViewModel(ClassificationLevel[] classificationLevels)
        {
            _searchCommand = new ClassificationViewCommand(this);

            _classificationLevels = new ReadOnlyCollection<ClassificationLevelViewModel>(
                (from classificationLevel in classificationLevels
                 select new ClassificationLevelViewModel(classificationLevel))
                .ToList());
        }

        public ReadOnlyCollection<ClassificationLevelViewModel> ClassificationLevels {
            get { return _classificationLevels; }
        }

        #region SearchCommand

        /// <summary>
        /// Returns the command used to execute a search in the family tree.
        /// </summary>
        public ICommand SearchCommand {
            get { return _searchCommand; }
        }

        private class ClassificationViewCommand : ICommand {

            readonly ClassificationViewModel _classificationViewModel;

            public ClassificationViewCommand(ClassificationViewModel classificationViewModel) {
                _classificationViewModel = classificationViewModel;
            }

            public bool CanExecute(object parameter) {
                return true;
            }

            event EventHandler ICommand.CanExecuteChanged {
                // I intentionally left these empty because
                // this command never raises the event, and
                // not using the WeakEvent pattern here can
                // cause memory leaks.  WeakEvent pattern is
                // not simple to implement, so why bother.
                add { }
                remove { }
            }

            public void Execute(object parameter) {
                _classificationViewModel.PerformSearch((cst.ClassificationLevelId)parameter);
            }
        }

        #endregion // SearchCommand

        #region SearchText

        /// <summary>
        /// Gets/sets a fragment of the name to search for.
        /// </summary>
        public string SearchText {
            get { return _searchText; }
            set {
                if (value == _searchText)
                    return;

                _searchText = value;

            }
        }

        #endregion // SearchText

        public void PerformSearch(cst.ClassificationLevelId classificationLevel) {

            foreach(ClassificationLevelViewModel c in _classificationLevels)
                if (c.ClassificationLevelName == "Annonceur" && classificationLevel == cst.ClassificationLevelId.Advertiser) {
                    c.LoadChildren(_searchText, classificationLevel);
                    c.IsExpanded = true;
                    break;
                }
                else if (c.ClassificationLevelName == "Produit" && classificationLevel == cst.ClassificationLevelId.Product) {
                    c.LoadChildren(_searchText, classificationLevel);
                    c.IsExpanded = true;
                    break;
                }

        }

    }
}
