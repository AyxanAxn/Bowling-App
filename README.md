# Bowling Game Score Calculator

Welcome to the Bowling Game Score Calculator! This project calculates and displays the score of a bowling game for one player based on official bowling rules. Whether you're a seasoned pro or a weekend warrior, this app brings the fun of the bowling alley straight to your screen.

## Overview

A bowling game consists of 10 frames where a player attempts to knock down pins arranged at the end of a bowling lane. Here's a quick rundown of the rules:

- **Frames & Throws:**
  - A game has 10 frames.
  - In frames 1–9, each frame allows up to 2 throws.
  - In the 10th frame, you can have up to 3 throws if you score a strike or spare.
- **Scoring:**
  - **Open Frame:** If you don’t knock down all 10 pins, your score for that frame is the sum of the pins knocked down.
  - **Spare:** If you knock down all 10 pins in 2 throws, your score is 10 plus the pins knocked down on your next throw.
  - **Strike:** If you knock down all 10 pins in the first throw, your score is 10 plus the total pins knocked down on your next 2 throws.
- **Perfect Game:** A perfect game, where you score 12 consecutive strikes, totals 300 points—the maximum possible!

Each frame displays the number of pins knocked down per throw and the running total (cumulative score).





## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) (v14 or higher)
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (or higher)
- npm

### Frontend (Vue.js) Setup

1. Open your terminal and navigate to the `BowlingApp` folder.
2. Install the dependencies:
   ```bash
   npm install
