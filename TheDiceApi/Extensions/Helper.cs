using System;
using TheDiceApi.Models;

namespace TheDiceApi.Extensions
{
    public static class Helper
    {
        public static DieType ToDieType(this int facetCount)
        {
            switch (facetCount)
            {
                case 4:
                    return DieType.D4;
                case 6:
                    return DieType.D6;
                case 8:
                    return DieType.D8;
                case 10:
                    return DieType.D10;
                case 12:
                    return DieType.D12;
                case 20:
                    return DieType.D20;
                case 100:
                    return DieType.D100;
                default:
                    throw new ArgumentException("Unsupported DieType");
            }
        }

        public static int ToFacetCount(this DieType facetCount)
        {
            switch (facetCount)
            {
                case DieType.D4:
                    return 4;
                case DieType.D6:
                    return 6;
                case DieType.D8:
                    return 8;
                case DieType.D10:
                    return 10;
                case DieType.D12:
                    return 12;
                case DieType.D20:
                    return 20;
                case DieType.D100:
                    return 100;
                default:
                    throw new ArgumentException("Unsupported DieType");
            }
        }
    }
}
