using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObstacleManager : MonoBehaviour
{
	//Current state
	[Tooltip("The current state of the object.")]
	public bool isActivated = false;
    [Tooltip("Whether or not the object can be toggled off.")]
    public bool isDeactivatable = true;
	[Tooltip("Invisible walls that will be deactivated when this obstacle finishes moving.\nOnly applies to un-deactivatable obstacles.")]
	public GameObject[] targetInvisibleWalls = new GameObject[0];

	[Space(10)]

	//Movement values
	[Tooltip("How far the object moves when activated.")]
	public Vector3 activeDisplacement = Vector3.zero;
	private Vector3 origin;
	private Vector3 obstacleTop;
	private Vector3 moveCheckHalfExtents;
	[Tooltip("All player layers that the door should detect when trying to close.")]
	public LayerMask playerLayers;

	//Acting coroutine and progress
	private Coroutine actingCoroutine;
	private int activationProgress = 0;

	//Set values
	private void Start()
	{
		origin = transform.position;
		obstacleTop = new Vector3(0, transform.localScale.y / 2, 0);
		moveCheckHalfExtents = transform.localScale / 2;
		moveCheckHalfExtents.y = 0.1f;
	}

	//Handle activation/deactivation of the object
	public void HandleState(bool state)
	{
		if (!isDeactivatable && !state)
        {
            return;
        }
	
        isActivated = state;

        if (actingCoroutine != null)
		{
			StopCoroutine(actingCoroutine);
		}
		
		actingCoroutine = StartCoroutine(MoveObstacle(-((state ? 0 : 1) * 2 - 1)));
	}

	//Move the object
	IEnumerator MoveObstacle(int alt)
	{
		int lowerThreshold = 0 - alt;
		int upperThreshold = 180 - alt;

		Vector3 pos;

		while (lowerThreshold < activationProgress && activationProgress < upperThreshold)
		{
			pos = transform.position;

			if (isDeactivatable && alt == -1)
			{
				while (Physics.BoxCastAll(pos + obstacleTop, moveCheckHalfExtents, Vector3.up, transform.rotation, 0.1f, playerLayers).Length > 0)
				{
					yield return new WaitForEndOfFrame();
				}
			}

			activationProgress += 1 * alt;
			
			transform.position = origin + activeDisplacement * Mathf.Sin(Mathf.Deg2Rad * activationProgress * 0.5f);

			yield return new WaitForEndOfFrame();
		}

		if (!isDeactivatable)
		{
			foreach(GameObject obj in targetInvisibleWalls)
			{
				obj.SetActive(false);
			}
		}
	}
}
