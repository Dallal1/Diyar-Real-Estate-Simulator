using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class CleaningJob : MonoBehaviour, IInteractable
    {
        [SerializeField] private PlayerWallet playerWallet;
        [SerializeField, Min(1)] private int reward = 50;
        [SerializeField] private Color cleanColor = Color.green;

        private Material objectMaterial;
        private bool isCompleted;

        public bool CanInteract =>
            isActiveAndEnabled && !isCompleted && playerWallet != null;

        public string InteractionText => "E – Reinigen";

        private void Awake()
        {
            objectMaterial = GetComponent<MeshRenderer>().material;
        }

        public void Interact()
        {
            if (!CanInteract || objectMaterial == null)
            {
                return;
            }

            if (!playerWallet.AddMoney(reward))
            {
                return;
            }

            isCompleted = true;
            objectMaterial.color = cleanColor;
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