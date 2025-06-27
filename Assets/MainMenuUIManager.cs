using UnityEngine;
using UnityEngine.UI;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject machingView;

    [SerializeField]
    private Button randomMachButton;

    [SerializeField]
    private Button machCancelButton;

    private void Start()
    {
        randomMachButton.onClick.AddListener(StartRandomMatching);
        machCancelButton.onClick.AddListener(CancelMatching);
    }

    private void StartRandomMatching()
    {
        NetworkManager.Instance.StartRandomMatching();
        machingView.SetActive(true);
    }

    private void CancelMatching()
    {
        NetworkManager.Instance.CancelMatching();
        machingView.SetActive(false);
    }
}
