﻿using System;
using System.Collections.Generic;

namespace Catalogue.Gemini.Model
{
    /// <summary>
    /// A simple implementation of the UK Gemini 2.2 metadata standard.
    /// </summary>
    public class Metadata
    {
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string TopicCategory { get; set; }
        public List<string> Keywords { get; set; }
        public TemporalExtent TemporalExtent { get; set; }
        public string DatasetReferenceDate { get; set; }
        public string Lineage { get; set; }
        public DataFormat DataFormat { get; set; }
        public ResponsibleParty ResponsibleOrganisation { get; set; }
        public string LimitationsOnPublicAccess { get; set; }
        public string UseConstraints { get; set; }
        public string MetadataDate { get; set; }
        public string MetadataLanguage { get; set; }
        public ResponsibleParty MetadataPointOfContact { get; set; }
        public string UniqueResourceIdentifier { get; set; }
        public string ResourceType { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }

    public class DataFormat
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }

    public class TemporalExtent
    {
        public string Begin { get; set; }
        public string End { get; set; }
    }

    public class ResponsibleParty
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class BoundingBox
    {
        public decimal North { get; set; }
        public decimal South { get; set; }
        public decimal East { get; set; }
        public decimal West { get; set; }
    }
}