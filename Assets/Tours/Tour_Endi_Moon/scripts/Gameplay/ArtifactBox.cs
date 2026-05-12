using System.Collections.Generic;
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Ящик для артефактов. Trigger Collider у горловины.
    /// При попадании артефакта внутрь — регистрирует его в QuestManager.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ArtifactBox : MonoBehaviour
    {
        private readonly HashSet<int> storedIds = new();

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            var artifact = other.GetComponentInParent<Artifact>();
            if (artifact == null || !artifact.IsUncovered) return;

            int id = artifact.GetInstanceID();
            if (storedIds.Contains(id)) return;
            storedIds.Add(id);

            Debug.Log($"[ArtifactBox] получен {artifact.type}");

            // "Пристёгиваем" артефакт к ящику, чтобы он не выпал
            artifact.transform.SetParent(transform);
            var rb = artifact.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (GameManager.Instance != null && GameManager.Instance.Quest != null)
                GameManager.Instance.Quest.RegisterStored(artifact.type);
        }
    }
}
