using UnityEngine;

namespace Diyar.Game
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private float targetHeight = 1.3f;
        [SerializeField, Min(1f)] private float followDistance = 5f;
        [SerializeField, Min(0.1f)] private float mouseSensitivity = 3f;
        [SerializeField] private float pitch = 25f;
        [SerializeField] private LayerMask obstacleLayers = Physics.DefaultRaycastLayers;

        private float yaw;

        private void Start()
        {
            if (followTarget != null)
            {
                yaw = followTarget.eulerAngles.y;
            }
        }

        private void LateUpdate()
        {
            if (followTarget == null)
            {
                return;
            }

            // Orbit only while the right mouse button is held; the pointer stays available.
            if (Input.GetMouseButton(1))
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, 10f, 65f);
            }

            Vector3 lookTarget = followTarget.position + Vector3.up * targetHeight;
            Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 cameraDirection = cameraRotation * Vector3.back;
            float cameraDistance = followDistance;

            if (Physics.SphereCast(lookTarget, 0.2f, cameraDirection, out RaycastHit obstacle,
                followDistance, obstacleLayers, QueryTriggerInteraction.Ignore))
            {
                cameraDistance = Mathf.Max(0.1f, obstacle.distance - 0.1f);
            }

            transform.SetPositionAndRotation(lookTarget + cameraDirection * cameraDistance, cameraRotation);
        }
    }
}
