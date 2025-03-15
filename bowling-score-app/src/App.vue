<template>
  <div class="app-container">
    <div class="container">
      <h1 class="page-title">Bowling Score Calculator</h1>
      
      <!-- Player Setup Form -->
      <div v-if="!gameStarted" class="setup-form">
        <h2 class="form-title">New Game</h2>
        <div class="form-group">
          <label for="playerName" class="form-label">Player Name</label>
          <input
            type="text" 
            id="playerName" 
            v-model="playerName" 
            class="form-input"
            placeholder="Enter player name"
          >
        </div>
        <button 
          @click="startGame" 
          class="btn btn-primary"
          :disabled="!playerName"
          :class="{'btn-disabled': !playerName}"
        >
          Start Game
        </button>
      </div>
      
      <!-- Game Play Area -->
      <div v-else class="game-area">
        <div class="game-panel">
          <div class="panel-header">
            <h2 class="player-name">Player: {{ playerName }}</h2>
            <div>
              <span class="frame-badge">Frame: {{ currentFrame }}</span>
              <span v-if="gameComplete" class="complete-badge">Game Complete</span>
            </div>
          </div>
          
          <!-- Pins Input -->
          <div v-if="!gameComplete" class="pins-input">
            <h3 class="roll-title">Roll {{currentRoll}}: Pins Knocked Down</h3>
            <div class="pins-buttons">
              <button 
                v-for="pins in availablePins" 
                :key="pins" 
                @click="recordRoll(pins)"
                class="pin-button"
              >
                {{ pins }}
              </button>
            </div>
          </div>
          
          <!-- Error Message -->
          <div v-if="errorMessage" class="error-message">
            <p>{{ errorMessage }}</p>
          </div>
          
          <!-- New Game Button (when game is complete) -->
          <button 
            v-if="gameComplete" 
            @click="resetGame"
            class="btn btn-primary"
          >
            Start New Game
          </button>
        </div>
        
        <!-- Scoreboard -->
        <div class="scoreboard-panel">
          <h2 class="scoreboard-title">Scoreboard</h2>
          <div class="table-container">
            <table class="scoreboard-table">
              <thead>
                <tr>
                  <th class="table-header">Frame</th>
                  <th class="table-header" colspan="3">Rolls</th>
                  <th class="table-header">Frame Score</th>
                  <th class="table-header">Total Score</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="frame in frames" :key="frame.frameNumber" :class="{'current-frame': frame.frameNumber === currentFrame && !gameComplete}">
                  <td class="table-cell frame-number">{{ frame.frameNumber }}</td>
                  <td class="table-cell" :class="{'strike': frame.isStrike}">
                    {{ formatRoll(frame.firstRoll, frame.isStrike) }}
                  </td>
                  <td class="table-cell" :class="{'spare': frame.isSpare}">
                    {{ formatRoll(frame.secondRoll, false, frame.isSpare) }}
                  </td>
                  <td class="table-cell" v-if="frame.frameNumber === 10">
                    {{ formatRoll(frame.thirdRoll) }}
                  </td>
                  <td class="table-cell" v-else></td>
                  <td class="table-cell frame-score">
                    {{ getFrameScore(frame.frameNumber) }}
                  </td>
                  <td class="table-cell total-score">
                    {{ getCumulativeScore(frame.frameNumber) }}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from 'vue';
import axios from 'axios';

