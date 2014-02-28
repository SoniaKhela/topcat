﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Model;
using Catalogue.Gemini.Model;
using Catalogue.Gemini.Roles;
using Catalogue.Gemini.Templates;
using Catalogue.Gemini.Vocabs;
using Catalogue.Utilities.Clone;
using Catalogue.Utilities.Expressions;
using Catalogue.Utilities.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Catalogue.Data.Write
{
    public interface IRecordValidator
    {
        RecordValidationErrorSet Validate(Record record);
    }

    public class RecordValidator : IRecordValidator
    {
        public RecordValidationErrorSet Validate(Record record)
        {
            var errors = new RecordValidationErrorSet();

            // path_must_not_be_blank
            if (record.Path.IsBlank())
            {
                errors.Add("Path must not be blank.", r => r.Path);
            }

            // title_must_not_be_blank
            if (record.Gemini.Title.IsBlank())
            {
                errors.Add("Title must not be blank.", r => r.Gemini.Title);
            }

            ValidateTopicCategory(record, errors);
            ValidateResourceLocator(record, errors);
            ValidateResponsibleOrganisation(record, errors);
            ValidateMetadataPointOfContact(record, errors);

            // non_open_records_must_describe_their_limitations_on_public_access
            if (record.Security != Security.Open && record.Gemini.LimitationsOnPublicAccess.IsBlank())
            {
                errors.Add("Non-open records must describe their limitations on public access.",
                    r => r.Security,
                    r => r.Gemini.LimitationsOnPublicAccess);
            }

            // publishable_records_must_have_a_resource_locator
            if (record.Status == Status.Publishable && record.Gemini.ResourceLocator.IsBlank())
            {
                errors.Add("Publishable records must have a resource locator.",
                    r => r.Status, r => r.Gemini.ResourceLocator);
            }

            return errors;
        }

        void ValidateResourceLocator(Record record, RecordValidationErrorSet errors)
        {
            // resource_locator_must_be_a_well_formed_http_url
            if (record.Gemini.ResourceLocator.IsNotBlank())
            {
                Uri url;

                if (Uri.TryCreate(record.Gemini.ResourceLocator, UriKind.Absolute, out url))
                {
                    if (url.Scheme != Uri.UriSchemeHttp)
                    {
                        errors.Add("Resource locator must be an http url",
                            r => r.Gemini.ResourceLocator);
                    }
                }
                else
                {
                    errors.Add("Resource locator must be a valid url",
                        r => r.Gemini.ResourceLocator);
                }
            }
        }

        void ValidateTopicCategory(Record record, RecordValidationErrorSet errors)
        {
            // topic_category_must_be_valid

            string s = record.Gemini.TopicCategory;

            if (s.IsNotBlank() && !TopicCategories.Values.Keys.Any(k => k == s))
            {
                errors.Add(String.Format("Topic Category '{0}' is not valid.", record.Gemini.TopicCategory),
                    r => r.Gemini.TopicCategory);
            }
        }

        void ValidateResponsibleOrganisation(Record record, RecordValidationErrorSet errors)
        {
            // responsible_organisation_role_must_be_an_allowed_role
            string role = record.Gemini.ResponsibleOrganisation.Role;
            if (role.IsNotBlank() && !ResponsiblePartyRoles.Allowed.Contains(role))
            {
                errors.Add(String.Format("Responsible Organisation Role '{0}' is not valid.", role),
                    r => r.Gemini.ResponsibleOrganisation.Role);
            }
        }

        void ValidateMetadataPointOfContact(Record record, RecordValidationErrorSet errors)
        {
            // metadata_point_of_contact_role_must_be_an_allowed_role
            string role = record.Gemini.MetadataPointOfContact.Role;
            if (role.IsNotBlank() && !ResponsiblePartyRoles.Allowed.Contains(role))
            {
                errors.Add(String.Format("Metadata Point of Contact Role '{0}' is not valid.", role),
                    r => r.Gemini.MetadataPointOfContact.Role);
            }
        }
    }


    public class RecordValidationError
    {
        public RecordValidationError(string message, List<Expression<Func<Record, object>>> fields)
        {
            Message = message;
            FieldExpressions = fields;
        }

        public string Message { get; private set; }
        List<Expression<Func<Record, object>>> FieldExpressions { get; set; }

        /// <summary>
        /// A representation of the property accessor expression(s) suitable for eg a json client.
        /// </summary>
        public List<string> Fields
        {
            get
            {
                return (from e in FieldExpressions
                        let expression = e.Body.RemoveUnary()
                        let fullDottedPath = expression.ToString().Replace("r.", String.Empty)
                        let camelCasedProperties = fullDottedPath.Split('.').Select(StringUtility.ToCamelCase)
                        select String.Join(".", camelCasedProperties))
                       .ToList();
            }
        }

    }

    public class RecordValidationErrorSet : Collection<RecordValidationError>
    {
        public void Add(string message, params Expression<Func<Record, object>>[] fields)
        {
            this.Add(new RecordValidationError(message, fields.ToList()));
        }
    }

    class record_validator_tests
    {
        Record BasicRecord()
        {
            return new Record
                {
                    Path = @"X:\some\path",
                    Gemini = Library.Blank().With(m => m.Title = "Some title"),
                };
        }

        [Test]
        public void title_must_not_be_blank([Values("", " ", null)] string blank)
        {
            var errors = new RecordValidator().Validate(BasicRecord().With(r => r.Gemini.Title = blank));

            errors.Single().Message.Should().StartWith("Title must not be blank");
            errors.Single().Fields.Single().Should().Be("gemini.title");
        }

        [Test]
        public void path_must_not_be_blank([Values("", " ", null)] string blank)
        {
            var errors = new RecordValidator().Validate(BasicRecord().With(r => r.Path = blank));

            errors.Single().Message.Should().StartWith("Path must not be blank");
            errors.Single().Fields.Single().Should().Be("path");
        }

        [Test]
        public void topic_category_must_be_valid()
        {
            var record = BasicRecord().With(r => r.Gemini.TopicCategory = "anInvalidTopicCategory");
            var errors = new RecordValidator().Validate(record);

            errors.Single().Message.Should().Contain("Topic Category 'anInvalidTopicCategory' is not valid.");
            errors.Single().Fields.Single().Should().Be("gemini.topicCategory");
        }

        [Test]
        public void topic_category_may_be_blank([Values("", null)] string blank)
        {
            var record = BasicRecord().With(r => r.Gemini.TopicCategory = blank);
            var errors = new RecordValidator().Validate(record);
            errors.Should().BeEmpty();
        }

        [Test]
        public void non_open_records_must_describe_their_limitations_on_public_access(
            [Values(Security.Classified, Security.Restricted)] Security nonOpen,
            [Values("", " ", null)] string blank)
        {
            var record = BasicRecord().With(r =>
                {
                    r.Security = nonOpen;
                    r.Gemini.LimitationsOnPublicAccess = blank;
                });

            var errors = new RecordValidator().Validate(record);
            
            errors.Single().Fields.Should().Contain("security");
            errors.Single().Fields.Should().Contain("gemini.limitationsOnPublicAccess");
        }

        [Test]
        public void publishable_records_must_have_a_resource_locator([Values("", " ", null)] string blank)
        {
            var record = BasicRecord().With(r =>
                {
                    r.Status = Status.Publishable;
                    r.Gemini.ResourceLocator = blank;
                });

            var errors = new RecordValidator().Validate(record);

            errors.Single().Fields.Should().Contain("status");
            errors.Single().Fields.Should().Contain("gemini.resourceLocator");
        }

        [Test]
        public void resource_locator_must_be_a_well_formed_http_url([Values(@"Z:\some\path", "utter rubbish")] string nonHttpUrl)
        {
            var record = BasicRecord().With(r => r.Gemini.ResourceLocator = nonHttpUrl);
            var errors = new RecordValidator().Validate(record);
            errors.Single().Fields.Should().Contain("gemini.resourceLocator");
        }

        [Test]
        public void resource_locator_may_be_set()
        {
            var record = BasicRecord().With(r => r.Gemini.ResourceLocator = "http://example.org/resource/locator");
            var errors = new RecordValidator().Validate(record);
            errors.Should().BeEmpty();
        }

        [Test]
        public void responsible_party_role_must_be_an_allowed_role()
        {
            var record = BasicRecord().With(r => r.Gemini.ResponsibleOrganisation = new ResponsibleParty
                {
                    Email = "a.mann@example.com",
                    Name = "A. Mann",
                    Role = "some role that isn't allowed",
                });
            var errors = new RecordValidator().Validate(record);
            errors.Single().Fields.Should().Contain("gemini.responsibleOrganisation.role");
        }

        [Test]
        public void metadata_point_of_contact_role_must_be_an_allowed_role()
        {
            var record = BasicRecord().With(r => r.Gemini.MetadataPointOfContact = new ResponsibleParty
                {
                    Email = "a.mann@example.com",
                    Name = "A. Mann",
                    Role = "some role that isn't allowed",
                });
            var errors = new RecordValidator().Validate(record);
            errors.Single().Fields.Should().Contain("gemini.metadataPointOfContact.role");
        }

        // todo
        // 
        // warning for blank use constraints - could be "no conditions apply" if that's what's meant
        // warning for unknown data format
        // valid email addresses, dates, ...
        // resource_locator_must_be_a_public_url (validator might need an http service)
    }
}
