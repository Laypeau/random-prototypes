using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// Based on DitzelGames FabrIK tutorial: https://youtu.be/qqOAzn05fvk
// FabrIK Algorithm Explanation: https://youtu.be/qqOAzn05fvk?t=856
//
//       |BODY
// 2     0____
//  \   /
//   \ /
//    1

public class IK_Solver : MonoBehaviour
{
    public GameObject asd;
    /// <summary>
    /// How many bones will there be? Bones = gaps between points
    /// </summary>
    public int chainLength = 2;
    /// <summary>
    /// 
    /// </summary>
    public Transform target;
    public bool usingPole = false;
    /// <summary>
    /// Pole position, relative to the base point in the chain
    /// </summary>
    public Vector3 pole;

    [Header("Solver Parameters")]
    /// <summary>
    /// How many times to do the IK thing per frame
    /// </summary>
    public int iterations = 10;
    /// <summary>
    /// If the end point is closer to the target than this, then it will stop the iterations
    /// </summary>
    public float minDelta = 0.001f;
    /// <summary>
    /// Strength of going back to the start position.
    /// </summary>
    [Range(0, 1)]
    public float SnapBackStrength = 1f;

    /// <summary>
    /// An array of the transforms of each joint/pivot thing in the chain
    /// </summary>
    protected Transform[] Bones;
    /// <summary>
    /// A temporary array for iterating over the data from Bones[] before it is applied
    /// </summary>
    protected Vector3[] Positions;
    /// <summary>
    /// Array of the distance between joints
    /// </summary>
    protected float[] BonesLength;
    /// <summary>
    /// Total length of the chain
    /// </summary>
    protected float TotalLength;

    protected Vector3[] StartDirectionSucc;
    protected Quaternion[] StartRotationBone;
    protected Quaternion StartRotationTarget;
    protected Transform Root;

    void Awake()
    {
        Initialise();
    }

    private void Initialise()
    {
        Bones = new Transform[chainLength + 1];
        Positions = new Vector3[chainLength + 1];
        BonesLength = new float[chainLength + 1];
        StartDirectionSucc = new Vector3[chainLength + 1];
        StartRotationBone = new Quaternion[chainLength + 1];
        TotalLength = 0;

        // Slides from the end to the base of the IK chain
        Root = transform;
        for (int i = 0; i <= chainLength; i++)
        {
            if (Root == null)
                throw new UnityException("You wrote the wrong chainlength for IK and it didn't find the proper root bone");
            Root = Root.parent;
        }

        StartRotationTarget = GetRotationRootSpace(target);

        Transform _current = transform;
        for (int i = Bones.Length - 1; i >= 0; i--)
        {
            Bones[i] = _current;
            StartRotationBone[i] = GetRotationRootSpace(_current);

            if (i == Bones.Length - 1) //
            {
                StartDirectionSucc[i] = GetPositionRootSpace(target.position) - GetPositionRootSpace(_current.position);
            }
            else
            {
                BonesLength[i] = (Bones[i + 1].position - _current.position).magnitude;
                StartDirectionSucc[i] = GetPositionRootSpace(Bones[i + 1].position) - GetPositionRootSpace(_current.position);
                TotalLength += BonesLength[i];
            }

            _current = _current.parent;
        }
    }

    void LateUpdate()
    {
        ResolveIK();
    }

