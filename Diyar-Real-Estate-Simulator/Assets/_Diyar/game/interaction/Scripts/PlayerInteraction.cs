using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private Collider[] interactionTargets = new Collider[0];
        [SerializeField, Min(0.1f)] private float interactionRange = 2f;

        private CharacterController characterController;

        public bool IsTargetInRange { get; private set; }
        public string InteractionText { get; private set; } = "";

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            IInteractable target = FindClosestAvailableTarget();
            IsTargetInRange = target != null;
            InteractionText = target != null ? target.InteractionText : "";

            if (IsTargetInRange && Input.GetKeyDown(KeyCode.E))
            {
                target.Interact();
                IsTargetInRange = target.CanInteract;
            }
        }

        private IInteractable FindClosestAvailableTarget()
        {
            Vector3 playerCenter = transform.TransformPoint(characterController.center);
            IInteractable closestTarget = null;
            float closestDistance = float.PositiveInfinity;

            foreach (Collider targetCollider in interactionTargets)
            {
                if (targetCollider == null || !targetCollider.enabled || !targetCollider.gameObject.activeInHierarchy)
                {
                    continue;
                }

                IInteractable target = targetCollider.GetComponent<IInteractable>();

                if (target == null || !target.CanInteract)
                {
                    continue;
                }

                // Measure to the collider surface, not the object's pivot.
                Vector3 targetPoint = targetCollider.ClosestPoint(playerCenter);
                float distanceToTarget = Vector3.Distance(playerCenter, targetPoint);

                if (distanceToTarget > interactionRange)
                    continue;
                

                Vector3 directionToTarget = targetPoint - playerCenter;

                if (Physics.Raycast( playerCenter, directionToTarget.normalized, out RaycastHit hit, distanceToTarget, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider != targetCollider)
                    {
                        continue;
                    }
                }

                // Equal distances keep the first available target in the list.
                if (distanceToTarget <= interactionRange && distanceToTarget < closestDistance)
                {
                    closestTarget = target;
                    closestDistance = distanceToTarget;
                }
            }

            return closestTarget;
        }

        private void OnDisable()
        {
            IsTargetInRange = false;
        }
    }
}
