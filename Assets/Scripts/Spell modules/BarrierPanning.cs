using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierPanning : MonoBehaviour
{
    public Vector2 panningSpeed;
    Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        mat.mainTextureScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (Info.isPaused) return;
        mat.mainTextureOffset = (mat.mainTextureOffset + panningSpeed / 1000f);
    }
}
