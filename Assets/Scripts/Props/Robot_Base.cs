using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot_Base : MonoBehaviour
{
    Animator animator;
    Vector3[] initial_robot_parts_positions = null;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Transform bot_tr = GameObject.Find("Robot").transform.GetChild(0);
        initial_robot_parts_positions = new Vector3[bot_tr.childCount];
        for (int i = 0; i < bot_tr.childCount; i++)
            initial_robot_parts_positions[i] = bot_tr.GetChild(i).localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //This is called from animation event, at start of open animation
    public void Appear() 
    {
        //float spd = animator.GetCurrentAnimatorStateInfo(0).speed;
        //if (spd > 0)
        Transform bot_tr = BOT.bot_obj.transform.GetChild(0);
        for (int i = 0; i < bot_tr.childCount; i++) {
            bot_tr.GetChild(i).localPosition = initial_robot_parts_positions[i];
            bot_tr.GetChild(i).localRotation = Quaternion.identity;
        }

        bot_tr.GetComponent<MeshRenderer>().enabled = true;
        bot_tr.GetChild(6).localScale = new Vector3(1f, 1f, 1f);                        //arm left
        bot_tr.GetChild(6).localRotation = Quaternion.Euler(0f, 71.913f, 38.708f);
        bot_tr.GetChild(6).GetChild(1).localRotation = Quaternion.identity;             //arm left -> wrist
        bot_tr.GetChild(7).localScale = new Vector3(1f, 1f, 1f);                        //arm right
        bot_tr.GetChild(7).localRotation = Quaternion.Euler(0f, -82.081f, -51.78f);
        bot_tr.GetChild(7).GetChild(1).localRotation = Quaternion.identity;             //arm right -> wrist

        BOT.bot_obj.SetActive(true);
    }

    //This is called from animation event, at the middle of open animation
    public void UnParent_BOT() 
    {
        BOT.bot_obj.transform.SetParent(null, true);
        BOT.bot_obj.transform.localScale = new Vector3(3f, 3f, 3f);
    }

    //This is called from animation event, at the middle of dissolving animation
    public void Dissolving() 
    {
        float spd = animator.GetCurrentAnimatorStateInfo(0).speed;
        //Debug.Log(spd.ToString());
        //Speed 1   - first part of animation, i.e. base appear
        //Speed -1  = second part of animation, i.e. base dissolving
        if (spd < 0) {
            BOT.bot_obj.GetComponent<BoxCollider>().enabled = true;
            BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false;
            Control_UI.enable_canvas();
        }
    }

    public void CancelAnimation()
    {
        UnParent_BOT(); Appear();
        BOT.bot_obj.GetComponent<BoxCollider>().enabled = true;
        BOT.bot_obj.GetComponent<Rigidbody>().isKinematic = false;
        Control_UI.enable_canvas();
        animator.Play("Anim_Dissolve 1", 0, 1f);
    }

}
