using DruidsCornerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace DruidsCornerAPI.Controllers
{
    [ApiController]
    [Route("recipe")]
    //[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class RecipeController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(ILogger<RecipeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        [HttpGet(Name = "ListAllRecipes")]
        public async Task<IActionResult> ListAllRecipes()
        {
            return Ok("This is a fake recipe !");
        }

    }
}