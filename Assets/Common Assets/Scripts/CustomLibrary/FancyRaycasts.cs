using UnityEngine;
using System.Collections;

namespace CustomLibrary
{
	public static class FancyRaycasts
	{
		/// <summary>
		/// Makes a circle of rays around the origin, pointing in the direction specified.
		/// </summary>
		/// <param name="Origin"> The origin that the circle of raycasts will start </param>
		/// <param name="RaycastDirection"> The direction the raycasts will point, and the direction the circle will face </param>
		/// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
		/// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
		/// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
		/// <param name="Layermask"> Layermask for raycasts </param>
		/// <param name="CastFromOrigin"> Should an additional raycast be cast downward from the origin </param>
		public static RaycastHit[] RayhitCircle(Vector3 Origin, Quaternion RaycastDirection, int NumberofSamples, float Distance, float Radius, LayerMask Layermask, bool CastFromOrigin)
		{
			ArrayList RaycastHits = new ArrayList();

			// Ring of raycasts
			for (int i = 0; i < NumberofSamples; i++)
			{
				Quaternion rotation = RaycastDirection * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
				Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), RaycastDirection * Vector3.down);


				if (Physics.Raycast(ray, out RaycastHit RayHit, Distance, Layermask))
				{
					RaycastHits.Add(RayHit);
				}
			}

			// Additional raycast from origin
			if (CastFromOrigin)
			{
				if (Physics.Raycast(Origin, Vector3.down, out RaycastHit RayHit, Distance, Layermask))
				{
					RaycastHits.Add(RayHit);
				}
			}

