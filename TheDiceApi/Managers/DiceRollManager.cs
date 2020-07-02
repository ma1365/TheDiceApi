using System;
using System.Collections.Generic;
using TheDiceApi.Extensions;
using TheDiceApi.Models;

namespace TheDiceApi.Managers
{
    public class DiceRollManager : IDiceRollManager
    {
        private static readonly Random Random = new Random();
        private static readonly object SyncLock = new object();

        public List<Die> RollDie(DieType dieType, uint requestRollCount)
        {
            return GetRolls(dieType.ToFacetCount(), requestRollCount);
        }

        public DiceRolls RollDice(Dictionary<string, uint> rollRequest)
        {
            var diceRows = new Dictionary<string, List<Die>>();

            foreach (var rollRequestCount in rollRequest)
            {
                var rolls = RollDie(Enum.Parse<DieType>(rollRequestCount.Key), rollRequestCount.Value);

                diceRows.Add(rollRequestCount.Key.ToString(), rolls);
            }

            return new DiceRolls
            {
                DiceRollResults = diceRows
            };
        }

        private static List<Die> GetRolls(int dieFacetCount, uint rollCount)
        {
            var rolls = new List<Die>();

            for (var i = 0; i < rollCount; i++)
            {
                rolls.Add(GetRoll(dieFacetCount));
            }

            return rolls;
        }

        private static Die GetRoll(int dieFacetCount)
        {
            var roll = 0;
            lock (SyncLock)
            {
                // The max value in .Next() is noninclusive
                roll = Random.Next(1, dieFacetCount +1 );
            }

            return new Die{ DieType = dieFacetCount.ToDieType(), RollValue = (ushort)roll };
        }

    }
}
