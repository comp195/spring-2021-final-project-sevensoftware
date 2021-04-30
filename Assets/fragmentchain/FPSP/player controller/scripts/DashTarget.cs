using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashTarget : MonoBehaviour
{
    private float counter;
    public float lifetime;

    void Update()
    {
        counter += Time.deltaTime;
        if(counter >= lifetime)
        {
            Destroy(this.gameObject);
        }
    }
}
