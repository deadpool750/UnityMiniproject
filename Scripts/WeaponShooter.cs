using UnityEngine;

public class WeaponShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameUI gameUI;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private WeaponVisualRecoil weaponVisualRecoil;

    [Header("Shooting")]
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float impactForce = 35f;

    [Header("Target Detection")]
    [SerializeField] private string targetTag = "ShootableObject";

    [Header("Bullet Holes")]
    [SerializeField] private GameObject bulletHolePrefab;
    [SerializeField] private float bulletHoleLifetime = 20f;
    [SerializeField] private float bulletHoleOffset = 0.01f;
    [SerializeField] private float bulletHoleMinScale = 0.045f;
    [SerializeField] private float bulletHoleMaxScale = 0.075f;

    [Header("Ammo")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private int startingReserveAmmo = 90;
    [SerializeField] private int ammoBoxReserveAmount = 120;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Ammo Box Interaction")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private bool requireCompletelyEmptyForAmmoBox = true;
    [SerializeField] private string ammoBoxPromptMessage = "Press E to replenish ammo";

    [Header("Recoil")]
    [SerializeField] private float recoilVertical = 1.2f;
    [SerializeField] private float recoilHorizontal = 0.35f;

    private int currentAmmo;
    private int reserveAmmo;
    private float nextFireTime;
    private int shotsFired;
    private bool isReloading;
    private float reloadStartTime;

    private void Start()
    {
        currentAmmo = magazineSize;
        reserveAmmo = startingReserveAmmo;

        UpdateAmmoUI();

        if (gameUI != null)
        {
            gameUI.HideReloadUI();
            gameUI.HideAmmoBoxPrompt();
        }
    }

    private void Update()
    {
        UpdateAmmoBoxPrompt();

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }

        if (Input.GetKeyDown(interactKey))
        {
            TryUseAmmoBox();
        }

        if (isReloading)
        {
            UpdateReloadProgress();
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
            {
                Debug.Log("Magazine empty. Press R to reload.");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            shotsFired = 0;
        }
    }

    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        ApplyRecoil();

        if (weaponVisualRecoil != null)
        {
            weaponVisualRecoil.PlayRecoil();
        }

        bool didHitTarget = false;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            SpawnBulletHole(hit);
            ApplyPhysicsForce(hit);

            didHitTarget = IsShootableTarget(hit.collider);

            Debug.Log("Hit object: " + hit.collider.name + " | Counted as target: " + didHitTarget);
        }

        if (gameUI != null)
        {
            gameUI.RegisterShot(didHitTarget);
        }

        shotsFired++;
    }

    private void StartReload()
    {
        if (isReloading)
        {
            return;
        }

        if (currentAmmo == magazineSize)
        {
            Debug.Log("Magazine already full.");
            return;
        }

        if (reserveAmmo <= 0)
        {
            Debug.Log("No reserve ammo left.");
            return;
        }

        isReloading = true;
        reloadStartTime = Time.time;
        shotsFired = 0;

        if (gameUI != null)
        {
            gameUI.ShowReloadUI();
            gameUI.UpdateReloadProgress(0f);
        }

        Debug.Log("Reloading...");
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void UpdateReloadProgress()
    {
        if (gameUI == null)
        {
            return;
        }

        float progress = (Time.time - reloadStartTime) / reloadTime;
        gameUI.UpdateReloadProgress(progress);
    }

    private void FinishReload()
    {
        int ammoNeeded = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

        currentAmmo += ammoToLoad;
        reserveAmmo -= ammoToLoad;

        isReloading = false;
        shotsFired = 0;

        UpdateAmmoUI();

        if (gameUI != null)
        {
            gameUI.UpdateReloadProgress(1f);
            gameUI.HideReloadUI();
        }

        Debug.Log("Reloaded. Loaded " + ammoToLoad + " bullets.");
    }

    private void TryUseAmmoBox()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (!Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            Debug.Log("No ammo box in range.");
            return;
        }

        AmmoBox ammoBox = hit.collider.GetComponentInParent<AmmoBox>();

        if (ammoBox == null)
        {
            Debug.Log("Looking at: " + hit.collider.name + " but it is not an ammo box.");
            return;
        }

        if (requireCompletelyEmptyForAmmoBox && (currentAmmo > 0 || reserveAmmo > 0))
        {
            Debug.Log("You still have ammo. Ammo box can only be used when completely empty.");
            return;
        }

        reserveAmmo = ammoBoxReserveAmount;
        UpdateAmmoUI();

        Debug.Log("Ammo box used. Reserve ammo refilled to " + reserveAmmo + ". Magazine unchanged.");
    }

    private void UpdateAmmoBoxPrompt()
    {
        if (gameUI == null || playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
        {
            AmmoBox ammoBox = hit.collider.GetComponentInParent<AmmoBox>();

            if (ammoBox != null)
            {
                gameUI.ShowAmmoBoxPrompt(ammoBoxPromptMessage);
                return;
            }
        }

        gameUI.HideAmmoBoxPrompt();
    }

    private bool IsShootableTarget(Collider hitCollider)
    {
        if (hitCollider.CompareTag(targetTag))
        {
            return true;
        }

        if (hitCollider.transform.parent != null && hitCollider.transform.parent.CompareTag(targetTag))
        {
            return true;
        }

        if (hitCollider.transform.root.CompareTag(targetTag))
        {
            return true;
        }

        return false;
    }

    private void SpawnBulletHole(RaycastHit hit)
    {
        if (bulletHolePrefab == null)
        {
            return;
        }

        Quaternion bulletHoleRotation = Quaternion.LookRotation(-hit.normal);

        GameObject bulletHole = Instantiate(
            bulletHolePrefab,
            hit.point + hit.normal * bulletHoleOffset,
            bulletHoleRotation
        );

        bulletHole.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));

        float randomScale = Random.Range(bulletHoleMinScale, bulletHoleMaxScale);
        bulletHole.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        bulletHole.transform.SetParent(hit.collider.transform);

        Destroy(bulletHole, bulletHoleLifetime);
    }

    private void ApplyPhysicsForce(RaycastHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.GetComponentInParent<Rigidbody>();

        if (hitRigidbody == null)
        {
            return;
        }

        Vector3 forceDirection = playerCamera.transform.forward;

        hitRigidbody.AddForceAtPosition(
            forceDirection * impactForce,
            hit.point,
            ForceMode.Impulse
        );
    }

    private void ApplyRecoil()
    {
        float horizontalKick = Mathf.Sin(shotsFired * 1.7f) * recoilHorizontal;

        if (mouseLook != null)
        {
            mouseLook.AddRecoil(recoilVertical);
        }

        transform.Rotate(Vector3.up * horizontalKick);
    }

    private void UpdateAmmoUI()
    {
        if (gameUI != null)
        {
            gameUI.UpdateAmmo(currentAmmo, magazineSize, reserveAmmo);
        }
    }
}