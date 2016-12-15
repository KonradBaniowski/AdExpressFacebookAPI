
using System.Collections.Generic;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class WebSessionResponse
    {
        public WebSessionResponse()
        {
            ControllerDetails = new ControllerDetails();
        }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public StudyStep StudyStep { get; set; }
        public ControllerDetails ControllerDetails { get; set; }
    }

    public enum StudyStep
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

    public class SaveMediaSelectionRequest
    {
        public SaveMediaSelectionRequest ()
            {
            
            }
        public SaveMediaSelectionRequest(List<long> media, string webSessionId, List<Tree> trees, Dimension dimension, Security security, bool mediaSupportRequired, string nextStep)
        {
            MediaIds = media;
            WebSessionId = webSessionId;
            Trees = trees;
            Dimension = dimension;
            Security = security;
            MediaSupportRequired = mediaSupportRequired;
            NextStep = nextStep;
        }
        public List<long> MediaIds { get; set; }
        public string WebSessionId { get; set; }
        public List<Tree> Trees { get; set; }
        public Dimension Dimension { get; set; }
        public Security Security { get; set; }
        public bool MediaSupportRequired { get; set; }
        public string NextStep { get; set; }
        public ClientInformation ClientInformation { get; set; }
    }

    public class SaveMarketSelectionRequest
    {
        public SaveMarketSelectionRequest()
        {

        }
        public SaveMarketSelectionRequest(string webSessionId, List<Tree> trees, Dimension dimension, Security security, bool required, string nextStep)
        {
            WebSessionId = webSessionId;
            Trees = trees;
            Dimension = dimension;
            Security = security;
            Required = required;
            NextStep = nextStep;
        }
        public string WebSessionId { get; set; }
        public List<Tree> Trees { get; set; }
        public Dimension Dimension { get; set; }
        public Security Security { get; set; }
        public bool Required { get; set; }
        public string NextStep { get; set; }
    }
    public class WebSessionDetails
    {
        public WebSessionDetails()
        {
            ControllerDetails = new ControllerDetails();
        }
        public WebSession WebSession { get; set; }
        public ControllerDetails ControllerDetails { get; set; }
    }
}
