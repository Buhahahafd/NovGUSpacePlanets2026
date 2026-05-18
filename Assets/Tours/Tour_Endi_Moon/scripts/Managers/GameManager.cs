using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Главный менеджер приложения. Singleton, точка входа.
    /// Инициализирует StoryManager и запускает сценарий с этапа Intro.
    /// Вся игра проходит в одной сцене moon.unity.
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
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

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

            // Всегда стартуем с Intro (внутри корабля — вращающаяся Луна + озвучка)
            storyManager.SetStageForce(GameState.Intro);
        }
    }
}
