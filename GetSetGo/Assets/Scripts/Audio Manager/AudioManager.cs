using UnityEngine;

/// <summary>
/// Manages all audio playback for the game, including sound effects for card flips,
/// matches, mismatches, and game states.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // Audio clips for different game events
    [SerializeField] private AudioClip m_flipSound;        // Played when a card is flipped
    [SerializeField] private AudioClip m_matchSound;       // Played when two cards match
    [SerializeField] private AudioClip m_mismatchSound;    // Played when two cards don't match
    [SerializeField] private AudioClip m_winSound;         // Played when the player wins
    [SerializeField] private AudioClip m_gameOverSound;    // Played when the game is over

    private AudioSource m_audioSource;    // Component that handles playing the audio

    /// <summary>
    /// Initializes the AudioSource component on awake
    /// </summary>
    private void Awake()
    {
        m_audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Public methods to play specific sound effects
    public void PlayFlipSound() => PlaySound(m_flipSound);
    public void PlayMatchSound() => PlaySound(m_matchSound);
    public void PlayMismatchSound() => PlaySound(m_mismatchSound);
    public void PlayWinSound() => PlaySound(m_winSound);
    public void PlayGameOverSound() => PlaySound(m_gameOverSound);

    /// <summary>
    /// Plays the specified audio clip if it exists
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            m_audioSource.PlayOneShot(clip);
        }
    }
}