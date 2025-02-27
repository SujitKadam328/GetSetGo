using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Handles the behavior and interactions of individual cards in the memory matching game
/// </summary>
public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    // Card identification
    public int m_cardId;                                    // Unique identifier for the card
    
    [Space]
    [SerializeField] private SpriteRenderer m_frontFace;    // Renderer for the front face of the card
    [SerializeField] private SpriteRenderer m_backFace;     // Renderer for the back face of the card
    
    [Space]
    public bool m_isMatched = false;                        // Tracks if the card has been matched with its pair
    
    [Space]
    public bool m_isFlipped = false;                        // Tracks if the card is currently face up
    private float m_flipDuration = 0.3f;                    // Duration of the card flip animation

    /// <summary>
    /// Initializes the card with its ID and sprite
    /// </summary>
    /// <param name="a_id">The unique identifier for the card</param>
    /// <param name="a_cardSprite">The sprite to be displayed on the card's front face</param>
    public void Initialize(int a_id, Sprite a_cardSprite)
    {
        m_cardId = a_id;
        m_frontFace.sprite = a_cardSprite;
        m_isFlipped = false;
        m_isMatched = false;
    }

    /// <summary>
    /// Flips the card if it hasn't been matched yet
    /// Uses DOTween for smooth rotation animation
    /// </summary>
    public void Flip()
    {
        if (m_isMatched) return;

        m_isFlipped = !m_isFlipped;
        float targetRotation = m_isFlipped ? 180f : 0f;
        transform.DORotate(new Vector3(0, targetRotation, 0), m_flipDuration)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Special flip method used when loading the game state
    /// Similar to regular Flip but without the matched check
    /// </summary>
    public void FlipOnLoad()
    {
        m_isFlipped = !m_isFlipped;
        
        float targetRotation = m_isFlipped ? 180f : 0f;
        transform.DORotate(new Vector3(0, targetRotation, 0), m_flipDuration)
            .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Marks the card as matched and plays a celebration animation
    /// </summary>
    public void SetMatched()
    {
        m_isMatched = true;
        StartCoroutine(PlayMatchAnimation());
    }

    /// <summary>
    /// Coroutine that handles the match celebration animation
    /// Scales the card up briefly and then returns it to normal size
    /// </summary>
    private IEnumerator PlayMatchAnimation()
    {
        float duration = 0.2f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale * 1.1f; // Just 10% bigger

        // Quick scale up
        transform.localScale = targetScale;
        yield return new WaitForSeconds(duration);
        
        // Back to normal
        transform.localScale = startScale;
    }

    /// <summary>
    /// Called when the pointer enters the card's collider
    /// Can be used for hover effects
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hover start logic
        // Debug.Log($"Entered card number {m_cardId}");
    }

    /// <summary>
    /// Called when the pointer exits the card's collider
    /// Can be used for hover exit effects
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        // Hover end logic
        // Debug.Log($"Exited card number {m_cardId}");
    }

    /// <summary>
    /// Called when the card is clicked
    /// Notifies the GameManager of the interaction
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // Click logic
        // Debug.Log($"Clicked card number {m_cardId}");

        GameManager.Instance.OnCardClicked(this);
    }
}