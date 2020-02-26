using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_My : MonoBehaviour, Engine.IResetable
{
    bool is_opened = false;
    Animator anim = null;

    void Start() {
        anim = transform.GetComponent<Animator>();
    }
    void OnEnable() {
        is_opened = false;
    }

    public void Open() {
        if (!Control_UI.isPlaying()) { Close(); return; }

        if (!is_opened) {
            anim.SetTrigger("open"); is_opened = true;
        }
    }

    public void Close() {
        if (is_opened) {
            anim.SetTrigger("close"); is_opened = false;
        }
    }

    public void Reset() { Close(); }
}

