using System.Collections.Generic;
using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// Manages the 5-column grid, kinematic placement, and merge checks.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int columnCount = 5;
        [SerializeField] private float columnWidth = 1.2f;
        [SerializeField] private float baseY = -3.5f;
        [SerializeField] private float rowHeight = 1.1f;
        [SerializeField] private Transform shapesParent = null;
        [SerializeField] private GameObject shapePrefab = null; // Prefab with ShapeController
        [SerializeField] private MergeMechanic mergeMechanic = null;

        // Array of lists, each list is bottom->top order (0 = bottom)
        private List<ShapeController>[] columns;

        public int ColumnCount => columnCount;

        private void Awake()
        {
            columns = new List<ShapeController>[columnCount];
            for (int i = 0; i < columnCount; i++)
                columns[i] = new List<ShapeController>();

            if (shapesParent == null)
                shapesParent = this.transform;
        }

        /// <summary>
        /// Calculate world X for a given column index.
        /// </summary>
        public float GetColumnX(int colIndex)
        {
            float center = (columnCount - 1) * 0.5f;
            return this.transform.position.x + (colIndex - center) * columnWidth;
        }

        /// <summary>
        /// Returns the target Y for the next shape in the column (top position).
        /// </summary>
        public float GetTargetY(int colIndex)
        {
            int count = Mathf.Clamp(columns[colIndex].Count, 0, int.MaxValue);
            return baseY + (count * rowHeight);
        }

        /// <summary>
        /// Drops shape into a column. The shape's GameObject is parented to shapesParent.
        /// </summary>
        public void DropShape(int colIndex, ShapeController shape)
        {
            if (colIndex < 0 || colIndex >= columnCount) colIndex = Mathf.Clamp(colIndex, 0, columnCount - 1);
            // Add to logical column
            columns[colIndex].Add(shape);
            shape.transform.SetParent(shapesParent, true);

            Vector3 target = new Vector3(GetColumnX(colIndex), GetTargetY(colIndex), 0f);
            shape.MoveToPosition(target);

            // After it lands, schedule merge check. We'll run a coroutine to wait until Landed
            StartCoroutine(WaitAndCheckMerge(colIndex, shape));
        }

        private System.Collections.IEnumerator WaitAndCheckMerge(int colIndex, ShapeController shape)
        {
            // Wait until shape state is Landed
            while (shape.CurrentState != ShapeController.State.Landed)
                yield return null;

            // Ensure shape is top of column
            if (columns[colIndex].Count >= 2)
            {
                CheckMerge(colIndex);
            }
        }

        /// <summary>
        /// Checks the top two shapes in the given column. If they match, remove both and spawn evolution.
        /// Now supports multi-shape combos using MergeMechanic.
        /// </summary>
        public void CheckMerge(int colIndex)
        {
            var list = columns[colIndex];
            if (list.Count < 2) return;

            var top = list[list.Count - 1];
            var second = list[list.Count - 2];

            if (top.Data == null || second.Data == null) return;

            // Match based on same ShapeData reference
            if (top.Data == second.Data)
            {
                // Collect merge candidates including magnet neighbors
                List<ShapeController> toMerge = CollectMergeCandidates(colIndex, top.Data);

                // Remove them from their columns
                foreach (var s in toMerge)
                {
                    RemoveShapeFromColumns(s);
                }

                // Calculate resulting shape (single evolution step)
                ShapeData result = top.Data.NextEvolution;

                if (mergeMechanic != null)
                {
                    // Use MergeMechanic coroutine to animate merge and spawn result
                    StartCoroutine(mergeMechanic.ProcessMerge(toMerge, result, shapePrefab, shapesParent, (spawned) => {
                        if (spawned != null)
                        {
                            // Add spawned shape logically to this column and move to correct Y
                            columns[colIndex].Add(spawned);
                            spawned.MoveToPosition(new Vector3(GetColumnX(colIndex), GetTargetY(colIndex), 0f));
                            spawned.PlaySquishAnimation();

                            // After spawning, run another check for chain merges
                            StartCoroutine(DelayedMergeCheck(colIndex));
                        }
                    }));
                }
                else
                {
                    // Fallback: immediate spawn
                    Vector3 mergeCenter = Vector3.zero;
                    foreach (var s in toMerge) mergeCenter += s.transform.position;
                    mergeCenter /= Mathf.Max(1, toMerge.Count);

                    if (result != null && shapePrefab != null)
                    {
                        GameObject go = Instantiate(shapePrefab, mergeCenter, Quaternion.identity, shapesParent);
                        var sc = go.GetComponent<ShapeController>();
                        sc.Initialize(result);
                        columns[colIndex].Add(sc);
                        sc.MoveToPosition(new Vector3(GetColumnX(colIndex), GetTargetY(colIndex), 0f));
                        sc.PlaySquishAnimation();

                        MagnetCheck(colIndex, result);
                    }
                }
            }
        }

        /// <summary>
        /// Collects merge candidates starting from the top two in column, then expanding to adjacent columns for matching shapes (magnet effect).
        /// </summary>
        private List<ShapeController> CollectMergeCandidates(int startCol, ShapeData matchData)
        {
            List<ShapeController> found = new List<ShapeController>();

            // First, add all consecutive matching shapes from the top of the start column
            var colList = columns[startCol];
            for (int i = colList.Count - 1; i >= 0; i--)
            {
                var s = colList[i];
                if (s != null && s.Data == matchData)
                    found.Add(s);
                else
                    break; // stop when a different shape is encountered
            }

            // Check neighbors for their top matches and recursively add if matching
            int[] neighbors = new int[] { startCol - 1, startCol + 1 };
            foreach (int n in neighbors)
            {
                if (n < 0 || n >= columnCount) continue;
                var nList = columns[n];
                if (nList.Count == 0) continue;
                var top = nList[nList.Count - 1];
                if (top != null && top.Data == matchData)
                {
                    // Add neighbor top and then also check further magnet chain
                    found.Add(top);

                    // If neighbor now has additional matching items below top, also collect them
                    for (int j = nList.Count - 2; j >= 0; j--)
                    {
                        var s = nList[j];
                        if (s != null && s.Data == matchData)
                            found.Add(s);
                        else
                            break;
                    }
                }
            }

            return found;
        }

        /// <summary>
        /// Removes a shape from whichever column list it belongs to.
        /// </summary>
        private void RemoveShapeFromColumns(ShapeController s)
        {
            for (int c = 0; c < columns.Length; c++)
            {
                if (columns[c].Contains(s))
                {
                    columns[c].Remove(s);
                    return;
                }
            }
        }

        /// <summary>
        /// If adjacent columns have the same shape as "mergeShape", those shapes are drawn into this column and animated.
        /// Retained for backward compatibility but main merge now uses CollectMergeCandidates.
        /// </summary>
        private void MagnetCheck(int colIndex, ShapeData mergeShape)
        {
            int[] neighbors = new int[] { colIndex - 1, colIndex + 1 };
            List<ShapeController> attracted = new List<ShapeController>();

            foreach (int n in neighbors)
            {
                if (n < 0 || n >= columnCount) continue;
                var list = columns[n];
                if (list.Count == 0) continue;
                var top = list[list.Count - 1];
                if (top.Data == mergeShape)
                {
                    // Remove from neighbor and add to current column
                    list.RemoveAt(list.Count - 1);
                    attracted.Add(top);
                }
            }

            if (attracted.Count == 0) return;

            // Move attracted shapes to the merge column and animate
            foreach (var s in attracted)
            {
                s.transform.SetParent(shapesParent, true);
                columns[colIndex].Add(s);
                Vector3 target = new Vector3(GetColumnX(colIndex), GetTargetY(colIndex), 0f);
                s.MoveToPosition(target);
            }

            // After movement, check merges recursively
            StartCoroutine(DelayedMergeCheck(colIndex));
        }

        private System.Collections.IEnumerator DelayedMergeCheck(int colIndex)
        {
            // Wait a short time for moves to approximate finish
            yield return new WaitForSeconds(0.24f);
            CheckMerge(colIndex);
        }
    }
}
