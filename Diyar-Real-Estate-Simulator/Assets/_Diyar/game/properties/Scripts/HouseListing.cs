using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class HouseListing : MonoBehaviour, IInteractable
    {
        private string propertyId;
        private string houseName;
        private int price;
        private int resalePrice;
        private PlayerWallet playerWallet;
        private PlayerMovement playerMovement;
        private ThirdPersonCamera thirdPersonCamera;
        private TextMesh signText;
        private Renderer signBoard;
        private Material forSaleMaterial;
        private Material soldMaterial;
        private bool forSaleTextIsOnTexture;
        private int signBoardMaterialIndex;

        private bool isOwned;
        private bool isSold;
        private bool offerIsOpen;
        private bool wasMovementEnabled;
        private bool wasCameraEnabled;
        private float previousTimeScale;
        private string message = "";

        private GUIStyle panelStyle;
        private GUIStyle titleStyle;
        private GUIStyle priceStyle;
        private GUIStyle messageStyle;
        private GUIStyle buyButtonStyle;
        private GUIStyle leaveButtonStyle;
        private Texture2D overlayTexture;
        private Texture2D panelTexture;
        private Texture2D buyTexture;
        private Texture2D buyHoverTexture;
        private Texture2D leaveTexture;
        private Texture2D leaveHoverTexture;

        public bool CanInteract =>
            isActiveAndEnabled && !isSold && playerWallet != null;

        public string InteractionText =>
            !CanInteract || offerIsOpen
                ? ""
                : isOwned
                    ? $"E – Verkaufsangebot ansehen ({resalePrice:N0} €)"
                    : $"E – Kaufangebot ansehen ({price:N0} €)";

        public void Initialize(
            string id,
            string listingName,
            int listingPrice,
            PlayerWallet wallet,
            TextMesh label,
            Renderer board,
            Material availableMaterial,
            Material unavailableMaterial,
            bool saleTextIsOnTexture = false,
            int boardMaterialIndex = 0)
        {
            propertyId = id;
            houseName = listingName;
            price = listingPrice;
            resalePrice = listingPrice;
            playerWallet = wallet;
            signText = label;
            signBoard = board;
            forSaleMaterial = availableMaterial;
            soldMaterial = unavailableMaterial;
            forSaleTextIsOnTexture = saleTextIsOnTexture;
            signBoardMaterialIndex = boardMaterialIndex;

            playerMovement = FindObjectOfType<PlayerMovement>();
            thirdPersonCamera = FindObjectOfType<ThirdPersonCamera>();

            PropertySaveData savedProperty = SaveManager.FindProperty(propertyId);
            isOwned = savedProperty != null && savedProperty.IsOwned;
            isSold = savedProperty != null && savedProperty.IsSold;
            UpdateSign();
        }

        public void Interact()
        {
            if (!CanInteract || offerIsOpen)
            {
                return;
            }

            previousTimeScale = Time.timeScale;
            wasMovementEnabled = playerMovement != null && playerMovement.enabled;
            wasCameraEnabled = thirdPersonCamera != null && thirdPersonCamera.enabled;

            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            Time.timeScale = 0f;
            Cursor.visible = true;
            message = "";
            offerIsOpen = true;
        }

        private void OnGUI()
        {
            if (!offerIsOpen)
            {
                return;
            }

            EnsureStyles();

            float scale = Mathf.Clamp(Screen.height / 900f, 0.75f, 1f);
            float panelWidth = Mathf.Min(460f * scale, Screen.width - 32f);
            float panelHeight = 380f * scale;
            Rect panelRect = new Rect(
                (Screen.width - panelWidth) * 0.5f,
                (Screen.height - panelHeight) * 0.5f,
                panelWidth,
                panelHeight);

            Color previousColor = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), overlayTexture);
            GUI.color = previousColor;

            GUI.Box(panelRect, GUIContent.none, panelStyle);
            GUILayout.BeginArea(new Rect(
                panelRect.x + 24f * scale,
                panelRect.y + 24f * scale,
                panelRect.width - 48f * scale,
                panelRect.height - 48f * scale));

            GUILayout.Space(16f * scale);
            GUILayout.Label(houseName, titleStyle);
            GUILayout.Space(10f * scale);
            GUILayout.Label(
                isOwned ? "Möchtest du dieses Haus verkaufen?" : "Möchtest du dieses Haus kaufen?",
                priceStyle);
            GUILayout.Space(8f * scale);
            GUILayout.Label(
                isOwned
                    ? $"Verkaufspreis: {resalePrice:N0} €"
                    : $"Kaufpreis: {price:N0} €",
                priceStyle);

            if (!string.IsNullOrEmpty(message))
            {
                GUILayout.Space(8f * scale);
                GUILayout.Label(message, messageStyle);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(
                    isOwned ? "VERKAUFEN" : "KAUFEN",
                    buyButtonStyle,
                    GUILayout.Height(52f * scale)))
            {
                if (isOwned)
                {
                    SellProperty();
                }
                else
                {
                    BuyProperty();
                }
            }

            GUILayout.Space(10f * scale);

            if (GUILayout.Button(
                    isOwned ? "BEHALTEN" : "LASSEN",
                    leaveButtonStyle,
                    GUILayout.Height(48f * scale)))
            {
                CloseOffer();
            }

            GUILayout.Space(8f * scale);
            GUILayout.EndArea();
        }

        private void BuyProperty()
        {
            if (!CanInteract)
            {
                CloseOffer();
                return;
            }

            if (!playerWallet.TrySpendMoney(price))
            {
                message = "Nicht genug Guthaben.";
                return;
            }

            isOwned = true;
            SaveManager.SavePropertyState(propertyId, isOwned, false);
            UpdateSign();
            CloseOffer();
        }

        private void SellProperty()
        {
            if (!CanInteract || !isOwned)
            {
                CloseOffer();
                return;
            }

            if (resalePrice <= 0 || !playerWallet.AddMoney(resalePrice))
            {
                message = "Verkauf fehlgeschlagen.";
                return;
            }

            isOwned = false;
            isSold = true;
            SaveManager.SavePropertyState(propertyId, false, true);
            UpdateSign();
            CloseOffer();
        }

        private void CloseOffer()
        {
            offerIsOpen = false;
            Time.timeScale = previousTimeScale;
            Cursor.visible = true;

            if (playerMovement != null)
            {
                playerMovement.enabled = wasMovementEnabled;
            }

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = wasCameraEnabled;
            }
        }

        private void UpdateSign()
        {
            if (signText != null)
            {
                signText.text = isSold
                    ? "SOLD"
                    : isOwned
                        ? "VERKAUFEN"
                        : forSaleTextIsOnTexture ? "" : "FOR SALE";
                signText.fontSize = isOwned ? 60 : 90;
            }

            if (signBoard != null)
            {
                Material[] materials = signBoard.sharedMaterials;

                if (signBoardMaterialIndex >= 0 && signBoardMaterialIndex < materials.Length)
                {
                    materials[signBoardMaterialIndex] = isOwned || isSold
                        ? soldMaterial
                        : forSaleMaterial;
                    signBoard.sharedMaterials = materials;
                }
            }
        }

        private void EnsureStyles()
        {
            if (panelStyle != null)
            {
                return;
            }

            overlayTexture = CreateTexture(new Color(0.05f, 0.07f, 0.06f, 0.72f));
            panelTexture = CreateTexture(new Color(0.12f, 0.16f, 0.14f, 0.98f));
            buyTexture = CreateTexture(new Color(0.25f, 0.43f, 0.32f));
            buyHoverTexture = CreateTexture(new Color(0.34f, 0.53f, 0.40f));
            leaveTexture = CreateTexture(new Color(0.27f, 0.29f, 0.26f));
            leaveHoverTexture = CreateTexture(new Color(0.38f, 0.40f, 0.35f));

            panelStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = panelTexture }
            };

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 28,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.94f, 0.88f, 0.75f) }
            };

            priceStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                normal = { textColor = new Color(0.91f, 0.90f, 0.84f) }
            };

            messageStyle = new GUIStyle(priceStyle)
            {
                normal = { textColor = new Color(1f, 0.68f, 0.45f) }
            };

            buyButtonStyle = CreateButtonStyle(buyTexture, buyHoverTexture, 19);
            leaveButtonStyle = CreateButtonStyle(leaveTexture, leaveHoverTexture, 17);
        }

        private GUIStyle CreateButtonStyle(Texture2D normalTexture, Texture2D hoverTexture, int fontSize)
        {
            return new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                normal =
                {
                    background = normalTexture,
                    textColor = new Color(0.97f, 0.95f, 0.88f)
                },
                hover =
                {
                    background = hoverTexture,
                    textColor = Color.white
                },
                active =
                {
                    background = hoverTexture,
                    textColor = Color.white
                }
            };
        }

        private Texture2D CreateTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            texture.hideFlags = HideFlags.HideAndDontSave;
            return texture;
        }

        private void OnDestroy()
        {
            if (offerIsOpen)
            {
                CloseOffer();
            }

            DestroyTexture(overlayTexture);
            DestroyTexture(panelTexture);
            DestroyTexture(buyTexture);
            DestroyTexture(buyHoverTexture);
            DestroyTexture(leaveTexture);
            DestroyTexture(leaveHoverTexture);
        }

        private void DestroyTexture(Texture2D texture)
        {
            if (texture != null)
            {
                Destroy(texture);
            }
        }
    }
}
