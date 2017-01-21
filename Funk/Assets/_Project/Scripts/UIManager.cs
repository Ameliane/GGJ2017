using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance = null;

    [HideInInspector]
    public bool m_IsPaused = true;

    [Header("Splash Screen")]

    [SerializeField]
    GameObject m_SplashScreen;

    [SerializeField]
    float m_SplashScreenTime;

    [Header("HUD")]

    [SerializeField]
    GameObject m_HUD;

    [SerializeField]
    Text m_AbilityText;

    [Header("Pause Menu")]

    [SerializeField]
    GameObject m_PauseMenu;



    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        StartCoroutine(SplashScreen_cr());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_IsPaused = !m_IsPaused;
            m_PauseMenu.SetActive(m_IsPaused);
        }
    }

    public void SetAbility(string aAbility)
    {
        m_AbilityText.text = aAbility;
    }

    public void OnQuit()
    {
        Application.Quit();
    }

    private IEnumerator SplashScreen_cr()
    {
        float t = 0;
        while (t < m_SplashScreenTime)
        {
            t += Time.deltaTime;
            yield return null;
        }
        m_SplashScreen.SetActive(false);
        m_HUD.SetActive(true);
        m_IsPaused = false;

    }
}
