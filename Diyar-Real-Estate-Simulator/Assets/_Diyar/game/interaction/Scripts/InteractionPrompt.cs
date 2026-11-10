using UnityEngine;

namespace Diyar.Game
{
    public sealed class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;

        private GUIStyle promptStyle;

        private void OnGUI()
        {
            if (playerInteraction == null
                || !playerInteraction.IsTargetInRange
                || string.IsNullOrWhiteSpace(playerInteraction.InteractionText))
            {
                return;
            }

            if (promptStyle == null)
            {
                promptStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true,
                    padding = new RectOffset(16, 16, 10, 10),
                    normal = { textColor = Color.white }
                };
            }

            GUIContent promptContent = new GUIContent(playerInteraction.InteractionText);
            float maxWidth = Mathf.Max(180f, Screen.width - 32f);
            float promptWidth = Mathf.Min(480f, Mathf.Max(300f, promptStyle.CalcSize(promptContent).x));
            promptWidth = Mathf.Min(promptWidth, maxWidth);
            float promptHeight = promptStyle.CalcHeight(promptContent, promptWidth);
            Rect promptArea = new Rect(
                (Screen.width - promptWidth) * 0.5f,
                Screen.height - promptHeight - 28f,
                promptWidth,
                promptHeight);

            GUI.Box(promptArea, promptContent, promptStyle);
        }
    }
}
