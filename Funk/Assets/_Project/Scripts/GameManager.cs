using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [HideInInspector]
    public bool m_IsPaused;

    Player m_Player;
    Vector3 m_StartPos = Vector3.zero;
    Vector3 m_CheckPointPos = Vector3.zero;

    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        m_IsPaused = !m_IsPaused;
        UIManager.Instance.Pause(m_IsPaused);
        if (m_IsPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void End(bool aWin)
    {
        Pause();

        if (aWin)
        {
            SetCheckPoint(m_StartPos);
            UIManager.Instance.End("You Win!");
        }
        else
            UIManager.Instance.End("Game Over");
    }

    public void Restart()
    {
        // Reset Player position
        m_Player.gameObject.transform.position = m_CheckPointPos;

        m_IsPaused = true;
        Pause();
    }

    public void Register(Player aPlayer)
    {
        m_Player = aPlayer;
        m_StartPos = aPlayer.gameObject.transform.position;
        SetCheckPoint(m_StartPos);
    }

    public void SetCheckPoint(Vector3 aPos)
    {
        m_CheckPointPos = aPos;
    }
}
