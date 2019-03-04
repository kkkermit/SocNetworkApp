using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocNetworkApp.API.Helpers;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Data
{
    public interface IDataRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(Guid id);
        Task<Photo> GetPhoto(Guid id);
        Task<Photo> GetMainPhotoForUser(Guid userId);
        Task<Like> GetLike(Guid userId, Guid recipientId);
        Task<Message> GetMessage(Guid id);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(Guid userId, Guid recipientId);
    }
}