using System;

namespace SocNetworkApp.API.Models
{
    public class Photo
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public bool IsApproved { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}