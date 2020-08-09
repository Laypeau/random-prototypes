using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class Gen_WheelTest : MonoBehaviour
{
	public float sprungMass;
	public float unsprungMass;
	public Rigidbody carBody;
	public float wheelRadius;
	[Range(0f,1f)] public float gravityPower;
	[SerializeField] private float jounce;

	[Header("Spring settings")]
	public float restDist;
	private float currentPosition;
	public float maxCompression;
	public float stiffness;

	void OnDrawGizmos()
	{
		//Restpos
		Gizmos.color = Color.gray;
		Vector3 restPos = transform.position + (transform.rotation * Vector3.down * restDist);
		Gizmos.DrawSphere(restPos, wheelRadius/5f);
		Gizmos.DrawWireSphere(restPos, wheelRadius);

		//Raycast
		Physics.Raycast(transform.position, transform.rotation * Vector3.down, out RaycastHit rayHit, 100f);
		Debug.DrawRay(transform.position, transform.rotation * Vector3.down * 100f, Color.magenta);
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere(rayHit.point, 0.1f);

		//Wheelpos
		Vector3 wheelPos = transform.position + (transform.rotation * Vector3.down * (rayHit.distance - wheelRadius));
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(wheelPos, wheelRadius / 5f);
		Gizmos.DrawWireSphere(wheelPos, wheelRadius);
	}

	void Start()
	{
		try
		{
			carBody = transform.parent.GetComponent<Rigidbody>();
			sprungMass = carBody.mass; //TEMP
		}
		catch
		{
			throw new UnityException("Rigidbody not set");
		}
	}

	void FixedUpdate()
	{
		Ray ray = new Ray(transform.position, transform.rotation * Vector3.down);
		Physics.Raycast(ray, out RaycastHit rayHit, 100f);

		jounce = restDist + wheelRadius - rayHit.distance; //Positive when compressed

		float force = 0;
		force = (sprungMass * -Physics.gravity.y * gravityPower) + (jounce * stiffness);
		
		Debug.Log("Force: " + force);
		carBody.AddForce(new Vector3(0f, force, 0f), ForceMode.Force);
		//carBody.AddForceAtPosition(new Vector3(0f, force, 0f), carBody.position + transform.position, ForceMode.Force);
	}
}