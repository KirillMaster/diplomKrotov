using System.ComponentModel;

namespace Common.Enums
{
    public enum CharacteristicType
    {
        [Description("")]
        Unknown = 0,

        [Description("Площадь повреждений")]
        AreaOfDamage = 1,

        [Description("Цифровая модель рельефа")]
        DigitalReliefModel = 2,

        [Description("Температура подстилающей поверхности")]
        Temperature = 3,

        [Description("Индекс влажности NDWI")]
        NDWI = 4,

        [Description("Каналы 4-3-2")]
        Channels432 = 5,

        [Description("Каналы 5-4-3")]
        Channels543 = 6,

        [Description("Каналы 7-5-3")]
        Channels753 = 7,

    }
}
