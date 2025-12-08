using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if JACAMENO_USE_DOTWEEN
using DG.Tweening;
#endif

namespace Jacameno
{
    /// <summary>
    /// Handles complex merge animations and orchestration.
    /// </summary>
    public class MergeMechanic : MonoBehaviour
    {
        [SerializeField] private float attractSpeed = 8f;

        /// <summary>
        /// Processes a merge of multiple shapes into one evolution.
        /// Moves shapes to center, spawns new shape, plays squish.
        /// Calls onComplete with the spawned ShapeController when done.
        /// In EditMode (Application.isPlaying == false) the operation runs synchronously to keep tests deterministic.
        /// </summary>
        public IEnumerator ProcessMerge(List<ShapeController> shapesToMerge, ShapeData resultData, GameObject shapePrefab, Transform parent, Action<ShapeController> onComplete = null)
        {
            if (shapesToMerge == null || shapesToMerge.Count == 0)
            {
                onComplete?.Invoke(null);
                yield break;
            }

            // Calculate center
            Vector3 center = Vector3.zero;
            foreach (var s in shapesToMerge) center += s.transform.position;
            center /= shapesToMerge.Count;

            if (!Application.isPlaying)
            {
                // Immediate synchronous behavior for EditMode tests: destroy old shapes and spawn result without animation
                foreach (var s in shapesToMerge)
                {
                    if (s != null)
                        DestroyImmediate(s.gameObject);
                }

                if (resultData != null && shapePrefab != null)
                {
#if UNITY_EDITOR
                    GameObject go = PrefabUtility.InstantiatePrefab(shapePrefab, parent) as GameObject;
                    if (go != null)
                    {
                        go.transform.position = center;
                        var sc = go.GetComponent<ShapeController>();
                        sc.Initialize(resultData);
                        sc.PlaySquishAnimation();
                        onComplete?.Invoke(sc);
                    }
                    else
                    {
                        onComplete?.Invoke(null);
                    }
#else
                    onComplete?.Invoke(null);
#endif
                }
                else
                {
                    onComplete?.Invoke(null);
                }

                yield break;
            }

#if JACAMENO_USE_DOTWEEN
            // Use DOTween to move all shapes to center, then spawn
            List<Tween> tweens = new List<Tween>();
            foreach (var s in shapesToMerge)
            {
                if (s == null) continue;
#if UNITY_EDITOR
                // In Editor/Play mode DOTween works the same
#endif
                tweens.Add(s.transform.DOMove(center, 0.18f).SetEase(Ease.OutQuad));
            }
            if (tweens.Count > 0)
            {
                yield return DOTween.Sequence().Join(tweens.ToArray()).WaitForCompletion();
            }
#else
            // Animate all shapes moving to center
            float t = 0f;
            float dur = 0.18f;
            List<Vector3> starts = new List<Vector3>();
            foreach (var s in shapesToMerge) starts.Add(s.transform.position);

            while (t < 1f)
            {
                t += Time.deltaTime / dur;
                for (int i = 0; i < shapesToMerge.Count; i++)
                {
                    var s = shapesToMerge[i];
                    if (s == null) continue;
                    s.transform.position = Vector3.Lerp(starts[i], center, Mathf.SmoothStep(0f, 1f, t));
                }
                yield return null;
            }
#endif

            // Destroy old shapes
            foreach (var s in shapesToMerge)
            {
                if (s != null)
                    Destroy(s.gameObject);
            }

            // Spawn result
            if (resultData != null && shapePrefab != null)
            {
                GameObject go = Instantiate(shapePrefab, center, Quaternion.identity, parent);
                var sc = go.GetComponent<ShapeController>();
                sc.Initialize(resultData);
                sc.PlaySquishAnimation();
                onComplete?.Invoke(sc);
            }
            else
            {
                onComplete?.Invoke(null);
            }
        }
    }
}
