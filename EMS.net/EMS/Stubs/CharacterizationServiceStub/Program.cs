using System;
using System.Collections.Generic;
using BusContracts;
using Common.Constants;
using Common.Enums;

namespace CharacterizationServiceStub
{
    class Program
    {
        static void Main(string[] args)
        {
            var busManager = new BusManager.BusManager();

            var message = new
            {
                RequestId = Guid.NewGuid().ToString("N"),
                PhenomenonType = PhenomenonType.ForestPlantationsDeseases,
                Characteristics = new List<object>
                {
                    new
                    {
                        SatelliteType = SatelliteType.Landsat8,
                        DataFolder = @"C:\diplom\EMS.nodejs\external\sortDownloadData\Landsat_8_C1\2018-05-12\185026",
                        ResultFolder = @"C:\diplom\",
                        CharacteristicType = CharacteristicType.Temperature
                    }
                },
                LeftUpper = new
                {
                    Latitude = 48.9699,
                    Longitude = 23.7434
                },
                RightLower = new
                {
                    Latitude = 48.6425,
                    Longitude = 24.1691
                }
            };

            busManager.Send<ISpectralСharacterizationRequest>(BusQueueConstants.CharacterizationRequestQueueName, message).Wait();
        }
    }
}
