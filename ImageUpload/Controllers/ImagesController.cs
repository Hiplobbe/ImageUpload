using ImageUpload.AuthFilter;
using ImageUpload.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.IO.Pipes;
using System.Net;
using System.Net.Http.Headers;

namespace ImageUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [TypeFilter(typeof(ApiKeyValidation))]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ILogger<ImagesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Takes an image and stores it in the filesystem.
        /// </summary>
        /// <param name="key">Apikey for the request.</param>
        /// <param name="request">Object containing the username and image.</param>
        /// <response code="200">Image has been processed, and is saved under the username. https://http.cat/status/200</response>
        /// <response code="400">https://http.cat/status/400</response>
        /// <response code="401">https://http.cat/status/401</response>
        [HttpPost("SendImage")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SendImage(string key, [FromForm] PostImageRequest request)
        {
            try
            {
                var path = Path.Combine("Images/", request.Name, request.Image.FileName);

                Directory.CreateDirectory(Path.Combine("Images/", request.Name));

                using (FileStream stream = new FileStream(path + GetExtension(request.Image), FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                    stream.Close();
                }

                return Ok();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Unable to save file with key {key}. Error: {ex.Message} : {ex.InnerException}");
                return BadRequest("Was not able to save the file, try a different filetype.");
            }
        }

        [HttpGet("GetImage")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetImage(string key, [FromQuery] GetImageRequest request)
        {
            var path = Path.Combine("Images/", request.Username, request.Filename);

            if (System.IO.File.Exists(path))
            {
                try
                {
                    FileStream fileStream = new FileStream(path, FileMode.Open);
                    MemoryStream memoryStream = new MemoryStream();

                    var image = Image.Load(fileStream);
                    image.Mutate(x => x.Resize(request.Width, request.Height));
                    await image.SaveAsync(memoryStream, image.Metadata.DecodedImageFormat);
                    memoryStream.Position = 0;

                    return File(memoryStream, "application/octet-stream", request.Filename);
                }
                catch(Exception ex)
                {
                    _logger.LogError($"Unable to get file {request.Filename} with key {key}. Error: {ex.Message} : {ex.InnerException}");
                    return StatusCode(500);
                }
            }

            return NotFound("Image not found");
        }

        [HttpDelete("DeleteImage")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)] // https://http.cat/status/409
        public async Task<IActionResult> DeleteImage(string key, string username, string filename)
        {
            var path = Path.Combine("Images/", username, filename);

            if (System.IO.File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                    
                    return Ok();
                }
                catch(System.IO.IOException ex)
                {
                    _logger.LogError($"Unable to delete file {username}/{filename} with key {key}. Error: {ex.Message} : {ex.InnerException}");
                    return StatusCode(409); 
                }                
            }

            return NotFound("Image not found");
        }

        private string GetExtension(IFormFile image)
        {
            switch(image.ContentType)
            {
                case "image/jpeg":
                    return ".jpeg";

                case "image/png":
                    return ".png";
            }

            return ".jpg"; //Default to jpg.
        }
    }
}
