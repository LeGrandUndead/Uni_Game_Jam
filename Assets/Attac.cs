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
    public bool canUseRifle = false; 

    private bool isShooting = false;
    private bool canShoot = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !canUseRifle && canShoot)
        {
            ShootSingle();
        }

        if (canUseRifle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(ShootAuto());
            }
            if (Input.GetMouseButtonUp(0))
            {
                StopShooting();
            }
        }
    }

    void ShootSingle()
    {
        GameObject bullet = Instantiate(projectilePrefab, emitter.position, emitter.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(emitter.forward * projectileForce, ForceMode.Impulse);
        }

        Debug.Log("Single shot fired!");
    }

    IEnumerator ShootAuto()
    {
        if (isShooting) yield break;
        isShooting = true;

        while (Input.GetMouseButton(0))
        {
            if (!canUseRifle) break; 

            GameObject bullet = Instantiate(projectilePrefab, emitter.position, emitter.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(emitter.forward * projectileForce, ForceMode.Impulse);
            }

            Debug.Log("Automatic rifle shot!");
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
            GetComponent<Attac>().canUseRifle = true;
            Destroy(other.gameObject);
        }
    }
}
