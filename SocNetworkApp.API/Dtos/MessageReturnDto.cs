using System;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Dtos
{
    public class MessageReturnDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderKnownAs { get; set; }
        public string SenderPhotoUrl { get; set; }
        public Guid RecipientId { get; set; }
        public string RecipientKnownAs { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }

    }
}