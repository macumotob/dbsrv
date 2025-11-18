using dbs_test.Model;
using dbsrv;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace dbs_test.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TestController : Controller
    {
        static string _connString = $"Server=element-prod.cmeu2ebyhpap.eu-central-1.rds.amazonaws.com;Database=7element_prod_db"
              + ";port=3306;User Id=element;password=dmfll9exoW7kKZ8lm"
               + ";SslMode=none;"
               ;

        [HttpGet]
        public IActionResult Init()
        {

            dbs.Init(_connString);
            return Ok();
        }
        [HttpGet]
        public async Task<List<Localazy>> Get()
        {
            // dbs.Init(_connString);
            var key_id = "3b3008c1-02ea-4e98-acbd-a67a5c99934b";
            var items = await dbs.QueryAsync(
    "SELECT * FROM localazy where key_id=@key_id",
    reader => new Localazy
    {
        Id = reader.GetString("id"),
        Key = reader.GetString("key_id"),
        Translation = reader.GetString("translation"),
        Language = reader.GetString("language")
    },
    new[]
    {
        new MySqlParameter("@key_id", key_id)
    }
);
            return items;
        }
    }
}
