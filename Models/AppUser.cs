﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThienASPMVC08032023.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        public string? Name { get; set; }

        public List<Clip>? Clips { get; set; }

    }
}
