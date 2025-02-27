using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SplashScreenManager : MonoBehaviour
{
    [SerializeField] private float delayBeforeLoading = 3f;
    [SerializeField] private TextMeshProUGUI m_splashScreenText = null;
    [SerializeField] private GameObject m_mainMenuPanel = null;
    [SerializeField] private float tickInterval = 0.5f; // How fast the ! blinks

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadMainMenuAfterDelay());
        StartCoroutine(AnimateExclamation());
    }

    IEnumerator LoadMainMenuAfterDelay()
    {
        m_splashScreenText.text = "Get Set Go !";
        yield return new WaitForSeconds(delayBeforeLoading);
        this.gameObject.SetActive(false);
        m_mainMenuPanel.SetActive(true);
    }

    IEnumerator AnimateExclamation()
    {
        while (true)
        {
            m_splashScreenText.text = "Get Set Go.";
            yield return new WaitForSeconds(tickInterval);
            m_splashScreenText.text = "Get Set Go..";
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
