using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject questUI;
    [SerializeField] private GameObject quizUI;
    [SerializeField] private GameObject finalScreen;

    public void HideAllUI()
    {
        startScreen.SetActive(false);
        questUI.SetActive(false);
        quizUI.SetActive(false);
        finalScreen.SetActive(false);
    }

    public void ShowStartScreen()
    {
        HideAllUI();
        startScreen.SetActive(true);
    }

    public void ShowQuestUI()
    {
        HideAllUI();
        questUI.SetActive(true);
    }

    public void ShowQuiz()
    {
        HideAllUI();
        quizUI.SetActive(true);
    }

    public void ShowFinalScreen()
    {
        HideAllUI();
        finalScreen.SetActive(true);
    }
}
