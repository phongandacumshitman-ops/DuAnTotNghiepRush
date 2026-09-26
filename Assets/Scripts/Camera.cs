using Cinemachine;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform lookAtTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 25f;
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float minHeight = 12f;
    [SerializeField] private float maxHeight = 60f;
    [SerializeField] private float followHeight = 32f;
    [SerializeField] private Vector2 xBounds = new(-50f, 50f);
    [SerializeField] private Vector2 zBounds = new(-50f, 50f);

    private float currentHeight;

    private void Awake()
    {
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        if (virtualCamera == null)
        {
            Debug.LogError("RTSCameraController requires a CinemachineVirtualCamera component.");
            enabled = false;
            return;
        }

        if (lookAtTarget == null)
        {
            GameObject target = new GameObject("RTS Camera Look Target");
            target.transform.SetParent(transform);
            lookAtTarget = target.transform;
        }

        virtualCamera.Follow = transform;
        virtualCamera.LookAt = lookAtTarget;

        currentHeight = Mathf.Clamp(transform.position.y, minHeight, maxHeight);
        UpdateLookTarget();
        ApplyHeight();
    }

    private void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 move = input.normalized * moveSpeed * Time.deltaTime;
            Vector3 nextPosition = transform.position + move;

            nextPosition.x = Mathf.Clamp(nextPosition.x, xBounds.x, xBounds.y);
            nextPosition.z = Mathf.Clamp(nextPosition.z, zBounds.x, zBounds.y);

            transform.position = new Vector3(nextPosition.x, currentHeight, nextPosition.z);
            UpdateLookTarget();
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            currentHeight = Mathf.Clamp(currentHeight - scroll * zoomSpeed, minHeight, maxHeight);
            ApplyHeight();
            UpdateLookTarget();
        }
    }

    private void UpdateLookTarget()
    {
        if (lookAtTarget == null)
        {
            return;
        }

        lookAtTarget.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private void ApplyHeight()
    {
        Vector3 position = transform.position;
        position.y = currentHeight;
        transform.position = position;
    }
}

