using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attac : MonoBehaviour
{
    public GameObject Projectile;
    public Transform Emitter;
    public float frequency = 0.1f;
    public int Number = 3;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        for (int i = 0; i < Number; i++)
        {
            Instantiate(Projectile, Emitter.position, Emitter.rotation);
            yield return new WaitForSeconds(frequency);
        }
    }
}
