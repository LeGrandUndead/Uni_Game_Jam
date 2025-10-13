using UnityEngine;

public enum PowerUpType
{
    Heal,
    Rifle,
    Homing,
    Spreadshot,
    Shockwave
}

public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType;
    public float healAmount = 20f; // Only used if Heal

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            Debug.Log($"PowerUp {powerUpType} ignored collision with {other.name}");
            return;
        }

        Debug.Log($"Player touched powerup: {powerUpType}");

        Attac playerAttack = other.GetComponent<Attac>();
        systeme_sante playerHealth = other.GetComponent<systeme_sante>();

        switch (powerUpType)
        {
            case PowerUpType.Heal:
                if (playerHealth != null)
                {
                    playerHealth.Heal(healAmount);
                    Debug.Log($"Applied Heal: {healAmount} to player {other.name}");
                }
                else
                    Debug.LogWarning("Player has no systeme_sante component!");
                break;

            case PowerUpType.Rifle:
                if (playerAttack != null)
                {
                    playerAttack.rifleMode = true;
                    Debug.Log("Rifle mode enabled for player");
                }
                else
                    Debug.LogWarning("Player has no Attac component!");
                break;

            case PowerUpType.Homing:
                if (playerAttack != null)
                {
                    playerAttack.homingMode = true;
                    Debug.Log("Homing mode enabled for player");
                }
                else
                    Debug.LogWarning("Player has no Attac component!");
                break;

            case PowerUpType.Spreadshot:
                if (playerAttack != null)
                {
                    playerAttack.spreadShot = true;
                    Debug.Log("Spreadshot mode enabled for player");
                }
                else
                    Debug.LogWarning("Player has no Attac component!");
                break;

            case PowerUpType.Shockwave:
                if (playerAttack != null)
                {
                    playerAttack.shockwaveEnabled = true;
                    Debug.Log("Shockwave mode enabled for player");
                }
                else
                    Debug.LogWarning("Player has no Attac component!");
                break;
        }

        Destroy(transform.root.gameObject);
        Debug.Log($"PowerUp {powerUpType} destroyed after pickup");
    }
}
