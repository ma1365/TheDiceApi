using System.Collections.Generic;

namespace TheDiceApi.Models
{
    public class DiceRolls
    {
        public Dictionary<string, List<Die>> DiceRollResults { get; set; }
    }
}
