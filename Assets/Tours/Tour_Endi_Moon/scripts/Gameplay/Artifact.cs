using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace MoonGame
{
    /// <summary>
    /// Артефакт в грунте. Откапывается касаниями кисточки/лопатки.
    /// После откапывания поднимается из грунта и становится Grab Interactable,
    /// чтобы игрок мог его взять и положить в ящик.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class Artifact : MonoBehaviour
    {
        [Header("Тип артефакта")]
        public ArtifactType type;

        [Header("Сколько касаний инструментом требуется")]
        [SerializeField] private int requiredHits = 8;

        [Header("Насколько артефакт выходит из грунта при откопке")]
        [SerializeField] private float riseHeight = 0.2f;
        [SerializeField] private float riseDuration = 0.8f;

        [Header("XR Grab Interactable, который активируется после откопки")]
        [SerializeField] private XRGrabInteractable grabInteractable;

        public bool IsUncovered { get; private set; }
        private int currentHits;
        private Vector3 startPos;
        private Vector3 endPos;
        private float riseT;
        private bool rising;

        private void Awake()
        {
            startPos = transform.position;
            endPos = startPos + Vector3.up * riseHeight;

            if (grabInteractable == null) grabInteractable = GetComponent<XRGrabInteractable>();
            if (grabInteractable != null) grabInteractable.enabled = false; // нельзя схватить, пока не откопан
        }

        private void Update()
        {
            if (rising)
            {
                riseT += Time.deltaTime / riseDuration;
                transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, riseT));
                if (riseT >= 1f)
                {
                    rising = false;
                    if (grabInteractable != null) grabInteractable.enabled = true;
                }
            }
        }

        /// <summary>Вызывается из DiggingTool при касании.</summary>
        public void RegisterDigHit()
        {
            if (IsUncovered) return;

            currentHits++;
            Debug.Log($"[Artifact:{type}] касание {currentHits}/{requiredHits}");

            if (currentHits >= requiredHits)
                Uncover();
        }

        private void Uncover()
        {
            IsUncovered = true;
            rising = true;
            riseT = 0f;
            Debug.Log($"[Artifact:{type}] откопан!");

            if (GameManager.Instance != null && GameManager.Instance.Quest != null)
                GameManager.Instance.Quest.RegisterUncovered(type);

            if (GameManager.Instance != null && GameManager.Instance.Audio != null)
                GameManager.Instance.Audio.PlayArtifactFoundClip(type);
        }
    }
}
