/**
 * Battle Merge - Web Implementation
 * M2 Block merge mechanics + Lane Defense
 */

class BattleMergeGame {
    constructor() {
        // Constants
        this.GRID_COLS = 5;
        this.GRID_ROWS = 8;
        this.CELL_SIZE = 120;
        this.GRID_OFFSET_X = 60;
        this.GRID_OFFSET_Y = 60;
        this.VIRUS_SPAWN_INTERVAL = 10000; // 10 seconds
        
        // Canvas
        this.canvas = document.getElementById('gameCanvas');
        this.ctx = this.canvas.getContext('2d');
        
        // Game state
        this.grid = [];
        this.currentBlock = 2;
        this.nextBlock = 2;
        this.score = 0;
        this.coins = 0;
        this.gameOver = false;
        this.virusTimer = 0;
        this.lastTime = Date.now();
        this.nextIsJoker = false;
        
        // Item costs
        this.economy = {
            swap: 50,
            sledgehammer: 100,
            joker: 250,
            nuke: 500
        };
        
        // Colors (Neon Cyberpunk)
        this.blockColors = {
            2: '#00ffff',      // Cyan
            4: '#ff00ff',      // Magenta
            8: '#ffff00',      // Yellow
            16: '#00ff00',     // Green
            32: '#ff6600',     // Orange
            64: '#9900ff',     // Purple
            128: '#ff0000',    // Red
            256: '#0099ff',    // Light Blue
            512: '#ffffff',    // White
            1024: '#ffd700',   // Gold
            2048: '#66ff66'    // Light Green
        };
        this.virusColor = '#cc0000'; // Dark Red
        
        // Item mode
        this.itemMode = null; // 'hammer' or 'nuke'
        
        // Initialize
        this.init();
    }
    
    init() {
        // Initialize grid
        for (let col = 0; col < this.GRID_COLS; col++) {
            this.grid[col] = [];
            for (let row = 0; row < this.GRID_ROWS; row++) {
                this.grid[col][row] = 0;
            }
        }
        
        // Generate initial blocks
        this.generateNextBlock();
        this.currentBlock = this.nextBlock;
        this.generateNextBlock();
        
        // Setup input
        this.canvas.addEventListener('click', (e) => this.handleClick(e));
        this.canvas.addEventListener('touchstart', (e) => {
            e.preventDefault();
            const touch = e.touches[0];
            this.handleClick(touch);
        });
        
        // Start game loop
        this.updateUI();
        this.draw();
        this.gameLoop();
        
        console.log('Battle Merge initialized!');
    }
    
    gameLoop() {
        if (!this.gameOver) {
            const now = Date.now();
            const delta = now - this.lastTime;
            this.lastTime = now;
            
            // Update virus timer
            this.virusTimer += delta;
            if (this.virusTimer >= this.VIRUS_SPAWN_INTERVAL) {
                this.virusTimer = 0;
                this.spawnViruses();
            }
            
            this.draw();
        }
        
        requestAnimationFrame(() => this.gameLoop());
    }
    
    handleClick(e) {
        if (this.gameOver) return;
        
        const rect = this.canvas.getBoundingClientRect();
        const x = (e.clientX || e.pageX) - rect.left;
        const y = (e.clientY || e.pageY) - rect.top;
        
        // Scale for canvas size
        const scaleX = this.canvas.width / rect.width;
        const scaleY = this.canvas.height / rect.height;
        const canvasX = x * scaleX;
        const canvasY = y * scaleY;
        
        // Convert to grid coordinates
        const gridX = canvasX - this.GRID_OFFSET_X;
        const gridY = canvasY - this.GRID_OFFSET_Y;
        
        if (gridX < 0 || gridY < 0) return;
        
        const col = Math.floor(gridX / this.CELL_SIZE);
        const row = Math.floor(gridY / this.CELL_SIZE);
        
        if (col < 0 || col >= this.GRID_COLS) return;
        
        // Handle item modes
        if (this.itemMode === 'hammer') {
            if (row >= 0 && row < this.GRID_ROWS) {
                this.applySledgehammer(col, row);
            }
            this.itemMode = null;
            this.canvas.style.cursor = 'pointer';
            return;
        }
        
        if (this.itemMode === 'nuke') {
            this.applyNuke(col);
            this.itemMode = null;
            this.canvas.style.cursor = 'pointer';
            return;
        }
        
        // Normal block placement
        this.handleColumnTap(col);
    }
    
