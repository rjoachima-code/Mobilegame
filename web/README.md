# JACAMENO Web Version

This is the browser-based version of JACAMENO that can be played directly in any modern web browser.

## Features

- ✅ Full Tetris gameplay with tetromino pieces (I, O, T, L, J, S, Z)
- ✅ M2 Block merge mechanics (blocks with same value merge and double)
- ✅ Score tracking and leveling system
- ✅ Keyboard controls (Arrow keys, WASD, Space)
- ✅ Touch controls for mobile devices (swipe and tap)
- ✅ Responsive design works on desktop and mobile
- ✅ No installation required - runs in browser
- ✅ Works offline after first load

## How to Play

### Quick Start

1. **Open in Browser:**
   - Simply open `index.html` in any modern web browser
   - Or use a local web server for best experience

2. **Using a Local Web Server (Recommended):**
   ```bash
   # Option 1: Python 3
   python3 -m http.server 8000
   
   # Option 2: Python 2
   python -m SimpleHTTPServer 8000
   
   # Option 3: Node.js (if you have http-server installed)
   npx http-server -p 8000
   
   # Option 4: PHP
   php -S localhost:8000
   ```
   
   Then open: http://localhost:8000

### Controls

**Keyboard:**
- **Arrow Left/Right** or **A/D**: Move piece left/right
- **Arrow Down** or **S**: Soft drop (move down faster)
- **Arrow Up** or **W**: Rotate piece
- **Space**: Hard drop (instant drop to bottom)
- **P** or **ESC**: Pause game

**Touch (Mobile):**
- **Swipe Left/Right**: Move piece
- **Swipe Down**: Hard drop
- **Tap**: Rotate piece

### Game Rules

1. **Tetris Mechanics:**
   - Tetromino pieces fall from the top
   - Move and rotate pieces to create complete rows
   - Complete rows are cleared and award points

2. **M2 Block Merge:**
   - Each block has a value (starting at 2)
   - When blocks with the same value touch (horizontally or vertically), they merge
   - Merged blocks double in value (2→4→8→16→32...)
   - Merges award points based on the resulting value

3. **Scoring:**
   - Line clears: 100 × level × lines cleared
   - Block merges: Value of merged block
   - Hard drops: 2 points per row
   - Level increases every 10 lines cleared

4. **Game Over:**
   - Game ends when new pieces can't spawn at the top

## Deployment

### Option 1: GitHub Pages

1. Push the `web` folder to your repository
2. Go to Settings > Pages
3. Select branch and `/web` folder
4. Your game will be live at: `https://yourusername.github.io/Mobilegame/`

### Option 2: Netlify

1. Drag and drop the `web` folder to [Netlify Drop](https://app.netlify.com/drop)
2. Get instant deployment with a custom URL

### Option 3: Vercel

```bash
# Install Vercel CLI
npm install -g vercel

# Deploy from web directory
cd web
vercel
```

### Option 4: Any Static Host

The web version is pure HTML/CSS/JavaScript with no build process. Just upload the files to any static web hosting service:
- AWS S3 + CloudFront
- Google Cloud Storage
- Azure Static Web Apps
- Cloudflare Pages
- Firebase Hosting

## Browser Compatibility

Works on all modern browsers:
- ✅ Chrome/Edge 90+
- ✅ Firefox 88+
- ✅ Safari 14+
- ✅ Opera 76+
- ✅ Mobile browsers (iOS Safari, Chrome Mobile)

## Development

### File Structure

```
web/
├── index.html      # Main HTML structure
├── styles.css      # Styling and responsive design
├── game.js         # Game logic and mechanics
└── README.md       # This file
```

### Customization

**Change Colors:**
Edit the color values in `game.js` under `initTetrominoes()`:
```javascript
'I': { shape: [[1,1,1,1]], color: '#00f0f0', value: 2 }
```

**Adjust Difficulty:**
Modify starting drop interval in `game.js`:
```javascript
this.dropInterval = 1000; // milliseconds (lower = faster)
```

**Change Grid Size:**
Edit in `game.js` constructor:
```javascript
this.cols = 10;  // width
this.rows = 20;  // height
```

## Performance

- Lightweight: ~25KB total (uncompressed)
- 60 FPS target on all devices
- No external dependencies
- Canvas-based rendering for smooth graphics

## Differences from Native Version

The web version is a lightweight implementation focused on core gameplay:

**Included:**
- ✅ Core Tetris mechanics
- ✅ M2 Block merge system
- ✅ Scoring and levels
- ✅ Touch and keyboard controls
- ✅ Pause/resume

**Not Yet Implemented:**
- ⏳ Power-ups (Bomb, Freeze, etc.)
- ⏳ Sound effects
- ⏳ High score persistence (localStorage can be added)
- ⏳ Advanced visual effects

These features can be added in future updates!

## Testing Checklist

- [ ] Game loads without errors
- [ ] Main menu displays correctly
- [ ] Start button launches game
- [ ] Pieces fall automatically
- [ ] Keyboard controls work (all keys)
- [ ] Touch controls work (swipe/tap)
- [ ] Blocks merge when same value touches
- [ ] Lines clear when complete
- [ ] Score updates correctly
- [ ] Level increases every 10 lines
- [ ] Pause/resume works
- [ ] Game over triggers correctly
- [ ] Play again restarts game
- [ ] Works on mobile devices
- [ ] Responsive design adapts to screen size

## Troubleshooting

**Game doesn't load:**
- Check browser console for errors (F12)
- Ensure you're using a modern browser
- Try using a local web server instead of file://

**Touch controls don't work:**
- Ensure you're on a touch-enabled device
- Try tapping directly on the game canvas

**Performance issues:**
- Close other tabs/applications
- Try a different browser
- Check if hardware acceleration is enabled

## Contributing

To add features to the web version:

1. Fork the repository
2. Make changes in the `web/` folder
3. Test in multiple browsers
4. Submit a pull request

## License

Same as the main JACAMENO project.

---

**Enjoy playing JACAMENO in your browser! 🎮**

For the native Android version, see the main README in the project root.
