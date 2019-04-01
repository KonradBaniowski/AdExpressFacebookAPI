using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Classification.Universe {

    #region Access type
    /// <summary>
    /// Access type of the items group
    /// </summary>
    [System.Serializable]
    public enum AccessType {
        /// <summary>
        /// Indicate that the items group is composed by excluded items
        /// </summary>
        excludes,
        /// <summary>
        /// Indicate that the items group is composed by included items
        /// </summary>
        includes
    }
    #endregion

    #region Dimension
    /// <summary>
    /// Universe dimension
    /// </summary>
    [System.Serializable]
    public enum Dimension {
        /// <summary>
        /// Enum corresponding to  product universe dimension 
        /// </summary>
        product,
        /// <summary>
        /// Enum corresponding to media universe dimension 
        /// </summary>
        media,
        /// <summary>
        /// Enum corresponding to advertising Agency universe dimension 
        /// </summary>
        advertisingAgency,
        /// <summary>
        /// Enum corresponding to advertisement type universe dimension 
        /// </summary>
        advertisementType,
        /// <summary>
        /// Enum corresponding to profession universe dimension 
        /// </summary>
        profession,
        /// <summary>
        /// Enum corresponding to programme Genre  universe dimension 
        /// </summary>
        programmeGenre,
        /// <summary>
        /// Enum corresponding to rubric universe dimension 
        /// </summary>
        rubric
        
        

    }
    #endregion

    #region Security
    /// <summary>
    /// Security level ofthe Universe Object
    /// </summary>
    public enum Security {
        /// <summary>
        /// allow everything
        /// </summary>
        none=0,
        /// <summary>
        /// check some rules:
        ///  - 2 groups can have the same items level
        /// </summary>
        full=1
    }
    #endregion

    #region TNSClassificationLevels
    /// <summary>
    /// TNS Level identifiers
    /// </summary>
    public class TNSClassificationLevels {
        /// <summary>
        /// Sector level
        /// </summary>
        public const Int64 SECTOR=1;
        /// <summary>
        /// Subsector level
        /// </summary>
        public const Int64 SUB_SECTOR=2;
        /// <summary>
        /// Group level
        /// </summary>
        public const Int64 GROUP_=3;
        /// <summary>
        /// Segment level
        /// </summary>
        public const Int64 SEGMENT=4;
        /// <summary>
        /// Product level
        /// </summary>
        public const Int64 PRODUCT=5;
        /// <summary>
        /// Advertiser
        /// </summary>
        public const Int64 ADVERTISER=6;
        /// <summary>
        /// Holding company level
        /// </summary>
        public const Int64 HOLDING_COMPANY=7;
        /// <summary>
        /// Brand level
        /// </summary>
        public const Int64 BRAND=8;
        /// <summary>
        /// Mega brand level
        /// </summary>
        public const Int64 MEGA_BRAND=9;
        /// <summary>
        /// Mega Advertiser level
        /// </summary>
        public const Int64 MEGA_ADVERTISER=10;
        /// <summary>
        /// Vehicle level
        /// </summary>
        public const Int64 VEHICLE=11;
        /// <summary>
        /// Category level
        /// </summary>
        public const Int64 CATEGORY=12;
        /// <summary>
        /// Basic media level
        /// </summary>
        public const Int64 BASIC_MEDIA=13;
        /// <summary>
        /// Media level
        /// </summary>
        public const Int64 MEDIA=14;
        /// <summary>
        /// Interest center level
        /// </summary>
        public const Int64 INTEREST_CENTER = 15;
        /// <summary>
        /// Sub brand level
        /// </summary>
        public const Int64 SUB_BRAND = 16;
        /// <summary>
        /// Region level
        /// </summary>
        public const Int64 REGION = 17;
        /// <summary>
        /// Advertisement Type level
        /// </summary>
        public const Int64 ADVERTISEMENT_TYPE = 18;
        /// <summary>
        /// Advertising  agency level
        /// </summary>
        public const Int64 ADVERTISING_AGENCY = 19;
        /// <summary>
        /// Group Advertising  agency level
        /// </summary>
        public const Int64 GROUP_ADVERTISING_AGENCY = 20;
        /// <summary>
        /// Profession level
        /// </summary>
        public const Int64 PROFESSION = 21;
        /// <summary>
        /// rubric level
        /// </summary>
        public const Int64 RUBRIC = 22;
        /// <summary>
        /// programme level
        /// </summary>
        public const Int64 PROGRAMME = 23;
        /// <summary>
        /// programme genre  level
        /// </summary>
        public const Int64 PROGRAMME_GENRE = 24;
        /// <summary>
        /// Name level
        /// </summary>
        public const Int64 NAME = 25;
        /// <summary>
        /// Presence type level
        /// </summary>
        public const Int64 PRESENCE_TYPE = 26;
        /// <summary>
        /// Presence Title level
        /// </summary>
        public const Int64 TITLE = 27;
        /// <summary>
        /// Presence Media Seller level
        /// </summary>
        public const Int64 MEDIA_SELLER = 28;
        /// <summary>
        /// Canal level
        /// </summary>
        public const Int64 CANAL = 29;
        /// <summary>
        /// Medecin level
        /// </summary>
        public const Int64 MEDECIN = 30;
        /// <summary>
        /// Form sponsorship level
        /// </summary>
        public const Int64 FORM_SPONSORSHIP = 31;
        /// <summary>
        /// Grp Pharma level
        /// </summary>
        public const Int64 GRP_PHARMA = 32;
        /// <summary>
        /// Advertiser type level
        /// </summary>
        public const Int64 ADVERTISER_TYPE = 33;

        /// <summary>
        /// Ad Slogan level
        /// </summary>
        public const Int64 AD_SLOGAN = 34;
        /// <summary>
        /// Purchasing  agency level
        /// </summary>
        public const Int64 PURCHASING_AGENCY = 35;
        /// <summary>
        /// Creative  agency level
        /// </summary>
        public const Int64 CREATIVE_AGENCY = 36;
        /// <summary>
        /// Spot  genre level
        /// </summary>
        public const Int64 SPOT_GENRE = 37;
        /// <summary>
        /// Spot sub genre level
        /// </summary>
        public const Int64 SPOT_SUB_GENRE = 38;

        /// <summary>
        /// Media Owner level
        /// </summary>
        public const Int64 MEDIA_OWNER = 39;
        /// <summary>
        /// Program level
        /// </summary>
        public const Int64 PROGRAM = 40;
        /// <summary>
        /// Program Typology level
        /// </summary>
        public const Int64 PROGRAM_TYPOLOGY = 41;
        /// <summary>
        /// Spot Sub Type level
        /// </summary>
        public const Int64 SPOT_SUB_TYPE = 42;
    }
    #endregion
}
