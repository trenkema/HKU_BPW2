using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    [Header("Stats")]
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;
    public bool flickerLight;
    public float minFlickerTime, maxFlickerTime;

    // Bools
    bool shooting, readyToShoot, reloading;

    [Header("References")]
    public AudioClip gunShotSound;
    public AudioClip gunReloadSound;
    public AudioClip gunEmptySound;
    private AudioSource audioSource;
    private Camera mainCamera;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnememy;
    public LayerMask bulletHoleLayer;
    public GameObject flashLight;
    public KeyCode flashLightSwitch;
    private FSM fsm;

    [Header("Graphics")]
    public GameObject bulletHoleGraphic;
    public ParticleSystem muzzleFlash;
    private TextMeshProUGUI bulletsInformation;

    private void Start()
    {
        fsm = FindObjectOfType<FSM>();
        audioSource = GetComponent<AudioSource>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        bulletsInformation = GameObject.FindWithTag("BulletsInformation").GetComponent<TextMeshProUGUI>();
        bulletsLeft = magazineSize;
        readyToShoot = true;
        if (flickerLight)
        {
            StartCoroutine(FlickerFlashlight());
        }
    }

    private void Update()
    {
        if (fsm.state == FSM.StateEnum.Game)
        {
            MyInput();
        }

        bulletsInformation.SetText(bulletsLeft + " | " + magazineSize);
    }

    private void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading)
        {
            Reload();
        }

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
        else if (readyToShoot && shooting && !reloading && bulletsLeft == 0)
        {
            audioSource.clip = gunEmptySound;
            audioSource.Play();
        }

        if (Input.GetKeyDown(flashLightSwitch) && !flickerLight)
        {
            flashLight.SetActive(!flashLight.activeInHierarchy);
        }
    }

    private void Shoot()
    {
        readyToShoot = false;
        audioSource.clip = gunShotSound;
        audioSource.Play();

        // Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = mainCamera.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(mainCamera.transform.position, direction, out rayHit, range, whatIsEnememy))
        {
            rayHit.collider.GetComponent<IDamageable>()?.TakeDamage(damage);
        }

        if (Physics.Raycast(mainCamera.transform.position, direction, out rayHit, range, bulletHoleLayer, QueryTriggerInteraction.Ignore))
        {
            GameObject bulletHole = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, rayHit.normal));
            Destroy(bulletHole, 2f);
        }

        muzzleFlash.Play();

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        audioSource.clip = gunReloadSound;
        audioSource.Play();
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    IEnumerator FlickerFlashlight()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
            flashLight.SetActive(!flashLight.activeInHierarchy);
        }
    }
}
