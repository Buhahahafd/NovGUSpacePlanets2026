// Этот компонент устарел. Функциональность перенесена в IntroSequencer.
// Безопасно удалить из иерархии и заменить на IntroSequencer на объекте Managers.
using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Устаревший компонент. Используйте IntroSequencer вместо него.
    /// </summary>
    public class LandingSequencer : MonoBehaviour
    {
        private void Start()
        {
            Debug.LogWarning("[LandingSequencer] Устаревший компонент. Замените на IntroSequencer.");
        }
    }
}
