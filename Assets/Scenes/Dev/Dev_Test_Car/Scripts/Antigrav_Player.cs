using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CustomLibrary;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Antigrav_Player : MonoBehaviour
{
	#region [red]

	[Header("Velocity")]
	public AnimationCurve velCurve = AnimationCurve.Linear(0f, 0f, 5f, 10f);
	public float maxSpeed = 30f;
	public float minSpeed = 0.05f; //Min speed before it's set to 0
	public float accel = 1f;
	public float decel = 0.3f;

	[Header("Turning")]
	public float turnSpeed = 1f;

	private float accelTimer = 0f;
	private float turnTimer = 0f;
	private new Rigidbody rigidbody;

	#endregion

	#region [debug]

	[Header("Debug")]
	public TextMeshProUGUI debug_01;
	public TextMeshProUGUI debug_02;
	public TextMeshProUGUI debug_03;
	public TextMeshProUGUI debug_04;
	public TextMeshProUGUI debug_05;

	#endregion

	void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();

		if ((debug_01 == null) || (debug_02 == null) || (debug_03 == null) || (debug_04 == null))
			//throw new UnityException("Debug text not set");
			Debug.LogWarning("Debug text not set");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		#region [debug]
		debug_01.SetText(rigidbody.velocity.magnitude.ToString("F"));
		debug_02.SetText(maxSpeed.ToString("F"));

		#endregion
	}

	void FixedUpdate()
	{
		//Input
		if (Input.GetAxisRaw("Accel") != 0)
			accelTimer += Time.fixedDeltaTime;
		else
			accelTimer = 0f;
		accelTimer = Mathf.Clamp(accelTimer, 0f, velCurve[velCurve.length - 1].time);

		//Add manual deceleration
		if (Input.GetAxisRaw("Accel") == 0)
		{
			if (rigidbody.velocity.magnitude < minSpeed)
			{
				rigidbody.AddForce(-1 * rigidbody.velocity);
			}
			else
			{
				if (Input.GetKey(KeyCode.S))
				{
					rigidbody.AddForce(-rigidbody.velocity.normalized * 2 * ((1 / maxSpeed) * rigidbody.velocity.sqrMagnitude));
				}
				else
					rigidbody.AddForce(-rigidbody.velocity.normalized * ((1 / maxSpeed) * rigidbody.velocity.sqrMagnitude));
			}
		}
		else
		{
			var temp = rigidbody.velocity + (transform.rotation * Vector3.forward * velCurve.Evaluate(accelTimer));
			temp = Vector3.ClampMagnitude(temp, maxSpeed);
			rigidbody.AddForce(temp - rigidbody.velocity, ForceMode.VelocityChange);
		}

		//debug_03.SetText("sum: " + (acceleration + drag).ToString("F"));
		debug_04.SetText("Vel Eval:" + velCurve.Evaluate(accelTimer).ToString("F"));
		debug_05.SetText("Drag: " + ((1 / maxSpeed) * rigidbody.velocity.sqrMagnitude).ToString("F"));

		rigidbody.angularVelocity = Quaternion.Euler(0f, 0f, 0f) * new Vector3(0f, turnSpeed * Input.GetAxis("Turning"), 0f); //Use Turn curve
	}

}
