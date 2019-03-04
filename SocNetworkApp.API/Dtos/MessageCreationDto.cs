using System;

namespace SocNetworkApp.API.Dtos
{
    public class MessageCreationDto
    {
        public Guid SenderId { get; set; }
        public Guid RecipientId { get; set; }
        public DateTime MessageSent { get; set; }
        public string Content { get; set; }

        public MessageCreationDto()
        {
            MessageSent = DateTime.Now;
        }
    }
}