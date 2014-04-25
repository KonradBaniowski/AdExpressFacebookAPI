using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AdVolumeChecker.Search.DataBase;
using KMI.AdExpress.AdVolumeChecker.DAL;
using KMI.AdExpress.AdVolumeChecker.Domain;
using KMI.AdExpress.AdVolumeChecker.Domain.DataModel;
using cst = KMI.AdExpress.AdVolumeChecker.Domain.Constantes;

namespace AdVolumeChecker.Search.ViewModel {
    public class ClassificationLevelViewModel : TreeViewItemViewModel {

        readonly ClassificationLevel _classificationLevel;

        public ClassificationLevelViewModel(ClassificationLevel classificationLevel) 
            : base(null)
        {
            _classificationLevel = classificationLevel;
            foreach (ClassificationLevelItem classificationLevelItem in _classificationLevel.ClassificationLevelItems)
                LoadChildren(classificationLevelItem);
        }

        public string ClassificationLevelName
        {
            get { return _classificationLevel.classificationLevelName; }
        }

        /// <summary>
        /// Invoked when the child items need to be loaded on demand.
        /// Subclasses can override this to populate the Children collection.
        /// </summary>
        public void LoadChildren(string searchedText, cst.ClassificationLevelId classificationLevel) {

            DataSet ds = AdVolumeCheckerDAL.GetClassificationLevelItems(DataBaseInformation.ConnectionString, classificationLevel, searchedText);

            foreach (DataRow row in ds.Tables[0].Rows) {
                ClassificationLevelItem classificationLevelItem = new ClassificationLevelItem(row["label"].ToString() + " (" + row["id"].ToString() + ")");
                base.Children.Add(new ClassificationLevelItemViewModel(classificationLevelItem, this));
            }

        }

        public void RemoveChildren() {
            base.Children.Clear();
        }

        protected void LoadChildren(ClassificationLevelItem classificationLevelItem)
        {
            base.Children.Add(new ClassificationLevelItemViewModel(classificationLevelItem, this));
        }

        public void Load() {
            ClassificationLevelItem[] toto = Database.GetClassificationLevelItems2();
            foreach (ClassificationLevelItem c in toto)
                base.Children.Add(new ClassificationLevelItemViewModel(c, this));
        }

    }
}
