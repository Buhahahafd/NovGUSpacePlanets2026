using System.Collections;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Менеджер озвучки. Проигрывает голос за кадром на каждом этапе сценария.
    /// Подписывается на OnStateChanged StoryManager-а.
    /// Озвучки соответствуют сценарию из ТЗ "Луна".
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("AudioSource для голоса за кадром")]
        [SerializeField] private AudioSource narratorSource;

        [Header("Аудиоклипы по этапам сценария")]
        [Tooltip("Стартовый экран: 'Сегодня, дорогой исследователь, ты познакомишься...'")]
        [SerializeField] private AudioClip clipStart;

        [Tooltip("Высадка: рассказ о Луне, расстоянии, Аполлоне-11")]
        [SerializeField] private AudioClip clipLanding;

        [Tooltip("Начало квеста: 'Давай исследуем поверхность Луны...'")]
        [SerializeField] private AudioClip clipExplorationIntro;

        [Tooltip("Факты во время поиска: про кратеры, атмосферу, приливы")]
        [SerializeField] private AudioClip clipExplorationFacts;

        [Tooltip("При находке ботинка")]
        [SerializeField] private AudioClip clipBootFound;

        [Tooltip("При находке детали лунохода")]
        [SerializeField] private AudioClip clipMetalFound;

        [Tooltip("При находке блокнота")]
        [SerializeField] private AudioClip clipNotebookFound;

        [Tooltip("'Давай заберём всё это с собой...'")]
        [SerializeField] private AudioClip clipCollecting;

        [Tooltip("Финальная реплика (хороший результат, 3-4 верных)")]
        [SerializeField] private AudioClip clipEndGood;

        [Tooltip("Финальная реплика (отличный результат, 5 верных)")]
        [SerializeField] private AudioClip clipEndPerfect;

        [Tooltip("Финальная реплика (плохой результат, 0-2 верных)")]
        [SerializeField] private AudioClip clipEndBad;

        [Tooltip("'Ну что ж, отправимся в новое приключение'")]
        [SerializeField] private AudioClip clipNextAdventure;

        private StoryManager story;

        private void Start()
        {
            if (narratorSource == null)
            {
                narratorSource = gameObject.AddComponent<AudioSource>();
                narratorSource.playOnAwake = false;
                narratorSource.spatialBlend = 0f; // 2D — голос за кадром
            }

            // Подписка на смену этапов
            story = GameManager.Instance != null ? GameManager.Instance.Story : FindFirstObjectByType<StoryManager>();
            if (story != null)
                story.OnStateChanged += HandleStateChanged;
            else
                Debug.LogWarning("[AudioManager] StoryManager не найден.");
        }

        private void OnDestroy()
        {
            if (story != null)
                story.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Orbit:
                    // Можно проиграть короткий звук "посадки" или ничего
                    break;
                case GameState.Landing:
                    PlayAndAdvance(clipLanding, GameState.Exploration);
                    break;
                case GameState.Exploration:
                    Play(clipExplorationIntro);
                    StartCoroutine(PlayAfterDelay(
                        clipExplorationFacts,
                        clipExplorationIntro != null ? clipExplorationIntro.length + 1f : 5f));
                    break;
                case GameState.Quest:
                    break;
                case GameState.Collecting:
                    Play(clipCollecting);
                    break;
                case GameState.End:
                    PlayEnd();
                    break;
            }
        }

        /// <summary>Проигрывает звук по типу найденного артефакта.</summary>
        public void PlayArtifactFoundClip(ArtifactType type)
        {
            switch (type)
            {
                case ArtifactType.Boot:     Play(clipBootFound); break;
                case ArtifactType.Metal:    Play(clipMetalFound); break;
                case ArtifactType.Notebook: Play(clipNotebookFound); break;
            }
        }

        public void PlayStartScreenIntro() => Play(clipStart);
        public AudioClip GetStartClip() => clipStart;

        private void Play(AudioClip clip)
        {
            if (clip == null || narratorSource == null) return;
            narratorSource.Stop();
            narratorSource.clip = clip;
            narratorSource.Play();
            Debug.Log($"[AudioManager] Играет: {clip.name}");
        }

        private void PlayAndAdvance(AudioClip clip, GameState nextState)
        {
            Play(clip);
            float wait = clip != null ? clip.length : 1f;
            StartCoroutine(AdvanceAfter(wait, nextState));
        }

        private IEnumerator AdvanceAfter(float seconds, GameState nextState)
        {
            yield return new WaitForSeconds(seconds);
            if (story != null && story.GetCurrentStage() != nextState)
                story.SetStage(nextState);
        }

        private IEnumerator PlayAfterDelay(AudioClip clip, float delay)
        {
            yield return new WaitForSeconds(delay);
            Play(clip);
        }

        private void PlayEnd()
        {
            int correct = GameManager.Instance != null && GameManager.Instance.Quiz != null
                ? GameManager.Instance.Quiz.GetCorrectAnswersCount()
                : 0;

            AudioClip clip;
            if (correct == 5)       clip = clipEndPerfect;
            else if (correct >= 3)  clip = clipEndGood;
            else                    clip = clipEndBad;

            Play(clip);
            StartCoroutine(PlayAfterDelay(clipNextAdventure, clip != null ? clip.length + 1f : 4f));
        }
    }
}
