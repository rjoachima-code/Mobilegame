// JACAMENO - Web Version
// Tetris + M2 Block Merge Mechanics

class Game {
    constructor() {
        this.canvas = document.getElementById('game-canvas');
        this.ctx = this.canvas.getContext('2d');
        this.nextCanvas = document.getElementById('next-canvas');
        this.nextCtx = this.nextCanvas.getContext('2d');
        
        this.cols = 10;
        this.rows = 20;
        this.blockSize = 40;
        
        this.canvas.width = this.cols * this.blockSize;
        this.canvas.height = this.rows * this.blockSize;
        
        this.grid = [];
        this.currentPiece = null;
        this.nextPiece = null;
        this.score = 0;
        this.level = 1;
        this.lines = 0;
        this.gameLoop = null;
        this.dropInterval = 1000;
        this.lastDropTime = 0;
        this.isPaused = false;
        this.isGameOver = false;
        this.highScore = this.loadHighScore();
        
        this.initGrid();
        this.initControls();
        this.initTetrominoes();
        this.initSounds();
    }
    
    initSounds() {
        // Simple sound effects using Web Audio API
        this.audioContext = null;
        this.soundsEnabled = true;
        
        // Try to create audio context (will be initialized on first user interaction)
        try {
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            this.audioContext = new AudioContext();
        } catch (e) {
            console.log('Web Audio API not supported');
            this.soundsEnabled = false;
        }
    }
    
    playSound(type) {
        if (!this.soundsEnabled || !this.audioContext) return;
        
        const ctx = this.audioContext;
        const oscillator = ctx.createOscillator();
        const gainNode = ctx.createGain();
        
        oscillator.connect(gainNode);
        gainNode.connect(ctx.destination);
        
        switch(type) {
            case 'move':
                oscillator.frequency.value = 200;
                gainNode.gain.value = 0.1;
                oscillator.start();
                oscillator.stop(ctx.currentTime + 0.05);
                break;
            case 'rotate':
                oscillator.frequency.value = 300;
                gainNode.gain.value = 0.1;
                oscillator.start();
                oscillator.stop(ctx.currentTime + 0.05);
                break;
            case 'drop':
                oscillator.frequency.value = 100;
                gainNode.gain.value = 0.15;
                oscillator.start();
                oscillator.stop(ctx.currentTime + 0.1);
                break;
            case 'merge':
                oscillator.frequency.value = 400;
                gainNode.gain.value = 0.15;
                oscillator.start();
                oscillator.stop(ctx.currentTime + 0.15);
                break;
            case 'line':
                oscillator.frequency.value = 600;
                gainNode.gain.value = 0.2;
                oscillator.start();
                oscillator.stop(ctx.currentTime + 0.2);
                break;
            case 'gameover':
                oscillator.frequency.value = 150;
                oscillator.type = 'sawtooth';
                gainNode.gain.value = 0.2;
                oscillator.start();
                oscillator.stop(ctx.currentTime + 0.5);
                break;
        }
    }
    
    loadHighScore() {
        try {
            const saved = localStorage.getItem('jacameno_highscore');
            return saved ? parseInt(saved) : 0;
        } catch (e) {
            console.log('localStorage not available');
            return 0;
        }
    }
    
    saveHighScore() {
        try {
            if (this.score > this.highScore) {
                this.highScore = this.score;
                localStorage.setItem('jacameno_highscore', this.highScore.toString());
                return true;
            }
        } catch (e) {
            console.log('Could not save high score');
        }
        return false;
    }
    
    initGrid() {
        for (let row = 0; row < this.rows; row++) {
            this.grid[row] = [];
            for (let col = 0; col < this.cols; col++) {
                this.grid[row][col] = { value: 0, color: null };
            }
        }
    }
    
    initTetrominoes() {
        this.tetrominoes = {
            'I': {
                shape: [[1,1,1,1]],
                color: '#00f0f0',
                value: 2
            },
            'O': {
                shape: [[1,1],[1,1]],
                color: '#f0f000',
                value: 2
            },
            'T': {
                shape: [[0,1,0],[1,1,1]],
                color: '#a000f0',
                value: 2
            },
            'L': {
                shape: [[0,0,1],[1,1,1]],
                color: '#f0a000',
                value: 2
            },
            'J': {
                shape: [[1,0,0],[1,1,1]],
                color: '#0000f0',
                value: 2
            },
            'S': {
                shape: [[0,1,1],[1,1,0]],
                color: '#00f000',
                value: 2
            },
            'Z': {
                shape: [[1,1,0],[0,1,1]],
                color: '#f00000',
                value: 2
            }
        };
        
        this.tetrominoKeys = Object.keys(this.tetrominoes);
    }
    