    // SECTION 1: M2 BLOCK MECHANIC
    
    handleColumnTap(col) {
        // Find lowest empty spot
        const targetRow = this.findLowestEmpty(col);
        
        if (targetRow < 0) {
            this.triggerGameOver('Column is full!');
            return;
        }
        
        // Place block
        let valueToPlace = this.currentBlock;
        if (this.nextIsJoker) {
            // Joker matches the block it lands on
            if (targetRow < this.GRID_ROWS - 1 && this.grid[col][targetRow + 1] > 0) {
                valueToPlace = this.grid[col][targetRow + 1];
            }
            this.nextIsJoker = false;
        }
        
        this.grid[col][targetRow] = valueToPlace;
        
        // Check for merge
        this.checkMerge(col, targetRow);
        
        // Apply gravity
        this.applyGravity(col);
        
        // Move to next block
        this.currentBlock = this.nextBlock;
        this.generateNextBlock();
        
        // Virus mechanics
        this.descendViruses();
        
        this.updateUI();
        this.draw();
    }
    
    findLowestEmpty(col) {
        for (let row = this.GRID_ROWS - 1; row >= 0; row--) {
            if (this.grid[col][row] === 0) {
                return row;
            }
        }
        return -1;
    }
    
    checkMerge(col, row) {
        if (row >= this.GRID_ROWS - 1) return;
        
        const current = this.grid[col][row];
        const below = this.grid[col][row + 1];
        
        if (current === below && current > 0 && below > 0) {
            // Merge
            const newValue = below * 2;
            this.grid[col][row + 1] = newValue;
            this.grid[col][row] = 0;
            
            // Coin payout
            this.calculateCoinPayout(newValue, false);
            
            // Recursive merge
            this.checkMerge(col, row + 1);
        }
    }
    
    applyGravity(col) {
        let changed = true;
        while (changed) {
            changed = false;
            for (let row = this.GRID_ROWS - 1; row > 0; row--) {
                if (this.grid[col][row] === 0 && this.grid[col][row - 1] !== 0) {
                    this.grid[col][row] = this.grid[col][row - 1];
                    this.grid[col][row - 1] = 0;
                    changed = true;
                }
            }
        }
    }
    
    // SECTION 2: DEFENSE LAYER
    
    spawnViruses() {
        const numViruses = Math.floor(Math.random() * 2) + 1; // 1-2 viruses
        
        for (let i = 0; i < numViruses; i++) {
            const col = Math.floor(Math.random() * this.GRID_COLS);
            
            if (this.grid[col][0] === 0) {
                const virusValue = Math.random() < 0.5 ? 2 : 4;
                this.grid[col][0] = -virusValue; // Negative for virus
                console.log('Virus spawned:', col, virusValue);
            }
        }
        
        this.draw();
    }
    
    descendViruses() {
        // Process from bottom to top
        for (let col = 0; col < this.GRID_COLS; col++) {
            for (let row = this.GRID_ROWS - 1; row >= 0; row--) {
                if (this.grid[col][row] < 0) { // Virus
                    const virusValue = Math.abs(this.grid[col][row]);
                    
                    // Check if at bottom
                    if (row === this.GRID_ROWS - 1) {
                        this.triggerGameOver('Virus reached the bottom!');
                        return;
                    }
                    
                    const below = this.grid[col][row + 1];
                    
                    if (below === 0) {
                        // Move down
                        this.grid[col][row + 1] = this.grid[col][row];
                        this.grid[col][row] = 0;
                    } else if (below > 0) {
                        // Defense interaction
                        this.defenseInteraction(col, row + 1, below, virusValue);
                        this.grid[col][row] = 0;
                    }
                }
            }
        }
    }
    
    defenseInteraction(col, row, blockValue, virusValue) {
        if (blockValue >= virusValue) {
            // Block wins
            console.log('Block defends!', blockValue, '>=', virusValue);
        } else {
            // Both destroyed
            console.log('Both destroyed!', virusValue, '>', blockValue);
            this.grid[col][row] = 0;
        }
    }
    
