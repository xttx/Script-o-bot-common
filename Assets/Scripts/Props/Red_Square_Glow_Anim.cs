using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Red_Square_Glow_Anim : MonoBehaviour
{
    [ColorUsageAttribute(true, true)]
    public Color glow_color = new Color(4f, 0.586f, 0.586f);

    // Start is called before the first frame update
    void Start()
    {
        //color orig = new Color(2f, 0.293f, 0.293f)
        //Color newColor = new Color(4f, 0.586f, 0.586f);
        GetComponent<MeshRenderer>().material.DOColor(glow_color, "_EmissionColor", 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        //Material m = transform.parent.GetChild(0).GetComponent<MeshRenderer>().material;
        //m.DOColor(new Color(0.8f, 0.8f, 0.8f), 1.5f).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
