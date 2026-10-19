using UnityEngine;

namespace Diyar.Game
{
    public sealed class WalletDisplay : MonoBehaviour
    {
        [SerializeField] private PlayerWallet playerWallet;

        private GUIStyle balanceStyle;

        private void OnGUI()
        {
            if (playerWallet == null)
            {
                return;
            }

            if (balanceStyle == null)
            {
                balanceStyle = new GUIStyle(GUI.skin.box)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };
            }

            Rect balanceArea = new Rect(16f, 16f, 300f, 48f);
            GUI.Box(balanceArea, $"Guthaben: {playerWallet.Balance}", balanceStyle);
        }
    }
}
