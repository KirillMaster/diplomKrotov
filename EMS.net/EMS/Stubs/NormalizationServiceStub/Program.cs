using System;
using BusContracts;
using Common.Constants;
using Common.Enums;

namespace NormalizationServiceStub
{
    class Program
    {
        static void Main(string[] args)
        {
            var busManager = new BusManager.BusManager();

            var message = new 
            {
                RequestId = Guid.NewGuid().ToString("N"),
                Folder = @"C:\diplom\EMS.nodejs\external\sortDownloadData\Landsat_8_C1\2016-05-06\185026",
                SatelliteType = SatelliteType.Landsat8
            };

            busManager.Send<IDataNormalizationRequest>(BusQueueConstants.DataNormalizationRequestQueueName, message).Wait();
        }
    }
}
