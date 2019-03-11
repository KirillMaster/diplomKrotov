using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CharacterizationService.Abstraction;
using Common.Helpers;
using Common.Objects;
using Common.Objects.Landsat;
using DeterminingPhenomenonService.Helpers;
using DeterminingPhenomenonService.Objects;
using DrawImageLibrary;
using OSGeo.GDAL;
using Topshelf.Logging;

namespace CharacterizationService.Processors.Temperature
{
    public class TemperatureCharacterizationProcessor : AbstractCharacterizationProcessor
    {
        public TemperatureCharacterizationProcessor(LogWriter logger) : base(logger)
        {
        }

        public override string[] Process(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string dataFolder,
            string resultFolder)
        {
            var folderDescription = new LandsatDataDescription(dataFolder);

            var path = folderDescription; 
            LandsatMetadata metadataFile = JsonHelper.Deserialize<LandsatMetadata>(folderDescription.MetadataMtlJson);
            TirsThermalConstants thermalConstants = metadataFile.L1MetadataFile.TirsThermalConstants;
            var cuttedImageInfo =
               ClipImageHelper.GetCuttedImageInfoByPolygon(folderDescription.Channel10.Raw,
                   new GeographicPolygon
                   {
                       UpperLeft = leftUpper,
                       LowerRight = rigthLower
                   });

            using (var ds = Gdal.Open(folderDescription.Channel10.Raw, Access.GA_ReadOnly))
            {
                CalculateTemperature(ds.GetRasterBand(1),
                    metadataFile.L1MetadataFile.RadiometricRescaling.RadianceMultBand11,
                    metadataFile.L1MetadataFile.RadiometricRescaling.RadianceAddBand11,
                    thermalConstants.K1ConstantBand11,
                    thermalConstants.K2ConstantBand11,
                    resultFolder,
                    cuttedImageInfo
                );
            }

            return new[] {resultFolder + "\\temperature.png"};
        }

        public static void CalculateTemperature(Band band, 
            double ml,
            double al,
            double K1,
            double K2,
            string resultFolder,
            CuttedImageInfo cuttedImageInfo)
        {
            var resultFilename = resultFolder + "\\temperature.png";
            var temperatureRanges = new List<Legend.Range>
            {
                //new Legend.Range(-30, -25, Color.FromArgb(60, 0, 255)),
                //new Legend.Range(-25, -20, Color.FromArgb(0, 0, 255)),
                //new Legend.Range(-20, -15, Color.FromArgb(0, 80, 255)),
                //new Legend.Range(-15, -10, Color.FromArgb(0, 150, 255)),
                //new Legend.Range(-10, -5, Color.FromArgb(150, 255, 255)),
                //new Legend.Range(-5, 0, Color.FromArgb(255, 255, 255)),
                //new Legend.Range(0, 5, Color.FromArgb(255, 255, 150)),
                //new Legend.Range(5, 10, Color.FromArgb(255, 250, 50)),
                //new Legend.Range(10, 15, Color.FromArgb(255, 180, 50)),
                //new Legend.Range(15, 20, Color.FromArgb(255, 100, 30)),
                //new Legend.Range(20, 25, Color.FromArgb(255, 80, 0)),
                //new Legend.Range(25, 30, Color.FromArgb(255, 55, 0)),
                //new Legend.Range(30, 35, Color.FromArgb(255, 45, 0)),
                //new Legend.Range(35, 40, Color.FromArgb(255, 35, 0)),
                //new Legend.Range(40, 45, Color.FromArgb(255, 0, 0))
                //new Legend.Range(20, 25, Color.FromArgb(255, 80, 0)),
                //new Legend.Range(25, 30, Color.FromArgb(255, 55, 0)),
                //new Legend.Range(30, 35, Color.FromArgb(255, 45, 0)),
                //new Legend.Range(35, 40, Color.FromArgb(255, 35, 0)),
                //new Legend.Range(40, 45, Color.FromArgb(255, 0, 0))
            };
            using (band)
            {
                var width = cuttedImageInfo.Width;
                var heigth = cuttedImageInfo.Height;
                var legend = new Legend(5, 45, 5, Color.Yellow, Color.Red);
                double max = -100000;
                double min = 100000;



                using (var bmp = DrawLib.CreateImageWithLegend(cuttedImageInfo.Width, cuttedImageInfo.Height, @"..\..\Content\temperature.png"))
                {
                    for (var row = 0; row < heigth; row++)
                    {
                        var buffer = new int[width];

                        band.ReadRaster(cuttedImageInfo.Col, 
                            cuttedImageInfo.Row + row,
                            cuttedImageInfo.Width,
                            1, buffer, width, 1, 0, 0);

                        for (var col = 0; col < width; col++)
                        {
                            if (buffer[col] != 0)
                            {
                                var rad = buffer[col] * ml + al;
                                
                                var temp = (K2 / Math.Log((K1 / rad) + 1)) - 273.15;

                                max = temp > max ? temp : max;
                                min = temp < min ? temp : min;

                                bmp.SetPixel(col, row, legend.GetColor(temp));
                            }
                            else
                            {
                                bmp.SetPixel(col, row, Color.Black);
                            }
                        }
                    }

                    bmp.Save(resultFilename);
                }
            }
        }
    }
}