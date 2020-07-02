using Newtonsoft.Json.Linq;

namespace TheDiceApi.ViewModels
{
    public class CommandPayload
    {
        public JObject ExecutionData { get; set; }
        public JObject ReturnedData { get; set; }
    }
}
