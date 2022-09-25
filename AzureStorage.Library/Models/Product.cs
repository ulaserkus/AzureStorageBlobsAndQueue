using Azure;
using Azure.Data.Tables;
using System;

namespace AzureStorage.Library.Models
{
    public class Product : ITableEntity
    {
        public string Name { get; set; }

        public double Price { get; set; }

        public int Stock { get; set; }

        public string Color { get; set; }

        public string PartitionKey { get; set; }
        
        public string RowKey { get; set; }
        
        public DateTimeOffset? Timestamp { get; set; }
        
        public ETag ETag { get; set; }
    }
}
