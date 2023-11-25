using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TwitterClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CKEditorController: ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public CKEditorController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("GetCKEditorToken")]
        public async Task<IActionResult> GetCKEditorToken()
        {
            try
            {
                var tokenUrl = _configuration["TokenSettings:CKEditorTokenUrl"]
                        ?? Environment.GetEnvironmentVariable("TokenUrl");

                if (string.IsNullOrEmpty(tokenUrl))
                {
                    return BadRequest(new { error = "TokenUrl is not configured." });
                }
                
                
                var httpClient = _httpClientFactory.CreateClient();
                HttpResponseMessage response = await httpClient.GetAsync(tokenUrl);
                response.EnsureSuccessStatusCode();

                string token = await response.Content.ReadAsStringAsync();
                return Ok( token );
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Failed to retrieve token." });
            }
        }
    }
}