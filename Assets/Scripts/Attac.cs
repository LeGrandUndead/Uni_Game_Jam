using System.Collections;
using UnityEngine;

public class Attac : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform emitter;
    public float projectileForce = 100f;

    [Header("Shooting Settings")]
    public float fireRate = 0.15f;

    [Header("Powerup Toggles")]
    public bool rifleMode = false;    
    public bool homingMode = true; 
    public bool spreadShot = false;
    public bool shockwaveEnabled = false; 

    private bool isShooting = false;
    private bool canShoot = true;

    void Update()
    {
        if (!rifleMode && Input.GetMouseButtonDown(0) && canShoot)
        {
            FireBullet(emitter.rotation);
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerAttack);
        }

        if (rifleMode)
        {
            if (Input.GetMouseButtonDown(0)) { 
                AudioManager.Instance.PlaySFX(AudioManager.Instance.playerRifle);
                StartCoroutine(ShootAuto());
            }



            if (Input.GetMouseButtonUp(0))
                StopShooting();
        }
    }

    void FireBullet(Quaternion rotation)
    {
        int bulletCount = spreadShot ? 5 : 1;
        float spreadAngle = 15f;

        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion bulletRot = rotation;
            if (spreadShot)
            {
                float angle = Random.Range(-spreadAngle, spreadAngle);
                bulletRot *= Quaternion.Euler(0, angle, 0);
            }

            GameObject bulletObj = Instantiate(projectilePrefab, emitter.position, bulletRot);
            Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(bulletObj.transform.forward * projectileForce, ForceMode.Impulse);

            Bullet bulletComp = bulletObj.GetComponent<Bullet>();
            if (bulletComp != null)
            {
                bulletComp.homingEnabled = homingMode;
                if (shockwaveEnabled)
                {
                    float chance = rifleMode ? 0.01f : 0.1f;
                    bulletComp.isShockwave = Random.value < chance;
                }
            }
        }
    }

    IEnumerator ShootAuto()
    {
        if (isShooting) yield break;
        isShooting = true;

        while (Input.GetMouseButton(0))
        {
            FireBullet(emitter.rotation);
            yield return new WaitForSeconds(fireRate);
        }

        isShooting = false;
    }

    void StopShooting()
    {
        isShooting = false;
        StopAllCoroutines();
    }

    IEnumerator ShootCooldown(float delay)
    {
        canShoot = false;
        yield return new WaitForSeconds(delay);
        canShoot = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            rifleMode = true;
            Destroy(other.gameObject);
        }
    }

}
