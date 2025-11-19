using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace dbs_test.Model
{
   
        [Table("localazy")]
        public class Localazy
        {
            [Key]
            [Column("id")]
            [Required]
            [StringLength(36)]
            public string Id { get; set; }

            [Column("language")]
            [Required]
            [StringLength(10)]
            public string Language { get; set; }

            [Column("key_id")]
            [Required]
            [StringLength(36)]
            public string KeyId { get; set; }

            [Column("translation")]
            [Required]
            [StringLength(1000)]
            public string Translation { get; set; }
        }
    }
