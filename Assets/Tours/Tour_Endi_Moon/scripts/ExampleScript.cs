using UnityEngine;

public class ExampleScript : MonoBehaviour
{
    public int health = 100;
    public string playerName = "Player";
    public bool isAlive = true;
    public float speed = 5.5f;

    void Start()
    {
        Debug.Log("ExampleScript запущен!");
        PrintInfo();
    }

    void Update()
    {
        Debug.Log("Кадр обновлён");
    }

    void PrintInfo()
    {
        Debug.Log("Имя: " + playerName + ", Здоровье: " + health);
    }
}