using Microsoft.AspNetCore.Mvc;
using BowlingApi.Controllers;
using BowlingApi.Models;

namespace BowlingApi.Tests
{
    public class BowlingControllerTests
    {
        [Fact]
        public void CreateGame_WithValidPlayerName_ReturnsOkWithGameId()
        {
            // Arrange
            var controller = new BowlingController();
            var request = new CreateGameRequest { PlayerName = "TestPlayer" };
            
            // Act
            var result = controller.CreateGame(request);
            
            // Assert
            var okResult = Assert.IsType<ActionResult<CreateGameResponse>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var response = Assert.IsType<CreateGameResponse>(okObjectResult.Value);
            
            Assert.NotNull(response.GameId);
            Assert.Equal("TestPlayer", response.PlayerName);
        }
        
        [Fact]
        public void CreateGame_WithEmptyPlayerName_ReturnsBadRequest()
        {
            // Arrange
            var controller = new BowlingController();
            var request = new CreateGameRequest { PlayerName = "" };
            
            // Act
            var result = controller.CreateGame(request);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<CreateGameResponse>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
        
        [Fact]
        public void RecordRoll_WithValidGameIdAndPins_ReturnsOk()
        {
            // Arrange
            var controller = new BowlingController();
            var createRequest = new CreateGameRequest { PlayerName = "TestPlayer" };
            var createResult = controller.CreateGame(createRequest);
            var okObjectResult = Assert.IsType<OkObjectResult>(createResult.Result);
            var createResponse = Assert.IsType<CreateGameResponse>(okObjectResult.Value);
            var gameId = createResponse.GameId;
            
            var rollRequest = new RollRequest { Pins = 5 };
            
            // Act
            var result = controller.RecordRoll(gameId, rollRequest);
            
            // Assert
            Assert.IsType<OkResult>(result);
        }
        
        [Fact]
        public void RecordRoll_WithInvalidGameId_ReturnsNotFound()
        {
            // Arrange
            var controller = new BowlingController();
            var rollRequest = new RollRequest { Pins = 5 };
            
            // Act
            var result = controller.RecordRoll("invalid-id", rollRequest);
            
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
        
        [Fact]
        public void RecordRoll_WithInvalidPins_ReturnsBadRequest()
        {
            // Arrange
            var controller = new BowlingController();
            var createRequest = new CreateGameRequest { PlayerName = "TestPlayer" };
            var createResult = controller.CreateGame(createRequest);
            var okObjectResult = Assert.IsType<OkObjectResult>(createResult.Result);
            var createResponse = Assert.IsType<CreateGameResponse>(okObjectResult.Value);
            var gameId = createResponse.GameId;
            
            var rollRequest = new RollRequest { Pins = 11 }; // Invalid pins
            
            // Act
            var result = controller.RecordRoll(gameId, rollRequest);
            
            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        
        [Fact]
        public void GetGameState_WithValidGameId_ReturnsGameState()
        {
            // Arrange
            var controller = new BowlingController();
            var createRequest = new CreateGameRequest { PlayerName = "TestPlayer" };
            var createResult = controller.CreateGame(createRequest);
            var okObjectResult = Assert.IsType<OkObjectResult>(createResult.Result);
            var createResponse = Assert.IsType<CreateGameResponse>(okObjectResult.Value);
            var gameId = createResponse.GameId;
            
            // Act
            var result = controller.GetGameState(gameId);
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<GameStateResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<GameStateResponse>(okResult.Value);
            
            Assert.False(response.IsComplete);
            Assert.Equal(1, response.CurrentFrame);
            Assert.Equal(10, response.Frames.Count);
            Assert.Empty(response.Scores);
        }
        
        [Fact]
        public void GetGameState_WithInvalidGameId_ReturnsNotFound()
        {
            // Arrange
            var controller = new BowlingController();
            
            // Act
            var result = controller.GetGameState("invalid-id");
            
            // Assert
            var actionResult = Assert.IsType<ActionResult<GameStateResponse>>(result);
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }
    }
}