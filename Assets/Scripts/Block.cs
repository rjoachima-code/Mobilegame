using UnityEngine;

namespace JACAMENO
{
    /// <summary>
    /// Represents an individual block with value, color, and merge capabilities.
    /// </summary>
    public class Block : MonoBehaviour
    {
        [Header("Block Properties")]
        public int Value = 2;
        public Vector2Int GridPosition;
        public Vector2Int LocalPosition; // Position relative to tetromino center
        public bool IsLocked = false;

        [Header("Visual")]
        public SpriteRenderer SpriteRenderer;
        public TMPro.TextMeshPro ValueText;

        // Block colors based on value (neon minimal style)
        private static readonly Color[] BlockColors = new Color[]
        {
            new Color(0.2f, 0.8f, 1f),    // 2 - Cyan
            new Color(1f, 0.4f, 0.8f),    // 4 - Pink
            new Color(0.4f, 1f, 0.4f),    // 8 - Green
            new Color(1f, 0.8f, 0.2f),    // 16 - Yellow
            new Color(1f, 0.5f, 0.2f),    // 32 - Orange
            new Color(0.8f, 0.2f, 1f),    // 64 - Purple
            new Color(0.2f, 0.4f, 1f),    // 128 - Blue
            new Color(1f, 0.2f, 0.2f),    // 256 - Red
            new Color(0.2f, 1f, 0.8f),    // 512 - Teal
            new Color(1f, 1f, 0.4f),      // 1024 - Bright Yellow
            new Color(1f, 0.2f, 0.6f),    // 2048 - Magenta
        };

        private void Awake()
        {
            if (SpriteRenderer == null)
                SpriteRenderer = GetComponent<SpriteRenderer>();
            
            if (ValueText == null)
                ValueText = GetComponentInChildren<TMPro.TextMeshPro>();
        }

        private void Start()
        {
            UpdateVisuals();
        }

        /// <summary>
        /// Sets the block value and updates visuals.
        /// </summary>
        public void SetValue(int newValue)
        {
            Value = newValue;
            UpdateVisuals();
        }

        /// <summary>
        /// Updates the block's visual appearance based on its value.
        /// </summary>
        public void UpdateVisuals()
        {
            // Get color index based on value (power of 2)
            int colorIndex = GetColorIndex(Value);
            Color blockColor = BlockColors[Mathf.Min(colorIndex, BlockColors.Length - 1)];

            if (SpriteRenderer != null)
            {
                SpriteRenderer.color = blockColor;
            }

            if (ValueText != null)
            {
                ValueText.text = Value.ToString();
                ValueText.color = GetContrastColor(blockColor);
            }
        }

        /// <summary>
        /// Calculates color index from block value (power of 2).
        /// </summary>
        private int GetColorIndex(int value)
        {
            int index = 0;
            int temp = value;
            while (temp > 2)
            {
                temp /= 2;
                index++;
            }
            return index;
        }

        /// <summary>
        /// Gets a contrasting color for text visibility.
        /// </summary>
        private Color GetContrastColor(Color bgColor)
        {
            float luminance = 0.299f * bgColor.r + 0.587f * bgColor.g + 0.114f * bgColor.b;
            return luminance > 0.5f ? Color.black : Color.white;
        }

        /// <summary>
        /// Checks if this block can merge with another block.
        /// </summary>
        public bool CanMergeWith(Block other)
        {
            return other != null && other.Value == this.Value && !other.IsLocked == !this.IsLocked;
        }

        /// <summary>
        /// Merges this block with another, doubling the value.
        /// </summary>
        public void MergeWith(Block other)
        {
            if (!CanMergeWith(other))
                return;

            Value *= 2;
            UpdateVisuals();

            // Notify merge logic for combo tracking
            if (MergeLogic.Instance != null)
            {
                MergeLogic.Instance.OnBlockMerged(this, Value);
            }
        }

        /// <summary>
        /// Plays merge animation effect.
        /// </summary>
        public void PlayMergeEffect()
        {
            // Scale pulse animation
            StartCoroutine(MergeAnimation());
        }

        private System.Collections.IEnumerator MergeAnimation()
        {
            Vector3 originalScale = transform.localScale;
            Vector3 targetScale = originalScale * 1.2f;
            float duration = 0.15f;
            float elapsed = 0f;

            // Scale up
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
                yield return null;
            }

            elapsed = 0f;
            // Scale down
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        /// <summary>
        /// Destroys the block with an effect.
        /// </summary>
        public void DestroyBlock()
        {
            StartCoroutine(DestroyAnimation());
        }

        private System.Collections.IEnumerator DestroyAnimation()
        {
            float duration = 0.2f;
            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
                
                if (SpriteRenderer != null)
                {
                    Color c = SpriteRenderer.color;
                    c.a = 1f - t;
                    SpriteRenderer.color = c;
                }
                
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
