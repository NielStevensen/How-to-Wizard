using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ModuleLabel : MonoBehaviour
{
    Transform player;
	public SpriteRenderer Myimage;
	public Sprite[] sprites;
    // Start is called before the first frame update
    void Start()
    {
        Myimage.sprite = sprites[GetComponent<CrystalInfo>().moduleIndex];
    }
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            findplayer();
        }

        Vector3 relativePos = player.position - transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        Myimage.gameObject.transform.rotation = rotation;
    }

    void findplayer()
    {
        player = FindObjectOfType<CameraController>().gameObject.transform;
    }
}
