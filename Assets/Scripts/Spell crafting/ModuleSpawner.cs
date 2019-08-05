﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSpawner : MonoBehaviour
{

    public GameObject crystal;
    public string Module;
    public int Moduletype;
    // Start is called before the first frame update
    void Start()
    {
        AttemptSpawn();
    }

    void AttemptSpawn()
    {
        if(GetComponentInChildren<CrystalInfo>() == null)
        {
           GameObject crystalref = Instantiate(crystal, gameObject.transform);
           crystalref.GetComponent<CrystalInfo>().moduleType = Moduletype;
           crystalref.GetComponent<CrystalInfo>().module = Module;
            crystalref.transform.position = transform.position;
        }
    }

    private void OnTransformChildrenChanged()
    {
        AttemptSpawn();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}