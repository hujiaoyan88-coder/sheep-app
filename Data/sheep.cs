using System.ComponentModel.DataAnnotations;

namespace sheep.Data
{
    public class SheepEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string UserId { get; set; } // ログインユーザー識別用
    }
}
