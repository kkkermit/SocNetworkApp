using System;

namespace SocNetworkApp.API.Dtos
{
    public class PhotoDetailedDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        public bool IsApproved { get; set; }
    }
}