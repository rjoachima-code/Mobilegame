// JACAMENO - Geometric Sprite Renderer
// Cyberpunk aesthetic with neon glow effects

class GeometricRenderer {
    constructor(ctx) {
        this.ctx = ctx;
        this.shapes = {
            triangle: this.createTriangle.bind(this),
            square: this.createSquare.bind(this),
            pentagon: this.createPentagon.bind(this),
            hexagon: this.createHexagon.bind(this),
            circle: this.createCircle.bind(this)
        };
        
        // Shape properties per value tier
        this.shapeConfigs = {
            2: { 
                shape: 'triangle', 
                color: '#00d4ff', // Neon blue
                glowColor: 'rgba(0, 212, 255, 0.8)',
                effect: 'glass'
            },
            4: { 
                shape: 'square', 
                color: '#00ff88', // Neon green
                glowColor: 'rgba(0, 255, 136, 0.8)',
                effect: 'matte'
            },
            8: { 
                shape: 'pentagon', 
                color: '#ff00ff', // Neon magenta
                glowColor: 'rgba(255, 0, 255, 0.8)',
                effect: 'crystalline'
            },
            16: { 
                shape: 'hexagon', 
                color: '#ff8800', // Neon orange
                glowColor: 'rgba(255, 136, 0, 0.8)',
                effect: 'honeycomb'
            },
            32: { 
                shape: 'circle', 
                color: '#ffffff', // Bright white
                glowColor: 'rgba(255, 255, 255, 1.0)',
                effect: 'radiant'
            }
        };
    }
    
    // Get shape configuration based on block value
    getShapeConfig(value) {
        // For values higher than 32, cycle through configurations
        const keys = [2, 4, 8, 16, 32];
        const baseValue = value <= 32 ? value : keys[Math.floor(Math.log2(value / 32)) % keys.length];
        return this.shapeConfigs[baseValue] || this.shapeConfigs[2];
    }
    
    // Main draw function
    drawShape(x, y, size, value, alpha = 1.0) {
        const config = this.getShapeConfig(value);
        const cx = x + size / 2;
        const cy = y + size / 2;
        const radius = size * 0.35; // Leave margin for glow
        
        this.ctx.save();
        this.ctx.globalAlpha = alpha;
        
        // Draw bloom/glow effect (multiple layers)
        this.drawBloomEffect(cx, cy, radius, config);
        
        // Draw the shape
        this.shapes[config.shape](cx, cy, radius, config);
        
        // Draw value text
        if (value) {
            this.drawValueText(cx, cy, value, config);
        }
        
        this.ctx.restore();
    }
    
    // Bloom effect - multiple gaussian blur layers
    drawBloomEffect(cx, cy, radius, config) {
        const layers = [
            { offset: 0, blur: 20, alpha: 0.4 },
            { offset: 0, blur: 10, alpha: 0.5 },
            { offset: 0, blur: 5, alpha: 0.6 }
        ];
        
        layers.forEach(layer => {
            this.ctx.save();
            this.ctx.shadowBlur = layer.blur;
            this.ctx.shadowColor = config.glowColor;
            this.ctx.globalAlpha = layer.alpha;
            
            // Draw a simple circle for the glow base
            this.ctx.beginPath();
            this.ctx.arc(cx, cy, radius * 1.1, 0, Math.PI * 2);
            this.ctx.fillStyle = config.color;
            this.ctx.fill();
            
            this.ctx.restore();
        });
    }
    
    // Triangle - Glowing neon blue, glass texture
    createTriangle(cx, cy, radius, config) {
        this.ctx.save();
        
        // Draw main triangle
        this.ctx.beginPath();
        for (let i = 0; i < 3; i++) {
            const angle = (i * Math.PI * 2) / 3 - Math.PI / 2;
            const x = cx + Math.cos(angle) * radius;
            const y = cy + Math.sin(angle) * radius;
            if (i === 0) {
                this.ctx.moveTo(x, y);
            } else {
                this.ctx.lineTo(x, y);
            }
        }
        this.ctx.closePath();
        
        // Glass effect - gradient fill
        const gradient = this.ctx.createLinearGradient(cx - radius, cy - radius, cx + radius, cy + radius);
        gradient.addColorStop(0, this.adjustAlpha(config.color, 0.6));
        gradient.addColorStop(0.5, this.adjustAlpha(config.color, 0.8));
        gradient.addColorStop(1, this.adjustAlpha(config.color, 0.4));
        this.ctx.fillStyle = gradient;
        this.ctx.fill();
        
        // Rounded corners effect with bright edges
        this.ctx.strokeStyle = config.color;
        this.ctx.lineWidth = 3;
        this.ctx.shadowBlur = 8;
        this.ctx.shadowColor = config.glowColor;
        this.ctx.stroke();
        
        this.ctx.restore();
    }
    
