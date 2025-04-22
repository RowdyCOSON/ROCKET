using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mob_pool_manager : MonoBehaviour
{
    public static mob_pool_manager ins;
    public zombie mob_prf;
    public int mob_amount = 100;

    public GameObject spawn_pos;
    public List<zombie> mob_pool = new List<zombie>();


    private void Awake()
    {
        if(mob_pool_manager.ins == null)
        {
            ins = this;
        }
        else
        {
            Debug.LogError("ins error");
            Destroy(this.gameObject);
        }
    }

 

    private void Start()
    {
        for(int i = 0 ; i < mob_amount ; i++)
        {
            zombie z = Instantiate(mob_prf , spawn_pos.transform);
            z.id = i;
            z.gameObject.name = z.id.ToString();
            mob_pool.Add(z);
        }

        GM.ins.START = true;
    }
    public zombie get_pool()
    {
        for (int i = 0 ;i < mob_pool.Count ;i++)
        {
            if(mob_pool[i].is_alive() == false)
            {
                return mob_pool[i];
            }
        }

        zombie z = Instantiate(mob_prf , spawn_pos.transform);
        mob_pool.Add(z);
        z.id = mob_pool.Count;
        z.gameObject.name = z.id.ToString();

        return mob_pool[mob_pool.Count - 1];
    }

    public void refund_pool(zombie z)
    {
        z.gameObject.SetActive(false);
        z.transform.parent = spawn_pos.transform;
        z.transform.localPosition = Vector3.zero;
        z.reset_zombie();
        z.HP_bar.fillAmount = 1f;
        z.transform.rotation = Quaternion.Euler(Vector3.zero);
        z.GetComponent<CapsuleCollider2D>().enabled = true;
    }
}
