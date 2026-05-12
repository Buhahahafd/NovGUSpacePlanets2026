using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MoonGame
{
    /// <summary>
    /// World-Space UI квиза. Показывает текст вопроса и до 4 кнопок-вариантов.
    /// Кнопки нажимаются лучом от XR-контроллера (XR Ray Interactor + Canvas
    /// с TrackedDeviceGraphicRaycaster).
    /// </summary>
    public class QuizUI : MonoBehaviour
    {
        [Header("Корневой Canvas (включается на этапе квиза)")]
        [SerializeField] private GameObject root;

        [Header("Текстовое поле вопроса")]
        [SerializeField] private TMP_Text questionText;

        [Header("4 кнопки вариантов (А, Б, В, Г). Лишние скроются автоматически.")]
        [SerializeField] private Button[] optionButtons;
        [SerializeField] private TMP_Text[] optionLabels;

        [Header("Финальный экран")]
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TMP_Text resultText;

        private Action<int> currentCallback;

        public void SetVisible(bool visible)
        {
            if (root != null) root.SetActive(visible);
        }

        public void ShowQuestion(QuizQuestion q, Action<int> onAnswer)
        {
            if (resultPanel != null) resultPanel.SetActive(false);
            currentCallback = onAnswer;
            if (questionText != null) questionText.text = q.text;

            for (int i = 0; i < optionButtons.Length; i++)
            {
                bool active = i < q.options.Length;
                optionButtons[i].gameObject.SetActive(active);
                if (!active) continue;

                if (optionLabels != null && i < optionLabels.Length && optionLabels[i] != null)
                    optionLabels[i].text = q.options[i];

                int captured = i;
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => currentCallback?.Invoke(captured));
            }
        }

        /// <summary>Показывает финальный экран с результатом и итоговой репликой.</summary>
        public void ShowResult(int correct, int total)
        {
            if (resultPanel != null) resultPanel.SetActive(true);
            if (resultText != null) resultText.text = BuildResultText(correct, total);

            if (questionText != null) questionText.text = string.Empty;
            foreach (var b in optionButtons) if (b != null) b.gameObject.SetActive(false);
        }

        private static string BuildResultText(int correct, int total)
        {
            string phrase = correct == total
                ? "Это невероятный успех нашего с тобой исследования Луны!\nТвои знания помогут человечеству узнать больше о нашей Солнечной системе."
                : correct >= 3
                    ? "Твоих знаний уже достаточно, чтобы помочь учёным организовать новую миссию по исследованию Луны."
                    : "Думаю, нам стоит ещё раз посетить Луну, чтобы узнать немного больше.";

            return $"Правильных ответов: {correct} из {total}\n\n{phrase}";
        }
    }
}
