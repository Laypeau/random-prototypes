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
	/// <summary> Drag = Drag * (1 + Drag multiplier) </summary>
	public float manualDragMultiplier = 1f;
	/// <summary> Additional flat drag </summary>
	public float dragMore = 1f;

	[Header("Turning")]
	public float maxTurnSpeed = 1f;
	public float turnTimer = 0f;
	// public AnimationCurve turnCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	private float accelTimer = 0f;
	LayerMask groundMask;
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
		groundMask = ~LayerMask.GetMask("NotGround", "AlsoNotGround");
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

		if (Input.GetKey(KeyCode.Alpha2))
			rigidbody.velocity = transform.rotation * Vector3.forward * maxSpeed;
		#endregion
	}

	Vector3 upVector = Vector3.up;

	void FixedUpdate()
	{
		if (Input.GetKeyDown(KeyCode.W))
			accelTimer += Time.fixedDeltaTime;
		else
			accelTimer = 0f;
		accelTimer = Mathf.Clamp(accelTimer, 0f, velCurve[velCurve.length - 1].time);

		if (Input.GetAxisRaw("Horizontal") != 0)
			turnTimer += Input.GetAxisRaw("Horizontal") * Time.fixedDeltaTime;
		else
			turnTimer = 0f;
		turnTimer = Mathf.Clamp(turnTimer, -maxTurnSpeed, maxTurnSpeed);

		Ray downRay = new Ray(transform.position, transform.rotation * Vector3.down);
		if(Physics.Raycast(downRay, out RaycastHit rayHit, 10f, groundMask))
		{
			transform.position = rayHit.point + rayHit.normal;
		}

		rigidbody.velocity = Quaternion.FromToRotation(transform.rotation * Vector3.up, rayHit.normal) * rigidbody.velocity;
		rigidbody.angularVelocity = Quaternion.FromToRotation(transform.rotation * Vector3.up, rayHit.normal) * rigidbody.angularVelocity;

		transform.rotation = Quaternion.FromToRotation(transform.rotation * Vector3.up, rayHit.normal) * transform.rotation;

		if (!Input.GetKey(KeyCode.W))
		{
			if (rigidbody.velocity.magnitude < minSpeed)
			{
				rigidbody.AddForce(-1 * rigidbody.velocity);
			}
			else
			{
				rigidbody.AddForce(-rigidbody.velocity.normalized * (dragMore + (1/maxSpeed * rigidbody.velocity.sqrMagnitude) * (1 + (manualDragMultiplier * Input.GetAxis("Decel")))));
			}
		}
		else
		{
			var _accelCalc = Vector3.ClampMagnitude(rigidbody.velocity + (transform.rotation * Vector3.forward * velCurve.Evaluate(accelTimer)), maxSpeed) - rigidbody.velocity;
			rigidbody.AddForce(Quaternion.FromToRotation(transform.rotation * Vector3.up, rayHit.normal) * _accelCalc, ForceMode.VelocityChange);
		}

		if (Input.GetAxisRaw("Horizontal") == 0)
		{
			if (rigidbody.angularVelocity.magnitude < 0.1)
			{
				rigidbody.AddTorque(-1 * rigidbody.angularVelocity);
			}
			else
			{
				rigidbody.AddTorque(-rigidbody.angularVelocity.normalized * rigidbody.angularVelocity.sqrMagnitude);
			}
		}
		else
		{
			var _accelCalc = Vector3.ClampMagnitude(rigidbody.velocity + (transform.rotation * Vector3.forward * velCurve.Evaluate(accelTimer)), maxSpeed) - rigidbody.velocity;
			rigidbody.AddForce(Quaternion.FromToRotation(transform.rotation * Vector3.up, rayHit.normal) * (Vector3.ClampMagnitude(rigidbody.angularVelocity, 5f) - rigidbody.angularVelocity), ForceMode.VelocityChange);
		}

		rigidbody.angularVelocity = transform.rotation * new Vector3(0f, maxTurnSpeed * Input.GetAxis("Turning"), 0f);

		#region [debug]
		debug_03.SetText("Raw: " + Input.GetKey(KeyCode.S).ToString());
		debug_04.SetText("Vel Eval:" + velCurve.Evaluate(accelTimer).ToString("F"));
		debug_05.SetText("Drag: " + (-rigidbody.velocity.normalized * (((1 / maxSpeed) * rigidbody.velocity.sqrMagnitude + dragMore) * (1 + Input.GetAxis("Decel")))).magnitude.ToString("F"));

		Debug.DrawRay(transform.position, rigidbody.velocity, Color.red); //Velocity
		Debug.DrawRay(transform.position, rigidbody.angularVelocity, Color.yellow); //Angular Vel

		Debug.DrawRay(transform.position, transform.rotation * Vector3.up, Color.green); //Up vector
		Debug.DrawRay(rayHit.point, rayHit.normal, Color.magenta); //Surface Normal
		Debug.DrawRay(transform.position, transform.rotation * Vector3.down * 10f, Color.cyan); //Raycast
		#endregion
	}

}
