public class FrameDto
{
    public int FrameNumber { get; set; }
    public int? FirstRoll { get; set; }
    public int? SecondRoll { get; set; }
    public int? ThirdRoll { get; set; }
    public bool IsStrike { get; set; }
    public bool IsSpare { get; set; }
    public bool IsComplete { get; set; }
}