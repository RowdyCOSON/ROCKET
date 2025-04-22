using UnityEngine;

public class bullet : MonoBehaviour
{
    private Vector3 fire_target_pos_normal;
    public float speed = 10f;
    public int damage = 50;

    public void Update()
    {
        transform.position += Time.deltaTime * speed * fire_target_pos_normal;
    }

    public void set_fire_info(Vector3 click_pos)
    {
        click_pos = new Vector3(click_pos.x , click_pos.y , 0);
        fire_target_pos_normal = (click_pos - transform.position).normalized;
    }

    private void reset_bullet()
    {
        this.gameObject.SetActive(false);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {       
        if(collision.collider.tag =="land")
        {
            reset_bullet();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "zombie")
        {
            if(collision.GetComponent<zombie>().is_alive() == false)
            {
                return;
            }
            collision.GetComponent<zombie>().damaged(damage);
            reset_bullet();
        }
    }
}
