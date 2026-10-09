using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class TestInteractable : MonoBehaviour
    {
        [SerializeField] private Color interactionColor = Color.green;
        public bool HasInteracted { get; private set; }

        private Material objectMaterial;

        private void Awake()
        {
            // A separate material instance keeps other objects' colors unchanged.
            objectMaterial = GetComponent<MeshRenderer>().material;
        }

        [ContextMenu("Test Interaction (Play Mode)")]
        public void Interact()
        {
            if (HasInteracted)
            {
                return;
            }

            if (objectMaterial == null)
            {
                return;
            }

            HasInteracted = true;
            objectMaterial.color = interactionColor;
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
