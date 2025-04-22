using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class tower : MonoBehaviour
{
    public int idx;

    internal int default_hp = 100;
    public int HP;

    public Slider HP_bar;

    public SpriteRenderer tower_image;


    public virtual void damaged(int d)
    {
        Debug.Log("hit_z");
        HP -= d;
        HP_bar.gameObject.SetActive(true);
        if(HP <= 0)
        {
            HP = 0;
            tower_image.color = Color.white;
            if(IE_hit_effect != null)
            {
                StopCoroutine(IE_hit_effect);
                IE_hit_effect = null;
            }
            GM.ins.tower_dead(this);         
            return;
        }
        else
        {
            post_ie_hit_effect();
        }
        HP_bar.value = (float)HP / (float)default_hp;

    }

    private void Start()
    {
        HP = default_hp;
    }

    private IEnumerator IE_hit_effect;

    private void post_ie_hit_effect()
    {
        if(IE_hit_effect != null)
        {
            StopCoroutine(IE_hit_effect);
            IE_hit_effect = null;
        }
        IE_hit_effect = hit_effect();
        StartCoroutine(IE_hit_effect);
    }

    private IEnumerator hit_effect()
    {
        float cur_time = 0f;

        Color w = Color.white;
        Color o = Color.yellow;

        while(cur_time < 0.5f)
        {
            tower_image.color = tower_image.color == w ? Color.yellow : Color.white;
            cur_time += Time.deltaTime;

            yield return null;
        }
        tower_image.color = Color.white;
        yield break;
    }
    
}
