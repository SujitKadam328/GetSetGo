using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the main menu functionality and UI interactions
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Reference to the grid selection panel GameObject")]
    public GameObject m_gridSelectionPanel;

    /// <summary>
    /// Called when the Play button is clicked
    /// Shows the grid selection panel
    /// </summary>
    public void OnClickPlay()
    {
        m_gridSelectionPanel.SetActive(true);
    }

    /// <summary>
    /// Called when the Exit button is clicked
    /// Quits the application
    /// </summary>
    public void OnClickExit()
    {
        Application.Quit();
    }
}
