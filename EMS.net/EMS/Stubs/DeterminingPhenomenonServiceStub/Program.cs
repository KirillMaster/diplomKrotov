using BusContracts;
using Common.Constants;
using Common.Objects;
using Common.Enums;


namespace DeterminingPhenomenonServiceStub
{
    class Program
    {
        static void Main(string[] args)
        {
            var busManager = new BusManager.BusManager();

            var message = new
            {
                ResultFolder = @"C:\diplom\EMS.nodejs\external\resultUser\test2",
                LeftUpper = new GeographicPoint
                {
                    Latitude  = 48.9699,
                    Longitude = 23.7434,
                },
                RightLower = new GeographicPoint
                {
                    Latitude = 48.6425,
                    Longitude = 24.1691,
                },
                Phenomenon = PhenomenonType.ForestPlantationsDeseases,
                DataFolders = new[]
                {
                    @"C:\diplom\EMS.nodejs\external\sortDownloadData\Landsat_8_C1\2017-06-10\185026",
                    @"C:\diplom\EMS.nodejs\external\sortDownloadData\Landsat_8_C1\2018-05-12\185026",
                },
                RequestId = "531aab7b-02d2-49b4-81b6-077f0d3a24aa",
            };

            busManager.Send<IDeterminingPhenomenonRequest>(BusQueueConstants.DeterminingPhenomenonRequestsQueueName, message).Wait();
        }
    }
}
