using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [HideInInspector]
    public bool m_IsPaused;

    Player m_Player;
    Vector3 m_StartPos = Vector3.zero;
    Vector3 m_CheckPointPos = Vector3.zero;
    int m_Collectibles = 0;
    List<GameObject> m_CollectibleList;
    bool m_HasWon = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        m_CollectibleList = new List<GameObject>();
        RegisterCollectible();
    }

    private void Start()
    {
        UIManager.Instance.SetCollectible(m_Collectibles.ToString() + " / " + m_CollectibleList.Count.ToString());
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
            m_HasWon = true;
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
        m_Player.ResetAbility();

        // Only if won
        if (m_HasWon)
        {
            m_Collectibles = 0;
            UIManager.Instance.SetCollectible(m_Collectibles.ToString() + " / " + m_CollectibleList.Count.ToString());

            ResetCollectibles();
            m_HasWon = false;
        }

        m_IsPaused = true;
        Pause();
    }

    public void ResetCollectibles()
    {
        foreach (GameObject obj in m_CollectibleList)
        {
            obj.SetActive(true);
        }
    }

    public void RegisterCollectible()
    {
        foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Collectible"))
            {
            m_CollectibleList.Add(obj);
        }
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

    public void Collect()
    {
        m_Collectibles++;
        UIManager.Instance.SetCollectible(m_Collectibles.ToString() + " / " + m_CollectibleList.Count.ToString());
    }
}
