﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Catalogue.Data.Model;
using Catalogue.Utilities.Time;
using Raven.Client;

namespace Catalogue.Robot.Publishing.OpenData
{
    public class OpenDataRecordPublisher
    {
        readonly IDocumentSession db;
        readonly OpenDataPublisherConfig config;
        readonly IFtpClient ftpClient;

        public OpenDataRecordPublisher(IDocumentSession db, OpenDataPublisherConfig config, IFtpClient ftpClient)
        {
            this.db = db;
            this.config = config;
            this.ftpClient = ftpClient;
        }

        public void PublishRecord(Guid id)
        {
            var record = db.Load<Record>(id);
            Console.WriteLine("Publishing '{0}' to '{1}'.", record.Gemini.Title, config.FtpRootUrl);

            string metaPath = String.Format("waf/{0}.xml", record.Id);
            string dataPath = String.Format("data/{0}-{1}", record.Id, Path.GetFileName(record.Path).Replace(" ", "-")); // todo webify file name

            // save a not-yet-successful attempt to begin with
            var attempt = new PublicationAttempt { DateUtc = Clock.NowUtc };
            record.Publication.OpenData.LastAttempt = attempt;
            db.SaveChanges();

//            // check that the data exists
//            if (File.Exists(dataPath))
//            {
                // do the sequential actions
                UploadTheDataFile(record, dataPath);
                UpdateResourceLocatorInTheRecord(record, dataPath);
                UploadTheMetadataDocument(record, metaPath);
                UpdateTheWafIndexDocument(record);

                // mark the attempt successful
                record.Publication.OpenData.LastSuccess = attempt;

                // commit the changes (to the resource locator and the attempt object)
                db.SaveChanges();
//            }
//            else
//            {
//                Console.WriteLine("Data file does not exist for record {0} so skipping it.", id);
//                attempt.Message = "Data file does not exist.";
//                db.SaveChanges();
//            }
        }

//        PublicationAttempt AddNewAttempt(Record record)
//        {
//            var publicationInfo = record.Publication.OpenData;
//            if (publicationInfo.Attempts == null)
//                publicationInfo.Attempts = new List<PublicationAttempt>();
//
//            var attempt = new PublicationAttempt { DateUtc = DateTime.UtcNow };
//            publicationInfo.Attempts.Add(attempt);
//
//            return attempt;
//        }

        void UploadTheDataFile(Record record, string dataPath)
        {
            string dataFtpPath = config.FtpRootUrl + "/" + dataPath;
            Console.WriteLine("Uploading data file to {0}", dataFtpPath);

//            if (!File.Exists(record.Path))
//                throw new Exception(String.Format("Data file for record {0} doesn't exist.", record.Id));

            string dataFilePath = record.Path;
            // correct data path for unmapped drive X
            dataFilePath = dataFilePath.Replace(@"X:\OffshoreSurvey\", @"\\JNCC-CORPFILE\Marine Survey\OffshoreSurvey\");

            ftpClient.UploadFile(dataFtpPath, dataFilePath);
            Console.WriteLine("Uploaded data file successfully.");
        }

        void UpdateResourceLocatorInTheRecord(Record record, string dataPath)
        {
            string dataHttpPath = config.HttpRootUrl + "/" + dataPath;
            record.Gemini.ResourceLocator = dataHttpPath;
        }

        void UploadTheMetadataDocument(Record record, string metaPath)
        {
            var doc = new global::Catalogue.Gemini.Encoding.XmlEncoder().Create(record.Id, record.Gemini);

            var s = new MemoryStream();
            doc.Save(s);
            var metaXmlDoc = s.ToArray();

            string metaFtpPath = config.FtpRootUrl + "/" + metaPath;

            ftpClient.UploadBytes(metaFtpPath, metaXmlDoc);
        }

        void UpdateTheWafIndexDocument(Record record)
        {
            string indexDocFtpPath = String.Format("{0}/waf/index.html", config.FtpRootUrl);
            string indexDocHtml = ftpClient.DownloadString(indexDocFtpPath);

            var doc = XDocument.Parse(indexDocHtml);
            var body = doc.Root.Element("body");

            var newLink = new XElement("a", new XAttribute("href", record.Id + ".xml"), record.Gemini.Title);
            var existingLinks = body.Elements("a").ToList();
            
            existingLinks.Remove();

            var newLinks = existingLinks
                .Concat(new [] { newLink })
                .GroupBy(a => a.Attribute("href").Value)
                .Select(g => g.First()); // poor man's DistinctBy

            body.Add(newLinks);

            ftpClient.UploadString(indexDocFtpPath, doc.ToString());
        }
    }
}
