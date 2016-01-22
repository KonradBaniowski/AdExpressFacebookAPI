using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kantar.AdExpress.Service.Core.Domain.Mau
{
    public class Login
    {
        public int Id { get; set; }
        public int IdType { get; set; }
        public int IdContact { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public DateTime DateExpiration { get; set; }
        public int Activation { get; set; }
    }
}
