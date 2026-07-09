using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VetAmb.Controllers;

[AllowAnonymous]
public class ErrorController : Controller
{
    [Route("error/{statusCode:int?}")]
    public IActionResult StatusCodePage(int? statusCode)
    {
        var code = statusCode ?? HttpContext.Response.StatusCode;

        ViewData["StatusCode"] = code;
        ViewData["Title"] = code == 404 ? "Stranica nije pronađena" : "Dogodila se pogreška";

        return View("~/Views/Shared/StatusCode.cshtml");
    }
}
