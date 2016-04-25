
using TNS.AdExpress.Classification;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class WebSessionResponse
    {
        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        public MediaScheduleStep MediaScheduleStep { get; set; }
    }

    public enum MediaScheduleStep
    {
        Market,
        Media,
        Period,
        Result
    }

    public class AdExpressUnivers
    {
        public AdExpressUnivers (TNS.Classification.Universe.Dimension dimension)
        {
            AdExpressUniverse = new AdExpressUniverse(dimension);
        }
        public AdExpressUniverse AdExpressUniverse { get; set; }
        public bool Success { get; set; }
        public string Message { get;set;}
    }
}
