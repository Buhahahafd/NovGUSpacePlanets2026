using TMPro;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Менеджер UI. Показывает субтитры и подсказки.
    /// Подписан на смену этапов сценария.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("World-Space Canvas с подсказками")]
        [SerializeField] private GameObject hintCanvas;
        [SerializeField] private TMP_Text hintText;

        [Header("Счётчик собранных артефактов")]
        [SerializeField] private TMP_Text artifactCounterText;

        private StoryManager story;
        private QuestManager quest;

        private void Start()
        {
            story = GameManager.Instance != null ? GameManager.Instance.Story : FindFirstObjectByType<StoryManager>();
            quest = GameManager.Instance != null ? GameManager.Instance.Quest : FindFirstObjectByType<QuestManager>();

            if (story != null) story.OnStateChanged += HandleStateChanged;
            if (quest != null)
            {
                quest.OnArtifactUncovered += _ => RefreshCounter();
                quest.OnArtifactStored += _ => RefreshCounter();
            }
            RefreshCounter();
        }

        private void OnDestroy()
        {
            if (story != null) story.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Landing:
                    ShowHint("Осмотрись вокруг — ты на Луне.");
                    break;
                case GameState.Exploration:
                    ShowHint("Возьми кисточку и лопатку у корабля и найди артефакты.");
                    break;
                case GameState.Quest:
                case GameState.Collecting:
                    ShowHint("Отнеси артефакты к ящику возле корабля.");
                    break;
                case GameState.Return:
                    ShowHint("Возвращайся на стартовую площадку.");
                    break;
                case GameState.Quiz:
                    ShowHint("Ответь на 5 вопросов о Луне.");
                    break;
                case GameState.End:
                    ShowHint("Спасибо за путешествие!");
                    break;
            }
        }

        public void ShowHint(string text)
        {
            if (hintCanvas != null) hintCanvas.SetActive(true);
            if (hintText != null) hintText.text = text;
            Debug.Log($"[UIManager] Подсказка: {text}");
        }

        public void HideHint()
        {
            if (hintCanvas != null) hintCanvas.SetActive(false);
        }

        private void RefreshCounter()
        {
            if (artifactCounterText == null || quest == null) return;
            artifactCounterText.text = $"Артефакты: {quest.UncoveredCount}/3   В ящике: {quest.StoredCount}/3";
        }
    }
}
