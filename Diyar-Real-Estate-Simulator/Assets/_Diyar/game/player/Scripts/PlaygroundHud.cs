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
        }
    }
}