    // SECTION 3: ECONOMY
    
    calculateCoinPayout(newValue, isChain) {
        const n = Math.log2(newValue);
        let payout = Math.ceil(n / 3) * 2;
        
        if (isChain) {
            payout *= 2;
        }
        
        this.coins += payout;
        this.score += newValue;
    }
    
    useSwap() {
        if (this.coins < this.economy.swap) {
            this.showNotification('Not enough coins!', '#ff0000');
            return;
        }
        
        this.coins -= this.economy.swap;
        const temp = this.currentBlock;
        this.currentBlock = this.nextBlock;
        this.nextBlock = temp;
        this.updateUI();
        this.showNotification('Blocks swapped!', '#00ffff');
        console.log('Swapped blocks');
    }
    
    useSledgehammer() {
        if (this.coins < this.economy.sledgehammer) {
            this.showNotification('Not enough coins!', '#ff0000');
            return;
        }
        
        this.coins -= this.economy.sledgehammer;
        this.itemMode = 'hammer';
        this.canvas.style.cursor = 'crosshair';
        this.updateUI();
        this.showNotification('Click a block to remove it', '#ff6600');
    }
    
    applySledgehammer(col, row) {
        if (this.grid[col][row] === 0) {
            this.coins += this.economy.sledgehammer; // Refund
            this.showNotification('No block at that position', '#ff0000');
            return;
        }
        
        this.grid[col][row] = 0;
        this.applyGravity(col);
        this.updateUI();
        this.draw();
        this.showNotification('Block destroyed!', '#ff6600');
        console.log('Sledgehammer used at', col, row);
    }
    
    useJoker() {
        if (this.coins < this.economy.joker) {
            this.showNotification('Not enough coins!', '#ff0000');
            return;
        }
        
        this.coins -= this.economy.joker;
        this.nextIsJoker = true;
        this.updateUI();
        this.showNotification('Next block is a Joker!', '#ff00ff');
        console.log('Next block is a Joker');
    }
    
    useNuke() {
        if (this.coins < this.economy.nuke) {
            this.showNotification('Not enough coins!', '#ff0000');
            return;
        }
        
        this.coins -= this.economy.nuke;
        this.itemMode = 'nuke';
        this.canvas.style.cursor = 'crosshair';
        this.updateUI();
        this.showNotification('Click a column to nuke it', '#ff0000');
    }
    
    applyNuke(col) {
        for (let row = 0; row < this.GRID_ROWS; row++) {
            this.grid[col][row] = 0;
        }
        this.updateUI();
        this.draw();
        console.log('Column', col, 'nuked');
    }
    
    // HELPERS
    
    generateNextBlock() {
        const rand = Math.random();
        if (rand < 0.6) {
            this.nextBlock = 2;
        } else if (rand < 0.9) {
            this.nextBlock = 4;
        } else {
            this.nextBlock = 8;
        }
    }
    
    triggerGameOver(reason) {
        this.gameOver = true;
        console.log('GAME OVER:', reason);
        
        document.getElementById('finalScore').textContent = `Final Score: ${this.score}`;
        document.getElementById('gameOverReason').textContent = reason;
        document.getElementById('gameOver').style.display = 'block';
    }
    
    restart() {
        // Clear grid
        for (let col = 0; col < this.GRID_COLS; col++) {
            for (let row = 0; row < this.GRID_ROWS; row++) {
                this.grid[col][row] = 0;
            }
        }
        
        // Reset state
        this.score = 0;
        this.coins = 0;
        this.gameOver = false;
        this.virusTimer = 0;
        this.nextIsJoker = false;
        this.itemMode = null;
        
        this.generateNextBlock();
        this.currentBlock = this.nextBlock;
        this.generateNextBlock();
        
        document.getElementById('gameOver').style.display = 'none';
        this.canvas.style.cursor = 'pointer';
        
        this.updateUI();
        this.draw();
        console.log('Game restarted');
    }
    
