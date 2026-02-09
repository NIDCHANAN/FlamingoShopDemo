using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using FlamingoModel.Models;
using System.Net.Http.Headers;

namespace BouquetAI.Controllers;

[ApiController]
[Route("api/generate-image")]
public class GenerateImageController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public GenerateImageController(IConfiguration config)
    {
        _config = config;
        _http = new HttpClient();
    }

    [HttpPost]
    public async Task<IActionResult> Generate([FromBody] BouquetRequestDto request)
    {
        // 🔑 Cloudflare config
        var apiToken = _config["Cloudflare:ApiToken"]!.Trim();
        var accountId = _config["Cloudflare:AccountId"]!.Trim();

        if (string.IsNullOrWhiteSpace(request.Flower) || request.Stems <= 0)
            return BadRequest("Invalid bouquet data");

        // 🧠 Prompt
        var prompt = $"""
                    A luxury florist-style hand-tied flower bouquet.
                    Round bouquet shape, compact and symmetrical.
                    Tightly packed flowers with no gaps.
                    Main flower: {request.Flower}.
                    Exactly {request.Stems} flower stems only.
                    Wrapped in soft matte wrapping paper.
                    Wrapping paper color: {request.Wrap}.
                    No vase, no table, no props.
                    Front view, centered composition.
                    Professional studio product photography.
                    Ultra realistic, sharp focus, white background.
                    """;



        var body = new
        {
            prompt = prompt,
            steps = 25
        };

        var req = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.cloudflare.com/client/v4/accounts/{accountId}/ai/run/@cf/stabilityai/stable-diffusion-xl-base-1.0"
        );

        req.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiToken);

        req.Content = new StringContent(
            JsonSerializer.Serialize(body),
            Encoding.UTF8,
            "application/json"
        );

        var resp = await _http.SendAsync(req);

        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync();
            return StatusCode((int)resp.StatusCode, error);
        }

        var imageBytes = await resp.Content.ReadAsByteArrayAsync();

        return File(imageBytes, "image/png");
    }
}
