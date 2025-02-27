using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using System.Collections;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public int m_cardId;
    [Space]
    [SerializeField] private SpriteRenderer m_frontFace;
    [SerializeField] private SpriteRenderer m_backFace;
    [Space]
    public bool m_isMatched = false;
    [Space]
    public bool m_isFlipped = false;
    private float m_flipDuration = 0.3f;
    public void Initialize(int a_id, Sprite a_cardSprite)
    {
        m_cardId = a_id;
        m_frontFace.sprite = a_cardSprite;
        m_isFlipped = false;
        m_isMatched = false;
    }
    public void Flip()
    {
        if (m_isMatched) return;

        m_isFlipped = !m_isFlipped;
        float targetRotation = m_isFlipped ? 180f : 0f;
        transform.DORotate(new Vector3(0, targetRotation, 0), m_flipDuration)
            .SetEase(Ease.InOutSine);
    }

    public void FlipOnLoad()
    {
        m_isFlipped = !m_isFlipped;
        
        float targetRotation = m_isFlipped ? 180f : 0f;
        transform.DORotate(new Vector3(0, targetRotation, 0), m_flipDuration)
            .SetEase(Ease.InOutSine);
    }
    public void SetMatched()
    {
        m_isMatched = true;
        StartCoroutine(PlayMatchAnimation());
    }

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Hover start logic
        Debug.Log($"Entered card number {m_cardId}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hover end logic
        Debug.Log($"Exited card number {m_cardId}");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Click logic
        Debug.Log($"Clicked card number {m_cardId}");

        GameManager.Instance.OnCardClicked(this);
    }
}