    showNotification(message, color) {
        // Create notification element if it doesn't exist
        let notification = document.getElementById('notification');
        if (!notification) {
            notification = document.createElement('div');
            notification.id = 'notification';
            notification.style.cssText = `
                position: fixed;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
                background: rgba(10, 10, 30, 0.95);
                border: 2px solid ${color};
                border-radius: 10px;
                padding: 20px 40px;
                font-size: 20px;
                font-weight: bold;
                color: ${color};
                text-shadow: 0 0 10px ${color};
                box-shadow: 0 0 20px ${color};
                z-index: 2000;
                pointer-events: none;
                opacity: 0;
                transition: opacity 0.3s;
            `;
            document.body.appendChild(notification);
        }
        
        // Update and show notification
        notification.textContent = message;
        notification.style.borderColor = color;
        notification.style.color = color;
        notification.style.textShadow = `0 0 10px ${color}`;
        notification.style.boxShadow = `0 0 20px ${color}`;
        notification.style.opacity = '1';
        
        // Hide after 2 seconds
        setTimeout(() => {
            notification.style.opacity = '0';
        }, 2000);
    }
    
    updateUI() {
        document.getElementById('score').textContent = this.score;
        document.getElementById('coins').textContent = this.coins;
        document.getElementById('current').textContent = this.nextIsJoker ? 'JOKER' : this.currentBlock;
        document.getElementById('next').textContent = this.nextBlock;
        
        // Update button states
        document.getElementById('swapBtn').disabled = this.coins < this.economy.swap;
        document.getElementById('hammerBtn').disabled = this.coins < this.economy.sledgehammer;
        document.getElementById('jokerBtn').disabled = this.coins < this.economy.joker;
        document.getElementById('nukeBtn').disabled = this.coins < this.economy.nuke;
    }
    
    // RENDERING
    
    draw() {
        // Clear canvas
        this.ctx.fillStyle = '#0a0a1a';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Draw grid lines
        this.ctx.strokeStyle = 'rgba(100, 100, 255, 0.3)';
        this.ctx.lineWidth = 1;
        
        for (let col = 0; col <= this.GRID_COLS; col++) {
            const x = this.GRID_OFFSET_X + col * this.CELL_SIZE;
            this.ctx.beginPath();
            this.ctx.moveTo(x, this.GRID_OFFSET_Y);
            this.ctx.lineTo(x, this.GRID_OFFSET_Y + this.GRID_ROWS * this.CELL_SIZE);
            this.ctx.stroke();
        }
        
        for (let row = 0; row <= this.GRID_ROWS; row++) {
            const y = this.GRID_OFFSET_Y + row * this.CELL_SIZE;
            this.ctx.beginPath();
            this.ctx.moveTo(this.GRID_OFFSET_X, y);
            this.ctx.lineTo(this.GRID_OFFSET_X + this.GRID_COLS * this.CELL_SIZE, y);
            this.ctx.stroke();
        }
        
        // Draw blocks
        for (let col = 0; col < this.GRID_COLS; col++) {
            for (let row = 0; row < this.GRID_ROWS; row++) {
                const value = this.grid[col][row];
                if (value !== 0) {
                    this.drawBlock(col, row, value);
                }
            }
        }
    }
    
    drawBlock(col, row, value) {
        const x = this.GRID_OFFSET_X + col * this.CELL_SIZE;
        const y = this.GRID_OFFSET_Y + row * this.CELL_SIZE;
        const padding = 5;
        
        // Negative values represent virus blocks
        const isVirus = value < 0;
        const absValue = Math.abs(value);
        
        // Get color
        let color = this.blockColors[absValue] || '#666666';
        if (isVirus) {
            color = this.virusColor;
        }
        
        // Draw block with glow
        this.ctx.shadowBlur = 20;
        this.ctx.shadowColor = color;
        this.ctx.fillStyle = color;
        this.ctx.fillRect(x + padding, y + padding, 
                         this.CELL_SIZE - padding * 2, 
                         this.CELL_SIZE - padding * 2);
        this.ctx.shadowBlur = 0;
        
        // Draw value
        this.ctx.fillStyle = '#ffffff';
        this.ctx.font = 'bold 32px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        this.ctx.fillText(absValue.toString(), 
                         x + this.CELL_SIZE / 2, 
                         y + this.CELL_SIZE / 2);
    }
}

// Initialize game
let game;
window.addEventListener('load', () => {
    game = new BattleMergeGame();
});
