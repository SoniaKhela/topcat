﻿using System;

namespace Catalogue.Web.Controllers.Publishing
{
    public class SignOffRequest
    {
        public Guid Id { get; set; }
        public string Comment { get; set; }
    }
}
