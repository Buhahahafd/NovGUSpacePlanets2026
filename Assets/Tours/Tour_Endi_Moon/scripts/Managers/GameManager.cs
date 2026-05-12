using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Главный менеджер приложения. Singleton, точка входа.
    /// Инициализирует StoryManager и запускает сценарий.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Ссылки на менеджеры")]
        [SerializeField] private StoryManager storyManager;
        [SerializeField] private AudioManager audioManager;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private QuestManager questManager;
        [SerializeField] private QuizManager quizManager;

        public StoryManager Story => storyManager;
        public AudioManager Audio => audioManager;
        public UIManager UI => uiManager;
        public QuestManager Quest => questManager;
        public QuizManager Quiz => quizManager;

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Если ссылки не проставлены в Inspector — пробуем найти автоматически на том же объекте
            if (storyManager == null) storyManager = GetComponent<StoryManager>();
            if (audioManager == null) audioManager = GetComponent<AudioManager>();
            if (uiManager == null) uiManager = GetComponent<UIManager>();
            if (questManager == null) questManager = GetComponent<QuestManager>();
            if (quizManager == null) quizManager = GetComponent<QuizManager>();
        }

        private void Start()
        {
            Debug.Log("[GameManager] Игра запущена.");

            if (storyManager == null)
            {
                Debug.LogError("[GameManager] StoryManager не найден! Сценарий не будет запущен.");
                return;
            }

            storyManager.StartStory();
        }
    }
}
