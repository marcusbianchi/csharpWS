using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardEngine2._0.ModelShow
{
    public class Cache
    {
        public long Id { get; set; }
        public long DashboardConfigId { get; set; }
        public string Content { get; set; }
        public string code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
