using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GM : MonoBehaviour
{
    public static GM ins;
    public bool START;

    [SerializeField]
    private int loop_mark = 0;

    public Transform[] start_pos = new Transform[3];


    [SerializeField]
    private float spwan_weight = 1;
    [SerializeField]
    private float cur_time = 0f;

    /// <summary>
    /// start에서 시작시 스폰위치에 따라 생성
    /// </summary>
    public List<Dictionary<int , LinkedList<zombie>>> line_list = new List<Dictionary<int , LinkedList<zombie>>>();


    private void Awake()
    {
        if(ins == null)
        {
            ins = this;
        }
        else
        {
            Debug.LogError("ins fail");
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        for(int i = 0; i < start_pos.Length; i++)
        {
            line_list.Add(new Dictionary<int , LinkedList<zombie>>());
        }

        for(int i = 0 ; i < towers.Count ; i++)
        {
            towers_abs_pos.Add(towers[i].transform.position);
        }
    }


    public void Update()
    {
        if(START == false)
        {
            return;
        }

        cur_time += Time.deltaTime;

        if(cur_time > spwan_weight)
        {
            z_spawn();
            cur_time = 0f;
        }
    }

    private void z_spawn()
    {
        int line = Random.Range(0 , 3);

        zombie z = mob_pool_manager.ins.get_pool();
        z.transform.parent = start_pos[line];
        z.transform.localPosition = Vector2.zero;
        z.act_zomebie(line);

        z.damage = Random.Range(5 , 10);

        z.gameObject.SetActive(true);
    }


  
    public void tower_impact_set_line(zombie z)
    {
        if(z.my_node.List != null)
        {
            z.my_node.List.Remove(z.my_node);
        }

        if(line_list[z.line].ContainsKey(z.floor))
        {
            line_list[z.line][z.floor].AddFirst(z.my_node);
        }
        else
        {
            line_list[z.line].Add(z.floor , new LinkedList<zombie>());
            line_list[z.line][z.floor].AddFirst(z.my_node);
        }
    }

    public LinkedList<zombie> get_line(int line, int floor)
    {
        if(line_list[line].ContainsKey(floor))
        {
            return line_list[line][floor];
        }
        else
        {
            return null;
        }
    }

    public bool is_up_me(zombie z)
    {
        if(line_list[z.line].ContainsKey(z.floor + 1))
        {
            return line_list[z.line][z.floor + 1].Count != 0;
        }
        else
        {
            return false;
        }
    }



    public List<Transform> towers = new List<Transform>();
    [SerializeField]
    private List<Vector3> towers_abs_pos = new List<Vector3>();
    public void tower_dead(tower t)
    {
        t.gameObject.SetActive(false);

        List<Vector3> alive_towers_start_pos = new List<Vector3>();
        List<Transform> alive_towers = new List<Transform>();

        for(int i = 0 ; i < towers.Count ; i++)
        {
            if(towers[i].gameObject.activeSelf == true)
            {
                alive_towers_start_pos.Add(towers[i].position);
                alive_towers.Add(towers[i]);
            }
        }
        post_ie_tower_down(alive_towers_start_pos , alive_towers);
    }

    private void post_ie_tower_down(List<Vector3> alive_towers_start_pos , List<Transform> alive_towers)
    {
        if(IE_tower_down != null)
        {
            StopCoroutine(IE_tower_down);
            IE_tower_down = null;          
        }
        IE_tower_down = towers_down(alive_towers_start_pos , alive_towers);
        StartCoroutine(IE_tower_down);
    }
    private IEnumerator IE_tower_down;

    private IEnumerator towers_down(List<Vector3> alive_towers_start_pos , List<Transform> alive_towers)
    {
        float cur_time = 0f;

        while(cur_time < 0.5f)
        {
            for(int i = 0 ; i < alive_towers_start_pos.Count ; i++)
            {
                alive_towers[i].transform.position = Vector3.Lerp(alive_towers_start_pos[i] , towers_abs_pos[i] , cur_time / 0.5f);
            }

            cur_time+= Time.deltaTime;

            yield return null;
        }

        for(int i = 0 ; i < alive_towers_start_pos.Count ; i++)
        {
            alive_towers[i].transform.position = towers_abs_pos[i];
        }
        yield break;
    }

    public TextMeshProUGUI dead_text;

    public void tower_dead(hero h)
    {
        game_over = true;
        dead_text.gameObject.SetActive(true);
    }

    private bool game_over = false;
    public bool is_game_over()
    {
        return game_over;
    }
 
}
