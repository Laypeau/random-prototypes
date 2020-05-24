using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_Targeter : MonoBehaviour
{
    public IK_Solver IK_Solver;
    public float stepDistance = 1f;
    [Range(0f, 2f)]
    public float stepTime = 0.2f;

    [Header("Foot Placement Raycasts")]
    /// <summary>
    /// The raycast for leg placement will be relative to this body
    /// </summary>
    public Transform raycastBodyTransform;
    public Vector3 offset;
    public Vector3 raycastAngle = new Vector3(90f, 0f, 0f);
    public float raycastDistance = 5f;

    [Header("Leg Parameters")]
    public float legLiftHeight = 0.1f;
    public AnimationCurve stepCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);

    [Header("Paired Leg Settings (Optional)")]
    public IK_Targeter pairedLeg;
    public bool myTurn = false;

    [HideInInspector] public Vector3 desired;
    [HideInInspector] public bool stepping = false;

    private LayerMask stepMask;

    void Awake()
    {
        stepMask = LayerMask.GetMask("Terrain");
    }
    void Start()
    {
        IK_Solver.target = transform;

        Ray ray = new Ray(raycastBodyTransform.position + (raycastBodyTransform.rotation * offset), (raycastBodyTransform.rotation * Quaternion.Euler(raycastAngle)) * Vector3.up * 10f);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, raycastDistance, stepMask))
            desired = hitinfo.point;
        else
            desired = transform.position;

        if (pairedLeg != null && myTurn)
            transform.position = desired;
    }

    void Update()
    {
        if (pairedLeg == null)
        {
            if (TestStep())
            {
                StartCoroutine(MoveLeg(stepTime));
            }
        }
        else
        {
            if (TestStep() && !pairedLeg.stepping && myTurn)
            {
                myTurn = false;
                pairedLeg.myTurn = true;
                StartCoroutine(MoveLeg(stepTime));
            }
        }
    }

    public bool TestStep()
    {
        Ray ray = new Ray(raycastBodyTransform.position + (raycastBodyTransform.rotation * offset), (raycastBodyTransform.rotation * Quaternion.Euler(raycastAngle)) * Vector3.up * 10f);
        if (Physics.Raycast(ray, out RaycastHit hitinfo, raycastDistance, stepMask))
            desired = hitinfo.point;

        if (!stepping && Vector3.Distance(transform.position, desired) > stepDistance)
            return true;
        else
            return false;
    }

    IEnumerator MoveLeg(float _duration)
    {
        Vector3 _targetPos = transform.position;
        Vector3 _desiredPos = desired;
        Vector3 mid = Vector3.Lerp(_targetPos, _desiredPos, 0.5f) + (Vector3.up * legLiftHeight);

        stepping = true;
        float _start = Time.unscaledTime;

        // For the first half of _duration, lerp from _targetPos to _mid
        while (_start + (_duration / 2) > Time.unscaledTime)
        {
            transform.position = Vector3.Lerp(_targetPos, Vector3.Lerp(_targetPos, desired, 0.5f) + (Vector3.up * legLiftHeight), stepCurve.Evaluate((Time.unscaledTime - _start) / (_duration / 2)));
            yield return null;
        }

        var variable = transform.position;
        // For the second half of _duration, lerp from _mid to _desiredPos
        while (_start + _duration > Time.unscaledTime)
        {
            Debug.Log((Time.unscaledTime - _start - (_duration/2)) / (_duration/2));
            transform.position = Vector3.Lerp(variable, desired, stepCurve.Evaluate((Time.unscaledTime - _start - (_duration / 2)) / (_duration / 2))); //glorious brackets
            yield return null;
        }
        transform.position = desired;

        stepping = false;
        yield return null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.099f);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(raycastBodyTransform.position + (raycastBodyTransform.rotation * offset), (raycastBodyTransform.rotation * Quaternion.Euler(raycastAngle)) * Vector3.up * 10f);
    }
}
