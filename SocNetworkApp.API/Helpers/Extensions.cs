using System;
using Microsoft.AspNetCore.Http;

namespace SocNetworkApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse respose, string message) 
        {
            respose.Headers.Add("Application-Error", message);
            respose.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            respose.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static int CalculateAge(this DateTime dateTime)
        {
            int age = DateTime.Today.Year - dateTime.Year;

            if (dateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        }
    }
}