			return (RaycastHit[])RaycastHits.ToArray(typeof(RaycastHit));
		}

		#region [red] PositionAverageCircle() using quaternions
		/// <summary>
		/// Makes a circle of rays around the origin, pointing in the direction specified. Returns the average position of all the colliders below.
		/// </summary>
		/// <param name="Origin"> The origin that the circle of raycasts will start </param>
		/// <param name="RaycastDirection"> The direction the raycasts will point, and the normal direction the circle will face </param>
		/// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
		/// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
		/// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
		/// <param name="Layermask"> Layermask for raycasts </param>
		/// <param name="CastFromOrigin"> Should an additional raycast be cast downward from the origin </param>
		public static Vector3 PositionAvgCircle(Vector3 Origin, Quaternion RaycastDirection, int NumberofSamples, float Distance, float Radius, LayerMask Layermask, bool CastFromOrigin)
		{
			Vector3 CumulativeFloorPosition = Vector3.zero;
			int NumOfRayhits = 0;

			// Ring of raycasts
			for (int i = 1; i < NumberofSamples + 1; i++)
			{
				Quaternion rotation = RaycastDirection * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
				Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), RaycastDirection * Vector3.down);

				if (Physics.Raycast(ray, out RaycastHit RayHit, Distance, Layermask))
				{
					CumulativeFloorPosition += RayHit.point;
					NumOfRayhits += 1;
				}
			}

			// Additional raycast from origin
			if (CastFromOrigin)
			{
				if (Physics.Raycast(Origin, Vector3.down, out RaycastHit RayHit, Distance, Layermask))
				{
					CumulativeFloorPosition += RayHit.point;
					NumOfRayhits += 1;
				}
			}

			return CumulativeFloorPosition / NumOfRayhits;
		}
		/// <summary>
		/// Makes a circle of rays around the origin, pointing in the direction specified. Returns the average position of all the colliders below.
		/// </summary>
		/// <param name="Origin"> The origin that the circle of raycasts will start </param>
		/// <param name="RaycastDirection"> The direction the raycasts will point, and the normal direction the circle will face </param>
		/// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
		/// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
		/// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
		/// <param name="Layermask"> Layermask for raycasts </param>
		public static Vector3 PositionAvgCircle(Vector3 Origin, Quaternion RaycastDirection, int NumberofSamples, float Distance, float Radius, LayerMask Layermask)
		{
			Vector3 CumulativeFloorPosition = Vector3.zero;
			int NumOfRayhits = 0;

			// Ring of raycasts
			for (int i = 1; i < NumberofSamples + 1; i++)
			{
				Quaternion rotation = RaycastDirection * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
				Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), RaycastDirection * Vector3.down);

				if (Physics.Raycast(ray, out RaycastHit RayHit, Distance, Layermask))
				{
					CumulativeFloorPosition += RayHit.point;
					NumOfRayhits += 1;
				}
			}

			return CumulativeFloorPosition / NumOfRayhits;
		}
		/// <summary>
		/// Makes a circle of rays around the origin, pointing in the direction specified. Returns the average position of all the colliders below.
		/// </summary>
		/// <param name="Origin"> The origin that the circle of raycasts will start </param>
		/// <param name="RaycastDirection"> The direction the raycasts will point, and the normal direction the circle will face </param>
		/// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
		/// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
		/// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
		public static Vector3 PositionAvgCircle(Vector3 Origin, Quaternion RaycastDirection, int NumberofSamples, float Distance, float Radius)
		{
			Vector3 CumulativeFloorPosition = Vector3.zero;
			int NumOfRayhits = 0;

			// Ring of raycasts
			for (int i = 1; i < NumberofSamples + 1; i++)
			{
				Quaternion rotation = RaycastDirection * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
				Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), RaycastDirection * Vector3.down);

				if (Physics.Raycast(ray, out RaycastHit RayHit, Distance))
				{
					CumulativeFloorPosition += RayHit.point;
					NumOfRayhits += 1;
				}
			}

			return CumulativeFloorPosition / NumOfRayhits;
		}
		#endregion

		#region [orange] PositionAverageCircle using euler angles
		/// <summary>
		/// Makes a circle of rays around the origin, pointing in the direction specified. Returns the average position of all the colliders below.
		/// </summary>
		/// <param name="Origin"> The origin that the circle of raycasts will start </param>
		/// <param name="RaycastDirection"> The direction the raycasts will point, and the normal direction the circle will face </param>
		/// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
		/// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
		/// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
		/// <param name="Layermask"> Layermask for raycasts </param>
		/// <param name="CastFromOrigin"> Should an additional raycast be cast downward from the origin </param>
		public static Vector3 PositionAvgCircle(Vector3 Origin, Vector3 RaycastDirection, int NumberofSamples, float Distance, float Radius, LayerMask Layermask, bool CastFromOrigin)
        {
            Vector3 CumulativeFloorPosition = Vector3.zero;
            int NumOfRayhits = 0;

            // Ring of raycasts
            for (int i = 1; i < NumberofSamples + 1; i++)
            {
                Quaternion rotation = Quaternion.Euler(RaycastDirection) * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
                Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), Quaternion.Euler(RaycastDirection) * Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit RayHit, Distance, Layermask))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            // Additional raycast from origin
            if (CastFromOrigin)
            {
                if (Physics.Raycast(Origin, Vector3.down, out RaycastHit RayHit, Distance, Layermask))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            return CumulativeFloorPosition / NumOfRayhits;
        }
		/// <summary>
        /// Makes a circle of rays around the origin, pointing in the direction specified. Returns the average position of all the colliders below.
        /// </summary>
        /// <param name="Origin"> The origin that the circle of raycasts will start </param>
        /// <param name="RaycastDirection"> The direction the raycasts will point, and the normal direction the circle will face </param>
        /// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
        /// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
        /// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
        /// <param name="Layermask"> Layermask for raycasts </param>
		public static Vector3 PositionAvgCircle(Vector3 Origin, Vector3 RaycastDirection, int NumberofSamples, float Distance, float Radius, LayerMask Layermask)
        {
            Vector3 CumulativeFloorPosition = Vector3.zero;
            int NumOfRayhits = 0;

            // Ring of raycasts
            for (int i = 1; i < NumberofSamples + 1; i++)
            {
                Quaternion rotation = Quaternion.Euler(RaycastDirection) * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
                Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), Quaternion.Euler(RaycastDirection) * Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit RayHit, Distance, Layermask))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            return CumulativeFloorPosition / NumOfRayhits;
        }
		/// <summary>
        /// Makes a circle of rays around the origin, pointing in the direction specified. Returns the average position of all the colliders below.
        /// </summary>
        /// <param name="Origin"> The origin that the circle of raycasts will start </param>
        /// <param name="RaycastDirection"> The direction the raycasts will point, and the normal direction the circle will face </param>
        /// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
        /// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
        /// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
		public static Vector3 PositionAvgCircle(Vector3 Origin, Vector3 RaycastDirection, int NumberofSamples, float Distance, float Radius)
        {
            Vector3 CumulativeFloorPosition = Vector3.zero;
            int NumOfRayhits = 0;

            // Ring of raycasts
            for (int i = 1; i < NumberofSamples + 1; i++)
            {
                Quaternion rotation = Quaternion.Euler(RaycastDirection) * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
                Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), Quaternion.Euler(RaycastDirection) * Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit RayHit, Distance))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            return CumulativeFloorPosition / NumOfRayhits;
        }
		#endregion


		//UNFINISHED
		/// <summary>
        /// Makes an oval of rays around the origin, pointing in the direction specified. Returns the average position of all the colliders below.
        /// </summary>
        /// <param name="Origin"> The origin that the circle of raycasts will start </param>
        /// <param name="RaycastDirection"> The direction the raycasts will point, and the normal direction the circle will face </param>
        /// <param name="NumberofSamples"> The number of raycasts to be cast around the origin </param>
        /// <param name="Distance"> The length of the raycasts to be cast around the origin </param>
        /// <param name="Radius"> The radius of the circle the raycasts will form around the centre </param>
        /// <param name="Layermask"> Layermask for raycasts </param>
        /// <param name="CastFromOrigin"> Should an additional raycast be cast downward from the origin </param>
		public static Vector3 PositionAvgEllipse(Vector3 Origin, Vector3 RaycastDirection, int NumberofSamples, float Distance, float Radius, LayerMask Layermask, bool CastFromOrigin)
        {
            Vector3 CumulativeFloorPosition = Vector3.zero;
            int NumOfRayhits = 0;

            // Ring of raycasts
            for (int i = 1; i < NumberofSamples + 1; i++)
            {
                Quaternion rotation = Quaternion.Euler(RaycastDirection) * Quaternion.Euler(0f, (360f / NumberofSamples) * i, 0f);
                Ray ray = new Ray(Origin + (rotation * new Vector3(0f, 0f, Radius)), Quaternion.Euler(RaycastDirection) * Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit RayHit, Distance, Layermask))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            // Additional raycast from origin
            if (CastFromOrigin)
            {
                if (Physics.Raycast(Origin, Vector3.down, out RaycastHit RayHit, Distance, Layermask))
                {
                    CumulativeFloorPosition += RayHit.point;
                    NumOfRayhits += 1;
                }
            }

            return CumulativeFloorPosition / NumOfRayhits;
        }

	}
}
