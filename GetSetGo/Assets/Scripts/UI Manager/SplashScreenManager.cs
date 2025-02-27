using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Manages the splash screen behavior and transition to main menu
/// This script handles the initial game loading screen animations and timing
/// </summary>
public class SplashScreenManager : MonoBehaviour
{
    [Header("Timing Settings")]
    // Delay before transitioning to main menu (in seconds)
    [SerializeField] private float delayBeforeLoading = 3f;
    // Time interval between animation states (in seconds)
    [SerializeField] private float tickInterval = 0.5f; // How fast the dots animate

    [Header("UI References")]
    // Reference to the splash screen text component that displays "Get Set Go"
    [SerializeField] private TextMeshProUGUI m_splashScreenText = null;
    // Reference to the main menu panel that will be shown after splash screen
    [SerializeField] private GameObject m_mainMenuPanel = null;

    /// <summary>
    /// Called when the script instance is being loaded
    /// Initializes the splash screen animations and loading sequence
    /// </summary>
    void Start()
    {
        // Start both coroutines for loading and animation
        StartCoroutine(LoadMainMenuAfterDelay());
        StartCoroutine(AnimateExclamation());
    }

    /// <summary>
    /// Coroutine that handles the transition from splash screen to main menu
    /// Waits for specified delay time before switching screens
    /// </summary>
    /// <returns>IEnumerator for the coroutine system</returns>
    IEnumerator LoadMainMenuAfterDelay()
    {
        m_splashScreenText.text = "Get Set Go !";
        // Wait for the specified delay duration
        yield return new WaitForSeconds(delayBeforeLoading);
        // Deactivate the splash screen
        this.gameObject.SetActive(false);
        // Activate the main menu panel
        m_mainMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Coroutine that creates an animation effect by changing the dots
    /// Creates a loading-like animation effect in the splash screen text
    /// </summary>
    /// <returns>IEnumerator for the coroutine system</returns>
    IEnumerator AnimateExclamation()
    {
        // Infinite loop to continuously animate the text until object is disabled
        while (true)
        {
            // Cycle between different numbers of dots
            m_splashScreenText.text = "Get Set Go.";
            yield return new WaitForSeconds(tickInterval);
            m_splashScreenText.text = "Get Set Go..";
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
