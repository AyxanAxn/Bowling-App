namespace BowlingScoringService;

public class Frame
{
    public int FrameNumber { get; }
    public int? FirstRoll { get; private set; }
    public int? SecondRoll { get; private set; }
    public int? ThirdRoll { get; private set; }
    public bool IsComplete { get; private set; }

    public Frame(int frameNumber) => FrameNumber = frameNumber;

    public void AddRoll(int pins)
    {
        if (IsComplete) throw new InvalidOperationException("Frame is complete");
    
        if (!FirstRoll.HasValue)
        {
            ValidateRoll(pins, 0, 10);
            FirstRoll = pins;
        
            if (FrameNumber < 10 && pins == 10) // Strike in frames 1-9
                IsComplete = true;
        }
        else if (!SecondRoll.HasValue)
        {
            // In 10th frame after a strike, all 10 pins are reset
            int maxPins = (FrameNumber == 10 && FirstRoll == 10) ? 10 : 10 - FirstRoll.Value;
            ValidateRoll(pins, 0, maxPins);
            SecondRoll = pins;
        
            // Frame is complete if:
            // - It's not the 10th frame, OR
            // - It's the 10th frame but no bonus roll is earned (no strike or spare)
            if (FrameNumber < 10 || (FrameNumber == 10 && FirstRoll != 10 && FirstRoll + SecondRoll < 10))
                IsComplete = true;
        }
        else if (FrameNumber == 10)
        {
            // For the third roll in 10th frame:
            // - After a spare, max is 10
            // - After a strike + strike, max is 10
            // - After a strike + non-strike, max is (10 - SecondRoll)
            int maxPins = (FirstRoll == 10 && SecondRoll == 10) || (FirstRoll + SecondRoll == 10) ? 10 : 10 - SecondRoll.Value;
            ValidateRoll(pins, 0, maxPins);
            ThirdRoll = pins;
            IsComplete = true;
        }
    }
    public bool IsStrike => FirstRoll is 10;
    public bool IsSpare => !IsStrike 
                           && FirstRoll.HasValue 
                           && SecondRoll.HasValue 
                           && (FirstRoll.Value + SecondRoll.Value == 10);
    private void ValidateRoll(int pins, int min, int max)
    {
        if (pins < min || pins > max)
            throw new ArgumentException($"Invalid roll. Must be between {min}-{max}");
    }
}