using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomLibrary
{
	public class General
	{
		/// <summary> Conventionally rounds each component of a Vector3 to the nearest value </summary>
		/// <param name="vector"> Vector3 to round </param>
		/// <param name="round"> Value to round each component to </param>
		public static Vector3 Vector3Round(Vector3 vector, float round)
		{			
			return new Vector3(Mathf.Floor((vector.x / round) + 0.5f) * round, Mathf.Floor((vector.y / round) + 0.5f) * round, Mathf.Floor((vector.z / round) + 0.5f) * round);
		}

		/// <summary> Conventionally rounds each component of a Vector2 to the nearest value </summary>
		/// <param name="vector"> Vector3 to round </param>
		/// <param name="round"> Value to round each component to </param>
		public static Vector2 Vector2Round(Vector2 vector, float round)
		{
			return new Vector2(Mathf.Floor((vector.x / round) + 0.5f) * round, Mathf.Floor((vector.y / round) + 0.5f) * round);
		}
	}
}

