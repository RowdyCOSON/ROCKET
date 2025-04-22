using UnityEngine;

public class loop_bg : MonoBehaviour
{
    public Transform target_bg;

    public float speed = 1.0f;
    public float move_distance;

    void Update()
    {
        if(GM.ins.START == false || GM.ins.is_game_over() == true)
        {
            return;
        }

        transform.position += Vector3.left * speed * Time.deltaTime;

        if(transform.position.x <= -move_distance)
        {
            transform.position = target_bg.position - (Vector3.left * move_distance);
        }
    }
}
