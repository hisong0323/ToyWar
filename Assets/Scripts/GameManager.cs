using Fusion;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject gameBoard;

    [SerializeField]
    private GameObject[] playerBase;
    public static GameManager Instance { get; private set; }

    public GameObject GameBoard => gameBoard;
    public GameObject[] PlayerBase => playerBase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
