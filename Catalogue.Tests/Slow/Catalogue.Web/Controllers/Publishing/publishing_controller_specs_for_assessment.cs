﻿using Catalogue.Data.Model;
using Catalogue.Data.Test;
using Catalogue.Data.Write;
using Catalogue.Gemini.Templates;
using Catalogue.Utilities.Clone;
using Catalogue.Web.Account;
using Catalogue.Web.Controllers.Publishing;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Raven.Client;
using System;

namespace Catalogue.Tests.Slow.Catalogue.Web.Controllers.Publishing
{
    class publishing_controller_specs_for_assessment
    {
        [Test]
        public void assessment_completed_test()
        {
            var recordId = new Guid("1a86bbbe-7f19-4fe2-82ff-7847e68266da");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true
                    }
                };
                r.Footer = new Footer();
            });

            var resultRecord = TestAssessment(record);

            resultRecord.Publication.Should().NotBeNull();
            resultRecord.Footer.ModifiedByUser.DisplayName.Should().Be("Test User");
            resultRecord.Footer.ModifiedOnUtc.Should().NotBe(DateTime.MinValue);

            var openDataInfo = resultRecord.Publication.OpenData;
            openDataInfo.Should().NotBeNull();
            openDataInfo.LastAttempt.Should().BeNull();
            openDataInfo.LastSuccess.Should().BeNull();
            openDataInfo.Resources.Should().BeNull();
            openDataInfo.Paused.Should().BeFalse();
            openDataInfo.Assessment.Completed.Should().BeTrue();
            openDataInfo.Assessment.CompletedByUser.DisplayName.Should().Be("Test User");
            openDataInfo.Assessment.CompletedByUser.Email.Should().Be("tester@example.com");
            openDataInfo.Assessment.CompletedOnUtc.Should().NotBe(DateTime.MinValue);
            resultRecord.Gemini.MetadataDate.Should().Be(openDataInfo.Assessment.CompletedOnUtc);
        }

        [Test]
        public void assessment_completed_with_unc_path_test()
        {
            var recordId = new Guid("b69f47c1-4c17-42d0-a396-8209aa5568b1");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"\\jncc-corpfile\testfile";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example();
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true
                    }
                };
                r.Footer = new Footer();
            });

            var resultRecord = TestAssessment(record);
            var openDataInfo = resultRecord.Publication.OpenData;
            openDataInfo.Assessment.Completed.Should().BeTrue();
        }

        [Test]
        public void assessment_started_then_completed_test()
        {
            var recordId = new Guid("ec0db5b3-8b9d-42c3-ac70-2fd50ff3bbca");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 21);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = false,
                            CompletedByUser = null,
                            CompletedOnUtc = DateTime.MinValue,
                            InitialAssessmentWasDoneOnSpreadsheet = false
                        }
                    }
                };
                r.Footer = new Footer();
            });

            var resultRecord = TestAssessment(record);

            resultRecord.Publication.Should().NotBeNull();

            var openDataInfo = resultRecord.Publication.OpenData;
            openDataInfo.Should().NotBeNull();
            openDataInfo.LastAttempt.Should().BeNull();
            openDataInfo.LastSuccess.Should().BeNull();
            openDataInfo.Resources.Should().BeNull();
            openDataInfo.Paused.Should().BeFalse();
            openDataInfo.Assessment.Completed.Should().BeTrue();
            openDataInfo.Assessment.CompletedByUser.DisplayName.Should().Be("Test User");
            openDataInfo.Assessment.CompletedByUser.Email.Should().Be("tester@example.com");
            openDataInfo.Assessment.CompletedOnUtc.Should().NotBe(DateTime.MinValue);
        }

        [Test]
        public void assessment_already_completed_test()
        {
            var recordId = new Guid("1a86bbbe-7f19-4fe2-82ff-7847e68266da");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true,
                            CompletedOnUtc = new DateTime(2017, 07, 30)
                        }
                    }
                };
                r.Footer = new Footer();
            });

            Action a = () => TestAssessment(record);
            a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Assessment has already been completed and is up to date");
        }

        [Test]
        public void assessment_already_completed_on_spreadsheet_test()
        {
            var recordId = new Guid("170001cf-1117-459d-a554-b1fc031d439c");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true,
                            InitialAssessmentWasDoneOnSpreadsheet = true,
                            CompletedOnUtc = DateTime.MinValue
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = DateTime.MinValue
                        },
                        LastAttempt = new PublicationAttempt
                        {
                            DateUtc = new DateTime(2017, 07, 30)
                        }
                    }
                };
                r.Footer = new Footer();
            });

            Action a = () => TestAssessment(record);
            a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Assessment has already been completed and is up to date");
        }

        [Test]
        public void assessment_for_non_Gemini_record_should_fail_test()
        {
            var recordId = new Guid("aeda73dc-4723-427d-8555-19558087370a");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Basic;
                r.Gemini = Library.Blank().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true
                    }
                };
                r.Footer = new Footer();
            });

            Action a = () => TestAssessment(record);
            a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Validation level must be Gemini");
        }

        [Test]
        public void successful_reassessment_for_republishing()
        {
            var recordId = new Guid("18c4b5bc-353f-4f0c-a243-1d7d06cacbec");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true,
                            CompletedOnUtc = new DateTime(2017, 07, 21),
                            CompletedByUser = new UserInfo
                            {
                                DisplayName = "Pete",
                                Email = "pete@example.com"
                            },
                            InitialAssessmentWasDoneOnSpreadsheet = false
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 07, 22)
                        },
                        LastAttempt = new PublicationAttempt
                        {
                            DateUtc = new DateTime(2017, 07, 23)
                        }
                    }
                };
                r.Footer = new Footer();
            });

            var resultRecord = TestAssessment(record);
            resultRecord.Publication.Should().NotBeNull();

            var openDataInfo = resultRecord.Publication.OpenData;
            openDataInfo.Should().NotBeNull();
            openDataInfo.LastAttempt.DateUtc.Should().Be(new DateTime(2017, 07, 23));
            openDataInfo.LastSuccess.Should().BeNull();
            openDataInfo.Resources.Should().BeNull();
            openDataInfo.Paused.Should().BeFalse();
            openDataInfo.Assessment.Completed.Should().BeTrue();
            openDataInfo.Assessment.CompletedByUser.DisplayName.Should().Be("Test User");
            openDataInfo.Assessment.CompletedByUser.Email.Should().Be("tester@example.com");
            openDataInfo.Assessment.CompletedOnUtc.Should().NotBe(new DateTime(2017, 07, 21));
            openDataInfo.Assessment.CompletedOnUtc.Should().NotBe(DateTime.MinValue);
            openDataInfo.Assessment.InitialAssessmentWasDoneOnSpreadsheet.Should().BeFalse();
        }

        [Test]
        public void fail_when_reassessing_up_to_date_record_at_sign_off_stage()
        {
            var recordId = new Guid("4fbdba6e-9b5c-40bc-842c-e99b6c976a08");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true,
                            CompletedOnUtc = new DateTime(2017, 07, 21),
                            CompletedByUser = new UserInfo
                            {
                                DisplayName = "Pete",
                                Email = "pete@example.com"
                            },
                            InitialAssessmentWasDoneOnSpreadsheet = false
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 07, 30)
                        },
                        LastAttempt = new PublicationAttempt
                        {
                            DateUtc = new DateTime(2017, 07, 20)
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

                var publishingController = GetTestOpenDataPublishingController(db);

                var request = new AssessmentRequest
                {
                    Id = record.Id
                };

                Action a = () => publishingController.Assess(request);
                a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Assessment has already been completed and is up to date");

                var resultRecord = db.Load<Record>(record.Id);
                resultRecord.Publication.Should().NotBeNull();

                var openDataInfo = resultRecord.Publication.OpenData;
                openDataInfo.Should().NotBeNull();
                openDataInfo.Assessment.Completed.Should().BeTrue();
                openDataInfo.Assessment.CompletedByUser.DisplayName.Should().Be("Pete");
                openDataInfo.Assessment.CompletedByUser.Email.Should().Be("pete@example.com");
                openDataInfo.Assessment.CompletedOnUtc.Should().Be(new DateTime(2017, 07, 21));
                resultRecord.Gemini.MetadataDate.Should().NotBe(openDataInfo.Assessment.CompletedOnUtc);
            }
        }

        [Test]
        public void fail_when_reassessing_up_to_date_record_at_upload_stage()
        {
            var recordId = new Guid("593177d9-885f-4450-903b-ecb9ea667575");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true,
                            CompletedOnUtc = new DateTime(2017, 07, 21),
                            CompletedByUser = new UserInfo
                            {
                                DisplayName = "Pete",
                                Email = "pete@example.com"
                            },
                            InitialAssessmentWasDoneOnSpreadsheet = false
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 07, 29)
                        },
                        LastAttempt = new PublicationAttempt
                        {
                            DateUtc = new DateTime(2017, 07, 30)
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

                var publishingController = GetTestOpenDataPublishingController(db);

                var request = new AssessmentRequest
                {
                    Id = record.Id
                };

                Action a = () => publishingController.Assess(request);
                a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Assessment has already been completed and is up to date");

                var resultRecord = db.Load<Record>(record.Id);
                resultRecord.Publication.Should().NotBeNull();

                var openDataInfo = resultRecord.Publication.OpenData;
                openDataInfo.Should().NotBeNull();
                openDataInfo.Assessment.Completed.Should().BeTrue();
                openDataInfo.Assessment.CompletedByUser.DisplayName.Should().Be("Pete");
                openDataInfo.Assessment.CompletedByUser.Email.Should().Be("pete@example.com");
                openDataInfo.Assessment.CompletedOnUtc.Should().Be(new DateTime(2017, 07, 21));
                resultRecord.Gemini.MetadataDate.Should().NotBe(openDataInfo.Assessment.CompletedOnUtc);
            }
        }

        [Test]
        public void successful_reassessment_from_spreadsheet_assessment()
        {
            var recordId = new Guid("39af3c76-8769-4994-bb1a-4f1a1017c0b0");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true,
                            CompletedOnUtc = new DateTime(2017, 07, 21),
                            CompletedByUser = new UserInfo
                            {
                                DisplayName = "Pete",
                                Email = "pete@example.com"
                            },
                            InitialAssessmentWasDoneOnSpreadsheet = true
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 07, 22)
                        },
                        LastAttempt = new PublicationAttempt
                        {
                            DateUtc = new DateTime(2017, 07, 23)
                        }
                    }
                };
                r.Footer = new Footer();
            });

            var resultRecord = TestAssessment(record);
            resultRecord.Publication.Should().NotBeNull();

            var openDataInfo = resultRecord.Publication.OpenData;
            openDataInfo.Should().NotBeNull();
            openDataInfo.Assessment.Completed.Should().BeTrue();
            openDataInfo.Assessment.CompletedByUser.DisplayName.Should().Be("Test User");
            openDataInfo.Assessment.CompletedByUser.Email.Should().Be("tester@example.com");
            openDataInfo.Assessment.CompletedOnUtc.Should().NotBe(new DateTime(2017, 07, 21));
            openDataInfo.Assessment.CompletedOnUtc.Should().NotBe(DateTime.MinValue);
            openDataInfo.Assessment.InitialAssessmentWasDoneOnSpreadsheet.Should().BeTrue();
            resultRecord.Gemini.MetadataDate.Should().Be(openDataInfo.Assessment.CompletedOnUtc);
        }

        static readonly Record[] NotEligibleForAssessmentRecords =
        {
            new Record().With(r =>
            {
                r.Id = new Guid("dc370d41-c8b4-4eba-8e39-6e2d70c50c07");
                r.Path = @"http://www.example.com";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example();
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true
                    }
                };
                r.Footer = new Footer();
            }),
            new Record().With(r =>
            {
                r.Id = new Guid("60df47fc-d4df-48ce-9bdd-289c145f7de0");
                r.Path = @"https://www.example.com";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example();
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true
                    }
                };
                r.Footer = new Footer();
            }),
            new Record().With(r =>
            {
                r.Id = new Guid("d82afb6c-2699-4570-a72f-cdf2cf93fa4c");
                r.Path = @"postgres://username@hostname/databasename";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Example();
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = true
                    }
                };
                r.Footer = new Footer();
            })
        };

        [Test, TestCaseSource(nameof(NotEligibleForAssessmentRecords))]
        public void not_eligible_for_assessment_when_path_is_not_file_path(Record record)
        {
            var store = new InMemoryDatabaseHelper().Create();
            using (var db = store.OpenSession())
            {
                db.Store(record);
                db.SaveChanges();

                var publishingController = GetTestOpenDataPublishingController(db);

                var request = new AssessmentRequest
                {
                    Id = record.Id
                };

                Action a = () => publishingController.Assess(request);
                a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Must have a file path for publishing");

                var resultRecord = db.Load<Record>(record.Id);
                resultRecord.Publication.OpenData.Assessment.Should().BeNull();
            }
        }

        [Test]
        public void assessment_for_not_publishable_record_should_fail_test()
        {
            var recordId = new Guid("3e3fef01-1c43-4a16-82b3-4caab363a55a");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Blank().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = false
                    }
                };
                r.Footer = new Footer();
            });

            Action a = () => TestAssessment(record);
            a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Record must be publishable as Open Data");
        }

        [Test]
        public void reassessment_for_not_publishable_record_should_fail_test()
        {
            var recordId = new Guid("13da2640-9241-4ad8-a14f-aad02109cf59");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Blank().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Publication = new PublicationInfo
                {
                    OpenData = new OpenDataPublicationInfo
                    {
                        Publishable = false,
                        Assessment = new OpenDataAssessmentInfo
                        {
                            Completed = true,
                            CompletedOnUtc = new DateTime(2017, 07, 21),
                            CompletedByUser = new UserInfo
                            {
                                DisplayName = "Pete",
                                Email = "pete@example.com"
                            },
                            InitialAssessmentWasDoneOnSpreadsheet = false
                        },
                        SignOff = new OpenDataSignOffInfo
                        {
                            DateUtc = new DateTime(2017, 07, 22)
                        },
                        LastAttempt = new PublicationAttempt
                        {
                            DateUtc = new DateTime(2017, 07, 23)
                        }
                    }
                };
                r.Footer = new Footer();
            });

            Action a = () => TestAssessment(record);
            a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Record must be publishable as Open Data");
        }

        [Test]
        public void assessment_for_record_with_null_publishable_field_should_fail_test()
        {
            var recordId = new Guid("71639cc0-94bd-430a-a0f2-37247698abe3");
            var record = new Record().With(r =>
            {
                r.Id = recordId;
                r.Path = @"X:\path\to\assessment\test";
                r.Validation = Validation.Gemini;
                r.Gemini = Library.Blank().With(m =>
                {
                    m.MetadataDate = new DateTime(2017, 07, 30);
                });
                r.Footer = new Footer();
            });

            Action a = () => TestAssessment(record);
            a.ShouldThrow<InvalidOperationException>().And.Message.Should().Be("Record must be publishable as Open Data");
        }

        private static Record TestAssessment(Record record)
        {
            var store = new InMemoryDatabaseHelper().Create();
            using (var db = store.OpenSession())
            {
                db.Store(record);
                db.SaveChanges();

                var publishingController = GetTestOpenDataPublishingController(db);

                var request = new AssessmentRequest
                {
                    Id = record.Id
                };

                return ((RecordServiceResult) publishingController.Assess(request)).Record;
            }
        }

        private static OpenDataPublishingController GetTestOpenDataPublishingController(IDocumentSession db)
        {
            var testUserContext = new TestUserContext();
            var userContextMock = new Mock<IUserContext>();
            userContextMock.Setup(u => u.User).Returns(testUserContext.User);

            var publishingService = new OpenDataPublishingRecordService(db, new RecordValidator());

            return new OpenDataPublishingController(db, publishingService, userContextMock.Object);
        }
    }
}
