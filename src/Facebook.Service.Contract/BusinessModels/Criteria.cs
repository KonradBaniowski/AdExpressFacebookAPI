using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Service.Contract.BusinessModels
{
    public class Criteria
    {
        public TypeCriteria TypeCriteria { get; set; }
        public TypeNomenclature TypeNomenclature { get; set; }
        public LevelType TypeKantar { get; set; }
        public List<int> Filter { get; set; }
    }

    public enum TypeCriteria
    {
        Include = 0,
        Exclude = 1
    }

    public enum TypeNomenclature
    {
        Product = 0,
        Media = 1
    }

    public enum LevelType
    {
        Media = 1,
        Categorie = 2,
        Support = 3,
        Famille = 61,
        Classe = 62,
        Groupe = 63,
        Variete = 64,
        Annonceur = 65, 
        Marque = 66
    }

}
