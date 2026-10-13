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

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            TestInteractable target = FindClosestAvailableTarget();
            IsTargetInRange = target != null;

            if (IsTargetInRange && Input.GetKeyDown(KeyCode.E))
            {
                target.Interact();
                IsTargetInRange = !target.HasInteracted;
            }
        }

        private TestInteractable FindClosestAvailableTarget()
        {
            Vector3 playerCenter = transform.TransformPoint(characterController.center);
            TestInteractable closestTarget = null;
            float closestDistance = float.PositiveInfinity;

            foreach (Collider targetCollider in interactionTargets)
            {
                if (targetCollider == null || !targetCollider.enabled || !targetCollider.gameObject.activeInHierarchy)
                {
                    continue;
                }

                TestInteractable target = targetCollider.GetComponent<TestInteractable>();

                if (target == null || !target.isActiveAndEnabled || target.HasInteracted)
                    continue;
                

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
