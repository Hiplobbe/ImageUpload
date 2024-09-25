using System.ComponentModel.DataAnnotations;

namespace ImageUpload.Models
{
    public class PostImageRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public IFormFile Image { get; set; }
    }
}
