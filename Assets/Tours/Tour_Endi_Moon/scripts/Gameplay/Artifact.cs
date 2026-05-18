using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MoonGame
{
    /// <summary>
    /// Артефакт, спрятанный в кучке земли.
    /// После откапывания остаётся лежать на месте с лунной гравитацией,
    /// пока игрок сам не возьмёт его в руку.
    /// После укладки в ящик — полностью блокируется (см. ArtifactBox).
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Artifact : MonoBehaviour
    {
        [Header("Тип артефакта")]
        public ArtifactType type;

        [Header("Подъём после откопки")]
        [SerializeField] private float riseHeight = 0.15f;
        [SerializeField] private float riseDuration = 0.6f;

        [Header("Лунная гравитация (м/с²). Реальная ~1.62, для геймплея ~1.2–1.8)")]
        [SerializeField] private float lunarGravity = 1.62f;

        [Header("Линейное сопротивление для плавного падения (0.5–1.5)")]
        [SerializeField] private float linearDamping = 1.0f;

        [Header("XR Grab Interactable (будет активирован после откопки)")]
        [SerializeField] private XRGrabInteractable grabInteractable;

        private Rigidbody rb;
        private Coroutine gravityCoroutine;

        public bool IsUncovered { get; private set; }

        /// <summary>true после того как артефакт зафиксирован в ящике.</summary>
        public bool IsStored { get; private set; }

        private Vector3 riseStartPos;
        private Vector3 riseEndPos;
        private float riseT;
        private bool rising;

        private void Awake()
        {
            if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
                // Прерываем анимацию подъёма при захвате, чтобы Update не перезаписывал позицию
                grabInteractable.selectEntered.AddListener(_ => CancelRise());
            }

            rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }

        private void Update()
        {
            if (!rising) return;

            riseT += Time.deltaTime / riseDuration;
            transform.position = Vector3.Lerp(riseStartPos, riseEndPos, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(riseT)));

            if (riseT >= 1f)
            {
                rising = false;
                EnablePhysics();
                if (grabInteractable != null) grabInteractable.enabled = true;
                Debug.Log($"[Artifact:{type}] готов к подъёму игроком.");
            }
        }

        /// <summary>Прерывает анимацию подъёма при захвате артефакта игроком.</summary>
        private void CancelRise()
        {
            if (!rising) return;
            rising = false;
            EnablePhysics();
        }

        /// <summary>Вызывается из SandPile, когда кучка полностью откопана.</summary>
        public void Uncover(Vector3 spawnPosition)
        {
            if (IsUncovered || IsStored) return;
            IsUncovered = true;

            gameObject.SetActive(true);
            transform.position = spawnPosition;

            riseStartPos = spawnPosition;
            riseEndPos = spawnPosition + Vector3.up * riseHeight;
            riseT = 0f;
            rising = true;

            Debug.Log($"[Artifact:{type}] откопан на позиции {spawnPosition}.");

            if (GameManager.Instance?.Quest != null)
                GameManager.Instance.Quest.RegisterUncovered(type);

            if (GameManager.Instance?.Audio != null)
                GameManager.Instance.Audio.PlayArtifactFoundClip(type);
        }

        /// <summary>
        /// Вызывается из ArtifactBox при фиксации артефакта в коробке.
        /// После этого SandPile не сможет повторно «открыть» этот артефакт.
        /// </summary>
        public void MarkStored()
        {
            IsStored = true;
            IsUncovered = true; // на всякий случай
        }

        /// <summary>Включает лунную гравитацию через кастомную силу.</summary>
        private void EnablePhysics()
        {
            if (rb == null) return;
            rb.isKinematic = false;
            rb.useGravity = false;            // стандартная гравитация отключена — применяем лунную
            rb.linearDamping = linearDamping; // плавное торможение — имитация лёгкого сопротивления
            if (gravityCoroutine != null) StopCoroutine(gravityCoroutine);
            gravityCoroutine = StartCoroutine(ApplyLunarGravity());
        }

        private IEnumerator ApplyLunarGravity()
        {
            while (rb != null && !rb.isKinematic)
            {
                rb.AddForce(Vector3.down * lunarGravity, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
