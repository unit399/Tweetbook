﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tweetbook.Contracts.V1.Requests.Queries
{
    public class GetAllPostsQuery
    {
        [FromQuery(Name= "userId")]
        public string UserId { get; set; }
    }
}
