namespace BowlingScoringService;

public class ScoreCalculator
{
    public record FrameScore(int FrameNumber, int Score, int CumulativeScore);

    public IEnumerable<FrameScore> CalculateScores(List<Frame> frames)
    {
        var scores = new List<FrameScore>();
        var cumulative = 0;

        for (var i = 0; i < frames.Count; i++)
        {
            var frame = frames[i];
            if (!CanScoreFrame(frame, frames, i)) continue;

            var frameScore = CalculateFrameScore(frame, frames, i);
            cumulative += frameScore;
            scores.Add(new FrameScore(frame.FrameNumber, frameScore, cumulative));
        }

        return scores;
    }

    private bool CanScoreFrame(Frame frame, List<Frame> allFrames, int index)
    {
        // Can't score incomplete frames
        if (!frame.IsComplete) return false;
    
        // 10th frame can always be scored if complete
        if (frame.FrameNumber == 10) return true;
    
        // For strike frames, need 2 more rolls
        if (frame.IsStrike)
            return HasFutureRolls(allFrames, index, 2);
    
        // For spare frames, need 1 more roll
        if (frame.IsSpare)
            return HasFutureRolls(allFrames, index, 1);
    
        // Open frames can always be scored
        return true;
    }

    private bool HasFutureRolls(List<Frame> frames, int currentIndex, int requiredRolls)
    {
        var collected = 0;
        for (var i = currentIndex + 1; i < frames.Count; i++)
        {
            var rolls = GetRolls(frames[i]);
            foreach (var _ in rolls)
            {
                collected++;
                if (collected >= requiredRolls) return true;
            }
        }
        return false;
    }

    private int CalculateFrameScore(Frame frame, List<Frame> allFrames, int index)
    {
        // Special handling for 10th frame
        if (frame.FrameNumber == 10)
        {
            int score = frame.FirstRoll ?? 0;
        
            // Add second roll if it exists
            if (frame.SecondRoll.HasValue)
                score += frame.SecondRoll.Value;
        
            // Add third roll if it exists
            if (frame.ThirdRoll.HasValue)
                score += frame.ThirdRoll.Value;
            
            return score;
        }

        // For frames 1-9
        var baseScore = frame.IsStrike ? 10 :
            frame.IsSpare ? 10 :
            (frame.FirstRoll ?? 0) + (frame.SecondRoll ?? 0);

        return baseScore + CalculateBonus(allFrames, index, frame.IsStrike ? 2 : (frame.IsSpare ? 1 : 0));
    }
    private int CalculateBonus(List<Frame> frames, int frameIndex, int rollsNeeded)
    {
        var bonus = 0;
        var collected = 0;
        
        // Iterate through subsequent frames
        for (var i = frameIndex + 1; i < frames.Count; i++)
        {
            // Get all valid rolls in current frame
            var rolls = GetRolls(frames[i]);
            
            foreach (var roll in rolls)
            {
                if (collected >= rollsNeeded) break;
                bonus += roll;
                collected++;
            }
        }
        return bonus;
    }

    private IEnumerable<int> GetRolls(Frame frame)
    {
        var rolls = new List<int>();
        if (frame.FirstRoll.HasValue) rolls.Add(frame.FirstRoll.Value);
        if (frame.SecondRoll.HasValue) rolls.Add(frame.SecondRoll.Value);
        if (frame.ThirdRoll.HasValue) rolls.Add(frame.ThirdRoll.Value);
        return rolls;
    }
}