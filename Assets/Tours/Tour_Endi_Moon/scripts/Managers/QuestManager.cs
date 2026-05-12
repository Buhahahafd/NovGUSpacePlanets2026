using System;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Менеджер квеста: отслеживает откопанные и сложенные в ящик артефакты,
    /// инициирует переходы по сценарию.
    /// </summary>
    public class QuestManager : MonoBehaviour
    {
        public const int TotalArtifacts = 3;

        public int UncoveredCount { get; private set; }
        public int StoredCount { get; private set; }

        public event Action<ArtifactType> OnArtifactUncovered;
        public event Action<ArtifactType> OnArtifactStored;

        private StoryManager story;

        private void Start()
        {
            story = GameManager.Instance != null ? GameManager.Instance.Story : FindFirstObjectByType<StoryManager>();
        }

        /// <summary>Вызывается из Artifact, когда его откопали.</summary>
        public void RegisterUncovered(ArtifactType type)
        {
            UncoveredCount++;
            Debug.Log($"[QuestManager] Откопан артефакт {type}. Всего откопано: {UncoveredCount}/{TotalArtifacts}");

            // Озвучка реакции
            if (GameManager.Instance != null && GameManager.Instance.Audio != null)
                GameManager.Instance.Audio.PlayArtifactFoundClip(type);

            OnArtifactUncovered?.Invoke(type);

            if (UncoveredCount >= TotalArtifacts && story != null && story.GetCurrentStage() == GameState.Exploration)
            {
                story.SetStage(GameState.Quest);
                story.SetStage(GameState.Collecting);
            }
        }

        /// <summary>Вызывается из ArtifactBox при попадании артефакта в ящик.</summary>
        public void RegisterStored(ArtifactType type)
        {
            StoredCount++;
            Debug.Log($"[QuestManager] В ящик положен {type}. Всего в ящике: {StoredCount}/{TotalArtifacts}");

            OnArtifactStored?.Invoke(type);

            if (StoredCount >= TotalArtifacts && story != null)
            {
                story.SetStage(GameState.Return);
                // Дальше игрок сам подходит к корабля → ReturnZone триггерит Quiz
            }
        }
    }
}
