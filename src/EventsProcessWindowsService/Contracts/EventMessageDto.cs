namespace EventsProcessWindowsService.Contracts
{
    public class EventMessageDto
    {
        public MessageType Type { get; set; }

        public object Data { get; set; }
    }
}