using UnityEngine;

public class GunPhysics : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float recoilDistance = 0.1f;      // How far the gun moves back during recoil
    public float recoilDuration = 0.1f;      // Duration of the recoil motion
    public float recoilRotation = 5f;        // Degrees of upward rotation during recoil

    [Header("Reload Settings")]
    public float reloadDistance = 0.5f;      // How far the gun moves back during reload
    public float reloadDuration = 1f;        // Total duration of the reload motion

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool isRecoiling = false;
    private bool isReloading = false;

    void Start()
    {
        // Store the original local position and rotation of the gun
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    // Starts the recoil animation.
    public void StartRecoil()
    {
        if (!isRecoiling && !isReloading)
            StartCoroutine(RecoilCoroutine());
    }

    // Coroutine to handle the recoil motion over time.
    private System.Collections.IEnumerator RecoilCoroutine()
    {
        isRecoiling = true;
        float elapsedTime = 0f;

        while (elapsedTime < recoilDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / recoilDuration;

            // Calculate the recoil offset using an ease-out curve
            float zOffset = Mathf.Lerp(0, -recoilDistance, Mathf.Sin(percentage * Mathf.PI / 2));
            float rotationOffset = Mathf.Lerp(0, recoilRotation, Mathf.Sin(percentage * Mathf.PI / 2));

            // Apply position and rotation offsets
            transform.localPosition = originalPosition + new Vector3(0, 0, zOffset);
            transform.localRotation = originalRotation * Quaternion.Euler(-rotationOffset, 0, 0);

            yield return null;
        }

        // Return to the original position and rotation
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        isRecoiling = false;
    }

    /// Starts the reload animation.
    public void StartReload()
    {
        if (!isReloading && !isRecoiling)
            StartCoroutine(ReloadCoroutine());
    }

    /// Coroutine to handle the reload motion over time.
    private System.Collections.IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        float halfDuration = reloadDuration / 2f;
        float elapsedTime = 0f;

        // Move gun backward
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / halfDuration;

            float zOffset = Mathf.Lerp(0, -reloadDistance, percentage);
            transform.localPosition = originalPosition + new Vector3(0, 0, zOffset);

            yield return null;
        }


        elapsedTime = 0f;

        // Move gun forward to original position
        while (elapsedTime < halfDuration)
        {
            elapsedTime += Time.deltaTime;
            float percentage = elapsedTime / halfDuration;

            float zOffset = Mathf.Lerp(-reloadDistance, 0, percentage);
            transform.localPosition = originalPosition + new Vector3(0, 0, zOffset);

            yield return null;
        }

        // Ensure gun is in original position and rotation
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        isReloading = false;
    }
}