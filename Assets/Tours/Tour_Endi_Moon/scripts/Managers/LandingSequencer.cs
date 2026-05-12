using System.Collections;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Управляет последовательностью высадки:
    /// ждёт <see cref="orbitPauseDuration"/> секунд после входа в состояние <see cref="GameState.Orbit"/>,
    /// затем переводит сценарий в <see cref="GameState.Landing"/>, давая пользователю
    /// осмотреться и услышать озвучку о Луне.
    /// После окончания озвучки автоматически переходит в <see cref="GameState.Exploration"/>.
    /// </summary>
    public class LandingSequencer : MonoBehaviour
    {
        [Header("Пауза после загрузки сцены перед переходом в Landing, сек")]
        [SerializeField] private float orbitPauseDuration = 3f;

        private StoryManager storyManager;
        private AudioManager audioManager;

        private void Start()
        {
            storyManager = GameManager.Instance != null
                ? GameManager.Instance.Story
                : FindFirstObjectByType<StoryManager>();

            audioManager = GameManager.Instance != null
                ? GameManager.Instance.Audio
                : FindFirstObjectByType<AudioManager>();

            if (storyManager == null)
            {
                Debug.LogError("[LandingSequencer] StoryManager не найден.");
                return;
            }

            storyManager.OnStateChanged += HandleStateChanged;

            // Если StoryManager уже перешёл в Orbit до того, как мы успели подписаться
            if (storyManager.GetCurrentStage() == GameState.Orbit)
                StartCoroutine(BeginLandingSequence());
        }

        private void OnDestroy()
        {
            if (storyManager != null)
                storyManager.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Orbit)
                StartCoroutine(BeginLandingSequence());
        }

        /// <summary>
        /// Пауза → Landing (озвучка запускается в AudioManager) → ждём конца озвучки → Exploration.
        /// </summary>
        private IEnumerator BeginLandingSequence()
        {
            // Даём пользователю осмотреться
            yield return new WaitForSeconds(orbitPauseDuration);

            storyManager.SetStage(GameState.Landing);

            // Ждём, пока AudioManager проигрывает озвучку о Луне
            yield return new WaitForSeconds(GetLandingClipDuration() + 1f);

            // Переходим к исследованию
            if (storyManager.GetCurrentStage() == GameState.Landing)
                storyManager.SetStage(GameState.Exploration);
        }

        /// <summary>Возвращает длину клипа Landing из AudioManager (если есть), иначе — дефолт.</summary>
        private float GetLandingClipDuration()
        {
            const float defaultDuration = 55f;
            return audioManager != null ? audioManager.GetLandingClipDuration() : defaultDuration;
        }
    }
}
