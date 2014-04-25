using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.Domain.DataModel;

namespace AdVolumeChecker.Search.ViewModel {
    class ClassificationLevelItemViewModel : TreeViewItemViewModel {

        readonly ClassificationLevelItem _classificationLevelItem;

        public ClassificationLevelItemViewModel(ClassificationLevelItem classificationLevelItem, ClassificationLevelViewModel classificationLevel)
            : base(classificationLevel)
        {
            _classificationLevelItem = classificationLevelItem;
        }

        public string ClassificationLevelItemName
        {
            get { return _classificationLevelItem.ClassificationLevelItemName; }
        }

    }
}
