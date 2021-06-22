using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationScript : MonoBehaviour
{
    [SerializeField]
    private float x;    
    [SerializeField]
    private float z;

    // Start is called before the first frame update
    void Start()
    {
        x = 0.5f;
        z = 0.5f; //velocity.
    }
    void Update()
    {
        gameObject.transform.Rotate(new Vector3(x, 0, z)); //applying rotation.
    }
}
