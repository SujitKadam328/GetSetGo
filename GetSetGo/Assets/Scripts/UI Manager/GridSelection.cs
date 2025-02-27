using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSelection : MonoBehaviour
{
    [SerializeField] private GameObject m_mainMenuPanel = null;

    public void OnClickTwoByTwo()
    {
        GameManager.Instance.InitializeGame(2, 2);
        m_mainMenuPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void OnClickTwoByThree()
    {
        GameManager.Instance.InitializeGame(2, 3);
        m_mainMenuPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void OnClickFiveBySix()
    {
        GameManager.Instance.InitializeGame(5, 6);
        m_mainMenuPanel.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void OnClickBack()
    {
        this.gameObject.SetActive(false);
    }
}

