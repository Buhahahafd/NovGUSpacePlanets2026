using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Зона возврата возле корабля. Когда игрок входит в неё после сбора артефактов —
    /// сценарий переходит к квизу.
    /// Поставить пустой объект с большим Collider (Trigger) у стартовой площадки.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ReturnZone : MonoBehaviour
    {
        [Tooltip("Тег игрока (на XR Origin или его коллайдере)")]
        [SerializeField] private string playerTag = "Player";

        private bool triggered;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggered) return;
            if (!other.CompareTag(playerTag)) return;

            var story = GameManager.Instance != null ? GameManager.Instance.Story : null;
            if (story == null) return;

            // Запускаем квиз только если уже собрали всё и идём на возврат
            if (story.GetCurrentStage() == GameState.Return)
            {
                triggered = true;
                story.SetStage(GameState.Quiz);
            }
        }
    }
}
