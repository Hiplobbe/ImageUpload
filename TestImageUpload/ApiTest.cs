using ImageUpload.Controllers;
using ImageUpload.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Mime;

namespace TestImageUpload
{
    public class ApiTest : IDisposable
    {
        private ImagesController _controller;

        public ApiTest()
        {
            _controller = new ImagesController(NullLogger<ImagesController>.Instance);
        }

        [Fact]
        public async void Post()
        {
            PostImageRequest req = new PostImageRequest
            {
                Name = "test",
                Image = CreateMockFile("test")
            };

            await _controller.SendImage("Nycke1en", req);

            Assert.True(File.Exists("Images/test/test.jpg"));
        }

        [Fact]
        public async void StandardFlow()
        {
            PostImageRequest pReq = new PostImageRequest
            {
                Name = "test",
                Image = CreateMockFile("testflow")
            };

            await _controller.SendImage("Nycke1en", pReq);

            GetImageRequest gReq = new GetImageRequest
            {
                Username = "test",
                Filename = "testflow.jpg",
                Width = 360,
                Height = 360
            };

            var result = _controller.GetImage("Nycke1en", gReq).Result;

            Assert.NotNull(result);
            Assert.True(result is FileStreamResult); //If a FileStreamResult is returned the file was found.
        }

        private FormFile CreateMockFile(string filename)
        {
            using (FileStream fStream = new FileStream("test_lilo.jpg", FileMode.Open))
            {
                var memStream = new MemoryStream();

                fStream.CopyTo(memStream);
                memStream.Position = 0;

                var file = new FormFile(memStream, 0, memStream.Length, "id_from_form", filename)
                {
                    Headers = new HeaderDictionary()
                };
                file.ContentType = "image/jpg";

                return file;
            }                
        }

        public void Dispose()
        {
            DirectoryInfo di = new DirectoryInfo("Images/test");

            try
            {
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.EnumerateDirectories())
                {
                    dir.Delete();
                }
            }
            catch (Exception ex) 
            { 
                //File is in use deletes on next test.
            }
        }
    }
}