export default {
  name: 'BowlingApp',
  setup() {
    // State variables
    const playerName = ref('');
    const gameStarted = ref(false);
    const gameId = ref('');
    const frames = ref([]);
    const scores = ref([]);
    const gameComplete = ref(false);
    const currentFrame = ref(1);
    const errorMessage = ref('');
    const maxPins = ref(10);
    
    // API base URL - change this to match your API
    const apiBaseUrl = 'https://localhost:7245/api/bowling';

    // Computed properties
    const availablePins = computed(() => {
      // Calculate available pins based on current frame state
      if (frames.value.length === 0) return Array.from({ length: maxPins.value + 1 }, (_, i) => i);
      
      const frame = frames.value.find(f => f.frameNumber === currentFrame.value);
      if (!frame) return Array.from({ length: maxPins.value + 1 }, (_, i) => i);
      
      // First roll of the frame
      if (frame.firstRoll === null) {
        return Array.from({ length: maxPins.value + 1 }, (_, i) => i);
      }
      
      // Second roll - limit by remaining pins unless 10th frame with strike
      if (frame.secondRoll === null) {
        if (frame.frameNumber === 10 && frame.firstRoll === 10) {
          return Array.from({ length: maxPins.value + 1 }, (_, i) => i);
        }
        return Array.from({ length: maxPins.value - frame.firstRoll + 1 }, (_, i) => i);
      }
      
      // Third roll in 10th frame
      if (frame.frameNumber === 10 && frame.thirdRoll === null) {
        // If first roll was strike or second roll was strike, all pins are available
        if (frame.firstRoll === 10 || frame.secondRoll === 10) {
          return Array.from({ length: maxPins.value + 1 }, (_, i) => i);
        }
        // If spare (first + second = 10), all pins are available
        if (frame.firstRoll + frame.secondRoll === 10) {
          return Array.from({ length: maxPins.value + 1 }, (_, i) => i);
        }
        // If neither strike nor spare in 10th frame, no third roll is allowed
        return [];
      }
      
      return [];
    });
    
    const currentRoll = computed(() => {
      if (frames.value.length === 0) return 1;
      
      const frame = frames.value.find(f => f.frameNumber === currentFrame.value);
      if (!frame) return 1;
      
      if (frame.firstRoll === null) return 1;
      if (frame.secondRoll === null) return 2;
      if (frame.frameNumber === 10 && frame.thirdRoll === null) return 3;
      
      return 1;
    });

    // Methods
    const startGame = async () => {
      try {
        errorMessage.value = '';
        const response = await axios.post(`${apiBaseUrl}/games`, {
          playerName: playerName.value
        });
        
        gameId.value = response.data.gameId;
        gameStarted.value = true;
        
        // Initialize frames
        frames.value = Array.from({ length: 10 }, (_, i) => ({
          frameNumber: i + 1,
          firstRoll: null,
          secondRoll: null,
          thirdRoll: null,
          isStrike: false,
          isSpare: false,
          isComplete: false
        }));
        
        await fetchGameState();
      } catch (error) {
        handleApiError(error);
      }
    };
    
    const recordRoll = async (pins) => {
      try {
        errorMessage.value = '';
        await axios.post(`${apiBaseUrl}/games/${gameId.value}/rolls`, {
          pins: pins
        });
        
        await fetchGameState();
      } catch (error) {
        handleApiError(error);
      }
    };
    
    const fetchGameState = async () => {
      try {
        const response = await axios.get(`${apiBaseUrl}/games/${gameId.value}`);
        const gameState = response.data;
        
        frames.value = gameState.frames;
        scores.value = gameState.scores;
        gameComplete.value = gameState.isComplete;
        currentFrame.value = gameState.currentFrame;
      } catch (error) {
        handleApiError(error);
      }
    };
    
    const resetGame = () => {
      gameStarted.value = false;
      gameId.value = '';
      frames.value = [];
      scores.value = [];
      gameComplete.value = false;
      currentFrame.value = 1;
      errorMessage.value = '';
    };
    
    const handleApiError = (error) => {
      console.error('API Error:', error);
      if (error.response && error.response.data) {
        errorMessage.value = error.response.data;
      } else {
        errorMessage.value = 'An error occurred while communicating with the server';
      }
    };
    
    const formatRoll = (roll, isStrike = false, isSpare = false) => {
      if (roll === null) return '';
      if (isStrike) return 'X';
      if (isSpare) return '/';
      if (roll === 0) return '-';
      return roll.toString();
    };
    
    const getFrameScore = (frameNumber) => {
      const score = scores.value.find(s => s.frameNumber === frameNumber);
      return score ? score.frameScore : '';
    };
    
    const getCumulativeScore = (frameNumber) => {
      const score = scores.value.find(s => s.frameNumber === frameNumber);
      return score ? score.cumulativeScore : '';
    };
    
    return {
      playerName,
      gameStarted,
      frames,
      scores,
      currentFrame,
      gameComplete,
      errorMessage,
      availablePins,
      currentRoll,
      startGame,
      recordRoll,
      resetGame,
      formatRoll,
      getFrameScore,
      getCumulativeScore
    };
  }
};
</script>

