using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam_Lerp_Fixed : MonoBehaviour
{
	public Transform following;
	[Header("Offset")]
	public Vector3 positionOffset = Vector3.zero;
	public Vector3 rotationOffset = Vector3.zero;

	[Header("Lerp Settings")]
	[Range(0f, 1f)] public float positionLerp = 0.9f;
	[Range(0f, 1f)] public float rotationLerp = 0.9f;

	void Awake()
	{
		if (following == null)
			throw new UnityException("Camera not set to follow");
	}

	void Update()
	{
		transform.position = Vector3.Lerp(transform.position, following.position + (following.rotation * positionOffset), positionLerp);
		transform.rotation = Quaternion.Lerp(transform.rotation, following.rotation * Quaternion.Euler(rotationOffset), rotationLerp);
	}
}