    initControls() {
        // Touch gesture thresholds
        const TOUCH_TAP_THRESHOLD = 30; // pixels
        
        // Keyboard controls
        document.addEventListener('keydown', (e) => {
            if (this.isGameOver || this.isPaused) {
                if (e.key === 'Escape' || e.key === 'p' || e.key === 'P') {
                    if (this.isPaused) this.resume();
                }
                return;
            }
            
            switch(e.key) {
                case 'ArrowLeft':
                case 'a':
                case 'A':
                    this.movePiece(-1, 0);
                    e.preventDefault();
                    break;
                case 'ArrowRight':
                case 'd':
                case 'D':
                    this.movePiece(1, 0);
                    e.preventDefault();
                    break;
                case 'ArrowDown':
                case 's':
                case 'S':
                    this.movePiece(0, 1);
                    e.preventDefault();
                    break;
                case 'ArrowUp':
                case 'w':
                case 'W':
                    this.rotatePiece();
                    e.preventDefault();
                    break;
                case ' ':
                    this.hardDrop();
                    e.preventDefault();
                    break;
                case 'Escape':
                case 'p':
                case 'P':
                    this.pause();
                    e.preventDefault();
                    break;
            }
        });
        
        // Touch controls
        let touchStartX = 0;
        let touchStartY = 0;
        
        this.canvas.addEventListener('touchstart', (e) => {
            touchStartX = e.touches[0].clientX;
            touchStartY = e.touches[0].clientY;
            e.preventDefault();
        });
        
        this.canvas.addEventListener('touchend', (e) => {
            if (this.isGameOver || this.isPaused) return;
            
            const touchEndX = e.changedTouches[0].clientX;
            const touchEndY = e.changedTouches[0].clientY;
            
            const dx = touchEndX - touchStartX;
            const dy = touchEndY - touchStartY;
            
            if (Math.abs(dx) < TOUCH_TAP_THRESHOLD && Math.abs(dy) < TOUCH_TAP_THRESHOLD) {
                // Tap - rotate
                this.rotatePiece();
            } else if (Math.abs(dx) > Math.abs(dy)) {
                // Horizontal swipe
                if (dx > 0) {
                    this.movePiece(1, 0);
                } else {
                    this.movePiece(-1, 0);
                }
            } else {
                // Vertical swipe
                if (dy > 0) {
                    this.hardDrop();
                }
            }
            e.preventDefault();
        });
    }
    
    spawnPiece() {
        if (!this.nextPiece) {
            this.nextPiece = this.createRandomPiece();
        }
        
        this.currentPiece = this.nextPiece;
        this.nextPiece = this.createRandomPiece();
        
        this.currentPiece.x = Math.floor(this.cols / 2) - Math.floor(this.currentPiece.shape[0].length / 2);
        this.currentPiece.y = 0;
        
        if (this.checkCollision(this.currentPiece, this.currentPiece.x, this.currentPiece.y)) {
            this.gameOver();
        }
        
        this.drawNextPiece();
    }
    
    createRandomPiece() {
        const key = this.tetrominoKeys[Math.floor(Math.random() * this.tetrominoKeys.length)];
        const tetromino = this.tetrominoes[key];
        return {
            shape: tetromino.shape.map(row => [...row]),
            color: tetromino.color,
            value: tetromino.value,
            x: 0,
            y: 0
        };
    }
    
    movePiece(dx, dy) {
        if (!this.currentPiece) return;
        
        const newX = this.currentPiece.x + dx;
        const newY = this.currentPiece.y + dy;
        
        if (!this.checkCollision(this.currentPiece, newX, newY)) {
            this.currentPiece.x = newX;
            this.currentPiece.y = newY;
            if (dx !== 0) this.playSound('move');
            return true;
        }
        
        if (dy > 0) {
            this.lockPiece();
        }
        
        return false;
    }
    
    rotateMatrix(matrix) {
        // Rotate matrix 90 degrees clockwise
        // Transpose and reverse each row
        return matrix[0].map((_, i) =>
            matrix.map(row => row[i]).reverse()
        );
    }
    
    rotatePiece() {
        if (!this.currentPiece) return;
        
        const rotated = this.rotateMatrix(this.currentPiece.shape);
        
        const originalShape = this.currentPiece.shape;
        this.currentPiece.shape = rotated;
        
        if (this.checkCollision(this.currentPiece, this.currentPiece.x, this.currentPiece.y)) {
            this.currentPiece.shape = originalShape;
        } else {
            this.playSound('rotate');
        }
    }
    
    hardDrop() {
        if (!this.currentPiece) return;
        
        while (this.movePiece(0, 1)) {
            this.score += 2;
        }
        this.playSound('drop');
    }
    
