using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Gdo.Models
{

    public static class Aws
    {


    }
    public class FlowQuery
    {
        public Source source { get; set; }
        public Destination destination { get; set; }
    }
    public class Source
    {
        public string connectionString { get; set; }
        public string container { get; set; }
        public string filePath { get; set; }
        public Companion companion { get; set; }
    }
    public class Destination
    {
        public string xapikey { get; set; }
        public string apiurl { get; set; }
        public string stage { get; set; }
        public Contract contract { get; set; }
    }
    public class Contract
    {
        public Flow flow { get; set; }
        public DataFormat data_format { get; set; }
        public Payload payload { get; set; }
    }
    public class Flow
    {
        public string id { get; set; }
    }
    public class DataFormat
    {
        public string type { get; set; }
        public bool is_quoted { get; set; }
        public string delimiter { get; set; }
        public string compression { get; set; }
        public bool multiline { get; set; }
    }
    public class Payload
    {
        public string mode { get; set; }
        public int rows_count { get; set; }
        public DateTime window_end { get; set; }
    }
    public class AwsCred
    {
        public string awsAccessKeyId { get; set; }
        public string awsSecretAccessKey { get; set; }
        public string region { get; set; }
    }
    public class Companion
    {
        public DataSchema data_schema { get; set; }
        public Metadata metadata { get; set; }
    }
    public class DataSchema
    {
        public string name { get; set; }
        public List<DataSchemaField> fields { get; set; }

    }
    public class DataSchemaField
    {
        public string name { get; set; }
        public string type { get; set; }
        public string consistency { get; set; }
    }
    public class Metadata
    {
        public string wbl { get; set; }
        public string system { get; set; }
        public string zone { get; set; }
        public string domain { get; set; }
        public string confidentiality { get; set; }
        public string region { get; set; }
        public string dataset { get; set; }
        public string data_owner { get; set; }
    }
    public class SignedUrlContent
    {
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("headers")]
        public SignedUrlHeaders signedUrlHeaders { get; set; }
    }
    public class SignedUrlHeaders
    {
        [JsonProperty("host")]
        public string host { get; set; }
        [JsonProperty("content-type")]
        public string contentType { get; set; }
        [JsonProperty("x-amz-meta-companion")]
        public string xAmzMetaCompanion { get; set; }
        [JsonProperty("x-amz-server-side-encryption")]
        public string xAmzServerSideEncryption { get; set; }
        [JsonProperty("x-amz-server-side-encryption-aws-kms-key-id")]
        public string xAmzServerSideEncryptionAwsKmsKeyId { get; set; }
        [JsonProperty("x-amz-tagging")]
        public string xAmzTagging { get; set; }

    }
    public class SignedUrlErrorContent
    {
        public string error { get; set; }

        public string message { get; set; }
        public string trace { get; set; }
    }
}
