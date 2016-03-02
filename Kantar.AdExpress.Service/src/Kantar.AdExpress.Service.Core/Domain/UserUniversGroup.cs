﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain
{
    public class UserUniversGroup
    {
        public long Id { get; set; }
        public int Count { get; set; }
        public string Description { get; set; }
        public List<ClientUnivers> ClientUnivers { get; set; }
    }

    public class ClientUnivers
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public long ParentId { get; set; }
        public string ParentDescription { get; set; }
        public List<UniversLevel> Levels { get; set; }
    }
}
