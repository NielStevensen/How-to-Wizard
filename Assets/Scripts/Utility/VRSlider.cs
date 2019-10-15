using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VRSlider : MonoBehaviour
{

    public bool isToggle; // is this a toggle switch or is it scalar
    public Transform Maxpoint;
    public Transform Minpoint;

    public float scalarValue;
    public bool booleanValue;

    public enum Option { movement , BGM, SFX }
    public Option myOption;
    private void Start()
    {
        switch (myOption)
        {
            case Option.movement:
                {
                    StartCoroutine(lerpTovalue(Info.optionsData.useTeleportation));                  
                    break;
                }
            case Option.BGM:
                {
                    transform.position = Minpoint.position - Info.optionsData.bgmLevel * (Minpoint.transform.position - Maxpoint.transform.position);
                    break;
                }
            case Option.SFX:
                {
                    transform.position = Minpoint.position - Info.optionsData.sfxLevel * (Minpoint.transform.position - Maxpoint.transform.position);
                    break;
                }
        }
    }

    // Start is called before the first frame update
    private void OnTriggerExit(Collider other) // when the hand leaves the slider
    {
        FixedJoint[] toCheck = GameObject.FindObjectsOfType<FixedJoint>();
        foreach (FixedJoint a in toCheck)
        {
            if (a.gameObject == other.gameObject)
            {
                a.GetComponent<PickupSpell>().ReleaseObject();
                Exit();
            }
        }
    }

    public void Exit() // update value when relesed
    {
        float length = (Minpoint.transform.position - Maxpoint.transform.position).magnitude; // measure the lenght of the slidable area
        float value = (Minpoint.transform.position - transform.position).magnitude / length;
        scalarValue = value;
        if (isToggle)
        {
            if (value >= 0.5) booleanValue = true;
            else booleanValue = false;
            StartCoroutine(lerpTovalue(booleanValue));
        }

        switch(myOption)
        {
            case Option.movement:
                {
                    Info.optionsData.useTeleportation = booleanValue;
                    FindObjectOfType<VRMovement>().isTeleportation = booleanValue;
                    break;
                }
            case Option.BGM:
                {
                    Info.optionsData.bgmLevel = scalarValue;
                    break;
                }
            case Option.SFX:
                {
                    Info.optionsData.sfxLevel = scalarValue;
                    break;
                }
        }
        SaveSystem.SaveOptions(Info.optionsData);       
    }

    IEnumerator lerpTovalue(bool side)
    {
        Vector3 origin = transform.position;
        for (int i = 0; i <= 90; i++)
        {
            Vector3 direction = new Vector3(0, 0, 0);
           
            
            float rate = i / 120.0f;
            float percentage = 1 / (1 + Mathf.Pow((float)Math.E, -12.5f * (rate - 0.5f)));

            if (side)
            {
                direction = Maxpoint.transform.position - origin;
            }
            else
            {
                direction = Minpoint.transform.position - origin;
            }

            transform.position = origin + direction * percentage;
            yield return new WaitForEndOfFrame();
        }

    }
}