    // Square - Glowing neon green, matte finish with edge lighting
    createSquare(cx, cy, radius, config) {
        this.ctx.save();
        
        const size = radius * 1.4;
        
        // Draw main square
        this.ctx.beginPath();
        this.ctx.rect(cx - size / 2, cy - size / 2, size, size);
        
        // Matte finish - solid color
        this.ctx.fillStyle = config.color;
        this.ctx.fill();
        
        // Edge lighting - bright edges
        this.ctx.strokeStyle = '#ffffff';
        this.ctx.lineWidth = 2;
        this.ctx.shadowBlur = 12;
        this.ctx.shadowColor = config.glowColor;
        this.ctx.stroke();
        
        // Additional inner shadow for depth
        this.ctx.strokeStyle = this.adjustAlpha(config.color, 0.5);
        this.ctx.lineWidth = 1;
        this.ctx.shadowBlur = 0;
        this.ctx.strokeRect(cx - size / 2 + 2, cy - size / 2 + 2, size - 4, size - 4);
        
        this.ctx.restore();
    }
    
    // Pentagon - Glowing neon magenta, crystalline structure
    createPentagon(cx, cy, radius, config) {
        this.ctx.save();
        
        // Draw main pentagon
        this.ctx.beginPath();
        for (let i = 0; i < 5; i++) {
            const angle = (i * Math.PI * 2) / 5 - Math.PI / 2;
            const x = cx + Math.cos(angle) * radius;
            const y = cy + Math.sin(angle) * radius;
            if (i === 0) {
                this.ctx.moveTo(x, y);
            } else {
                this.ctx.lineTo(x, y);
            }
        }
        this.ctx.closePath();
        
        // Crystalline effect - radial gradient
        const gradient = this.ctx.createRadialGradient(cx, cy, 0, cx, cy, radius);
        gradient.addColorStop(0, '#ffffff');
        gradient.addColorStop(0.3, config.color);
        gradient.addColorStop(1, this.adjustAlpha(config.color, 0.5));
        this.ctx.fillStyle = gradient;
        this.ctx.fill();
        
        // Crystal facets
        this.ctx.strokeStyle = '#ffffff';
        this.ctx.lineWidth = 2;
        this.ctx.shadowBlur = 10;
        this.ctx.shadowColor = config.glowColor;
        this.ctx.stroke();
        
        // Inner crystal lines
        this.ctx.strokeStyle = this.adjustAlpha(config.color, 0.6);
        this.ctx.lineWidth = 1;
        this.ctx.shadowBlur = 5;
        for (let i = 0; i < 5; i++) {
            const angle = (i * Math.PI * 2) / 5 - Math.PI / 2;
            const x = cx + Math.cos(angle) * radius * 0.6;
            const y = cy + Math.sin(angle) * radius * 0.6;
            this.ctx.beginPath();
            this.ctx.moveTo(cx, cy);
            this.ctx.lineTo(x, y);
            this.ctx.stroke();
        }
        
        this.ctx.restore();
    }
    
