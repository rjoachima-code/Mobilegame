# Geometric Sprite System Documentation

## Overview

The JACAMENO web game features a custom geometric sprite rendering system with a minimalist cyberpunk aesthetic. Each block value is represented by a unique geometric shape with distinct visual properties and neon glow effects.

## Geometric Shapes

### Triangle (Value: 2)
- **Color**: Neon Blue (#00d4ff)
- **Effect**: Glass texture with gradient fill
- **Style**: Slightly rounded corners with bright edge lighting
- **Description**: The entry-level shape featuring a translucent glass effect that creates depth through gradient shading

### Square (Value: 4)
- **Color**: Neon Green (#00ff88)
- **Effect**: Matte finish with edge lighting
- **Style**: Heavy weight appearance with bright white edges
- **Description**: Solid matte block with prominent edge highlights for a bold, weighted look

### Pentagon (Value: 8)
- **Color**: Neon Magenta (#ff00ff)
- **Effect**: Crystalline structure
- **Style**: Radial gradient with internal facet lines
- **Description**: Crystal-like appearance with geometric facets radiating from center

### Hexagon (Value: 16)
- **Color**: Neon Orange (#ff8800)
- **Effect**: Honeycomb pattern
- **Style**: Internal honeycomb cells with glowing borders
- **Description**: Features a tessellated honeycomb pattern within the hexagonal shape

### Circle (Value: 32+)
- **Color**: Bright White (#ffffff)
- **Effect**: "God tier" radiant aura
- **Style**: Multi-layer radiant glow with bright core
- **Description**: The ultimate shape featuring multiple concentric glow layers for a divine appearance

## Visual Effects System

### Bloom Effect
Each shape features a multi-layer bloom effect:
- **Layer 1**: Blur 15px, Alpha 0.3 (outer glow)
- **Layer 2**: Blur 8px, Alpha 0.4 (mid glow)
- **Layer 3**: Blur 3px, Alpha 0.5 (inner glow)

### Rendering Pipeline
1. **Background**: Dark cyberpunk background (#0a0a0f)
2. **Bloom layers**: Multiple gaussian blur passes
3. **Shape geometry**: Vector-based shape rendering
4. **Edge effects**: Bright edge lighting with shadows
5. **Value text**: Centered text with glow for visibility

## Technical Implementation

### GeometricRenderer Class
Located in `sprites.js`, the renderer handles:
- Shape configuration management
- Dynamic shape selection based on block value
- Multi-layer bloom effect rendering
- Shape-specific drawing methods
- Color and effect management

### Integration with Game
The renderer integrates with `game.js`:
```javascript
// Initialize renderer
this.renderer = new GeometricRenderer(this.ctx);

// Draw shapes
this.renderer.drawShape(px, py, this.blockSize, value);
```

## Cyberpunk Aesthetic

### Color Palette
- **Primary**: Neon Blue (#00d4ff)
- **Secondary**: Neon Magenta (#ff00ff)
- **Accent 1**: Neon Green (#00ff88)
- **Accent 2**: Neon Orange (#ff8800)
- **Background**: Dark Navy (#0a0a0f)

### Design Principles
1. **High Contrast**: Dark background with bright neon elements
2. **Bloom Effects**: Generous use of glow for cyberpunk feel
3. **Geometric Precision**: Clean vector-style shapes
4. **3D Depth**: Shading and gradients create depth in 2D
5. **Minimalism**: No unnecessary decoration, focus on shapes

## Performance Considerations

### Optimization Techniques
- Canvas-based rendering for smooth 60 FPS
- Efficient bloom layer caching
- Minimal draw calls per frame
- No external image dependencies

### Browser Compatibility
- Works on all modern browsers with Canvas 2D support
- Hardware acceleration automatically utilized
- No WebGL required
- Mobile-optimized rendering

## Customization Guide

### Adding New Shapes
1. Add shape method to `GeometricRenderer` class
2. Define configuration in `shapeConfigs` object
3. Implement drawing logic with bloom effects

### Modifying Colors
Edit the `shapeConfigs` in `sprites.js`:
```javascript
this.shapeConfigs = {
    2: { 
        shape: 'triangle', 
        color: '#00d4ff',  // Change this
        glowColor: 'rgba(0, 212, 255, 0.8)',
        effect: 'glass'
    },
    // ...
};
```

### Adjusting Bloom Intensity
Modify bloom layers in `drawBloomEffect()`:
```javascript
const layers = [
    { offset: 0, blur: 15, alpha: 0.3 },  // Adjust these values
    { offset: 0, blur: 8, alpha: 0.4 },
    { offset: 0, blur: 3, alpha: 0.5 }
];
```

## Future Enhancements

### Planned Features
- Particle effects on block merges
- Shape morph animations during transitions
- Additional geometric shapes (octagon, star, etc.)
- Customizable color themes
- Dynamic bloom based on combo chains

### Accessibility
- High contrast mode option
- Reduced motion settings
- Colorblind-friendly palettes

## Credits

Inspired by:
- Cyberpunk 2077 UI design
- Tron Legacy aesthetic
- Synthwave visual style
- M2 Block Merge mechanics
