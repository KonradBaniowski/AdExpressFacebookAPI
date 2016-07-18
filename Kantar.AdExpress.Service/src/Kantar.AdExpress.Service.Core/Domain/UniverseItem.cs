using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UniversItem
    {
        public long Id { get; set; }
        public string Label { get; set; }
        //TODO USEFULL?
        public long IdLevelUniverse { get; set; }
    }
    public class SearchRequest
    {
        public SearchRequest (int levelId, string keyword, string webSessionId, Dimension dimension, List<int> media)
        {
            LevelId = levelId;
            Keyword = keyword;
            WebSessionId = webSessionId;
            Dimension = dimension;
            MediaList = media;           
        }
        public SearchRequest(int levelId, string keyword, string webSessionId, Dimension dimension)
        {
            LevelId = levelId;
            Keyword = keyword;
            WebSessionId = webSessionId;
            Dimension = dimension;
        }
        public SearchRequest()
        {
        }
        public int LevelId { get; set; }
        public string Keyword { get; set; }
        public string WebSessionId { get; set; }
        public Dimension Dimension { get; set; }
        public List<int> MediaList { get; set; }
    }
}
