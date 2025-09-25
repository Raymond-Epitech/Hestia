using Business.Interfaces;
using EntityFramework.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Exceptions;

namespace Api.Controllers;

public class ImageController(
    IImageService imageService,
    HestiaContext context) : Controller
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

    [HttpPost("test")]
    [Authorize]
    public async Task<ActionResult> LoadImages()
    {
        var users = context.Users.ToList();
        foreach (var user in users)
        {
            if (user.PathToProfilePicture.StartsWith("https://lh3.googleusercontent.com/"))
            {
                user.PathToProfilePicture = await imageService.DownloadImageAsFormFileAsync(user.PathToProfilePicture);
                context.Update(user);
                await context.SaveChangesAsync();
            }
        }
        return Ok();
    }
}

