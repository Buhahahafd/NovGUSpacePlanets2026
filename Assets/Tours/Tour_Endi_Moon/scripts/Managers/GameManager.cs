using UnityEngine;

public class GameManager : MonoBehaviour
{
    public StoryManager storyManager;

    void Start()
    {
        Debug.Log("GameManager: Игра запущена!");

        if (storyManager != null)
        {
            storyManager.StartStory();
            storyManager.NextStage();
            storyManager.NextStage();
        }
        else
        {
            Debug.LogWarning("StoryManager не назначен!");
        }
    }
}