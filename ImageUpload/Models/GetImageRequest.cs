using System.ComponentModel.DataAnnotations;

namespace ImageUpload.Models
{
    public class GetImageRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Filename { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int Width { get; set; }

        [Required]
        [Range(1, Int32.MaxValue)]
        public int Height { get; set; }
    }
}
