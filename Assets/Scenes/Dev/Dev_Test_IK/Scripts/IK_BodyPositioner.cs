using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_BodyPositioner : MonoBehaviour
{
    public float verticalOffset = 0.75f;

    public Transform LB;
    public Transform RB;
    public Transform LF;
    public Transform RF;

    [Range(0f,1f)]
    public float positionLerpSpeed = 0.4f;
    public float floorPosMeanRadius = 0.75f;

    [HideInInspector] public Vector3 UnrotatedRaw;
    [HideInInspector] public Vector3 RotatedRaw;

    private LayerMask layerMask;

    void Awake()
    {
        layerMask = LayerMask.GetMask("Terrain");
    }

    void Update()
    {
        float temp = Mathf.Lerp(transform.position.y, FindAverageFloorPosition(transform.position, 9, 3f, 0.75f, layerMask, false).y + verticalOffset, positionLerpSpeed);
        transform.position = new Vector3(transform.position.x, temp, transform.position.z);

        float _horizontal = Input.GetAxisRaw("Horizontal");
        transform.rotation = transform.rotation * Quaternion.Euler(0f, _horizontal * 50f * Time.deltaTime, 0f);
        float _vertical = Input.GetAxisRaw("Vertical");
        transform.position += Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f) * new Vector3(_vertical * 3f * Time.deltaTime, 0f, 0f);
    }

    /// <summary>
    /// Makes a circle of rays downwards around the origin, starting at positive Z, going clockwise. Returns the average position of all the colliders below.
    /// </summary>
    /// <param name="_Origin"> The origin that the circle of raycasts will start </param>
    /// <param name="_NumberofSamples"> The number of raycasts to be cast around the origin </param>
    /// <param name="_RaycastLength"> The length of the raycasts to be cast around the origin </param>
    /// <param name="_DistFromCentre"> The radius of the circle the raycasts will form around the centre </param>
    /// <param name="_layerMask"> Layermask for raycasts </param>
    /// <param name="_CastFromOrigin"> Should an additional raycast be cast downward from the origin </param>
    public Vector3 FindAverageFloorPosition(Vector3 _Origin, int _NumberofSamples, float _RaycastLength, float _DistFromCentre, LayerMask _layerMask, bool _CastFromOrigin)
    {
        Vector3 CumulativeFloorPosition = Vector3.zero;
        int NumOfRayhits = 0;

        // Ring of raycasts
        for (int i = 1; i < _NumberofSamples + 1; i++)
        {
            Quaternion rotation = Quaternion.Euler(0f, (360f / _NumberofSamples) * i, 0f);
            Ray ray = new Ray(_Origin + (rotation * new Vector3(0f, 0f, _DistFromCentre)), Vector3.down);

            Debug.DrawRay(transform.position + (rotation * new Vector3(0f, 0f, _DistFromCentre)), Vector3.down * _RaycastLength, Color.magenta);

            if (Physics.Raycast(ray, out RaycastHit RayHit, _RaycastLength, _layerMask))
            {
                CumulativeFloorPosition += RayHit.point;
                NumOfRayhits += 1;
            }
        }

        // Additional centre raycast
        if (_CastFromOrigin)
        {
            if (Physics.Raycast(_Origin, Vector3.down, out RaycastHit RayHit, _RaycastLength, _layerMask))
            {
                CumulativeFloorPosition += RayHit.point;
                NumOfRayhits += 1;
            }
        }

        return CumulativeFloorPosition / NumOfRayhits;
    }
}