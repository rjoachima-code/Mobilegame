Jacameno - Quick Setup

This folder contains the core scripts for the Jacameno prototype.

Quick Steps to get started in Unity:

1. Create ShapeData assets
   - Right click in Project -> Create -> Jacameno -> ShapeData
   - Configure ShapeName, VertexCount, Icon, NeonColor and set NextEvolution link.

2. Create a shape prefab
   - Create an empty GameObject, add SpriteRenderer and the ShapeController component.
   - Configure SpriteRenderer default sprite and tune ShapeController moveSpeed and squishDuration.
   - Save as prefab under Assets/Prefabs/Shapes/ (or use the Setup window)

3. Create a GridManager in the scene
   - Add the GridManager component to an empty GameObject.
   - Assign the shape prefab to the GridManager.shapePrefab field.
   - Tune columnCount, columnWidth, baseY and rowHeight to match your camera.

4. Create an InputController
   - Add InputController to an empty GameObject and link GridManager, mainCamera and the active shape prefab (the same prefab works).

5. Use the Jacameno Setup window
   - Window -> Jacameno -> Setup Window
   - Use it to create prefabs from ShapeData and assign the prefab to GridManager quickly.

Notes:
- This implementation uses code-driven movement (no Rigidbody2D) for deterministic stacking.
- Optionally add DOTween for smoother tweens (this project does not include DOTween by default).

If you want, I can also:
- Add DOTween integration with conditional compilation and sample tweens.
- Provide a simple scene that has everything wired and a few sample ShapeData assets.

