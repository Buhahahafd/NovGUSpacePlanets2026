using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Управляет доступностью инструментов (кисточка и лопатка).
    /// Инструменты неактивны до перехода в GameState.Exploration —
    /// чтобы игрок не мог взять их до окончания озвучки о Луне.
    /// </summary>
    public class ToolsController : MonoBehaviour
    {
        [Header("Инструменты для расчистки грунта")]
        [SerializeField] private GameObject shovel;
        [SerializeField] private GameObject paintBrush;

        private StoryManager storyManager;

        private void Start()
        {
            storyManager = GameManager.Instance != null
                ? GameManager.Instance.Story
                : FindFirstObjectByType<StoryManager>();

            // Скрываем инструменты в начале
            SetToolsActive(false);

            if (storyManager == null)
            {
                Debug.LogError("[ToolsController] StoryManager не найден.");
                return;
            }

            storyManager.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (storyManager != null)
                storyManager.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Exploration)
                SetToolsActive(true);
        }

        /// <summary>Показывает или скрывает инструменты.</summary>
        private void SetToolsActive(bool active)
        {
            if (shovel != null) shovel.SetActive(active);
            if (paintBrush != null) paintBrush.SetActive(active);
        }
    }
}
