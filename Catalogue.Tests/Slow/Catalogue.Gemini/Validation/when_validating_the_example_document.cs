﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Gemini.Encoding;
using Catalogue.Gemini.Templates;
using Catalogue.Gemini.Validation;
using FluentAssertions;
using NUnit.Framework;

namespace Catalogue.Tests.Slow.Catalogue.Gemini.Validation
{
    /// <summary>
    /// This is an end-to-end style test that exercises several bits of the system
    /// and depends on an external web service.
    /// </summary>
    class when_validating_the_example_document
    {
        [Test]
        public void should_be_valid_gemini()
        {
            // start with the example document
            var metadata = Library.Example();
            
            // ...encode it into xml
            var doc = new XmlEncoder().Create(new Guid("b97aac01-5e5d-4209-b626-514e40245bc1"), metadata);

            // ...validate it with the CEH validator
            var result = new Validator().Validate(doc);

            result.Results.Single(r => r.Validation.StartsWith("GEMINI2"))
                .Valid
                .Should().BeTrue();
        }
    }
}
