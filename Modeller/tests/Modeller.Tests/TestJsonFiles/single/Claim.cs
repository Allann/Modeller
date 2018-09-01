using Jbssa.Core;
using Jbssa.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jbssa.ASAP
{
    public partial class Claim : EntityAuditable
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        [StringLength(200)]
        public string ClaimValueType { get; set; }
        public bool AllowMultipleInstance { get; set; }
        public bool AlwaysIncludeInIdToken { get; set; }
        public bool IsResourceValue { get; set; }
        public bool IsRoleValue { get; set; }
        public bool IsUserValue { get; set; }
        public bool IsActive { get; set; }
        public override string BusinessKey => Name;
        public override string ToString()
        {
            return BusinessKey;
        }
    }
}
