using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TS_WebApp.Models.EntityModels;

namespace TS_WebApp.Services
{
    public class TsBlobService
    {
        private string connString { get; set; }
        private string blobTelemetryContainerName = "ts-telemetry-data";

        public TsBlobService()
        {
            connString = ConfigurationManager.AppSettings["Azure.BlobStorage.ConnectionString"];
        }

        public async Task<List<BulbTelemetryDTO>> GetBulbActivitiesFromBlobStorage()
        {
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClient blobServiceClient = new BlobServiceClient(connString);


            var blobContainerClient = blobServiceClient.GetBlobContainerClient(blobTelemetryContainerName);

            var last5Blobs = blobContainerClient.GetBlobs().OrderByDescending(x => x.Properties.CreatedOn).Take(5);
            List<BulbTelemetryDTO> bulbDeviceActivityRecords = new List<BulbTelemetryDTO>();

            foreach (var item in last5Blobs)
            {
                try
                {
                    BlobClient blobClient = blobContainerClient.GetBlobClient(item.Name);
                    if (await blobClient.ExistsAsync())
                    {
                        var response = await blobClient.DownloadAsync();
                        using (var streamReader = new StreamReader(response.Value.Content))
                        {
                            var bulbObjectToAdd = JsonConvert.DeserializeObject<BulbTelemetryDTO>(await streamReader.ReadToEndAsync());
                            bulbObjectToAdd.CreatedOn = item.Properties.CreatedOn.Value.AddHours(1).DateTime;
                            bulbDeviceActivityRecords.Add(bulbObjectToAdd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return bulbDeviceActivityRecords;
        }
    }
}