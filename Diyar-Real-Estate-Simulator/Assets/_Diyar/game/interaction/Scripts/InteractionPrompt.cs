using UnityEngine;

namespace Diyar.Game
{
    public sealed class InteractionPrompt : MonoBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;

        private GUIStyle promptStyle;

        private void OnGUI()
        {
            if (playerInteraction == null || !playerInteraction.IsTargetInRange)
            {
                return;
            }

            if (promptStyle == null)
            {
                promptStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };
            }

            Rect promptArea = new Rect((Screen.width - 260f) / 2f, Screen.height - 76f, 260f, 48f);
            GUI.Box(promptArea, "E – Interagieren", promptStyle);
        }
    }
}
