using System.Collections.Generic;

namespace TheDiceApi.ViewModels
{
    public class DiceRollInfoComponent
    {
        public DiceRollInfoComponent()
        {
            AvailableCommands = new List<CommandComponent>();
        }

        public List<CommandComponent> AvailableCommands { get; set; }
    }
}
