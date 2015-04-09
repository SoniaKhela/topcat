using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Catalogue.Data.Seed;
using Catalogue.Gemini.Model;
using Catalogue.Web.Code;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;

namespace Catalogue.Web.Controllers.Seed
{
    public class SeedController : ApiController
    {
        readonly IEnvironment environment;

        public SeedController(IEnvironment environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// Useful for non-live instances. Requires a POST so is difficult to do by accident.
        /// </summary>
        [HttpPost, Route("api/seed/all")]
        public HttpResponseMessage All()
        {
            if (environment.Name == "Live")
                throw new InvalidOperationException("Oops, you surely didn't mean to seed the live instance..?");

            Seeder.Seed(WebApiApplication.DocumentStore);

            return new HttpResponseMessage();
        }

        [HttpPost, Route("api/seed/vocabs")]
        public HttpResponseMessage Vocabs()
        {
            Seeder.SeedVocabsOnly(WebApiApplication.DocumentStore);

            return new HttpResponseMessage();
        }

        [HttpPost, Route("api/seed/deletemesh")]
        public HttpResponseMessage DeleteMesh()
        {
            if (environment.Name == "Live")
                throw new InvalidOperationException("Oops, you surely didn't mean to seed the live instance..?");

            WebApiApplication.DocumentStore.DatabaseCommands.DeleteByIndex("RecordIndex",
                new IndexQuery
                {
                    Query = "Keywords:\"http://vocab.jncc.gov.uk/jncc-broad-category/Seabed Habitat Maps\""
                });

            return new HttpResponseMessage();
        }

        [HttpPost, Route("api/seed/revampkeywordsx")]
        public HttpResponseMessage RevampKeywordsX()
        {
            return new HttpResponseMessage();
        }

        [HttpPost, Route("api/seed/xxx")]
        public HttpResponseMessage Xxx()
        {
            WebApiApplication.DocumentStore.DatabaseCommands.UpdateByIndex("RecordIndex",
                new IndexQuery
                {
                    Query = "Keywords:\"http://vocab.jncc.gov.uk/jncc-broad-category/Seabed Habitat Maps\""
                },
                new ScriptedPatchRequest
                {
                    Script = "this.Gemini.Keywords.push( { Value: 'aaa', Vocab: 'bbb' } );",
                }
//                new[]
//                    {
//                        new PatchRequest
//                        {
//                            Type = PatchCommandType.Set,
//                            Name = "path",
//                            Value = "jpjp" // RavenJObject.FromObject("New path")
//                            Name = "gemini",
//                            Nested = new []
//                            {
//                                new PatchRequest
//                                {
//                                    Type = PatchCommandType.Set,
//                                    Name = "path",
//                                    Value = RavenJObject.FromObject("New path")
//                                }
//                            }
//                        }
//                    }
        );

            return new HttpResponseMessage();
        }
        
        [HttpPost, Route("api/seed/revampkeywords")]
        public HttpResponseMessage RevampKeywords()
        {
            WebApiApplication.DocumentStore.DatabaseCommands.UpdateByIndex("RecordIndex",
                new IndexQuery
                {
                    Query = "Keywords:\"http://vocab.jncc.gov.uk/jncc-broad-category/Seabed Habitat Maps\""
                },
                new []
                    {
                        new PatchRequest
                        {
                            Type = PatchCommandType.Modify,
                            Name = "Gemini",
                            Nested = new []
                            {
                                new PatchRequest
                                {
                                    Type = PatchCommandType.Add,
                                    Name = "Keywords",
                                    Value = RavenJObject.FromObject(new MetadataKeyword { Vocab = "http://vocab.jncc.gov.uk/jncc-category", Value = "Marine" })
                                }
                            }
                        }
                    },
                new BulkOperationOptions {});

            return new HttpResponseMessage();
        }

        [HttpPost, Route("api/seed/inspire")]
        public HttpResponseMessage Inspire()
        {
            return new HttpResponseMessage();
        }
    }
}