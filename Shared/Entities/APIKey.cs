﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class APIKey
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public KeyType KeyType { get; set; }
        public bool Active { get; set; }
        public string UserId { get; set; }
        public IdentityUser USer { get; set; }
    }

    public enum KeyType
    {
        Free = 1,
        Professional = 2,
    }
}
