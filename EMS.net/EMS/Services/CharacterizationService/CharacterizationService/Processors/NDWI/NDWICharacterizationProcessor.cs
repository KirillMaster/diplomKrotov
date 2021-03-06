﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using CharacterizationService.Abstraction;
using Common.Helpers;
using Common.Objects;
using Common.Objects.Landsat;
using Common.PointsReaders;
using DeterminingPhenomenonService.Helpers;
using DeterminingPhenomenonService.Objects;
using DrawImageLibrary;
using Topshelf.Logging;

namespace CharacterizationService.Processors.NDWI
{
    public class NDWICharacterizationProcessor : AbstractCharacterizationProcessor
    {
        public NDWICharacterizationProcessor(LogWriter logger) : base(logger)
        {
        }

        public override string[] Process(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string dataFolder,
            string resultFolder)
        {
            var landsatDecription = new LandsatDataDescription(dataFolder);
            var cuttedImageInfo =
                ClipImageHelper.GetCuttedImageInfoByPolygon(landsatDecription.Channel5.Raw,
                    new GeographicPolygon
                    {
                        UpperLeft = leftUpper,
                        LowerRight = rigthLower
                    });

           
            try
            {
                string[] filePaths = Directory.GetFiles(@"C:\diplom\EMS.net\EMS\Services\CharacterizationService\CharacterizationService\Content");
                File.Copy(filePaths.Where(x => x.Contains("NDWI_legend.png")).First(), resultFolder + "\\NDWI_legend.png");
            }
            catch (Exception e)
            {
                Console.WriteLine("Файл уже существует");
            }
      
            CalculateNDWI(landsatDecription.Channel5.Normalized, landsatDecription.Channel6.Normalized,
                cuttedImageInfo, resultFolder);

            return new[] { resultFolder  + "\\ndwi.png" };
        }

        public void CalculateNDWI(string channel5, string channel6, CuttedImageInfo cuttedImageInfo, string resultFolder)
        {
            var nirChannel = new LandsatNormilizedSnapshotReader(channel5);
            var swirChannel = new LandsatNormilizedSnapshotReader(channel6);
            var ndwiRanges = new List<Legend.Range>
            {
                new Legend.Range(-1.0, -0.8, Color.FromArgb(153,92, 0)),
                new Legend.Range(-0.8, -0.6, Color.FromArgb(171, 122, 44)),
                new Legend.Range(-0.6, -0.4, Color.FromArgb(184, 146, 81)),
                new Legend.Range(-0.4, -0.2, Color.FromArgb(194, 158, 101)),
                new Legend.Range(-0.2, 0, Color.FromArgb(222, 206, 175)),
                new Legend.Range(0, 0.2, Color.FromArgb(141, 221,252)),
                new Legend.Range(0.2, 0.4, Color.FromArgb(77, 161, 250)),
                new Legend.Range(0.4, 0.6, Color.FromArgb(41, 111, 217)),
                new Legend.Range(0.6, 0.8, Color.FromArgb(13, 66, 158)),
                new Legend.Range(0.8, 1.0, Color.FromArgb(4, 19, 89)),
            };
            var legend = new Legend(ndwiRanges.ToArray());
            var nirBuffer = ClipImageHelper.ReadBufferByIndexes(cuttedImageInfo, nirChannel);
            var swirBuffer = ClipImageHelper.ReadBufferByIndexes(cuttedImageInfo, swirChannel);
           // using (var bmp = DrawLib.CreateImageWithLegend(cuttedImageInfo.Width, cuttedImageInfo.Height, @"..\..\Content\NDWI.png"))
            using (var bmp = new Bitmap(cuttedImageInfo.Width, cuttedImageInfo.Height))
            {
                for (var row = 0; row < cuttedImageInfo.Height; row++)
                {
                    for (int col = 0; col < cuttedImageInfo.Width; col++)
                    {
                        var operator1 = nirBuffer[row, col] - swirBuffer[row, col];
                        var operator2 = nirBuffer[row, col] + swirBuffer[row, col];

                        var ndwi = operator1 / operator2;

                        bmp.SetPixel(col, row, legend.GetColor(ndwi));
                    }
                }

                bmp.Save(resultFolder + "\\ndwi.png");
            }
        }


       
    }
}