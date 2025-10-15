using UnityEngine;

public enum PowerUpType
{
    Heal,
    Rifle,
    Homing,
    Spreadshot,
    Shockwave,
    Dash,       
    SpeedBoost, 
    Turret      
}

public class PowerUp : MonoBehaviour
{
    public GameObject turretPrefab;
    public Transform player;

    public PowerUpType powerUpType;
    public float healAmount = 20f; 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }


        Attac playerAttack = other.GetComponent<Attac>();
        Movement movement = other.GetComponent<Movement>();

        systeme_sante playerHealth = other.GetComponent<systeme_sante>();

        switch (powerUpType)
        {
            case PowerUpType.Heal:
                if (playerHealth != null)
                {
                    playerHealth.Heal(healAmount);
                }
                break;

            case PowerUpType.Rifle:
                if (playerAttack != null)
                {
                    playerAttack.rifleMode = true;
                }
                break;

            case PowerUpType.Homing:
                if (playerAttack != null)
                {
                    playerAttack.homingMode = true;
                }
                break;

            case PowerUpType.Spreadshot:
                if (playerAttack != null)
                {
                    playerAttack.spreadShot = true;
                }
                break;

            case PowerUpType.Shockwave:
                if (playerAttack != null)
                {
                    playerAttack.shockwaveEnabled = true;
                }
                break;

            case PowerUpType.Dash:
                if (movement != null)
                    movement.dashEnabled = true;
                break;

            case PowerUpType.SpeedBoost:
                if (movement != null)
                    movement.speedBoostEnabled = true;
                break;

            case PowerUpType.Turret:
                if (turretPrefab != null)
                {
                    Vector3 spawnPos = other.transform.position + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
                    GameObject turret = Instantiate(turretPrefab, spawnPos, Quaternion.identity);

                    Turret turretScript = turret.GetComponent<Turret>();
                    if (turretScript != null)
                        turretScript.player = other.transform;
                }
                break;
        }

        Destroy(transform.root.gameObject);
    }
}
