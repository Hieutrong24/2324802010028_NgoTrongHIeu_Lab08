using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Model
{
    public class MasterDataValue : BaseEntity
    {
        public string MasterDataKeyId { get; set; } = string.Empty;

        public MasterDataKey? MasterDataKey { get; set; }

        public string Value { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