    private void ResolveIK()
    {
        if (target == null)
            return;

        if (BonesLength.Length != chainLength + 1)
        {
            Initialise();
        }

        // Apply Bones[] data to Positions[]
        for (int i = 0; i < Bones.Length; i++)
            Positions[i] = GetPositionRootSpace(Bones[i].position);

        var targetPosition = GetPositionRootSpace(target.position);
        var targetRotation = GetRotationRootSpace(target);

        // calculations time
        // is the target further away than the total length of the thing?
        if ((targetPosition - GetPositionRootSpace(Bones[0].position)).sqrMagnitude >= TotalLength * TotalLength) //can replace with ().magnitude >= TotalLength, but not calculating a sqare root is more efficient
        {
            Debug.Log("total length: " + TotalLength);
            // make it straight
            Vector3 direction = (targetPosition - Positions[0]).normalized;
            for (int i = 1; i < Positions.Length; i++)
                Positions[i] = Positions[i - 1] + (direction * BonesLength[i - 1]);
        }
        else
        {
            // 
            for (int i = 0; i < Positions.Length - 1; i++)
                Positions[i + 1] = Vector3.Lerp(Positions[i + 1], Positions[i] + StartDirectionSucc[i], SnapBackStrength);

            for (int iterationNumber = 0; iterationNumber < iterations; iterationNumber++)
            {
                //looping through all Positions[] from end to root
                for (int i = Positions.Length - 1; i > 0; i--)
                {
                    // set the end Position[] to target
                    if (i == Positions.Length - 1)
                        Positions[i] = targetPosition;
                    // set the rest based on their original distances from eachother
                    else
                        Positions[i] = Positions[i + 1] + (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                }

                //looping through all Positions[] from root to end
                for (int i = 1; i < Positions.Length; i++)
                    Positions[i] = Positions[i - 1] + (Positions[i] - Positions[i - 1]).normalized * BonesLength[i - 1];

                // stop if its close enough
                if ((Positions[Positions.Length - 1] - targetPosition).sqrMagnitude <= minDelta * minDelta)
                    break;
            }
        }

        // Move to pole
        if (usingPole)
        {
            var polePosition = GetPositionRootSpace(Root.position + (Root.rotation * pole));

            for (int i = 1; i < Positions.Length - 1; i++) //loops through all but root and end
            {
                var plane = new Plane(Positions[i + 1] - Positions[i - 1], Positions[i - 1]); //a plane between the two joints before and after i, set where the one closest to root is
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(Positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - Positions[i - 1], projectedPole - Positions[i - 1], plane.normal);
                Positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (Positions[i] - Positions[i - 1]) + Positions[i - 1];
            }
        }

        // Apply Positions[] and rotations to Bones[]
        for (int i = 0; i < Positions.Length; i++)
        {
            if (i == Positions.Length - 1)
                SetRotationRootSpace(Bones[i], Quaternion.Inverse(targetRotation) * StartRotationTarget * Quaternion.Inverse(StartRotationBone[i]));
            else
                SetRotationRootSpace(Bones[i], Quaternion.FromToRotation(StartDirectionSucc[i], Positions[i + 1] - Positions[i]) * Quaternion.Inverse(StartRotationBone[i]));
            
            SetPositionRootSpace(Bones[i], Positions[i]);
        }
    }

    // do something about this
    private Vector3 GetPositionRootSpace(Vector3 current)
    {
        if (Root == null)
            return current;
        else
            return Quaternion.Inverse(Root.rotation) * (current - Root.position);
    }

    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if (Root == null)
            current.position = position;
        else
            current.position = Root.rotation * position + Root.position;
    }

    private Quaternion GetRotationRootSpace(Transform current)
    {
        //inverse(after) * before => rot: before -> after
        if (Root == null)
            return current.rotation;
        else
            return Quaternion.Inverse(current.rotation) * Root.rotation;
    }

    private void SetRotationRootSpace(Transform current, Quaternion rotation)
    {
        if (Root == null)
            current.rotation = rotation;
        else
            current.rotation = Root.rotation * rotation;
    }

    void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        Transform _current = this.transform;
        for (int i = 0; i < chainLength && _current != null && _current.parent != null; i++) //the checky bit is to make sure this is not the last joint in the chain
        {
            // Draw the leg
            float _scale = Vector3.Distance(_current.position, _current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(_current.position, Quaternion.FromToRotation(Vector3.up, _current.parent.position - _current.position), new Vector3(_scale, Vector3.Distance(_current.parent.position, _current.position), _scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up / 2, Vector3.one);

            // Draw pole
            if (i == chainLength - 1)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(Root.position + (Root.rotation * pole), 0.11f);
            }

            _current = _current.parent;
        }
#endif
    }
}
