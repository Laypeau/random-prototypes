using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Gimbal : MonoBehaviour
{
	public Transform following;
	public Vector3 offset = Vector3.zero;

	private void Update()
	{
		transform.rotation = following.rotation * Quaternion.Euler(offset);
	}
}
