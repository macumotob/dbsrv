using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dbs_test.Model
{
 

    [Table("wellness_course_lessons_titles")]
    public class WellnessCourseLessonTitle
    {
        [Key]
        [Column("id")]
        [Required]
        [StringLength(36)]
        public string Id { get; set; }

        [Column("title_id")]
        [Required]
        [StringLength(36)]
        public string TitleId { get; set; }

        [Column("language_id")]
        [Required]
        [StringLength(36)]
        public string LanguageId { get; set; }

        [Column("title")]
        [Required]
        [StringLength(145)]
        public string Title { get; set; }

        [Column("courseId")]
        [Required]
        [StringLength(36)]
        public string CourseId { get; set; }

        [Column("influencerId")]
        [Required]
        [StringLength(36)]
        public string InfluencerId { get; set; }

        [Column("lessonId")]
        [Required]
        [StringLength(36)]
        public string LessonId { get; set; }

        //// Navigation properties
        //[ForeignKey("CourseId")]
        //public virtual WellnessCourse Course { get; set; }

        //[ForeignKey("InfluencerId")]
        //public virtual WellnessInfluencer Influencer { get; set; }

        //[ForeignKey("LessonId")]
        //public virtual WellnessCourseLesson Lesson { get; set; }
    }
}