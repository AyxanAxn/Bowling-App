namespace BowlingScoringService.Tests;

public class ScoreCalculatorTests
{
    [Fact]
    public void CalculateScores_AllOnes_Returns20()
    {
        // Arrange
        var calculator = new ScoreCalculator();
        var frames = CreateFramesWithRolls(new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });

        // Act
        var scores = calculator.CalculateScores(frames).ToList();

        // Assert
        Assert.Equal(10, scores.Count);
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(i + 1, scores[i].FrameNumber);
            Assert.Equal(2, scores[i].Score);
            Assert.Equal(2 * (i + 1), scores[i].CumulativeScore);
        }
    }

    [Fact]
    public void CalculateScores_OneSpare_IncludesBonusRoll()
    {
        // Arrange
        var calculator = new ScoreCalculator();
        var frames = CreateFramesWithRolls(new[] { 5, 5, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        // Act
        var scores = calculator.CalculateScores(frames).ToList();

        // Assert
        Assert.Equal(10, scores.Count);
        Assert.Equal(13, scores[0].Score); // 5 + 5 + 3 (bonus) = 13
        Assert.Equal(3, scores[1].Score); // 3 + 0 = 3
        Assert.Equal(16, scores[1].CumulativeScore); // 13 + 3 = 16
    }

    [Fact]
    public void CalculateScores_OneStrike_IncludesTwoBonusRolls()
    {
        // Arrange
        var calculator = new ScoreCalculator();
        var frames = CreateFramesWithRolls(new[] { 10, 3, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

        // Act
        var scores = calculator.CalculateScores(frames).ToList();

        // Assert
        Assert.Equal(10, scores.Count);
        Assert.Equal(17, scores[0].Score); // 10 + 3 + 4 (bonus) = 17
        Assert.Equal(7, scores[1].Score); // 3 + 4 = 7
        Assert.Equal(24, scores[1].CumulativeScore); // 17 + 7 = 24
    }

    [Fact]
    public void CalculateScores_PerfectGame_Returns300()
    {
        // Arrange
        var calculator = new ScoreCalculator();
        var frames = CreateFramesWithRolls(new[] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 });

        // Act
        var scores = calculator.CalculateScores(frames).ToList();

        // Assert
        Assert.Equal(10, scores.Count);
        for (int i = 0; i < 9; i++)
        {
            Assert.Equal(30, scores[i].Score); // Each frame is 10 + 10 + 10 = 30
        }

        Assert.Equal(30, scores[9].Score); // 10th frame: 10 + 10 + 10 = 30
        Assert.Equal(300, scores[9].CumulativeScore); // Perfect game = 300
    }

    /// <summary>
    /// Creates a list of frames with the given rolls applied to them
    /// </summary>
    /// <param name="rolls">Array of pin counts for each roll</param>
    /// <returns>List of frames with rolls applied</returns>
    private static List<Frame> CreateFramesWithRolls(int[] rolls)
    {
        // Initialize 10 frames
        var frames = Enumerable.Range(1, 10).Select(i => new Frame(i)).ToList();

        int rollIndex = 0;
        int frameIndex = 0;

        // Process rolls until we run out of rolls or frames
        while (rollIndex < rolls.Length && frameIndex < 10)
        {
            var currentFrame = frames[frameIndex];

            // Apply the first roll to the current frame
            ApplyRoll(currentFrame, rolls[rollIndex++]);

            // If the frame is complete after first roll (strike in frames 1-9), move to next frame
            if (currentFrame.IsComplete)
            {
                frameIndex++;
                continue;
            }

            // If we have more rolls, apply the second roll
            if (rollIndex < rolls.Length)
            {
                ApplyRoll(currentFrame, rolls[rollIndex++]);

                // If the frame is complete after second roll, move to next frame
                if (currentFrame.IsComplete)
                {
                    frameIndex++;
                    continue;
                }

                // Handle the special case of the 10th frame with a third roll
                if (IsEligibleForThirdRoll(currentFrame) && rollIndex < rolls.Length)
                {
                    ApplyRoll(currentFrame, rolls[rollIndex++]);
                    frameIndex++;
                }
            }
        }

        return frames;
    }

    /// <summary>
    /// Applies a roll to a frame
    /// </summary>
    private static void ApplyRoll(Frame frame, int pins)
    {
        frame.AddRoll(pins);
    }

    /// <summary>
    /// Determines if a frame is eligible for a third roll
    /// </summary>
    private static bool IsEligibleForThirdRoll(Frame frame)
    {
        return frame.FrameNumber == 10 &&
               (frame.IsStrike || frame.IsSpare) &&
               !frame.ThirdRoll.HasValue;
    }
}