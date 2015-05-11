using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Raven.Abstractions.Data;

namespace Catalogue.Tests.Explicit
{
    class scripted_patch_request : DatabaseTestFixture
    {
        [Test, Explicit]
        public void try_out_scripted_patch_request()
        {
            var op = ReusableDocumentStore.DatabaseCommands.UpdateByIndex("RecordIndex",
                new IndexQuery
                {
                    Query = "Keywords:\"http://vocab.jncc.gov.uk/jncc-category/Seabed Habitat Maps\""
                },
                new ScriptedPatchRequest()
                {
                    //                    Script =
                    //                        @"for (var i = 0; i < this.Keywords.length; i++)  
                    //                            this.Keywords[i].Value = '" + value + "'"
                    Script = "gibberish bhal habad"

                },
                new BulkOperationOptions {AllowStale = false}
                );


            op.WaitForCompletion();

        }
    }
}
