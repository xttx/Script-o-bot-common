using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collector_Ball : MonoBehaviour
{
    public Color color = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        // transform.GetChild(4).localPosition = new Vector3(-0.05f, -0.18f, -0.05f);
        // transform.GetChild(4).DOLocalMoveX(0.05f, 0.5f).SetLoops(-1, LoopType.Yoyo);
        // transform.GetChild(4).DOLocalMoveZ(0.05f, 0.4f).SetLoops(-1, LoopType.Yoyo);
        // transform.GetChild(4).DOLocalMoveZ(-0.135f, 0.3f).SetLoops(-1, LoopType.Yoyo);
        transform.GetChild(0).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.12f);
        transform.GetChild(1).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.12f);
        transform.GetChild(2).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.12f);
        transform.GetChild(3).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 0.12f);
        transform.GetChild(4).GetComponent<MeshRenderer>().material.color = new Color(color.r, color.g, color.b, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
