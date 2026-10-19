using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class PropertyRepair : MonoBehaviour, IInteractable
    {
        [SerializeField] private PropertyPurchase property;
        [SerializeField] private PlayerWallet playerWallet;
        [SerializeField, Min(1)] private int repairCost = 30;
        [SerializeField] private Color repairedColor = Color.green;

        private Material objectMaterial;
        private bool isRepaired;

        public bool IsRepaired => isRepaired;

        public bool CanInteract =>
            isActiveAndEnabled
            && !isRepaired
            && property != null
            && property.IsOwned
            && playerWallet != null
            && repairCost > 0
            && objectMaterial != null;

        public string InteractionText =>
            $"E – Reparieren ({repairCost})";

        private void Awake()
        {
            objectMaterial = GetComponent<MeshRenderer>().material;
        }

        public void Interact()
        {
            if (!CanInteract)
            {
                return;
            }

            if (!playerWallet.TrySpendMoney(repairCost))
            {
                Debug.Log("Nicht genug Guthaben.", this);
                return;
            }

            isRepaired = true;
            objectMaterial.color = repairedColor;
        }

        private void OnDestroy()
        {
            if (objectMaterial != null)
            {
                Destroy(objectMaterial);
            }
        }
    }
}