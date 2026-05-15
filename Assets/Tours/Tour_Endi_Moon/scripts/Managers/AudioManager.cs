using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Менеджер озвучки. Проигрывает голос за кадром на каждом этапе сценария.
    /// Реплики ставятся в очередь и никогда не перебивают друг друга.
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

        // Очередь клипов — реплики никогда не перебивают друг друга
        private readonly Queue<AudioClip> clipQueue = new();
        private Coroutine playbackCoroutine;

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
                    break;

                case GameState.Landing:
                    Enqueue(clipLanding);
                    StartCoroutine(AdvanceAfterClip(clipLanding, GameState.Exploration));
                    break;

                case GameState.Exploration:
                    Enqueue(clipExplorationIntro);
                    Enqueue(clipExplorationFacts);
                    break;

                case GameState.Quest:
                    break;

                case GameState.Collecting:
                    Enqueue(clipCollecting);
                    break;

                case GameState.End:
                    PlayEnd();
                    break;
            }
        }

        /// <summary>Проигрывает звук по типу найденного артефакта (ставит в очередь).</summary>
        public void PlayArtifactFoundClip(ArtifactType type)
        {
            switch (type)
            {
                case ArtifactType.Boot:     Enqueue(clipBootFound);     break;
                case ArtifactType.Metal:    Enqueue(clipMetalFound);    break;
                case ArtifactType.Notebook: Enqueue(clipNotebookFound); break;
            }
        }

        public void PlayStartScreenIntro() => Enqueue(clipStart);

        public AudioClip GetStartClip() => clipStart;

        /// <summary>Возвращает длину клипа высадки (Landing). Используется в LandingSequencer.</summary>
        public float GetLandingClipDuration() => clipLanding != null ? clipLanding.length : 55f;

        // ─── Очередь воспроизведения ─────────────────────────────────────────

        /// <summary>Добавляет клип в очередь. Если очередь была пуста — запускает воспроизведение.</summary>
        private void Enqueue(AudioClip clip)
        {
            if (clip == null) return;
            clipQueue.Enqueue(clip);
            Debug.Log($"[AudioManager] Очередь +«{clip.name}» (в очереди: {clipQueue.Count})");
            if (playbackCoroutine == null)
                playbackCoroutine = StartCoroutine(PlayQueue());
        }

        private IEnumerator PlayQueue()
        {
            while (clipQueue.Count > 0)
            {
                AudioClip next = clipQueue.Dequeue();
                if (next == null) continue;

                narratorSource.clip = next;
                narratorSource.Play();
                Debug.Log($"[AudioManager] Играет: «{next.name}»");

                yield return new WaitWhile(() => narratorSource.isPlaying);
                yield return new WaitForSeconds(0.3f);
            }
            playbackCoroutine = null;
        }

        // ─── Вспомогательные методы ──────────────────────────────────────────

        private IEnumerator AdvanceAfterClip(AudioClip clip, GameState nextState)
        {
            float wait = clip != null ? clip.length : 1f;
            yield return new WaitForSeconds(wait + 1f);
            if (story != null && story.GetCurrentStage() != nextState)
                story.SetStage(nextState);
        }

        private void PlayEnd()
        {
            int correct = GameManager.Instance != null && GameManager.Instance.Quiz != null
                ? GameManager.Instance.Quiz.GetCorrectAnswersCount()
                : 0;

            AudioClip resultClip = correct == 5 ? clipEndPerfect
                                 : correct >= 3 ? clipEndGood
                                 : clipEndBad;

            Enqueue(resultClip);
            Enqueue(clipNextAdventure);
        }
    }
}