using UnityEngine;

namespace Diyar.Game
{
    public sealed class PropertyPurchase : MonoBehaviour, IInteractable
    {
        [SerializeField] private string propertyId = "damascus_apartment_001";
        [SerializeField, Min(1)] private int purchasePrice = 150;
        [SerializeField] private PlayerWallet playerWallet;

        [SerializeField] private PropertyRepair propertyRepair;
        [SerializeField, Min(0)] private int repairValueIncrease = 60;

        private bool isOwned;
        private bool isSold;

        public string PropertyId => propertyId;
        public int PurchasePrice => purchasePrice;
        public bool IsOwned => isOwned;
        public bool IsSold => isSold;

        public long SalePrice =>
            (long)purchasePrice +
            (propertyRepair != null && propertyRepair.IsRepaired
                ? repairValueIncrease
                : 0);

        public bool CanInteract =>
            isActiveAndEnabled
            && !isSold
            && playerWallet != null
            && purchasePrice > 0
            && repairValueIncrease >= 0
            && !string.IsNullOrWhiteSpace(propertyId);

        public string InteractionText => isOwned
            ? $"E – Verkaufen ({SalePrice})"
            : $"E – Kaufen ({purchasePrice})";

        public void Interact()
        {
            if (!CanInteract)
            {
                return;
            }

            if (isOwned)
            {
                SellProperty();
            }
            else
            {
                BuyProperty();
            }
        }

        private void BuyProperty()
        {
            if (!playerWallet.TrySpendMoney(purchasePrice))
            {
                Debug.Log("Nicht genug Guthaben.", this);
                return;
            }

            isOwned = true;

            Debug.Log($"Immobilie gekauft: {propertyId}", this);
        }

        private void SellProperty()
        {
            long salePrice = SalePrice;

            if (salePrice <= 0 || salePrice > int.MaxValue)
            {
                Debug.LogWarning("Ungültiger Verkaufspreis.", this);
                return;
            }

            if (!playerWallet.AddMoney((int)salePrice))
            {
                Debug.LogWarning("Verkauf fehlgeschlagen.", this);
                return;
            }

            isOwned = false;
            isSold = true;

            Debug.Log($"Immobilie verkauft: {propertyId}", this);
        }
    }
}