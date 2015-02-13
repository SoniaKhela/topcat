﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Catalogue.Data.Seed;
using Catalogue.Web.Code;
using Raven.Client;

namespace Catalogue.Web.Controllers.Seed
{
    public class SeedController : ApiController
    {
        readonly IDocumentStore store;
        readonly IEnvironment environment;

        public SeedController(IDocumentStore store, IEnvironment environment)
        {
            this.store = store;
            this.environment = environment;
        }

        /// <summary>
        /// Useful for non-live instances. Requires a POST so is difficult to do by accident.
        /// </summary>
        public HttpResponseMessage Post()
        {
            if (environment.Name == "Live")
                throw new InvalidOperationException("Oops, you surely didn't mean to seed the live instance..?");

            Seeder.Seed(store);

            return new HttpResponseMessage();
        }
    }
}