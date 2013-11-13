﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Model;
using Catalogue.Gemini.Model;
using Catalogue.Gemini.Templates;
using Catalogue.Utilities.Spatial;
using Catalogue.Utilities.Text;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Raven.Client;

namespace Catalogue.Data.Write
{
    public interface IRecordService
    {
        Record Load(Guid id);
        RecordValidationResult Insert(Record record);
        RecordValidationResult Update(Record record);
    }

    public class RecordService : IRecordService
    {
        readonly IDocumentSession db;
        readonly IRecordValidator validator;

        public RecordService(IDocumentSession db, IRecordValidator validator)
        {
            this.db = db;
            this.validator = validator;
        }

        public Record Load(Guid id)
        {
            return db.Load<Record>(id);
        }

        public RecordValidationResult Insert(Record record)
        {
            return Upsert(record);
        }

        public RecordValidationResult Update(Record record)
        {
            // additional validation for updating
            if (record.ReadOnly)
                return new RecordValidationResult { Message = "Cannot update read-only record." };

            return Upsert(record);
        }

        internal RecordValidationResult Upsert(Record record)
        {
            this.SyncDenormalizations(record);

            var result = validator.Validate(record);

            if (result.Success)
                db.Store(record);

            return result;
        }

        void SyncDenormalizations(Record record)
        {
            // we store the bounding box as wkt so we can index it
            if (!BoundingBoxUtility.IsBlank(record.Gemini.BoundingBox))
                record.Wkt = BoundingBoxUtility.ToWkt(record.Gemini.BoundingBox);
        }
    }


    class when_inserting_a_record
    {
        [Test]
        public void should_fail_if_path_is_blank()
        {
            // this is really a test of the validator, so needs moving

            var record = new Record
                {
                    Path = null,
                    Gemini = new Metadata { Title = "A record without a path" }
                };

            var service = new RecordService(Mock.Of<IDocumentSession>(), new RecordValidator());

            var result = service.Insert(record);
            result.Success.Should().BeFalse();
            result.Message.Should().StartWith("Path must not be blank");
        }
    }

    class when_upserting_a_record
    {
        [Test]
        public void should_fail_if_record_is_readonly()
        {
            // erm, this is really testing the validator now

            var service = new RecordService(Mock.Of<IDocumentSession>(), new RecordValidator());

            var record = new Record { ReadOnly = true };

            var result = service.Update(record);
            result.Success.Should().BeFalse();
            result.Message.Should().StartWith("Cannot update read-only record");
        }

        [Test]
        public void bounding_box_should_be_stored_as_wkt()
        {
            var database = Mock.Of<IDocumentSession>();
            var service = new RecordService(database, GetValidatorStub());

            var e = Library.Example();
            var record = new Record { Gemini = e };

            service.Upsert(record);

            string expectedWkt = BoundingBoxUtility.ToWkt(e.BoundingBox);
            Mock.Get(database).Verify(db => db.Store(It.Is((Record r) => r.Wkt == expectedWkt)));
        }

        [Test]
        public void empty_bounding_box_should_be_stored_as_null_wkt()
        {
            // (to avoid raven / lucene errors)

            var database = Mock.Of<IDocumentSession>();
            var service = new RecordService(database, GetValidatorStub());

            var record = new Record { Gemini = Library.Blank() };

            service.Upsert(record);

            Mock.Get(database).Verify(db => db.Store(It.Is((Record r) => r.Wkt == null)));

        }

        IRecordValidator GetValidatorStub()
        {
            return Mock.Of<IRecordValidator>(v => v.Validate(It.IsAny<Record>()) == new RecordValidationResult { Success = true });
        }
    }
}
