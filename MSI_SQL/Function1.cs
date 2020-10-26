using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

namespace MSI_SQL
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            var tokenProvider = new Microsoft.Azure.Services.AppAuthentication.AzureServiceTokenProvider("RunAs=App;AppId=<your user defined MSI app id>");
            var accessToken = tokenProvider.GetAccessTokenAsync("https://database.windows.net/").Result;
            log.LogInformation(accessToken);
            int count = 0;
            if (accessToken != null)
            {
                string connectionString = "Data Source=<sql instance name>.database.windows.net; Initial Catalog=<database name>;";
                SqlConnection conn = new SqlConnection(connectionString);
                conn.AccessToken = accessToken;
                conn.Open();
                SqlCommand comm = new SqlCommand("SELECT COUNT(*) FROM <a table in your db>", conn);
                count = (Int32)comm.ExecuteScalar();

            }

            return new OkObjectResult($"Everything is ok!, there were {count} items returned from the query");
        }
    }
}
