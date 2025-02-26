using UnityEngine;
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip m_flipSound;
    [SerializeField] private AudioClip m_matchSound;
    [SerializeField] private AudioClip m_mismatchSound;
    [SerializeField] private AudioClip m_winSound;
    [SerializeField] private AudioClip m_gameOverSound;
    private AudioSource m_audioSource;
    private void Awake()
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void PlayFlipSound() => PlaySound(m_flipSound);
    public void PlayMatchSound() => PlaySound(m_matchSound);
    public void PlayMismatchSound() => PlaySound(m_mismatchSound);
    public void PlayWinSound() => PlaySound(m_winSound);
    public void PlayGameOverSound() => PlaySound(m_gameOverSound);
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            m_audioSource.PlayOneShot(clip);
        }
    }
}