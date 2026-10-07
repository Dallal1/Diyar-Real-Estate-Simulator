using UnityEngine;

namespace Diyar.Game
{
    public sealed class PlaygroundHud : MonoBehaviour
    {
        private GUIStyle panelStyle;
        private GUIStyle textStyle;

        private void OnGUI()
        {
            if (panelStyle == null)
            {
                panelStyle = new GUIStyle(GUI.skin.box);
                textStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 16,
                    wordWrap = true,
                    normal = { textColor = Color.white }
                };
            }

            float panelWidth = Mathf.Min(430f, Screen.width - 24f);
            GUI.Box(new Rect(12f, 12f, panelWidth, 126f), GUIContent.none, panelStyle);
            GUI.Label(new Rect(24f, 22f, panelWidth - 24f, 110f),
                "DIYAR · TESTGELÄNDE\n" +
                "WASD / Pfeiltasten: Gehen   ·   Shift: Rennen\n" +
                "Leertaste: Springen\n" +
                "Rechte Maustaste halten: Kamera drehen", textStyle);
        }
    }
}
