using BowlingScoringService;
using Microsoft.AspNetCore.Mvc;
using BowlingApi.Models;

namespace BowlingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BowlingController : ControllerBase
{
    private static readonly Dictionary<string, Game> Games = new();

    // Create a new game
    [HttpPost("games")]
    public ActionResult<CreateGameResponse> CreateGame([FromBody] CreateGameRequest request)
    {
        if (string.IsNullOrEmpty(request.PlayerName))
            return BadRequest("Player name is required");

        var gameId = Guid.NewGuid().ToString();
        Games[gameId] = new Game();

        return Ok(new CreateGameResponse(gameId, request.PlayerName));
    }

    // Record a roll
    [HttpPost("games/{gameId}/rolls")]
    public ActionResult RecordRoll(string gameId, [FromBody] RollRequest request)
    {
        if (!Games.TryGetValue(gameId, out var game))
            return NotFound("Game not found");

        try
        {
            game.Roll(request.Pins);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Get current game state
    [HttpGet("games/{gameId}")]
    public ActionResult<GameStateResponse> GetGameState(string gameId)
    {
        if (!Games.TryGetValue(gameId, out var game))
            return NotFound("Game not found");

        return Ok(new GameStateResponse
        {
            IsComplete = game.IsComplete,
            CurrentFrame = game.GetCurrentFrame().FrameNumber,
            Scores = game.GetScores().Select(s => new ScoreDto
            {
                FrameNumber = s.FrameNumber,
                FrameScore = s.Score,
                CumulativeScore = s.CumulativeScore
            }).ToList(),
            Frames = game.GetAllFrames().Select(f => new FrameDto
            {
                FrameNumber = f.FrameNumber,
                FirstRoll = f.FirstRoll,
                SecondRoll = f.SecondRoll,
                ThirdRoll = f.ThirdRoll,
                IsStrike = f.IsStrike,
                IsSpare = f.IsSpare,
                IsComplete = f.IsComplete
            }).ToList()
        });
    }
}