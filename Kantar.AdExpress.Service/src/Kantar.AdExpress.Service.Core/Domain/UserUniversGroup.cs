﻿using KM.Framework.Constantes;
using System;
using System.Collections.Generic;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UserUniversGroup
    {
        public long Id { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public List<UserUnivers> UserUnivers { get; set; }
        public int FirstColumnSize { get; set; }
        public int SecondeColumnSize { get; set; }
    }

    public class UserUnivers
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long ParentId { get; set; }
        public string ParentDescription { get; set; }
        public List<UniversLevel> Levels { get; set; }
        public bool IsDefault { get; set; }
    }

    public class UniversGroupsResponse
    {
        public UniversGroupsResponse()
        {
            UniversGroups = new List<UserUniversGroup>();
        }
        public List<UserUniversGroup> UniversGroups { get; set; }
        public int SiteLanguage { get; set; }
        public bool CanSetDefaultUniverse { get; set; }
    }

    public class UniversGroupSaveResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int UserUniversId { get; set; }
    }
    public class UniversGroupSaveRequest
    {
        public long UniversGroupId { get; set; }
        public long? UserUniversId { get; set; }
        public List<Tree> Trees { get; set; }
        public string Name { get; set; }
        public Dimension Dimension { get; set; }
        public string WebSessionId { get; set; }
        public long IdUniverseClientDescription { get; set; }
        public List<long> MediaIds { get; set; }
        public bool IsDefaultUniverse { get; set; }
    }

    public class UniversResponse
    {
        public List<Tree> Trees { get; set; }

        public List<long> UniversMediaIds { get; set; }
        public string Message { get; set; }
        public long ModuleId { get; set; }
    }

    public class AdExpressUniversResponse
    {
        public long NbrFolder { get; set; }
        public long NbrUnivers { get; set; }
        public List<UserUniversGroup> UniversGroups { get; set; }
        public UniversType UniversType { get; set; }
        public int SiteLanguage { get; set; }
        public Labels Labels { get; set; }
    }

    public class SearchItemsCriteria
    {
        public SearchItemsCriteria()
        {
            MediaIds = new List<int>();
        }
        public SearchItemsCriteria(string webSessionId, Dimension dimension, int universeLevel)
        {
            WebSessionId = webSessionId;
            Dimension = dimension;
            UniverseLevel = universeLevel;
            MediaIds = new List<int>();
        }
        public SearchItemsCriteria( string webSessionId, Dimension dimension, int universeLevel, List<int> mediaIds)
        {
            WebSessionId = webSessionId;
            Dimension = dimension;
            UniverseLevel = universeLevel;
            MediaIds = mediaIds;
        }
        public string WebSessionId { get; set; }
        public Dimension Dimension { get; set; }
        public int UniverseLevel { get; set; }
        public List<int> MediaIds { get; set; }
    }
    
    public enum UniversType
    {
        Result,
        Univers,
        Alert
    }

}
