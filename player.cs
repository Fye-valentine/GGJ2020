using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int ANumber;
    public int DNumber;
    public int SpaceNumber;
    public AudioSource jump, shoot;
    public LayerMask ground;
    public float JumpForce;
    public float speed;
    public float bulletspeed;
    public Rigidbody2D rb;
    public Rigidbody2D A;
    public Rigidbody2D D;
    public Collider2D foot;
    public GameObject[] No;
    private float Delay = 1.0f;
    public Text Anum;
    public Text Dnum;
    public Text SpaceNum;
    public Animator anim;
    public bool isHurt;
    void Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 1: DNumber = 1; break;
            case 2: DNumber = 3; break;
            case 3: DNumber = 0; ANumber = 1; SpaceNumber = 1; break;
            case 4: DNumber = 1; ANumber = 1; break;
            case 5: DNumber = 10000; ANumber = 10000; SpaceNumber = 10000; break;
        }
    }
    void Update()
    {
        SwitchAnim();
        MoveAndJump();
    }

    void MoveAndJump()
    {
        Anum.text = ANumber.ToString();
        Dnum.text = DNumber.ToString();
        SpaceNum.text = SpaceNumber.ToString();
        float Player_move = Input.GetAxis("Horizontal");
        float Player_facedercition = Input.GetAxisRaw("Horizontal");

        if (Player_move > 0 && !Input.GetButton("Fire3"))//移动
        {
            if (DNumber >= 1)
            {
                rb.velocity = new Vector2(Player_move * speed , rb.velocity.y);
                anim.SetFloat("running", Mathf.Abs(Player_move));
            }
            else
            {
                No[1].SetActive(true);
                Invoke("not1", 1.0f);
            }
        }

        if (Player_move < 0 && !Input.GetButton("Fire3"))//移动
        {
            if (ANumber >= 1)
            {
                rb.velocity = new Vector2(Player_move * speed , rb.velocity.y);
                anim.SetFloat("running", Mathf.Abs(Player_move));
            }
            else
            {
                No[0].SetActive(true);
                while (Delay > 0)
                {
                    Delay -= Time.deltaTime;
                }
                Invoke("not0", 1.0f);

            }
        }
        //面向
        if (Player_facedercition != 0)
        {
            transform.localScale = new Vector3(-1f * Player_facedercition, 1, 1);
        }
        //跳跃
        if (Input.GetButtonDown("Jump") && foot.IsTouchingLayers(ground))
        {

            if (SpaceNumber >= 1)
            {
                rb.velocity = new Vector2(rb.velocity.x, JumpForce );
                anim.SetBool("jumping", true);
                jump.Play();
            }
            else
            {
                No[2].SetActive(true);
                Invoke("not2", 1.0f);
            }

        }
        if (Input.GetButton("Fire3"))
        {
            if (Input.GetButtonDown("Horizontal") && Player_move > 0 && DNumber >= 1)
            {
                DNumber--;
                Rigidbody2D bulletInstance = Instantiate(D, new Vector3(transform.position.x + 1.3f, transform.position.y), transform.rotation);
                bulletInstance.velocity = new Vector2(bulletspeed , 0);
                shoot.Play();
            }
            if (Input.GetButtonDown("Horizontal") && Player_move < 0 && ANumber >= 1)
            {
                ANumber--;
                Rigidbody2D bulletInstance = Instantiate(A, new Vector3(transform.position.x - 1.3f, transform.position.y), transform.rotation);
                bulletInstance.velocity = new Vector2(-bulletspeed , 0);
                shoot.Play();
            }
        }

        if (Input.GetButtonDown("reset"))
        {
            Reload();
        }
    }
    void SwitchAnim()
    {
        anim.SetBool("idling", false);
        
        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0)
            {
                anim.SetBool("falling", true);
                anim.SetBool("jumping", false);
            }
        }

        if (foot.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
            anim.SetBool("idling", true);
        }

        if (DNumber==0&&ANumber==0&&SpaceNumber==0)
        {
            anim.SetBool("dead", true);
            Invoke(nameof(Reload), 2.0f);
        }
    }
    void not0()
    {
        No[0].SetActive(false);
    }

    void not1()
    {
        No[1].SetActive(false);
    }

    void not2()
    {
        No[2].SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "A")
        {
            ANumber++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "D")
        {
            DNumber++;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "Space")
        {
            SpaceNumber++;
            Destroy(collision.gameObject);
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "flag")
        {
            Destroy(collision.gameObject);
            anim.SetBool("win", true);
            Invoke(nameof(LoadNextScence), 1.0f);
        }
        if (collision.gameObject.tag == "spike")
        {
            anim.SetBool("dead", true);
            Invoke(nameof(Reload), 2.0f);
        }
    }
    void LoadNextScence()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    void Reload()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
