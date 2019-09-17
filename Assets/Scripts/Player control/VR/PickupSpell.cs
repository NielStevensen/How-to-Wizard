﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickupSpell : MonoBehaviour
{
    public SteamVR_Input_Sources hand;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabAction;
    public SteamVR_Action_Boolean holdAction;

    private GameObject collidingObject;
    private GameObject objectInHand;

    [Tooltip(" array of the belt slots and if they are acceptable placement locations")]
    public bool[] beltSlots;
    public GameObject[] beltObjects;

    private void Update()
    {
        
        if(grabAction.GetLastStateDown(hand) && holdAction.GetState(hand))
        {
            if(collidingObject)
            {
                GrabObject();
            }
        }

        if (grabAction.GetLastStateUp(hand))
        {
            if (collidingObject)
            {
                ReleaseObject();
            }
        }
    }

    private void setCollidingObject(Collider coll)
    {
        if (collidingObject || !coll.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = coll.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        setCollidingObject(other);
    }

    private void OnTriggerStay(Collider other)
    {
        setCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if(!collidingObject)
        {
            return;
        }
        collidingObject = null;

    }
    private void GrabObject()
    {
        objectInHand = collidingObject;

        if (objectInHand.GetComponent<HourglassControl>() != null)
        { 
            if (!objectInHand.GetComponent<HourglassControl>().interactable) return; 
        }
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();

        if (objectInHand.GetComponent<CrystalInfo>() || objectInHand.GetComponent<SpellModuleList>() || objectInHand.GetComponent<HourglassControl>())
        {
            objectInHand.GetComponent<Rigidbody>().isKinematic = false;
        }


        for (int i = 0; i < beltObjects.Length; i++) // check all slots to see if any are suitable
        {
            if (objectInHand.transform.parent == beltObjects[i].transform)
            {
                objectInHand.transform.SetParent(null);
                beltSlots[i] = false;
            }
        }
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();

        fx.breakForce = 200000;
        fx.breakTorque = 200000;

        return fx;
    }
    
    private void ReleaseObject()
    {
        if(GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());

            bool belt = false;
            foreach(bool a in beltSlots) // check all slots to see if any are suitable
            {
                if (a)
                {
                    belt = true;
                }
            }

            if (objectInHand.GetComponent<SpellModuleList>() != null && belt == false)
            {
                SpellModuleList sml = objectInHand.GetComponent<SpellModuleList>(); // if the object is spell

                sml.obj = this;
                sml.hand = hand;
                sml.handTransform = gameObject.transform;
                sml.projectileVelocity = controllerPose.GetVelocity();
                sml.projectileAngularV = controllerPose.GetAngularVelocity();

                objectInHand.GetComponent<Spell>().CallSpell();
                objectInHand.transform.position = new Vector3(0, -100, 0);
            }
            else if (objectInHand.GetComponent<SpellModuleList>() != null) // if the spell is on the belt
            {
                for (int i = 0; i < beltObjects.Length; i ++) // check all slots to see if any are suitable
                {
                    
                    if (beltSlots[i] == true)
                    {
                        objectInHand.transform.SetParent(beltObjects[i].transform);
                        objectInHand.GetComponent<Rigidbody>().isKinematic = true;
                        beltSlots[i] = false;
                    }
                }
            }
            else if (objectInHand.GetComponent<CrystalInfo>()) // dont apply force to certain released objects
            {
                objectInHand.GetComponent<Rigidbody>().isKinematic = true;
            }
            else if (objectInHand.GetComponent<HourglassControl>()) // call horglass back to belt
            {
                objectInHand.GetComponent<HourglassControl>().CallReturnToBelt();
                objectInHand.GetComponent<Rigidbody>().isKinematic = true;
                objectInHand.GetComponent<HourglassControl>().interactable = false;
            }
            else // throw other objects with normal velocity
            {
                objectInHand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();
                objectInHand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();
            }
        }
        objectInHand = null;
    }
}
