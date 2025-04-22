using System.Collections.Generic;
using UnityEngine;

public class bullet_pool_manager : MonoBehaviour
{
    public static bullet_pool_manager ins;
    public bullet bullet_prf;
    private List<bullet> bullet_pool = new List<bullet>();


    private void Awake()
    {
        if(ins == null)
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
        set_bullet_pool();
    }

    private void set_bullet_pool()
    {
        for(int i = 0 ; i < 100 ; i++)
        {
            bullet_pool.Add(Instantiate(bullet_prf));
        }
    }

    public bullet get_bullet_pool()
    {
        for(int i = 0 ; i < bullet_pool.Count ; i++)
        {
            if(bullet_pool[i].gameObject.activeSelf == false)
            {
                return bullet_pool[i];
            }
        }

        bullet_pool.Add(Instantiate(bullet_prf));
        return bullet_pool[bullet_pool.Count - 1];
    }
}
