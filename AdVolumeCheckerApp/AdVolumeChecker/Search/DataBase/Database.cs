using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.Domain.DataModel;

namespace AdVolumeChecker.Search.DataBase {
    public class Database {

        #region GetClassificationLevels

        public static ClassificationLevel[] GetClassificationLevels() {
            return new ClassificationLevel[]
            {
                new ClassificationLevel("Annonceur"),
                new ClassificationLevel("Produit")
                //, new ClassificationLevel("Version")
            };
        }

        #endregion 

        #region GetClassificationLevelItems

        public static ClassificationLevelItem[] GetClassificationLevelItems(ClassificationLevel ClassificationLevel) {
            switch (ClassificationLevel.classificationLevelName) {
                case "Annonceur":
                    return new ClassificationLevelItem[]
                    {
                        new ClassificationLevelItem("Annonceur 1"),
                        new ClassificationLevelItem("Annonceur 2")
                    };

                case "Produit":
                    return new ClassificationLevelItem[]
                    {
                        new ClassificationLevelItem("Produit 1"),
                        new ClassificationLevelItem("Produit 2")
                    };
                case "Version":
                    return new ClassificationLevelItem[]
                    {
                        new ClassificationLevelItem("Version 1"),
                        new ClassificationLevelItem("Version 2")
                    };
            }

            return null;
        }

        public static ClassificationLevelItem[] GetClassificationLevelItems2() {
            
                    return new ClassificationLevelItem[]
                    {
                        new ClassificationLevelItem("Annonceur 3"),
                        new ClassificationLevelItem("Annonceur 4")
                    };

        }

        #endregion 

    }
}
