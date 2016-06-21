

namespace Facebook.Service.Contract.ContractModels.ModuleFacebook
{
    public class LevelItemContract
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long SegmentId { get; set; }
        public string Segment { get; set; }
        public long GroupId { get; set; }
        public string Group { get; set; }
        public long SubSectorId { get; set; }
        public string SubSector { get; set; }
        public long SectorId { get; set; }
        public string Sector { get; set; }
        public long BrandId { get; set; }
        public string Brand { get; set; }
        public long AdvertiserId { get; set; }
        public string Advertiser { get; set; }
        public long HoldingCompanyId { get; set; }
        public string HoldingCompany { get; set; }
    }
}
