using System.Collections;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Кучка земли, которая погружается в грунт при каждом ударе лопатой.
    /// После достаточного числа ударов открывает вложенный артефакт.
    /// Опциональный артефакт: если не задан — кучка просто уходит вниз (пустая яма).
    /// </summary>
    public class SandPile : MonoBehaviour
    {
        [Header("Артефакт внутри кучки (null = пустая яма)")]
        [SerializeField] private Artifact hiddenArtifact;

        [Header("Сколько ударов лопатой нужно")]
        [SerializeField] private int requiredHits = 6;

        [Header("На сколько кучка опускается за удар (м)")]
        [SerializeField] private float sinkPerHit = 0.04f;

        [Header("Скорость погружения (м/с)")]
        [SerializeField] private float sinkSpeed = 0.3f;

        [Header("Задержка после последнего удара до раскрытия артефакта (сек)")]
        [SerializeField] private float revealDelay = 0.4f;

        [Header("Cooldown между ударами (сек)")]
        [SerializeField] private float hitCooldown = 0.3f;

        private int currentHits;
        private float lastHitTime;
        private bool isDug;
        private Vector3 targetPosition;
        private bool isSinking;

        public bool IsDug => isDug;

        private void Awake()
        {
            targetPosition = transform.position;

            // Прячем артефакт под землей, чтобы он не был виден и не interactable
            if (hiddenArtifact != null)
            {
                hiddenArtifact.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (!isSinking) return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                sinkSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                transform.position = targetPosition;
                isSinking = false;
            }
        }

        /// <summary>Вызывается из DiggingTool при попадании лопатой.</summary>
        public void RegisterHit()
        {
            if (isDug) return;
            if (Time.time - lastHitTime < hitCooldown) return;

            lastHitTime = Time.time;
            currentHits++;

            // Опускаем кучку на один шаг
            targetPosition -= Vector3.up * sinkPerHit;
            isSinking = true;

            Debug.Log($"[SandPile] {name}: удар {currentHits}/{requiredHits}");

            if (currentHits >= requiredHits)
                StartCoroutine(RevealArtifact());
        }

        private IEnumerator RevealArtifact()
        {
            isDug = true;
            yield return new WaitForSeconds(revealDelay);

            if (hiddenArtifact != null)
            {
                // Помещаем артефакт чуть выше поверхности кучки
                hiddenArtifact.gameObject.SetActive(true);
                hiddenArtifact.transform.position = transform.position + Vector3.up * 0.05f;
            }

            // Скрываем саму кучку
            gameObject.SetActive(false);
        }
    }
}
