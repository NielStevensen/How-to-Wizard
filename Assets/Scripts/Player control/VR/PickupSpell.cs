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

    private VRMovement playerBody;
    public bool isHandInArea = false;
    private Transform camTransform;
    [Tooltip("the list of layers that dont obsturct spell casting")]
    public LayerMask castingLayermask;

    [Tooltip(" array of the belt slots and if they are acceptable placement locations")]
    public bool[] beltSlots;
    public GameObject[] beltObjects;

    //Refernce to creation area to noptify that a spell has been picked up
    private SpellCreation creationArea;

    //Set reference
    private void Start()
    {
        creationArea = FindObjectOfType<SpellCreation>();

        playerBody = GetComponentInParent<VRMovement>();
        camTransform = gameObject.transform.parent.GetComponentInChildren<Camera>().transform;
    }

    private void Update()
    {
        isHandInArea = playerBody.isHeadInArea ? Mathf.Abs(transform.localPosition.x) < playerBody.currentPlayfieldSize.x / 2.0f && Mathf.Abs(transform.localPosition.z) < playerBody. currentPlayfieldSize.z / 2.0f : false;

        if (isHandInArea) // is the hand within the playspace
        {
            if (grabAction.GetLastStateDown(hand) && holdAction.GetState(hand))
            {
                if (collidingObject)
                {
                    GrabObject();
                }
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

        HourglassControl hourglassRef = objectInHand.GetComponent<HourglassControl>();
        SpellModuleList scrollRef = objectInHand.GetComponent<SpellModuleList>();

        if (hourglassRef != null)
        { 
            if (!hourglassRef.interactable) return; 
        }

        collidingObject = null;

        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();

        if (objectInHand.GetComponent<CrystalInfo>() || scrollRef != null || hourglassRef != null)
        {
            objectInHand.GetComponent<Rigidbody>().isKinematic = false;
        }
        
        if (scrollRef != null && scrollRef.transform.parent == null)
        {
            creationArea.isSpellCollected = true;
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

            bool isInBelt = false;

            foreach(bool a in beltSlots) // check all slots to see if any are suitable
            {
                if (a)
                {
                    isInBelt = true;
                }
            }

            bool shouldDestroy = true;

            #region Destroy on invalid hand
            if (!isHandInArea)
            {
                if(objectInHand.GetComponent<HourglassControl>() != null || (objectInHand.GetComponent<SpellModuleList>() != null && isInBelt == true))
                {
                    shouldDestroy = false;
                }

                if (shouldDestroy)
                {
                    DestroyImmediate(objectInHand);
                }
            }
            #endregion

            if(objectInHand != null)
            {
                if (objectInHand.GetComponent<SpellModuleList>() != null && isInBelt == false)
                {
                    if(Physics.Raycast(camTransform.position, transform.position - camTransform.position, out RaycastHit hit, 1000f, ~castingLayermask))
                    {
                        print(hit.collider.gameObject.name);

                        if (hit.collider.gameObject == gameObject)
                        {
                            SpellModuleList sml = objectInHand.GetComponent<SpellModuleList>(); // if the object is spell

                            sml.obj = this;
                            sml.hand = hand;
                            sml.handTransform = gameObject.transform;
                            sml.projectileVelocity = controllerPose.GetVelocity();
                            sml.projectileAngularV = controllerPose.GetAngularVelocity();

                            objectInHand.GetComponent<Spell>().CallSpell();

                            for (int i = 0; i < 5; i++)
                            {
                                objectInHand.transform.GetChild(0).GetChild(i).gameObject.GetComponent<Renderer>().enabled = false;
                            }
                        }
                        else
                        {
                            DestroyImmediate(objectInHand);
                        }
                    }
                    else
                    {
                        print("no hit");

                        DestroyImmediate(objectInHand);
                    }

                }
                else if (objectInHand.GetComponent<SpellModuleList>() != null) // if the spell is on the belt
                {
                    for (int i = 0; i < beltObjects.Length; i++) // check all slots to see if any are suitable
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
        }
        
        objectInHand = null;
    }
}
