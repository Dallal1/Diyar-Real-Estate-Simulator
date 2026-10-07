using UnityEngine;

namespace Diyar.Game
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        private static readonly int SpeedParameter = Animator.StringToHash("Speed");
        private static readonly int GroundedParameter = Animator.StringToHash("IsGrounded");

        [Header("References")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Animator characterAnimator;

        [Header("Movement")]
        [SerializeField, Min(0.1f)] private float walkSpeed = 2.5f;
        [SerializeField, Min(0.1f)] private float runSpeed = 5.5f;
        [SerializeField, Min(1f)] private float rotationSpeed = 720f;

        [Header("Jump")]
        [SerializeField, Min(0.1f)] private float jumpHeight = 1.2f;
        [SerializeField] private float gravity = -20f;

        private CharacterController characterController;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private float verticalSpeed;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            spawnPosition = transform.position;
            spawnRotation = transform.rotation;

            if (characterAnimator != null)
            {
                characterAnimator.applyRootMotion = false;
            }
        }

        private void Update()
        {
            Vector2 movementInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"));

            bool runRequested = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool jumpRequested = Input.GetButtonDown("Jump");

            MoveCharacter(movementInput, runRequested, jumpRequested, Time.deltaTime);

            if (transform.position.y < -10f)
            {
                ResetPosition();
            }
        }

        private void MoveCharacter(Vector2 movementInput, bool runRequested, bool jumpRequested, float deltaTime)
        {
            Vector3 movementDirection = GetMovementDirection(movementInput);
            float movementSpeed = runRequested ? runSpeed : walkSpeed;

            if (movementDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation, targetRotation, rotationSpeed * deltaTime);
            }

            if (characterController.isGrounded && verticalSpeed < 0f)
            {
                // A small downward speed keeps the controller in contact with the ground.
                verticalSpeed = -2f;
            }

            if (jumpRequested && characterController.isGrounded)
            {
                verticalSpeed = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalSpeed += gravity * deltaTime;
            Vector3 velocity = movementDirection * movementSpeed + Vector3.up * verticalSpeed;
            CollisionFlags collisions = characterController.Move(velocity * deltaTime);

            if ((collisions & CollisionFlags.Above) != 0 && verticalSpeed > 0f)
            {
                verticalSpeed = 0f;
            }

            UpdateAnimation(deltaTime);
        }

        private Vector3 GetMovementDirection(Vector2 movementInput)
        {
            // Clamping prevents diagonal movement from being faster than straight movement.
            movementInput = Vector2.ClampMagnitude(movementInput, 1f);

            if (cameraTransform == null)
            {
                return new Vector3(movementInput.x, 0f, movementInput.y);
            }

            Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);

            return cameraRight * movementInput.x + cameraForward * movementInput.y;
        }

        private void UpdateAnimation(float deltaTime)
        {
            if (characterAnimator == null)
            {
                return;
            }

            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterController.velocity, Vector3.up);
            characterAnimator.SetFloat(SpeedParameter, horizontalVelocity.magnitude, 0.1f, deltaTime);
            characterAnimator.SetBool(GroundedParameter, characterController.isGrounded && verticalSpeed <= 0f);
        }

        private void ResetPosition()
        {
            characterController.enabled = false;
            transform.SetPositionAndRotation(spawnPosition, spawnRotation);
            characterController.enabled = true;
            verticalSpeed = 0f;
        }

        private void OnValidate()
        {
            runSpeed = Mathf.Max(runSpeed, walkSpeed);
            gravity = Mathf.Min(gravity, -0.1f);
        }
    }
}
