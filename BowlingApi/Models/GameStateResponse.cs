public class GameStateResponse
{
    public bool IsComplete { get; set; }
    public int CurrentFrame { get; set; }
    public List<ScoreDto> Scores { get; set; }
    public List<FrameDto> Frames { get; set; }
}