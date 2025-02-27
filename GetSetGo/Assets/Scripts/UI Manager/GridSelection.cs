using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the grid size selection UI functionality for the memory card game
/// </summary>
public class GridSelection : MonoBehaviour
{
    // Reference to the main menu panel that needs to be hidden when game starts
    [SerializeField] private GameObject m_mainMenuPanel = null;

    /// <summary>
    /// Initializes a 2x2 grid game when the corresponding button is clicked
    /// </summary>
    public void OnClickTwoByTwo()
    {
        GameManager.Instance.InitializeGame(2, 2);
        m_mainMenuPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Initializes a 2x3 grid game when the corresponding button is clicked
    /// </summary>
    public void OnClickTwoByThree()
    {
        GameManager.Instance.InitializeGame(2, 3);
        m_mainMenuPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Initializes a 5x6 grid game when the corresponding button is clicked
    /// </summary>
    public void OnClickFiveBySix()
    {
        GameManager.Instance.InitializeGame(5, 6);
        m_mainMenuPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Hides the grid selection panel when the back button is clicked
    /// </summary>
    public void OnClickBack()
    {
        this.gameObject.SetActive(false);
    }
}

