using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace MoonGame
{
    /// <summary>
    /// Контроллер телепортации между кораблём и стартовой площадкой.
    /// Использует ScreenFader для градиентного перехода: локация A → чёрный → локация B.
    /// Подписывается на GameState для автоматического срабатывания:
    ///   Exploration → телепортирует игрока из корабля на площадку.
    ///   Quiz        → телепортирует игрока с площадки на корабль.
    ///
    /// Также предоставляет публичный метод Teleport() для ручного вызова.
    /// </summary>
    public class TeleportController : MonoBehaviour
    {
        [Header("Точки назначения")]
        [Tooltip("Позиция и поворот игрока на стартовой площадке (снаружи корабля)")]
        [SerializeField] private Transform landingSpot;

        [Tooltip("Позиция и поворот игрока на борту корабля (квиз)")]
        [SerializeField] private Transform shipSpot;

        [Header("XR Rig — объект, который телепортируем")]
        [SerializeField] private Transform xrRig;

        [Header("Фейдер экрана")]
        [SerializeField] private ScreenFader fader;

        [Header("Звук телепорта")]
        [SerializeField] private AudioSource teleportAudioSource;
        [SerializeField] private AudioClip teleportClip;

        [Header("Задержка после появления перед снятием чёрного (сек)")]
        [SerializeField] private float settleDelay = 0.15f;

        private StoryManager story;

        private void Start()
        {
            story = GameManager.Instance != null
                ? GameManager.Instance.Story
                : FindFirstObjectByType<StoryManager>();

            if (story != null)
                story.OnStateChanged += HandleStateChanged;

            // Авто-поиск XR Rig если не задан
            if (xrRig == null)
            {
                var rig = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
                if (rig != null) xrRig = rig.transform;
            }
        }

        private void OnDestroy()
        {
            if (story != null)
                story.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.Exploration && landingSpot != null)
                StartCoroutine(DoTeleport(landingSpot));
            else if (state == GameState.Quiz && shipSpot != null)
                StartCoroutine(DoTeleport(shipSpot));
        }

        /// <summary>Ручной вызов телепортации в произвольную точку.</summary>
        public void Teleport(Transform destination)
        {
            if (destination == null) return;
            StartCoroutine(DoTeleport(destination));
        }

        private IEnumerator DoTeleport(Transform destination)
        {
            if (fader == null)
            {
                MoveRig(destination);
                yield break;
            }

            // Затемнение
            yield return StartCoroutine(fader.FadeOut());

            // Звук телепорта
            PlayTeleportSound();

            // Небольшая пауза на чёрном
            yield return new WaitForSeconds(settleDelay);

            // Перемещение
            MoveRig(destination);

            // Осветление
            yield return StartCoroutine(fader.FadeIn());
        }

        private void MoveRig(Transform destination)
        {
            if (xrRig == null)
            {
                Debug.LogError("[TeleportController] xrRig не задан.");
                return;
            }

            xrRig.SetPositionAndRotation(destination.position, destination.rotation);
            Debug.Log($"[TeleportController] Телепорт → {destination.name}");
        }

        private void PlayTeleportSound()
        {
            if (teleportAudioSource == null || teleportClip == null) return;
            teleportAudioSource.PlayOneShot(teleportClip);
        }
    }
}
