using UnityEngine;

public enum GameState
{
    Start,
    Orbit,
    Landing,
    Exploration,
    Quest,
    Quiz,
    End
}

public class StoryManager : MonoBehaviour
{
    private GameState currentState;

    public void StartStory()
    {
        currentState = GameState.Start;
        Debug.Log("Сценарий запущен. Этап: " + currentState);
    }

    public void NextStage()
    {
        if (currentState < GameState.End)
        {
            currentState++;
            Debug.Log("Переход к этапу: " + currentState);
        }
        else
        {
            Debug.Log("Сценарий завершён.");
        }
    }

    public void SetStage(GameState state)
    {
        currentState = state;
        Debug.Log("Установлен этап: " + currentState);
    }

    public GameState GetCurrentStage()
    {
        return currentState;
    }
}