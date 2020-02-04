using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Robot : MonoBehaviour
{
    static Robot inst = null;
    List<Material> mats_to_dissolve = new List<Material>();

    float next_look_time = 6f;
    float next_tail_time = 5f;
    float next_blink_time = 0f;
    Transform eye_1;
    Transform eye_2;
    Animator robot_anim;

    public AudioClip SFX_Move = null;
    public AudioClip SFX_Rotate = null;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        robot_anim = transform.GetChild(0).GetComponent<Animator>();
        eye_1 = transform.GetChild(0).GetChild(0);
        eye_2 = transform.GetChild(0).GetChild(2);

        foreach (MeshRenderer mr in transform.GetChild(0).GetComponentsInChildren<MeshRenderer>()){
            if (mr.material.HasProperty("_DissolveCutoff")) mats_to_dissolve.Add(mr.material);
            //mr.material.SetFloat("_DissolveCutoff", 0.5f);
            //mr.material.SetFloat("_DissolveMaskOffset", 0f);
            //Debug.Log(mr.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= next_blink_time) {
            int r = Random.Range(0, 10);
            if (r == 0) robot_anim.SetTrigger("Blink");
            else        robot_anim.SetTrigger("Blink2");

            next_blink_time = Time.time + Random.Range(3f, 6f);
            //next_blink_time = Time.time + Random.Range(3f, 1f); //Debug, speed blink
        }

        if (Time.time >= next_tail_time) {
            robot_anim.SetTrigger("Tail");
            next_tail_time = Time.time + Random.Range(8f, 15f);
            //next_tail_time = Time.time + Random.Range(8f, 2f); //Debug, speed tail
        }

        if (Time.time >= next_look_time) {
            int look_type = Random.Range(0, 3); //0 - default, 1 - sync, 2 async
            if (look_type == 0) {
                eye_1.DOLocalRotate (new Vector3(0f, 0f, 0f), 0.1f);
                eye_2.DOLocalRotate (new Vector3(0f, 0f, 0f), 0.1f);
            } else if (look_type == 1) {
                float x = Random.Range(-20f, 20f);
                float y = Random.Range(-40f, 20f);
                eye_1.DOLocalRotate (new Vector3(x, y, 0f), 0.1f);
                eye_2.DOLocalRotate (new Vector3(x, y, 0f), 0.1f);
            } else if (look_type == 2) {
                float x1 = Random.Range(-20f, 20f);
                float y1 = Random.Range(-40f, 20f);
                float x2 = Random.Range(-20f, 20f);
                float y2 = Random.Range(-40f, 20f);
                eye_1.DOLocalRotate (new Vector3(x1, y1, 0f), 0.1f);
                eye_2.DOLocalRotate (new Vector3(x2, y2, 0f), 0.1f);
            }
            next_look_time = Time.time + Random.Range(1f, 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        BOT.cur_colliders.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        BOT.cur_colliders.Remove(other);
    }
    public void Dissolve()
    {
        foreach (Material m in mats_to_dissolve) {
            m.DOKill();
            m.DOFloat(0.75f, "_DissolveMaskOffset", 0.75f);
        }
    }
    public void Appear()
    {
        foreach (Material m in mats_to_dissolve) {
            m.DOKill();
            m.DOFloat(-0.75f, "_DissolveMaskOffset", 0.75f);
        }
    }

    public void Appear_Immediate()
    {
        foreach (Material m in mats_to_dissolve) {
            m.DOKill();
            m.SetFloat("_DissolveMaskOffset", -0.75f);
        }
    }

    public static void PlaySfx(int i) {
        var src = inst.GetComponent<AudioSource>();
        if (i >= 0) {
            AudioClip aud = null;
            if (i == 0) aud = inst.SFX_Move;
            else if (i == 1) aud = inst.SFX_Rotate;

            if (aud != null) {
                src.clip = aud; src.Play();
            }
        } else {
            src.Stop();
        }
    }
}
