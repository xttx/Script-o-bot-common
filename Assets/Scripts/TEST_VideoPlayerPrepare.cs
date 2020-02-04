using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TEST_VideoPlayerPrepare : MonoBehaviour
{
    public VideoPlayer vp = null;

    public GameObject prepare = null;

    // Start is called before the first frame update
    void Start()
    {
        vp.prepareCompleted += (VideoPlayer v)=> {
            prepare.SetActive(false);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