    checkCollision(piece, x, y) {
        for (let row = 0; row < piece.shape.length; row++) {
            for (let col = 0; col < piece.shape[row].length; col++) {
                if (piece.shape[row][col]) {
                    const newX = x + col;
                    const newY = y + row;
                    
                    if (newX < 0 || newX >= this.cols || newY >= this.rows) {
                        return true;
                    }
                    
                    if (newY >= 0 && this.grid[newY][newX].value !== 0) {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    lockPiece() {
        if (!this.currentPiece) return;
        
        for (let row = 0; row < this.currentPiece.shape.length; row++) {
            for (let col = 0; col < this.currentPiece.shape[row].length; col++) {
                if (this.currentPiece.shape[row][col]) {
                    const x = this.currentPiece.x + col;
                    const y = this.currentPiece.y + row;
                    
                    if (y >= 0) {
                        this.grid[y][x] = {
                            value: this.currentPiece.value,
                            color: this.currentPiece.color
                        };
                    }
                }
            }
        }
        
        this.checkMerges();
        this.clearLines();
        this.spawnPiece();
    }
    
    checkMerges() {
        const MAX_MERGE_ITERATIONS = 10; // Prevent infinite loops
        let mergeOccurred = true;
        let iterations = 0;
        
        while (mergeOccurred && iterations < MAX_MERGE_ITERATIONS) {
            mergeOccurred = false;
            iterations++;
            
            // Check horizontal merges
            for (let row = 0; row < this.rows; row++) {
                for (let col = 0; col < this.cols - 1; col++) {
                    if (this.grid[row][col].value > 0 && 
                        this.grid[row][col].value === this.grid[row][col + 1].value) {
                        this.grid[row][col].value *= 2;
                        this.grid[row][col + 1].value = 0;
                        this.grid[row][col + 1].color = null;
                        this.score += this.grid[row][col].value;
                        mergeOccurred = true;
                        this.playSound('merge');
                    }
                }
            }
            
            // Check vertical merges
            for (let row = 0; row < this.rows - 1; row++) {
                for (let col = 0; col < this.cols; col++) {
                    if (this.grid[row][col].value > 0 && 
                        this.grid[row][col].value === this.grid[row + 1][col].value) {
                        this.grid[row][col].value *= 2;
                        this.grid[row + 1][col].value = 0;
                        this.grid[row + 1][col].color = null;
                        this.score += this.grid[row][col].value;
                        mergeOccurred = true;
                        this.playSound('merge');
                    }
                }
            }
        }
    }
    
    clearLines() {
        let linesCleared = 0;
        
        for (let row = this.rows - 1; row >= 0; row--) {
            if (this.grid[row].every(cell => cell.value !== 0)) {
                this.grid.splice(row, 1);
                this.grid.unshift(Array(this.cols).fill(0).map(() => ({ value: 0, color: null })));
                linesCleared++;
                row++;
            }
        }
        
        if (linesCleared > 0) {
            this.lines += linesCleared;
            this.score += linesCleared * 100 * this.level;
            this.level = Math.floor(this.lines / 10) + 1;
            this.dropInterval = Math.max(100, 1000 - (this.level - 1) * 50);
            this.playSound('line');
        }
    }
    
    update(timestamp) {
        if (this.isPaused || this.isGameOver) return;
        
        if (!this.currentPiece) {
            this.spawnPiece();
        }
        
        if (timestamp - this.lastDropTime > this.dropInterval) {
            this.movePiece(0, 1);
            this.lastDropTime = timestamp;
        }
    }
    
    draw() {
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.8)';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw grid
        this.ctx.strokeStyle = 'rgba(255, 255, 255, 0.1)';
        for (let row = 0; row <= this.rows; row++) {
            this.ctx.beginPath();
            this.ctx.moveTo(0, row * this.blockSize);
            this.ctx.lineTo(this.canvas.width, row * this.blockSize);
            this.ctx.stroke();
        }
        for (let col = 0; col <= this.cols; col++) {
            this.ctx.beginPath();
            this.ctx.moveTo(col * this.blockSize, 0);
            this.ctx.lineTo(col * this.blockSize, this.canvas.height);
            this.ctx.stroke();
        }
        
        // Draw locked blocks
        for (let row = 0; row < this.rows; row++) {
            for (let col = 0; col < this.cols; col++) {
                if (this.grid[row][col].value > 0) {
                    this.drawBlock(col, row, this.grid[row][col].color, this.grid[row][col].value);
                }
            }
        }
        
        // Draw current piece
        if (this.currentPiece) {
            for (let row = 0; row < this.currentPiece.shape.length; row++) {
                for (let col = 0; col < this.currentPiece.shape[row].length; col++) {
                    if (this.currentPiece.shape[row][col]) {
                        this.drawBlock(
                            this.currentPiece.x + col,
                            this.currentPiece.y + row,
                            this.currentPiece.color,
                            this.currentPiece.value
                        );
                    }
                }
            }
        }
    }
    
    drawBlock(x, y, color, value) {
        const px = x * this.blockSize;
        const py = y * this.blockSize;
        
        this.ctx.fillStyle = color;
        this.ctx.fillRect(px + 2, py + 2, this.blockSize - 4, this.blockSize - 4);
        
        this.ctx.strokeStyle = 'rgba(255, 255, 255, 0.3)';
        this.ctx.lineWidth = 2;
        this.ctx.strokeRect(px + 2, py + 2, this.blockSize - 4, this.blockSize - 4);
        
        if (value) {
            this.ctx.fillStyle = 'white';
            this.ctx.font = 'bold 16px Arial';
            this.ctx.textAlign = 'center';
            this.ctx.textBaseline = 'middle';
            this.ctx.fillText(value, px + this.blockSize / 2, py + this.blockSize / 2);
        }
    }
    
    drawNextPiece() {
        this.nextCtx.fillStyle = 'rgba(0, 0, 0, 0.8)';
        this.nextCtx.fillRect(0, 0, this.nextCanvas.width, this.nextCanvas.height);
        
        if (!this.nextPiece) return;
        
        const blockSize = 24;
        const offsetX = (this.nextCanvas.width - this.nextPiece.shape[0].length * blockSize) / 2;
        const offsetY = (this.nextCanvas.height - this.nextPiece.shape.length * blockSize) / 2;
        
        for (let row = 0; row < this.nextPiece.shape.length; row++) {
            for (let col = 0; col < this.nextPiece.shape[row].length; col++) {
                if (this.nextPiece.shape[row][col]) {
                    const px = offsetX + col * blockSize;
                    const py = offsetY + row * blockSize;
                    
                    this.nextCtx.fillStyle = this.nextPiece.color;
                    this.nextCtx.fillRect(px + 2, py + 2, blockSize - 4, blockSize - 4);
                }
            }
        }
    }
    
    updateUI() {
        document.getElementById('score').textContent = this.score;
        document.getElementById('level').textContent = this.level;
        document.getElementById('lines').textContent = this.lines;
        document.getElementById('high-score').textContent = this.highScore;
    }
    
    start() {
        this.initGrid();
        this.score = 0;
        this.level = 1;
        this.lines = 0;
        this.isPaused = false;
        this.isGameOver = false;
        this.currentPiece = null;
        this.nextPiece = null;
        this.lastDropTime = 0;
        this.dropInterval = 1000;
        
        this.updateUI();
        this.spawnPiece();
        
        const gameLoop = (timestamp) => {
            this.update(timestamp);
            this.draw();
            this.updateUI();
            
            if (!this.isGameOver) {
                this.gameLoop = requestAnimationFrame(gameLoop);
            }
        };
        
        this.gameLoop = requestAnimationFrame(gameLoop);
    }
    
    pause() {
        this.isPaused = true;
        showScreen('pause-screen');
    }
    
    resume() {
        this.isPaused = false;
        this.lastDropTime = performance.now();
        showScreen('game-screen');
    }
    
    gameOver() {
        this.isGameOver = true;
        const isNewHighScore = this.saveHighScore();
        document.getElementById('final-score').textContent = this.score;
        if (isNewHighScore) {
            document.getElementById('new-high-score').style.display = 'block';
        } else {
            document.getElementById('new-high-score').style.display = 'none';
        }
        this.playSound('gameover');
        showScreen('gameover-screen');
    }
}

// Screen management
function showScreen(screenId) {
    document.querySelectorAll('.screen').forEach(screen => {
        screen.classList.remove('active');
    });
    document.getElementById(screenId).classList.add('active');
}

// Initialize game
let game = null;

document.getElementById('start-btn').addEventListener('click', () => {
    if (!game) {
        game = new Game();
    }
    game.start();
    showScreen('game-screen');
});

document.getElementById('pause-btn').addEventListener('click', () => {
    if (game) game.pause();
});

document.getElementById('resume-btn').addEventListener('click', () => {
    if (game) game.resume();
});

document.getElementById('restart-btn').addEventListener('click', () => {
    if (game) {
        game.start();
        showScreen('game-screen');
    }
});

document.getElementById('menu-btn').addEventListener('click', () => {
    showScreen('menu-screen');
});

document.getElementById('play-again-btn').addEventListener('click', () => {
    if (game) {
        game.start();
        showScreen('game-screen');
    }
});

document.getElementById('menu-btn-2').addEventListener('click', () => {
    showScreen('menu-screen');
});