<style>
/* Regular CSS styles to replace Tailwind */
.app-container {
  min-height: 100vh;
  background: linear-gradient(to bottom right, #e6f0ff, #e6e6ff);
  padding: 2rem 0;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 1rem;
}

.page-title {
  font-size: 2.5rem;
  font-weight: bold;
  text-align: center;
  margin-bottom: 2rem;
  color: #3730a3;
}

.setup-form {
  max-width: 28rem;
  margin: 0 auto;
  background: white;
  padding: 2rem;
  border-radius: 0.75rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
}

.form-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  color: #4338ca;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-label {
  display: block;
  color: #4b5563;
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  transition: all 0.2s;
}

.form-input:focus {
  outline: none;
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.2);
}

.btn {
  padding: 0.75rem 1rem;
  border-radius: 0.5rem;
  font-weight: 500;
  transition: all 0.2s;
  border: none;
}

.btn-primary {
  background-color: #4f46e5;
  color: white;
  width: 100%;
  font-size: 1.125rem;
}

.btn-primary:hover {
  background-color: #4338ca;
}

.btn-primary:focus {
  outline: none;
  box-shadow: 0 0 0 3px rgba(79, 70, 229, 0.4);
}

.btn-disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.game-area {
  max-width: 64rem;
  margin: 0 auto;
}

.game-panel, .scoreboard-panel {
  background: white;
  padding: 2rem;
  border-radius: 0.75rem;
  box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
  margin-bottom: 2rem;
}

.panel-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.player-name {
  font-size: 1.5rem;
  font-weight: 600;
  color: #4338ca;
}

.frame-badge {
  font-weight: 600;
  font-size: 1.125rem;
  background-color: #e0e7ff;
  color: #4338ca;
  padding: 0.25rem 0.75rem;
  border-radius: 0.5rem;
}

.complete-badge {
  margin-left: 0.75rem;
  background-color: #d1fae5;
  color: #065f46;
  padding: 0.25rem 0.75rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 500;
}

.pins-input {
  margin-bottom: 2rem;
}

.roll-title {
  font-weight: 500;
  margin-bottom: 1rem;
  font-size: 1.125rem;
  color: #4b5563;
}

.pins-buttons {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
}

.pin-button {
  width: 3rem;
  height: 3rem;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #e0e7ff;
  border: none;
  font-size: 1.125rem;
  font-weight: 500;
  color: #4338ca;
  transition: all 0.2s;
}

.pin-button:hover {
  background-color: #c7d2fe;
}

.pin-button:focus {
  outline: none;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.4);
}

.error-message {
  background-color: #fee2e2;
  border-left: 4px solid #ef4444;
  color: #b91c1c;
  padding: 1rem;
  margin-bottom: 1.5rem;
  border-radius: 0 0.5rem 0.5rem 0;
}

.scoreboard-title {
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  color: #4338ca;
}

.table-container {
  overflow-x: auto;
}

.scoreboard-table {
  width: 100%;
  border-collapse: collapse;
}

.table-header {
  border: 1px solid #c7d2fe;
  padding: 0.75rem 1rem;
  background-color: #eef2ff;
  color: #4338ca;
  text-align: center;
}

.table-cell {
  border: 1px solid #c7d2fe;
  padding: 0.75rem 1rem;
  text-align: center;
}

.frame-number {
  font-weight: 500;
}

.current-frame {
  background-color: #eef2ff;
}

.strike {
  color: #dc2626;
  font-weight: 600;
}

.spare {
  color: #16a34a;
  font-weight: 600;
}

.frame-score {
  font-weight: 500;
}

.total-score {
  font-weight: 700;
  font-size: 1.125rem;
  color: #4338ca;
}
</style>