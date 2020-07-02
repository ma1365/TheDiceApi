using TheDiceApi.Models;

namespace TheDiceApi.ViewModels
{
    public class MultipleDiceRollComponent : DiceRollInfoComponent
    {
        public DiceRolls RollValues { get; set; }
        public CommandComponent SelectedCommand { get; set; }
    }
}
