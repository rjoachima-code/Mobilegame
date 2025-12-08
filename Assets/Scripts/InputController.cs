using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// Cross-platform input controller for dragging and dropping shapes into columns.
    /// </summary>
    public class InputController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera = null;
        [SerializeField] private GridManager gridManager = null;
        [SerializeField] private GameObject activeShapePrefab = null;
        [SerializeField] private Transform shapesParent = null;
        [SerializeField] private float horizontalClamp = 2.4f; // world units half-width to clamp drag

        private ShapeController activeShape = null;

        private void Start()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            if (shapesParent == null && gridManager != null)
                shapesParent = gridManager.transform;

            SpawnNewActiveShape();
        }

        private void Update()
        {
            if (activeShape == null) return;

            if (Input.touchSupported && Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                Vector3 world = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10f));
                HandleDrag(world);

                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    HandleRelease(world);
            }
            else
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 world = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));

                if (Input.GetMouseButton(0))
                {
                    HandleDrag(world);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    HandleRelease(world);
                }
            }
        }

        private void HandleDrag(Vector3 worldPosition)
        {
            // Only follow X while keeping spawn Y
            Vector3 pos = activeShape.transform.position;
            // If gridManager is present, clamp to the leftmost/rightmost column X
            if (gridManager != null)
            {
                int cols = Mathf.Max(1, gridManager.ColumnCount);
                float leftX = gridManager.GetColumnX(0);
                float rightX = gridManager.GetColumnX(cols - 1);
                pos.x = Mathf.Clamp(worldPosition.x, leftX - 0.5f, rightX + 0.5f);
            }
            else
            {
                pos.x = Mathf.Clamp(worldPosition.x, -horizontalClamp, horizontalClamp);
            }
            activeShape.transform.position = new Vector3(pos.x, pos.y, pos.z);
            activeShape.gameObject.SetActive(true);
        }

        private void HandleRelease(Vector3 worldPosition)
        {
            if (gridManager == null)
            {
                // Fallback: choose middle column
                if (activeShape != null)
                {
                    activeShape.transform.SetParent(shapesParent, true);
                    gridManager?.DropShape(0, activeShape);
                    activeShape = null;
                    SpawnNewActiveShape();
                }
                return;
            }

            // Determine nearest column by X using GridManager.GetColumnX
            int best = 0;
            float minDist = float.MaxValue;
            int cols = Mathf.Max(1, gridManager.ColumnCount);
            for (int i = 0; i < cols; i++)
            {
                float colX = gridManager.GetColumnX(i);
                float d = Mathf.Abs(worldPosition.x - colX);
                if (d < minDist)
                {
                    minDist = d;
                    best = i;
                }
            }

            // Snap to column
            if (activeShape != null)
            {
                activeShape.transform.SetParent(shapesParent, true);
                gridManager.DropShape(best, activeShape);
                activeShape = null;
                // Spawn next
                SpawnNewActiveShape();
            }
        }

        private void SpawnNewActiveShape()
        {
            if (activeShapePrefab == null) return;
            GameObject go = Instantiate(activeShapePrefab, shapesParent);
            activeShape = go.GetComponent<ShapeController>();
            // active will be positioned by prefab default; ensure it's visible
            if (activeShape == null)
            {
                Debug.LogError("Active prefab must have ShapeController component");
            }
        }
    }
}
