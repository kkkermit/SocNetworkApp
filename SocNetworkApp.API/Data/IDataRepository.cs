using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Data
{
    public interface IDataRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(Guid id);
        Task<Photo> GetPhoto(Guid id);
    }
}