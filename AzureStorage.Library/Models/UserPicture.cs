using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace AzureStorage.Library.Models
{
    public class UserPicture : ITableEntity
    {
        public string RawPaths { get; set; }
        public string WatermarkRawPaths { get; set; }

        [IgnoreDataMember]
        public List<string> Paths
        {
            get => RawPaths == null ? null : JsonConvert.DeserializeObject<List<string>>(RawPaths);

            set => RawPaths = JsonConvert.SerializeObject(value);
        }

        [IgnoreDataMember]
        public List<string> WaterMarkPaths
        {
            get => WatermarkRawPaths == null ? null : JsonConvert.DeserializeObject<List<string>>(RawPaths);

            set => WatermarkRawPaths = JsonConvert.SerializeObject(value);
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
