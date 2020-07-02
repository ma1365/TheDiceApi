using System.Collections.Generic;
using TheDiceApi.Models;

namespace TheDiceApi.Managers
{
    public interface IDiceRollManager
    {
        List<Die> RollDie(DieType dieType, uint requestRollCount);

        DiceRolls RollDice(Dictionary<string, uint> rollRequest);
    }
}
