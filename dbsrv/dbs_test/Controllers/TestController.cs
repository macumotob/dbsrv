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
        static string _connString = $"Server=element-prod.cmeu2ebyhpap.eu-central-1.rds.amazonaws.com;Database=hour_test"
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
        public async Task<List<WellnessCourseLessonTitle>> GetLessonTitles()
        {
            const string sql = "SELECT * FROM wellness_course_lessons_titles where title_id=@title_id";

            var items = await dbs.QueryAsync(
                sql,
                reader => new WellnessCourseLessonTitle
                {
                    Id = reader.GetString("id"),
                    TitleId = reader.GetString("title_id"),
                    LanguageId = reader.GetString("language_id"),
                    Title = reader.GetString("title"),
                    CourseId = reader.GetString("courseId"),
                    InfluencerId = reader.GetString("influencerId"),
                    LessonId = reader.GetString("lessonId")
                },
                new[]
                {
                    new MySqlParameter("@title_id", "8bfbfc9d-304b-40ae-bfa2-a4e8d5c31879")
                }
            );

            return items;
        }
        [HttpGet]
        public async Task<List<Localazy>> GetLocalazy()
        {
            const string sql = "SELECT * FROM localazy WHERE key_id = @key_id";

            return await dbs.QueryAsync(
                sql,
                reader => new Localazy
                {
                    Id = reader.GetString("id"),
                    Language = reader.GetString("language"),
                    KeyId = reader.GetString("key_id"),
                    Translation = reader.GetString("translation")
                },
                new[]
                {
                    new MySqlParameter("@key_id", "8bfbfc9d-304b-40ae-bfa2-a4e8d5c31879")
                }
            );
        }
    }
}