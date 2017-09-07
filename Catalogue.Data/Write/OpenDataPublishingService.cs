﻿using Catalogue.Data.Model;
using Catalogue.Gemini.Model;
using Catalogue.Utilities.Text;
using Catalogue.Utilities.Time;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using static Catalogue.Utilities.Text.WebificationUtility;

namespace Catalogue.Data.Write
{
    public class OpenDataPublishingService : IOpenDataPublishingService
    {
        private readonly IRecordService recordService;

        public OpenDataPublishingService(IRecordService recordService)
        {
            this.recordService = recordService;
        }

        public Record SignOff(Record record, OpenDataSignOffInfo signOffInfo)
        {
            if (record.Publication?.OpenData == null)
                throw new Exception("Couldn't sign-off record for publication. Assessment not completed.");

            var openDataInfo = record.Publication.OpenData;

            if (openDataInfo?.SignOff != null && openDataInfo.SignOff.DateUtc != DateTime.MinValue)
                throw new Exception("The record has already been signed off and cannot be signed off again.");

            if (openDataInfo?.Assessment != null && !openDataInfo.Assessment.Completed)
                throw new Exception("Couldn't sign-off record for publication. Assessment not completed.");

            openDataInfo.SignOff = signOffInfo;

            var recordServiceResult = recordService.Update(record, signOffInfo.User);
            if (!recordServiceResult.Success)
            {
                throw new Exception("Error while saving sign off changes.");
            }

            return recordServiceResult.Record;
        }

        public Record Assess(Record record, OpenDataAssessmentInfo assessmentInfo)
        {
            if (!record.Validation.Equals(Validation.Gemini))
            {
                throw new Exception("Validation level must be Gemini.");
            }

            if (record.Publication == null)
            {
                record.Publication = new PublicationInfo();
            }

            if (record.Publication.OpenData == null)
            {
                record.Publication.OpenData = new OpenDataPublicationInfo
                {
                    Assessment = new OpenDataAssessmentInfo()
                };
            }

            var assessment = record.Publication.OpenData.Assessment;
            if (assessment != null && assessment.Completed)
            {
                throw new Exception("Assessment has already been completed.");
            }

            record.Publication.OpenData.Assessment = assessmentInfo;

            var recordServiceResult = recordService.Update(record, assessmentInfo.CompletedByUser);
            if (!recordServiceResult.Success)
            {
                throw new Exception("Error while saving assessment changes.");
            }

            return recordServiceResult.Record;
        }

        public IOpenDataPublishingUploadService Upload()
        {
            return new OpenDataPublishingUploadService(recordService);
        }
    }
}