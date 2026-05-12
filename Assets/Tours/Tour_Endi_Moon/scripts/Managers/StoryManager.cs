using System;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Менеджер сценария. Хранит текущее состояние и оповещает подписчиков о смене этапов.
    /// Согласно ТЗ-3: реализует StartStory, NextStage, SetStage, GetCurrentStage.
    /// </summary>
    public class StoryManager : MonoBehaviour
    {
        [Header("Текущее состояние (для отладки в Inspector)")]
        [SerializeField] private GameState currentState = GameState.Start;

        /// <summary>Событие смены этапа. На него подписываются AudioManager, UIManager, QuestManager и т.д.</summary>
        public event Action<GameState> OnStateChanged;

        /// <summary>Запуск сценария. Вызывается из GameManager.</summary>
        public void StartStory()
        {
            Debug.Log("[StoryManager] Сценарий стартовал.");
            SetStage(GameState.Orbit);
        }

        /// <summary>Переход на конкретный этап.</summary>
        public void SetStage(GameState newState)
        {
            if (newState == currentState)
            {
                Debug.LogWarning($"[StoryManager] Попытка установить тот же этап: {newState}");
                return;
            }

            currentState = newState;
            Debug.Log($"[StoryManager] Этап: {currentState}");
            OnStateChanged?.Invoke(currentState);
        }

        /// <summary>Переход на следующий этап по порядку enum.</summary>
        public void NextStage()
        {
            int next = (int)currentState + 1;
            if (next > (int)GameState.End)
            {
                Debug.Log("[StoryManager] Сценарий уже завершён.");
                return;
            }
            SetStage((GameState)next);
        }

        /// <summary>Получить текущий этап.</summary>
        public GameState GetCurrentStage() => currentState;
    }
}
