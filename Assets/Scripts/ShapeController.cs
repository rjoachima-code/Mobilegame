using System.Collections;
using UnityEngine;
#if JACAMENO_USE_DOTWEEN
using DG.Tweening;
#endif

namespace Jacameno
{
    /// <summary>
    /// Controls a single shape's visuals and state.
    /// Uses kinematic, code-driven movement (no Rigidbody2D).
    /// </summary>
    public class ShapeController : MonoBehaviour
    {
        public enum State { Spawning, Dragging, Falling, Landed, Merging }

        [SerializeField] private SpriteRenderer spriteRenderer = null;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float squishDuration = 0.12f;
        [SerializeField] private AnimationCurve squishCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private ShapeData shapeData;
        private State currentState = State.Spawning;

        // Smooth movement coroutine handle
        private Coroutine moveRoutine = null;

        public ShapeData Data => shapeData;
        public State CurrentState => currentState;

        private void Reset()
        {
            // Try to auto-assign SpriteRenderer if placed on same GameObject
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Initialize visuals and state.
        /// </summary>
        public void Initialize(ShapeData data)
        {
            shapeData = data;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = data.Icon;
                spriteRenderer.color = data.NeonColor;
            }
            currentState = State.Spawning;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Smoothly moves the object to target using Lerp until close enough.
        /// Cancels any existing move.
        /// </summary>
        public void MoveToPosition(Vector3 target)
        {
#if JACAMENO_USE_DOTWEEN
            // If DOTween is available and enabled via scripting define symbol, use it for smooth movement
            transform.DOMove(target, 0.18f).SetEase(Ease.OutQuad).OnStart(() => currentState = State.Falling).OnComplete(() => currentState = State.Landed);
#else
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(MoveRoutine(target));
#endif
        }

        private IEnumerator MoveRoutine(Vector3 target)
        {
            currentState = State.Falling;
            while (Vector3.SqrMagnitude(transform.position - target) > 0.001f)
            {
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);
                yield return null;
            }
            transform.position = target;
            currentState = State.Landed;
            moveRoutine = null;
        }

        /// <summary>
        /// Plays a quick squash-stretch "squish" animation to simulate soft-body impact.
        /// </summary>
        public void PlaySquishAnimation()
        {
#if JACAMENO_USE_DOTWEEN
            // Use DOTween if available
            transform.DOKill();
            transform.DOPunchScale(new Vector3(0.18f, -0.25f, 0f), squishDuration, 8, 0.4f);
#else
            StartCoroutine(SquishRoutine());
#endif
        }

        private IEnumerator SquishRoutine()
        {
            float elapsed = 0f;
            Vector3 start = transform.localScale;
            Vector3 squishTarget = new Vector3(1.2f, 0.75f, 1f);

            // Squash
            while (elapsed < squishDuration)
            {
                float t = squishCurve.Evaluate(elapsed / squishDuration);
                transform.localScale = Vector3.Lerp(start, squishTarget, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = squishTarget;

            // Return
            elapsed = 0f;
            float returnDur = squishDuration * 1.2f;
            while (elapsed < returnDur)
            {
                float t = squishCurve.Evaluate(elapsed / returnDur);
                transform.localScale = Vector3.Lerp(squishTarget, Vector3.one, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Call when this shape is merging - sets state and disables interactions.
        /// </summary>
        public void SetMerging()
        {
            currentState = State.Merging;
            // Optionally disable collider or input here
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
        }
    }
}
