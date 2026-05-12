using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Скрипт инструмента (кисточка или лопатка). Вешается на модель инструмента.
    /// При касании Collider-а артефакта регистрирует "удар" по артефакту.
    /// Требует Collider в режиме Trigger на инструменте (например, на наконечнике).
    /// </summary>
    public class DiggingTool : MonoBehaviour
    {
        [Header("Минимальный интервал между касаниями одного артефакта (сек)")]
        [SerializeField] private float hitCooldown = 0.25f;

        // Когда последний раз ударяли артефакт (по instanceId)
        private readonly System.Collections.Generic.Dictionary<int, float> lastHitTime = new();

        private void OnTriggerStay(Collider other)
        {
            // Артефакт должен иметь компонент Artifact на том же объекте, что и его Collider
            var artifact = other.GetComponent<Artifact>();
            if (artifact == null || artifact.IsUncovered) return;

            int id = artifact.GetInstanceID();
            float now = Time.time;
            if (lastHitTime.TryGetValue(id, out float prev) && now - prev < hitCooldown) return;

            lastHitTime[id] = now;
            artifact.RegisterDigHit();
        }
    }
}
