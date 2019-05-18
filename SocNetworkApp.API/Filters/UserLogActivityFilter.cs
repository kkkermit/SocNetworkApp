using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using SocNetworkApp.API.Data;
using Microsoft.Extensions.DependencyInjection;
using SocNetworkApp.API.Models;

namespace SocNetworkApp.API.Filters
{
    public class UserLogActivityFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ActionExecutedContext resultContext = await next();

            Guid userId = Guid.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            IDataRepository reposityory = resultContext.HttpContext.RequestServices.GetService<IDataRepository>();

            User user = await reposityory.GetUser(userId);

            user.LastActive = DateTime.UtcNow.AddHours(2);

            await reposityory.SaveAll();
        }
    }
}