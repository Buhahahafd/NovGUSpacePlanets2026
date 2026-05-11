using Newtonsoft.Json.Bson;
using System.Drawing;
using UnityEditor.TerrainTools;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private UIManager uiManager;

    [Header("Player")]
    [SerializeField] private Transform player;

    [Header("Checkpoints")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform atmospherePoint;
    [SerializeField] private Transform observationPoint;
    [SerializeField] private Transform questPoint;
    [SerializeField] private Transform returnPoint;

    [Header("Story Objects")]
    [SerializeField] private GameObject saturn;
    [SerializeField] private GameObject terrain;
    [SerializeField] private GameObject titan;
    [SerializeField] private GameObject cassini;
    [SerializeField] private GameObject transmitter;
    [SerializeField] private GameObject sun;

    private int _index;
    public Stage CurrentStage;

    public enum Stage
    {
        Start,
        Atmosphere,
        Observation,
        ResearchHistory,
        Quest,
        Return,
        Quiz,
        End
    }

    public void StoryStart()
    {
        _index = 0;
        SetStage();

        GetCurrentStage();
    }

    public void NextStage()
    {
        _index++;

        if (_index >= System.Enum.GetValues(typeof(Stage)).Length)
        {
            Debug.Log("Конец сценария");
            return;
        }

        SetStage();
    }

    public void SetStage()
    {
        CurrentStage = (Stage)_index;

        GetCurrentStage();

        //HideObjects();

        switch (CurrentStage)
        {
            case Stage.Start:
                MovePlayer(startPoint);

                saturn.SetActive(true);

                //audioManager.PlayStartAudio();
                //uiManager.ShowStartScreen();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Atmosphere:
                MovePlayer(atmospherePoint);

                saturn.SetActive(false);
                terrain.SetActive(true);

                //audioManager.PlayAtmosphereAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Observation:
                MovePlayer(observationPoint);

                sun.SetActive(true);
                titan.SetActive(true);

                //audioManager.PlayObservationAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.ResearchHistory:
                cassini.SetActive(true);

                //audioManager.PlayResearchAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Quest:
                MovePlayer(questPoint);

                transmitter.SetActive(true);

                //audioManager.PlayQuestAudio();
                //uiManager.ShowQuestUI();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Return:
                MovePlayer(returnPoint);

                HideObjects();
                saturn.SetActive(true);
                
                //audioManager.PlayReturnAudio();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.Quiz:
                //uiManager.ShowQuiz();

                Invoke(nameof(NextStage), 5f);

                break;

            case Stage.End:
                //uiManager.ShowFinalScreen();
                //audioManager.PlayEndAudio();

                Invoke(nameof(NextStage), 5f);

                break;
        }
    }

    public void GetCurrentStage()
    {
        Debug.Log($"Текущая стадия: {CurrentStage}");
    }

    private void MovePlayer(Transform point)
    {
        player.position = point.position;
        player.rotation = point.rotation;
    }

    private void HideObjects()
    {
        terrain.SetActive(false);
        titan.SetActive(false);
        cassini.SetActive(false);
        transmitter.SetActive(false);
    }
}
