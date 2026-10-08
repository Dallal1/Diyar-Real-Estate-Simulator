using System.Security.Cryptography;
using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private Collider interactionTarget;
        [SerializeField, Min(0.1f)] private float interactionRange = 2f;

        private CharacterController characterController;

        public bool IsTargetInRange { get; private set; }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (interactionTarget == null || !interactionTarget.enabled
                || !interactionTarget.gameObject.activeInHierarchy)
            {
                IsTargetInRange = false;
                return;
            }

            // Measure from the player's center to the nearest point on the box.
            Vector3 playerCenter = transform.TransformPoint(characterController.center);
            Vector3 targetPoint = interactionTarget.ClosestPoint(playerCenter);
            float distanceToTarget = Vector3.Distance(playerCenter, targetPoint);

            IsTargetInRange = distanceToTarget <= interactionRange;

           if (IsTargetInRange && Input.GetKeyDown(KeyCode.E))
            {
                TestInteractable target = interactionTarget.GetComponent<TestInteractable>();

                if (target != null)
                {
                    target.Interact();
                }
            }
        }
        private void OnDisable()
        {
            IsTargetInRange = false;
        }
    }
}
