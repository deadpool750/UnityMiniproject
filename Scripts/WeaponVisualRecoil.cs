using UnityEngine;

public class WeaponVisualRecoil : MonoBehaviour
{
    [Header("Position Recoil")]
    [SerializeField] private Vector3 recoilPosition = new Vector3(0f, 0f, -0.12f);
    [SerializeField] private float recoilReturnSpeed = 18f;
    [SerializeField] private float recoilSnappiness = 30f;

    [Header("Rotation Recoil")]
    [SerializeField] private Vector3 recoilRotation = new Vector3(-8f, 2f, 0f);
    [SerializeField] private float rotationReturnSpeed = 18f;
    [SerializeField] private float rotationSnappiness = 30f;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private Vector3 currentPositionOffset;
    private Vector3 targetPositionOffset;

    private Vector3 currentRotationOffset;
    private Vector3 targetRotationOffset;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        targetPositionOffset = Vector3.Lerp(
            targetPositionOffset,
            Vector3.zero,
            recoilReturnSpeed * Time.deltaTime
        );

        currentPositionOffset = Vector3.Lerp(
            currentPositionOffset,
            targetPositionOffset,
            recoilSnappiness * Time.deltaTime
        );

        targetRotationOffset = Vector3.Lerp(
            targetRotationOffset,
            Vector3.zero,
            rotationReturnSpeed * Time.deltaTime
        );

        currentRotationOffset = Vector3.Lerp(
            currentRotationOffset,
            targetRotationOffset,
            rotationSnappiness * Time.deltaTime
        );

        transform.localPosition = originalLocalPosition + currentPositionOffset;
        transform.localRotation = originalLocalRotation * Quaternion.Euler(currentRotationOffset);
    }

    public void PlayRecoil()
    {
        targetPositionOffset += recoilPosition;
        targetRotationOffset += recoilRotation;
    }
}