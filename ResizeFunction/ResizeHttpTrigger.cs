using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Company.Function
{
    public static class ResizeHttpTrigger
    {
        [FunctionName("ResizeHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // validate query params
            if (!int.TryParse(req.Query["w"], out int w) || w <= 0)
                return new BadRequestObjectResult("missing or invalid parameter: w");
            if (!int.TryParse(req.Query["h"], out int h) || h <= 0)
                return new BadRequestObjectResult("missing or invalid parameter: h");

            // check body not empty
            if (req.ContentLength == null || req.ContentLength == 0)
                return new BadRequestObjectResult("request body is empty");

            try
            {
                byte[] targetImageBytes;
                using (var msInput = new MemoryStream())
                {
                    await req.Body.CopyToAsync(msInput);
                    msInput.Position = 0;

                    using (var image = Image.Load(msInput))
                    {
                        log.LogInformation($"resizing image from {image.Width}x{image.Height} to {w}x{h}");
                        image.Mutate(x => x.Resize(w, h));

                        using (var msOutput = new MemoryStream())
                        {
                            image.SaveAsJpeg(msOutput);
                            targetImageBytes = msOutput.ToArray();
                        }
                    }
                }

                return new FileContentResult(targetImageBytes, "image/jpeg");
            }
            catch (UnknownImageFormatException)
            {
                return new BadRequestObjectResult("unsupported image format");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "unexpected error during image resize");
                return new ObjectResult("internal server error") { StatusCode = 500 };
            }
        }
    }
}
