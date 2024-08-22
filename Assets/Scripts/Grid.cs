using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private GameObject flag01;
    private GameObject flag02;
    void Start()
    {
        flag01 = transform.Find("Flag01").gameObject;
        flag02 = transform.Find("Flag02").gameObject;
    }

    public void SetFlag(int flag)
    {
        if (flag01 == null || flag02 == null) return;
        flag01.SetActive(flag == 1);
        flag02.SetActive(flag == -1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
