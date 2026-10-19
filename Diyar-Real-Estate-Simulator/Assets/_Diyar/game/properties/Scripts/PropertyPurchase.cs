using UnityEngine;

namespace Diyar.Game
{
    public sealed class PropertyPurchase : MonoBehaviour, IInteractable
    {
        [SerializeField] private string propertyId = "damascus_apartment_001";
        [SerializeField, Min(1)] private int purchasePrice = 150;
        [SerializeField] private PlayerWallet playerWallet;

        private bool isOwned;

        public string PropertyId => propertyId;
        public int PurchasePrice => purchasePrice;
        public bool IsOwned => isOwned;

        public bool CanInteract =>
            isActiveAndEnabled
            && !isOwned
            && playerWallet != null
            && purchasePrice > 0
            && !string.IsNullOrWhiteSpace(propertyId);

        public string InteractionText =>
            $"E – Kaufen ({purchasePrice})";

        public void Interact()
        {
            if (!CanInteract)
            {
                return;
            }

            if (!playerWallet.TrySpendMoney(purchasePrice))
            {
                Debug.Log("Nicht genug Guthaben.", this);
                return;
            }

            isOwned = true;

            Debug.Log($"Immobilie gekauft: {propertyId}", this);
        }
    }
}