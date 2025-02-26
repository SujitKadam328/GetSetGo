using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject m_gridSelectionPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPlay()
    {
        m_gridSelectionPanel.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
    
    
}
