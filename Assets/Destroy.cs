using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    float Timer = 10;

    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if (Timer > 0)
        {
            //Debug.Log(Timer);
            Timer -= Time.deltaTime;
        }
        if (Timer < 0)
        {
            Destroy(gameObject);
        }
    }

}
