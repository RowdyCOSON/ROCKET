using System.Collections;
using UnityEngine;

public class hero : tower
{
    public Transform bullet_start_pos;

    public Transform gun_arm;

    private void Update()
    {
        arm_look();

        if(fire_cooltime < fire_limit_time)
        {
            fire_cooltime += Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(fire_cooltime >= fire_limit_time)
            {
                fire();
            }
        }
        else if(Input.GetKey(KeyCode.Mouse0))
        {
            if(fire_cooltime >= fire_limit_time)
            {
                fire();
            }
        }
    }

    public override void damaged(int d)
    {
        Debug.Log("hit_z");
        HP -= d;
        HP_bar.gameObject.SetActive(true);
        if(HP <= 0)
        {
            HP = 0;
            GM.ins.tower_dead(this);
            StartCoroutine(dead_motion());
        }
        HP_bar.value = (float)HP / (float)default_hp;
    }

    private float fire_cooltime = 0f;
    private float fire_limit_time = 1f;

    public void fire()
    {
        fire_cooltime = 0f;
        bullet b = bullet_pool_manager.ins.get_bullet_pool();

        b.transform.position = bullet_start_pos.position;

        b.set_fire_info(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        b.gameObject.SetActive(true);
    }

    private void arm_look()
    {
        Vector3 look = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        look.z = 0f;

        Vector3 direction = look - transform.position;
        float angle = Mathf.Atan2(direction.y , direction.x) * Mathf.Rad2Deg;

        gun_arm.transform.rotation = Quaternion.Euler(0f , 0f , angle);
    }

    private IEnumerator dead_motion()
    {
        float cur_time = 0f;
        float dur = 1.5f;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0f , 0f , 90f); // Z축 90도 기울기

        while(cur_time < dur)
        {
            transform.rotation = Quaternion.Lerp(startRotation , endRotation , cur_time / dur);
            cur_time += Time.deltaTime;
            yield return null;
        }

        // 마지막 각도 정확히 설정 (보정)
        transform.rotation = endRotation;

        yield break;
    }
}
