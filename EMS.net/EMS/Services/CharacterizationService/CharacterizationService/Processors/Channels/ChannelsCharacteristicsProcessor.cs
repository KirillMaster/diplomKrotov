using CharacterizationService.Abstraction;
using Common.Enums;
using Common.Objects;
using Common.Objects.Landsat;
using DeterminingPhenomenonService.Helpers;
using DeterminingPhenomenonService.Objects;
using DrawImageLibrary;
using Topshelf.Logging;

namespace CharacterizationService.Processors
{
    public class ChannelsCharacteristicsProcessor : AbstractCharacterizationProcessor
    {
        private CharacteristicType _characteristicType;

        public ChannelsCharacteristicsProcessor(LogWriter logger, CharacteristicType characteristicType) : base(logger)
        {
            _characteristicType = characteristicType;
        }

        public override string[] Process(IGeographicPoint leftUpper, IGeographicPoint rigthLower, string dataFolder, string resultFolder)
        {
            LandsatDataDescription landsatDescription = new LandsatDataDescription(dataFolder);

            CuttedImageInfo cuttedImageInfo =
               ClipImageHelper.GetCuttedImageInfoByPolygon(landsatDescription.Channel4.Raw,
                   new GeographicPolygon
                   {
                       UpperLeft = leftUpper,
                       LowerRight = rigthLower
                   });

            string resultFileName;
            string redChannelFileName;
            string blueChannelFileName;
            string greenChannelFileName;

            switch (_characteristicType)
            {
                case CharacteristicType.Channels543:
                    {
                        redChannelFileName = landsatDescription.Channel5.Normalized;
                        greenChannelFileName = landsatDescription.Channel4.Normalized;
                        blueChannelFileName = landsatDescription.Channel3.Normalized;
                        resultFileName = resultFolder + "5-4-3.jpg";
                        break;
                    }
                case CharacteristicType.Channels753:
                    {
                        redChannelFileName = landsatDescription.Channel7.Normalized;
                        greenChannelFileName = landsatDescription.Channel5.Normalized;
                        blueChannelFileName = landsatDescription.Channel3.Normalized;
                        resultFileName = resultFolder + "7-5-3.jpg";
                        break;
                    }
                case CharacteristicType.Channels432:
                case CharacteristicType.Unknown:
                default: 
                    {
                        redChannelFileName = landsatDescription.Channel4.Normalized;
                        greenChannelFileName = landsatDescription.Channel3.Normalized;
                        blueChannelFileName = landsatDescription.Channel2.Normalized;
                        resultFileName = resultFolder + "4-3-2.jpg";
                        break; 
                    } 
            }

            DrawLib.DrawNaturalColor(redChannelFileName
                      , greenChannelFileName
                      , blueChannelFileName,
                    cuttedImageInfo, resultFileName);

            return new string[]{ resultFileName };
        }
    }
}
