namespace BowlingScoringService;

public class Game
{
    private List<Frame> _frames = new();
    private ScoreCalculator _calculator = new();
    private int _currentFrameIndex = 0;

    public Game()
    {
        // Initialize 10 frames
        for (var i = 1; i <= 10; i++)
        {
            _frames.Add(new Frame(i));
        }
    }

    public void Roll(int pins)
    {
        if (_currentFrameIndex >= 10)
            throw new InvalidOperationException("Game is complete");

        var currentFrame = _frames[_currentFrameIndex];
        currentFrame.AddRoll(pins);

        if (currentFrame.IsComplete)
            _currentFrameIndex++;
    }

    public List<ScoreCalculator.FrameScore> GetScores()
    {
        return _calculator.CalculateScores(_frames).ToList();
    }

    public bool IsComplete => _currentFrameIndex >= 10;

    public Frame GetCurrentFrame()
    {
        return _currentFrameIndex < 10 ? _frames[_currentFrameIndex] : _frames[9];
    }

    public List<Frame> GetAllFrames()
    {
        return _frames;
    }
}