using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
/// Rigidbody-based class for a thing that [Description]
/// Use Unity's physics system to move this object
///</summary>

[RequireComponent(typeof(Rigidbody))]
public abstract class Antigrav_Object : MonoBehaviour
{
    [Header("Antigravity")]
    protected Vector3 downVector = Vector3.down;
    public float gravityStrength = 1f;
    public float floatForce = 2f;

    protected Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        
    }
}
