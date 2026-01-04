using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageController(
    IImageService imageService) : Controller
{
    [HttpGet("{fileName}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetImage(string fileName)
    {
        var file = await imageService.GetImageByNameAsync(fileName);

        return File(file.Content, file.ContentType, file.FileName);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new InvalidEntityException("File is empty");
        var result =  await imageService.SaveImage(file);
        return Ok(result);
    }

    [HttpDelete("{fileName}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult DeleteImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
            throw new InvalidEntityException("File name is empty");

        return Ok(imageService.DeleteImage(fileName));
    }
}

