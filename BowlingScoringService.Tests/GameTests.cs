namespace BowlingScoringService.Tests;

public class GameTests
{
    [Fact]
    public void NewGame_HasTenFrames()
    {
        // Arrange & Act
        var game = new Game();
            
        // Assert
        Assert.Equal(10, game.GetAllFrames().Count);
        Assert.Equal(1, game.GetCurrentFrame().FrameNumber);
        Assert.False(game.IsComplete);
    }
        
    [Fact]
    public void Roll_UpdatesCurrentFrame()
    {
        // Arrange
        var game = new Game();
            
        // Act
        game.Roll(5);
            
        // Assert
        Assert.Equal(5, game.GetCurrentFrame().FirstRoll);
        Assert.Equal(1, game.GetCurrentFrame().FrameNumber);
    }
        
    [Fact]
    public void Roll_Strike_AdvancesToNextFrame()
    {
        // Arrange
        var game = new Game();
            
        // Act
        game.Roll(10); // Strike
            
        // Assert
        Assert.Equal(2, game.GetCurrentFrame().FrameNumber);
    }
        
    [Fact]
    public void Roll_TwoNonStrikeRolls_AdvancesToNextFrame()
    {
        // Arrange
        var game = new Game();
            
        // Act
        game.Roll(5);
        game.Roll(3);
            
        // Assert
        Assert.Equal(2, game.GetCurrentFrame().FrameNumber);
    }
        
    [Fact]
    public void Roll_AllFrames_CompletesGame()
    {
        // Arrange
        var game = new Game();
            
        // Act - Roll 20 times with 1 pin (no strikes or spares)
        for (int i = 0; i < 20; i++)
        {
            game.Roll(1);
        }
            
        // Assert
        Assert.True(game.IsComplete);
    }
        
    [Fact]
    public void Roll_AfterGameComplete_ThrowsInvalidOperationException()
    {
        // Arrange
        var game = new Game();
            
        // Roll 20 times with 1 pin (no strikes or spares)
        for (int i = 0; i < 20; i++)
        {
            game.Roll(1);
        }
            
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => game.Roll(1));
    }
        
    [Fact]
    public void GetScores_ReturnsCorrectScores_ForSimpleGame()
    {
        // Arrange
        var game = new Game();
            
        // Act - Roll all 1s
        for (int i = 0; i < 20; i++)
        {
            game.Roll(1);
        }
            
        var scores = game.GetScores();
            
        // Assert
        Assert.Equal(10, scores.Count);
        Assert.Equal(2, scores[0].Score); // Frame 1: 1 + 1 = 2
        Assert.Equal(2, scores[1].Score); // Frame 2: 1 + 1 = 2
        Assert.Equal(20, scores[9].CumulativeScore); // Total score: 10 frames * 2 pins = 20
    }
        
    [Fact]
    public void GetScores_ReturnsCorrectScores_ForPerfectGame()
    {
        // Arrange
        var game = new Game();
            
        // Act - Roll 12 strikes (perfect game)
        for (int i = 0; i < 12; i++)
        {
            game.Roll(10);
        }
            
        var scores = game.GetScores();
            
        // Assert
        Assert.Equal(10, scores.Count);
        Assert.Equal(30, scores[0].Score); // Frame 1: 10 + 10 + 10 = 30
        Assert.Equal(30, scores[1].Score); // Frame 2: 10 + 10 + 10 = 30
        Assert.Equal(300, scores[9].CumulativeScore); // Perfect game = 300
    }
}