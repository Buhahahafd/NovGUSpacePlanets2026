using Newtonsoft.Json.Bson;
using UnityEngine;

public class StoryManager : MonoBehaviour
{
    private Stage _currentStage;
    public int index;

    public enum Stage
    {
        Start,
        Orbit,
        Landing,
        Exploration,
        Quest,
        Quiz,
        End
    }

    public void StoryStart()
    {
        Debug.Log("Сценарий запущен");

        index = 0;
        SetStage();

        GetCurrentStage();
    }

    public void NextStage()
    {
        if (index <= 6)
            index++;
        else
            Debug.Log("Конец");

        SetStage();
    }

    public void SetStage()
    {
        _currentStage = (Stage)index;

        switch (_currentStage)
        {
            case Stage.Start:
                Debug.Log("Старт игры");
                break;

            case Stage.Orbit:
                Debug.Log("Игрок на орбите");
                break;

            case Stage.Landing:
                Debug.Log("Приземление");
                break;

            case Stage.Exploration:
                Debug.Log("Игрок исследует");
                break;

            case Stage.Quest:
                Debug.Log("Прохождение квеста");
                break;

            case Stage.Quiz:
                Debug.Log("Прохождение викторины");
                break;

            case Stage.End:
                Debug.Log("Конец");
                break;
        }
    }

    public void GetCurrentStage()
    {
        Debug.Log($"Текущая стадия: {_currentStage}");
    }
}
