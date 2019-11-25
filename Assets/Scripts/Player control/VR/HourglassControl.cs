using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HourglassControl : MonoBehaviour
{
	public bool isResetting = false;

    public float resetAngle;
    public float timeToReset;
    public float countdown;
    public GameObject slot;
    public bool interactable;

    public void CallReturnToBelt()
    {
        StartCoroutine(ReturnToBelt());
    }

    // Start is called before the first frame update
    public IEnumerator ReturnToBelt()
    {
        Quaternion originalRotation = transform.rotation;

        for (int i = 0; i <= 90; i++)
        {
            float rate = i / 90.0f;
            float percentage = 1 / (1 + Mathf.Pow((float)Math.E, -12.5f * (rate - 0.5f)));

            Vector3 origin = transform.position;
            Vector3 direction = slot.transform.position - origin;

            transform.position = origin + direction * percentage;
            transform.rotation = Quaternion.Lerp(originalRotation, Quaternion.identity, Mathf.Min(percentage * 2.0f, 1.0f));

            yield return new WaitForEndOfFrame();
        }

        interactable = true;
        countdown = timeToReset;
    }

    // Update is called once per frame
    void Update()
    {
		if (isResetting)
		{
			return;
		}

		if (Vector3.Angle(transform.up, Vector3.down) <= resetAngle)
        {
            countdown -= Time.deltaTime;
        }
        else
        {
            countdown = timeToReset;
        }

        if(countdown <= 0)
        {
			isResetting = true;

			FindObjectOfType<VRMovement>().CallResetFade();
        }
    }
}