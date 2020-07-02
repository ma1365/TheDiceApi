namespace TheDiceApi.ViewModels
{
    public class CommandComponent
    {
        public CommandComponent()
        {
            Payload = new CommandPayload();
        }

        public bool IsVisible { get; set; }
        public bool  IsEnabled { get; set; }
        public string CommandName { get; set; }
        public string FriendlyName { get; set; }
        public CommandPayload Payload { get; set; }
    }
}
