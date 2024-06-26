﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Bislerium.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BisleriumUser class
public class BisleriumUser : IdentityUser
{

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string? ProfilePicture { get; set; }
}