    // Hexagon - Glowing neon orange, honeycomb pattern
    createHexagon(cx, cy, radius, config) {
        this.ctx.save();
        
        // Draw main hexagon
        this.ctx.beginPath();
        for (let i = 0; i < 6; i++) {
            const angle = (i * Math.PI * 2) / 6;
            const x = cx + Math.cos(angle) * radius;
            const y = cy + Math.sin(angle) * radius;
            if (i === 0) {
                this.ctx.moveTo(x, y);
            } else {
                this.ctx.lineTo(x, y);
            }
        }
        this.ctx.closePath();
        
        // Base fill
        this.ctx.fillStyle = config.color;
        this.ctx.fill();
        
        // Honeycomb pattern
        const cellRadius = radius * 0.3;
        this.ctx.strokeStyle = this.adjustAlpha(config.color, 0.4);
        this.ctx.lineWidth = 1.5;
        
        // Center cell
        this.drawHoneycombCell(cx, cy, cellRadius);
        
        // Surrounding cells (partial)
        for (let i = 0; i < 6; i++) {
            const angle = (i * Math.PI * 2) / 6;
            const cellX = cx + Math.cos(angle) * cellRadius * 1.7;
            const cellY = cy + Math.sin(angle) * cellRadius * 1.7;
            this.drawHoneycombCell(cellX, cellY, cellRadius);
        }
        
        // Outer glow border
        this.ctx.beginPath();
        for (let i = 0; i < 6; i++) {
            const angle = (i * Math.PI * 2) / 6;
            const x = cx + Math.cos(angle) * radius;
            const y = cy + Math.sin(angle) * radius;
            if (i === 0) {
                this.ctx.moveTo(x, y);
            } else {
                this.ctx.lineTo(x, y);
            }
        }
        this.ctx.closePath();
        this.ctx.strokeStyle = '#ffffff';
        this.ctx.lineWidth = 2;
        this.ctx.shadowBlur = 10;
        this.ctx.shadowColor = config.glowColor;
        this.ctx.stroke();
        
        this.ctx.restore();
    }
    
    // Helper for honeycomb cells
    drawHoneycombCell(cx, cy, radius) {
        this.ctx.beginPath();
        for (let i = 0; i < 6; i++) {
            const angle = (i * Math.PI * 2) / 6;
            const x = cx + Math.cos(angle) * radius;
            const y = cy + Math.sin(angle) * radius;
            if (i === 0) {
                this.ctx.moveTo(x, y);
            } else {
                this.ctx.lineTo(x, y);
            }
        }
        this.ctx.closePath();
        this.ctx.stroke();
    }
    
    // Circle - Bright white core, "god tier" radiant aura
    createCircle(cx, cy, radius, config) {
        this.ctx.save();
        
        // Multiple radiant layers
        const layers = [
            { r: radius * 1.3, alpha: 0.1 },
            { r: radius * 1.1, alpha: 0.2 },
            { r: radius * 0.9, alpha: 0.4 },
            { r: radius * 0.7, alpha: 0.6 },
            { r: radius * 0.5, alpha: 0.8 }
        ];
        
        // Draw radiant layers
        layers.forEach(layer => {
            const gradient = this.ctx.createRadialGradient(cx, cy, 0, cx, cy, layer.r);
            gradient.addColorStop(0, `rgba(255, 255, 255, ${layer.alpha})`);
            gradient.addColorStop(0.5, `rgba(255, 255, 255, ${layer.alpha * 0.5})`);
            gradient.addColorStop(1, `rgba(255, 255, 255, 0)`);
            
            this.ctx.fillStyle = gradient;
            this.ctx.beginPath();
            this.ctx.arc(cx, cy, layer.r, 0, Math.PI * 2);
            this.ctx.fill();
        });
        
        // Core bright white circle
        this.ctx.fillStyle = '#ffffff';
        this.ctx.shadowBlur = 20;
        this.ctx.shadowColor = 'rgba(255, 255, 255, 1)';
        this.ctx.beginPath();
        this.ctx.arc(cx, cy, radius * 0.4, 0, Math.PI * 2);
        this.ctx.fill();
        
        // Outer ring
        this.ctx.strokeStyle = '#ffffff';
        this.ctx.lineWidth = 3;
        this.ctx.shadowBlur = 15;
        this.ctx.beginPath();
        this.ctx.arc(cx, cy, radius * 0.8, 0, Math.PI * 2);
        this.ctx.stroke();
        
        this.ctx.restore();
    }
    
    // Draw value text on shape
    drawValueText(cx, cy, value, config) {
        this.ctx.save();
        
        // Text styling
        this.ctx.fillStyle = '#000000';
        this.ctx.font = 'bold 16px Arial, sans-serif';
        this.ctx.textAlign = 'center';
        this.ctx.textBaseline = 'middle';
        
        // Text glow for visibility
        this.ctx.shadowBlur = 4;
        this.ctx.shadowColor = 'rgba(255, 255, 255, 0.8)';
        
        // Draw text
        this.ctx.fillText(value, cx, cy);
        
        this.ctx.restore();
    }
    
    // Helper to adjust color alpha
    adjustAlpha(hexColor, alpha) {
        // Convert hex to rgba
        const r = parseInt(hexColor.slice(1, 3), 16);
        const g = parseInt(hexColor.slice(3, 5), 16);
        const b = parseInt(hexColor.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    }
}
