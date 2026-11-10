using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class CleaningJob : MonoBehaviour, IInteractable
    {
        [SerializeField] private PlayerWallet playerWallet;
        [SerializeField, Min(1)] private int reward = 50;
        [SerializeField] private Color cleanColor = Color.green;
        [SerializeField] private string jobId = "cleaning_job_001";

        private Material objectMaterial;
        private bool isCompleted;

        public bool CanInteract =>
            isActiveAndEnabled && !isCompleted && playerWallet != null;
        public string JobId => jobId;
        public string InteractionText => "E – Reinigen";

        private void Awake()
        {
            objectMaterial = GetComponent<MeshRenderer>().material;
            isCompleted = SaveManager.IsJobCompleted(jobId);

            if (isCompleted && objectMaterial != null)
            {
                objectMaterial.color = cleanColor;
            }
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
            SaveManager.MarkJobCompleted(jobId);
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