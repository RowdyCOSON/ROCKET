using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class zombie : MonoBehaviour
{
    public int line = 0;
    public int id = 0;
    public int floor = 0;
    public float floor_weight = 1f;

    public LinkedListNode<zombie> my_node;

    [Header("STATE")]
    [SerializeField]
    private alive_state life_state = alive_state.dead;
    [SerializeField]
    private act_state action_state = act_state.move;
    public float dead_limit_time = 2f;

    public bool is_back_move = false;

    public Image HP_bar;

    public int damage = 50;
    public enum act_state
    {
        stop = -1,
        move = 0,
        hit = 1,
        jump = 2,
        down = 3,
    }

    public enum alive_state
    {
        dead = -1,
        live = 1
    }


    [Space(10)]
    [Header("status")]
    public float move_speed = 1f;

    public float Move_spped
    {
        get
        {
            if(is_back_move == false)
            {
                return move_speed;
            }
            else
            {
                return move_speed * -2f;
            }
        }
    }


    private void Start()
    {    
        my_node = new LinkedListNode<zombie>(this);
    }

    private void Update()
    {
        if(is_alive() == false)
        {           
            return;
        }

        if(GM.ins.is_game_over() == true)
        {
            return;
        }

        check_hit_time();


        switch(action_state)
        {
            case act_state.stop:

                jumper_checker();
                check_front_move();
                break;

            case act_state.move:

                if(is_end_point() == false)
                {
                    transform.localPosition += Time.deltaTime * Move_spped * Vector3.left;
                }
                break;

            case act_state.hit:

                if(hit_timer >= hit_dealy)
                {
                    hit_timer = 0f;
                    hit_tower();
                }
                if(GM.ins.is_up_me(this) == false)
                {
                    if(floor > 0)
                    {
                        floor -= 1;
                        set_action_stat(act_state.down);
                    }
                }

                break;

            case act_state.jump:

                transform.localPosition += Time.deltaTime * Move_spped * Vector3.left + 4 * Vector3.up * move_speed * Time.deltaTime;
                if(transform.localPosition.y >= floor * floor_weight)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x , floor * floor_weight , transform.localPosition.z);

                    set_action_stat(act_state.move);
                }
                break;

            case act_state.down:

                transform.localPosition += Vector3.down * move_speed * Time.deltaTime * 4;
                if(transform.localPosition.y < floor * floor_weight)
                {
                    GM.ins.tower_impact_set_line(this);
                    transform.localPosition = new Vector3(transform.localPosition.x , floor * floor_weight , transform.localPosition.z);

                    LinkedList<zombie> list = GM.ins.get_line(line , floor);
                    if(list != null)
                    {
                        foreach(zombie z in list)
                        {
                            if(z.action_state == act_state.jump)
                            {
                                continue;
                            }
                            z.set_action_stat(act_state.move);
                            z.is_back_move = false;
                        }
                    }
                    set_action_stat(act_state.hit);
                }
                else
                {
                    LinkedList<zombie> list = GM.ins.get_line(line , floor);
                    if(list != null)
                    {
                        foreach(zombie z in list)
                        {
                            if(z.action_state == act_state.jump)
                            {
                                continue;
                            }
                            z.set_action_stat(act_state.move);
                            z.is_back_move = true;
                        }
                    }
                }

                break;
        }
    }

    float check_ray = 0.05f;
    public Transform ray_pos;

    private void check_front_move()
    {
        if(action_state == act_state.jump)
        {
            return;
        }
        Vector2 origin = ray_pos.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(origin , Vector2.left , check_ray);
        if(hit == null)
        {
            if(my_node.List != null)
            {
                my_node.List.Remove(my_node);
            }
            set_action_stat(act_state.move);
            is_back_move = false;
        }
        else
        {
            for(int i = 0 ; i < hit.Length ; i++)
            {
                if(hit[i].collider.GetComponent<zombie>() && hit[i].collider.GetComponent<zombie>().line == line)
                {
                    return;
                }
            }
        }
    }

    private bool is_end_point()
    {
        int layer = 1 << LayerMask.NameToLayer("endpoint");
        Vector2 origin = ray_pos.position;
        RaycastHit2D hit = Physics2D.Raycast(origin , Vector2.left , check_ray, layer);

        if(hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Vector2 origin = ray_pos.position;
        RaycastHit2D hit = Physics2D.Raycast(origin , Vector2.left , check_ray);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(origin , Vector2.left * check_ray);
    }

    public float jumper_timer = 0f;
    private float jumper_limit_time = 1f;
    private void jumper_checker()
    {
        if(my_node.List != null && my_node.List.Last == my_node && my_node.List.First != my_node)
        {
            if(jumper_timer < jumper_limit_time)
            {
                jumper_timer += Time.deltaTime;
            }
            else
            {
                set_jump(floor);
                jumper_timer = 0f;
            }
        }
    }

    private float hit_timer = 1f;
    private float hit_dealy = 1f;

    private void hit_tower()
    {
        check_hit();
    }
    private void check_hit()
    {
        if(action_state == act_state.jump || action_state == act_state.down)
        {
            return;
        }
        Vector2 origin = ray_pos.position;
        RaycastHit2D hit = Physics2D.Raycast(origin , Vector2.left , check_ray);
        if(hit.collider != null && hit.collider.GetComponent<tower>() != null)
        {
            hit.collider.GetComponent<tower>().damaged(damage);
        }
        else if(hit.collider == null)
        {
            LinkedList<zombie> list = GM.ins.get_line(line , floor);
            foreach(zombie z in list)
            {
                if(z.action_state == act_state.jump)
                {
                    continue;
                }
                z.set_action_stat(act_state.move);
                z.is_back_move = false;
            }
        }
    }

    private void check_hit_time()
    {
        if(hit_timer < hit_dealy)
        {
            hit_timer += Time.deltaTime;
        }
    }

    private int default_hp = 100;
    private int HP = 100;


    public void damaged(int d)
    {
        HP -= d;
        HP_bar.transform.parent.gameObject.SetActive(true);
        if(HP <= 0)
        {
            HP = 0;
            die();
        }
        HP_bar.fillAmount = (float)HP / (float)default_hp;

    }

    private void die()
    {
        this.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        set_life_state(alive_state.dead);
        move_chain();
        StartCoroutine(dead_motion());
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
        transform.rotation = endRotation;
        yield return new WaitForSeconds(0.5f);
        mob_pool_manager.ins.refund_pool(this);

        yield break;
    }

    public void move_chain()
    {
        if(my_node.List != null)
        {
            my_node.List.Remove(my_node);
        }
    }



    public void reset_zombie()
    {
        if(my_node.List != null)
        {
            my_node.List.Remove(my_node);
        }
        HP = default_hp;
        HP_bar.transform.parent.gameObject.SetActive(false);

    }

    public void act_zomebie(int _line)
    {
        line = _line;
        set_life_state(alive_state.live);
        set_action_stat(act_state.move);
    }


    private void set_action_stat(act_state a)
    {
        action_state = a;

        if(a != act_state.stop)
        {
            jumper_timer = 0f;
        }

        if(action_state == act_state.jump)
        {
            is_back_move = false;
        }
    }

    private void set_life_state(alive_state a)
    {
        life_state = a;
    }
    public bool is_alive()
    {
        return life_state == alive_state.live;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "zombie")
        {
            zombie z = collision.collider.GetComponent<zombie>();

            if(z.line != this.line || z.floor != this.floor)
            {
                return;
            }
            jumper_timer = 0f;
            z.jumper_timer = 0f;
            if(z.transform.position.x < this.gameObject.transform.position.x)
            {
                if(my_node.List != null && z.my_node.List == my_node.List)
                {
                    set_action_stat(act_state.stop);
                    foreach(zombie z_list in my_node.List)
                    {
                        if(z_list.my_node.List.First == z_list.my_node)
                        {
                            continue;
                        }
                        else
                        {
                            z_list.set_action_stat(act_state.stop);
                        }
                    }
                    return;
                }
                if(z.action_state != act_state.jump)
                {
                    set_jump(z.floor);
                }
            }
        }
        else if(collision.collider.tag == "Hero")
        {
            if(action_state == act_state.down)
            {
                return;
            }
            GM.ins.tower_impact_set_line(this);
            set_action_stat(act_state.hit);
        }
    }

    private void set_jump(int floor_ref)
    {
        if(my_node.List != null)
        {
            my_node.List.Remove(my_node);
        }
        floor = floor_ref + 1;
        set_action_stat(act_state.jump);
    }
}
