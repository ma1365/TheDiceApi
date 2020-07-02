using System.Collections.Generic;

namespace TheDiceApi.ViewModels
{
    public class SingleDiceRollComponent : DiceRollInfoComponent
    {
        public SingleDiceRollComponent()
        {
            RollValues = new List<ushort>();
        }

        public List<ushort> RollValues { get; set; }

        public string DieType { get; set; }

        public CommandComponent SelectedCommand { get; set; }
    }
}
