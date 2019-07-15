using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PickupSpell : MonoBehaviour
{
    public SteamVR_Input_Sources hand;
    public SteamVR_Behaviour_Pose controllerPose;
    public SteamVR_Action_Boolean grabAction;

    private GameObject collidingObject;
    private GameObject objectInHand;


    private void Update()
    {
        if(grabAction.GetLastStateDown(hand))
        {
            if(collidingObject)
            {
                GrabObject();
            }
        }        if(grabAction.GetLastStateUp(hand))
        {
            if(collidingObject)
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
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 1000000;
        fx.breakTorque = 1000000;
        return fx;
    }
    
    private void ReleaseObject()
    {
        if(GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            if(objectInHand.GetComponent<Spell>())
            {
                objectInHand.GetComponent<Spell>().VRSpell();
                objectInHand.transform.position = new Vector3(0, -100, 0);
            }
            else
            {
                objectInHand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();
                objectInHand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();
            }
        }
        objectInHand = null;
    }
}
