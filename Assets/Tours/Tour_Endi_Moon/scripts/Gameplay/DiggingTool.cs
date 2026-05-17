using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Скрипт инструмента (лопата). Вешается на модель инструмента.
    /// При касании Collider-а SandPile регистрирует "удар" по кучке земли.
    /// При касании Artifact напрямую (без кучки) также регистрирует удар.
    /// Требует Collider в режиме Trigger на инструменте (например, на наконечнике).
    /// </summary>
    public class DiggingTool : MonoBehaviour
    {
        [Header("Минимальный интервал между касаниями одного объекта (сек)")]
        [SerializeField] private float hitCooldown = 0.25f;

        private readonly System.Collections.Generic.Dictionary<int, float> lastHitTime = new();

        private void OnTriggerStay(Collider other)
        {
            // Приоритет — кучка земли
            var pile = other.GetComponent<SandPile>();
            if (pile != null && !pile.IsDug)
            {
                TryHit(pile.GetInstanceID(), () => pile.RegisterHit());
                return;
            }

            // Fallback — прямой артефакт (без кучки)
            var artifact = other.GetComponent<Artifact>();
            if (artifact != null && !artifact.IsUncovered)
            {
                TryHit(artifact.GetInstanceID(), () => artifact.RegisterDigHit());
            }
        }

        private void TryHit(int id, System.Action onHit)
        {
            float now = Time.time;
            if (lastHitTime.TryGetValue(id, out float prev) && now - prev < hitCooldown) return;
            lastHitTime[id] = now;
            onHit?.Invoke();
        }
    }
}
