using System.Collections.Generic;
using System;

namespace Km.AdExpressClientWeb.Models.Insertions
{
    public class InsertionViewModel
    {
        public List<Medias> Medias { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEnd { get; set; }
        public List<Month> Months { get; set; }
        public List<string> datas { get; set; }
    }

    public class Month
    {
        public string Label { get; set; }
        public int Id { get; set; }
    }

    public class Medias
    {
        public string Label { get; set; }
        public int LabelID { get; set; }

    }
}