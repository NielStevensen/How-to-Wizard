using System.Collections;
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
        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();

        if (objectInHand.GetComponent<CrystalInfo>())
        {
            objectInHand.GetComponent<Rigidbody>().isKinematic = false;
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

            if (objectInHand.GetComponent<Spell>())
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
            else if (objectInHand.GetComponent<CrystalInfo>()) // dont apply force to crystals relased
            {
                objectInHand.GetComponent<Rigidbody>().isKinematic = true;
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
