﻿using Catalogue.Data.Model;
using Catalogue.Data.Test;
using Catalogue.Data.Write;
using Catalogue.Gemini.Model;
using Catalogue.Gemini.Templates;
using Catalogue.Utilities.Clone;
using Catalogue.Utilities.Time;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using Catalogue.Robot.Publishing.OpenData;
using Moq;

namespace Catalogue.Tests.Slow.Catalogue.Data.Write
{
    class open_data_publishing_service_specs
    {
        [Test]
        public void successful_upload()
        {
            var recordId = new Guid("eb189a2f-ebce-4232-8dc6-1ad486cacf21");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\upload\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.Title = "Publishing upload test";
                    m.Keywords.Add(new MetadataKeyword
                    {
                        Vocab = "http://vocab.jncc.gov.uk/jncc-domain",
                        Value = "Terrestrial"
                    });
                    m.Keywords.Add(new MetadataKeyword
                    {
                        Vocab = "http://vocab.jncc.gov.uk/jncc-category",
                        Value = "Example Collection"
                    });
                    m.ResourceLocator = null;
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 08, 02),
                            User = new UserInfo
                            {
                                DisplayName = "IAO User",
                                Email = "iaouser@example.com"
                            }
                        }
                    }
                };
                r.Footer = new Footer();
            });

            var store = new InMemoryDatabaseHelper().Create();
            using (var db = store.OpenSession())
            {
                db.Store(record);
                db.SaveChanges();

                var currentTime = Clock.CurrentUtcDateTimeGetter;
                var testTime = new DateTime(2017, 08, 18, 12, 0, 0);
                Clock.CurrentUtcDateTimeGetter = () => testTime;

                var uploadService = new OpenDataPublishingUploadService(db, new RecordService(db, new RecordValidator()));
                var uploadHelperMock = new Mock<IOpenDataUploadHelper>();
                var uploader = new RobotUploader(db, uploadService, uploadHelperMock.Object);

                uploadHelperMock.Setup(x => x.GetHttpRootUrl()).Returns("http://data.jncc.gov.uk");

                uploader.Upload(new List<Record>{record});

                var updatedRecord = db.Load<Record>(record.Id);
                updatedRecord.Publication.OpenData.LastAttempt.DateUtc.Should().Be(testTime);
                updatedRecord.Publication.OpenData.LastAttempt.Message.Should().BeNull();
                updatedRecord.Publication.OpenData.LastSuccess.DateUtc.Should().Be(testTime);
                updatedRecord.Publication.OpenData.LastSuccess.Message.Should().BeNull();
                updatedRecord.Gemini.ResourceLocator.Should().Be("http://data.jncc.gov.uk/data/eb189a2f-ebce-4232-8dc6-1ad486cacf21-test");
                uploadHelperMock.Verify(x => x.UploadDataFile(record.Id, record.Path, false), Times.Once);
                uploadHelperMock.Verify(x => x.UploadMetadataDocument(record), Times.Once);
                uploadHelperMock.Verify(x => x.UploadWafIndexDocument(record), Times.Once);

                Clock.CurrentUtcDateTimeGetter = currentTime;
            }
        }

        [Test]
        public void failed_upload()
        {
            var recordId = new Guid("8ad134fa-9045-40af-a0cb-02bc3e868f5a");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\upload\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.Title = "Publishing upload test";
                    m.Keywords.Add(new MetadataKeyword
                    {
                        Vocab = "http://vocab.jncc.gov.uk/jncc-domain",
                        Value = "Terrestrial"
                    });
                    m.Keywords.Add(new MetadataKeyword
                    {
                        Vocab = "http://vocab.jncc.gov.uk/jncc-category",
                        Value = "Example Collection"
                    });
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 08, 02),
                            User = new UserInfo
                            {
                                DisplayName = "IAO User",
                                Email = "iaouser@example.com"
                            }
                        },
                        Resources = new List<Resource> { new Resource{ Path = "x:\\test\\path" } }
                    }
                };
                r.Footer = new Footer();
            });

            var store = new InMemoryDatabaseHelper().Create();
            using (var db = store.OpenSession())
            {
                db.Store(record);
                db.SaveChanges();

                var currentTime = Clock.CurrentUtcDateTimeGetter;
                var testTime = new DateTime(2017, 08, 18, 12, 0, 0);
                Clock.CurrentUtcDateTimeGetter = () => testTime;

                var uploadService = new OpenDataPublishingUploadService(db, new RecordService(db, new RecordValidator()));
                var uploadHelperMock = new Mock<IOpenDataUploadHelper>();
                var uploader = new RobotUploader(db, uploadService, uploadHelperMock.Object);

                uploadHelperMock.Setup(x => x.UploadAlternativeResources(record, false))
                    .Throws(new WebException("test message"));

                uploader.Upload(new List<Record> { record });

                var updatedRecord = db.Load<Record>(record.Id);
                updatedRecord.Publication.OpenData.LastAttempt.DateUtc.Should().Be(testTime);
                updatedRecord.Publication.OpenData.LastAttempt.Message.Should().Be("test message");
                updatedRecord.Publication.OpenData.LastSuccess.Should().BeNull();

                Clock.CurrentUtcDateTimeGetter = currentTime;
            }
        }

        [Test]
        public void successful_corpulent_upload()
        {
            var recordId = new Guid("e9f1eb92-3fcb-441f-9fdf-520ff52bcf56");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\upload\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.Keywords.Add(new MetadataKeyword
                    {
                        Vocab = "http://vocab.jncc.gov.uk/jncc-domain",
                        Value = "Terrestrial"
                    });
                    m.Keywords.Add(new MetadataKeyword
                    {
                        Vocab = "http://vocab.jncc.gov.uk/jncc-category",
                        Value = "Example Collection"
                    });
                    m.Keywords.Add(new MetadataKeyword
                    {
                        Vocab = "http://vocab.jncc.gov.uk/metadata-admin",
                        Value = "Corpulent"
                    });
                    m.ResourceLocator = null;
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 08, 02),
                            User = new UserInfo
                            {
                                DisplayName = "IAO User",
                                Email = "iaouser@example.com"
                            }
                        }
                    }
                };
                r.Footer = new Footer();
            });

            var store = new InMemoryDatabaseHelper().Create();
            using (var db = store.OpenSession())
            {
                db.Store(record);
                db.SaveChanges();

                var currentTime = Clock.CurrentUtcDateTimeGetter;
                var testTime = new DateTime(2017, 08, 18, 12, 0, 0);
                Clock.CurrentUtcDateTimeGetter = () => testTime;

                var uploadService = new OpenDataPublishingUploadService(db, new RecordService(db, new RecordValidator()));
                var uploadHelperMock = new Mock<IOpenDataUploadHelper>();
                var uploader = new RobotUploader(db, uploadService, uploadHelperMock.Object);
                uploader.Upload(new List<Record> { record });

                var updatedRecord = db.Load<Record>(record.Id);
                updatedRecord.Publication.OpenData.LastAttempt.DateUtc.Should().Be(testTime);
                updatedRecord.Publication.OpenData.LastAttempt.Message.Should().BeNull();
                updatedRecord.Publication.OpenData.LastSuccess.DateUtc.Should().Be(testTime);
                updatedRecord.Publication.OpenData.LastSuccess.Message.Should().BeNull();
                updatedRecord.Gemini.ResourceLocator.Should().Be("http://jncc.defra.gov.uk/opendata");
                uploadHelperMock.Verify(x => x.UploadMetadataDocument(record), Times.Once);
                uploadHelperMock.Verify(x => x.UploadWafIndexDocument(record), Times.Once);

                Clock.CurrentUtcDateTimeGetter = currentTime;
            }
        }
    }
}
