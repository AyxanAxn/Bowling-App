namespace BowlingScoringService.Tests
{
    public class FrameTests
    {
        [Fact]
        public void NewFrame_HasCorrectFrameNumber()
        {
            // Arrange & Act
            var frame = new Frame(5);
            
            // Assert
            Assert.Equal(5, frame.FrameNumber);
            Assert.Null(frame.FirstRoll);
            Assert.Null(frame.SecondRoll);
            Assert.Null(frame.ThirdRoll);
            Assert.False(frame.IsComplete);
        }
        
        [Fact]
        public void AddRoll_FirstRoll_SetsFirstRollValue()
        {
            // Arrange
            var frame = new Frame(1);
            
            // Act
            frame.AddRoll(5);
            
            // Assert
            Assert.Equal(5, frame.FirstRoll);
            Assert.Null(frame.SecondRoll);
            Assert.False(frame.IsComplete);
        }
        
        [Fact]
        public void AddRoll_Strike_CompletesFrameIfNotTenth()
        {
            // Arrange
            var frame = new Frame(1);
            
            // Act
            frame.AddRoll(10);
            
            // Assert
            Assert.Equal(10, frame.FirstRoll);
            Assert.Null(frame.SecondRoll);
            Assert.True(frame.IsComplete);
            Assert.True(frame.IsStrike);
            Assert.False(frame.IsSpare);
        }
        
        [Fact]
        public void AddRoll_Strike_DoesNotCompleteFrameIfTenth()
        {
            // Arrange
            var frame = new Frame(10);
            
            // Act
            frame.AddRoll(10);
            
            // Assert
            Assert.Equal(10, frame.FirstRoll);
            Assert.Null(frame.SecondRoll);
            Assert.False(frame.IsComplete);
            Assert.True(frame.IsStrike);
        }
        
        [Fact]
        public void AddRoll_SecondRoll_SetsSecondRollValue()
        {
            // Arrange
            var frame = new Frame(1);
            frame.AddRoll(5);
            
            // Act
            frame.AddRoll(3);
            
            // Assert
            Assert.Equal(5, frame.FirstRoll);
            Assert.Equal(3, frame.SecondRoll);
            Assert.True(frame.IsComplete);
            Assert.False(frame.IsStrike);
            Assert.False(frame.IsSpare);
        }
        
        [Fact]
        public void AddRoll_Spare_SetsIsSpareProperty()
        {
            // Arrange
            var frame = new Frame(1);
            frame.AddRoll(5);
            
            // Act
            frame.AddRoll(5);
            
            // Assert
            Assert.Equal(5, frame.FirstRoll);
            Assert.Equal(5, frame.SecondRoll);
            Assert.True(frame.IsComplete);
            Assert.False(frame.IsStrike);
            Assert.True(frame.IsSpare);
        }
        
        [Fact]
        public void AddRoll_Spare_DoesNotCompleteFrameIfTenth()
        {
            // Arrange
            var frame = new Frame(10);
            frame.AddRoll(5);
            
            // Act
            frame.AddRoll(5);
            
            // Assert
            Assert.Equal(5, frame.FirstRoll);
            Assert.Equal(5, frame.SecondRoll);
            Assert.False(frame.IsComplete);
            Assert.False(frame.IsStrike);
            Assert.True(frame.IsSpare);
        }
        
        [Fact]
        public void AddRoll_ThirdRoll_SetsThirdRollValueInTenthFrame()
        {
            // Arrange
            var frame = new Frame(10);
            frame.AddRoll(10); // Strike
            frame.AddRoll(10); // Strike
            
            // Act
            frame.AddRoll(10); // Strike
            
            // Assert
            Assert.Equal(10, frame.FirstRoll);
            Assert.Equal(10, frame.SecondRoll);
            Assert.Equal(10, frame.ThirdRoll);
            Assert.True(frame.IsComplete);
        }
        
        [Fact]
        public void AddRoll_InvalidPins_ThrowsArgumentException()
        {
            // Arrange
            var frame = new Frame(1);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => frame.AddRoll(11));
            Assert.Throws<ArgumentException>(() => frame.AddRoll(-1));
        }
        
        [Fact]
        public void AddRoll_TooManyPinsInSecondRoll_ThrowsArgumentException()
        {
            // Arrange
            var frame = new Frame(1);
            frame.AddRoll(5);
            
            // Act & Assert
            Assert.Throws<ArgumentException>(() => frame.AddRoll(6));
        }
        
        [Fact]
        public void AddRoll_ToCompleteFrame_ThrowsInvalidOperationException()
        {
            // Arrange
            var frame = new Frame(1);
            frame.AddRoll(10); // Strike completes the frame
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => frame.AddRoll(5));
        }
    }
}