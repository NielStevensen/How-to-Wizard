using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalInfo : MonoBehaviour
{
    public string module;
    public int moduleType;
	public int moduleIndex;
    public bool unused = true;
    public Material[] materials;
    public Mesh[] meshes;


    //Used for pc crafting outlines
    [HideInInspector]
	public bool isSelected = false;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = meshes[moduleType];
        GetComponent<Renderer>().material = materials[moduleType];
        var col = GetComponentInChildren<ParticleSystem>().colorOverLifetime.color;
        GetComponentInChildren<ParticleSystem>().startColor = GetComponent<Renderer>().material.color;
    }
}
