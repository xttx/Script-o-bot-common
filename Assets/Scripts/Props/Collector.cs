using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{

    public Loot.loot_type_enum required_loot_type;
    public required_loot_quantity_type req_quantity_type;
    public int required_loot_quantity;

    public int status = 0;

    int put_quantity = 0;

    public enum required_loot_quantity_type {
        set,
        last_n_taken
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Put(Loot.loot_type_enum loot_Type, int quantity) {
        if (loot_Type == required_loot_type) {
            put_quantity += quantity;
            if (req_quantity_type == required_loot_quantity_type.set) {
                if (put_quantity == required_loot_quantity) {
                    //Debug.Log("Collector trigger OK");
                    PutOK();
                } else {
                    Debug.Log("You put " + put_quantity.ToString() + ", but i'm waiting for " + required_loot_quantity.ToString());
                }
            } else if (req_quantity_type == required_loot_quantity_type.last_n_taken) {
                int found_count = 0;
                int found_q = 0;
                if (Engine.Loot_list.Count == 0) {
                    Debug.Log("Loot_list is empty."); return;
                }

                for (int i = Engine.Loot_list.Count - 1; i >= 0; i--) {
                    //Debug.Log("Check loot list @" + i.ToString());
                    if (Engine.Loot_list[i].Key == required_loot_type) {
                        found_count += 1;
                        found_q += Engine.Loot_list[i].Value;
                        if (found_count == required_loot_quantity) break;
                    }
                }
                if (found_count == required_loot_quantity) {
                    if (quantity == found_q) {
                        //Debug.Log("Collector trigger OK");
                        PutOK();
                    } else {
                        Debug.Log("Required " + found_q.ToString() + ", that you got from last " + found_count.ToString() + " containers, but you only put " + quantity.ToString());
                    }
                } else {
                    Debug.Log("Required " + required_loot_quantity.ToString() + " continers. You only got " + found_count.ToString());
                }
            }
        } else {
            Debug.Log("Wrong loot type.");
        }
    }

    void PutOK() {
        Debug.Log("Collector trigger OK");
        status = 1;
    }
}
