using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject m_gridSelectionPanel;

    public void OnClickPlay()
    {
        m_gridSelectionPanel.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
    
    
}
