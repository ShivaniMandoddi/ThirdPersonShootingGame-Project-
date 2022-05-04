using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyObjectPool;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float playerSpeed;
    public float rotationSpeed;
    Animator animator;
    AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip walkClip;
    public Transform bulletPosition;
    Player player = new Player();
    public Text healthText;
    public Text ammoText;
    public int health;
    public Slider healthSlider;
    public GameObject gameEnd;
    public bool isGameOver = false;
    public int score;
    public GameObject bulleteffect;
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player.health = player.maxhealth;
        health = player.health;
        player.ammo = player.maxAmmo;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver == false)
        {
            healthSlider.value = (float)player.health / player.maxhealth;
            healthText.text = "Health: " + player.health;
            ammoText.text = "Ammo: " + player.ammo +"\nScore: "+score;

            Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")); // Taking inputs

            transform.Translate(movement.x * Time.deltaTime * playerSpeed, 0f, movement.z * Time.deltaTime * playerSpeed); // PLayer moving
            if (movement.x != 0 || movement.z != 0)
                animator.SetFloat("Blend", movement.magnitude);          // animation based on movement
            else
                animator.SetFloat("Blend", 0f);

            float rotateX = Input.GetAxis("Mouse X") * rotationSpeed;   // player rotation left and right
            transform.Rotate(0f, rotateX, 0f);

            if (Input.GetKeyDown(KeyCode.Space) && player.ammo>=0)     // PLayer Attacking
            {
                player.ammo--;
                Instantiate(bulleteffect, bulletPosition.position,Quaternion.identity);
                animator.SetTrigger("IsAttack");
                //GameObject temp=Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);
                GameObject temp = SpawnManager.instance.GetFromPool("Bullet");     // Spawing bullet from object pool
                if (temp != null)
                {
                    temp.SetActive(true);
                    temp.transform.position = bulletPosition.position;
                    temp.GetComponent<Rigidbody>().AddForce(transform.forward * 1500f);
                }

            }
        }
        if(player.health==0)
        {
            isGameOver = true;
            animator.SetTrigger("IsDead");
            gameEnd.SetActive(true);
        }
        if(isGameOver)
        {
            SpawnManager.instance.SetAllFalse();
        }
        if(score==10)
        {
            isGameOver = true;
            animator.SetBool("Dance", true);
            gameEnd.SetActive(true);
        }
        

    }
    public void PlayerHealth(int value)         // player health decreases , when enemy attacks
    {

        player.health = Mathf.Clamp(player.health-value,0,player.maxhealth);
        health = player.health;
        
    }
    public void ShotFire()       //Animation Event function for Firing
    {
        audioSource.clip = shootClip;
        audioSource.Play();
    }
    public void Walk()    // Animation Event function for Walking
    {
        audioSource.clip = walkClip;
        audioSource.Play();
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
    public void PlayerDead()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Medical")           // WHen medical kit collected, health increases
        {
            player.health = Mathf.Clamp(player.health + 20, 0, player.maxhealth);
            health = player.health;
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Ammo") // When ammo collected, bullets get increased
        {
            player.ammo = Mathf.Clamp(player.ammo + 20, 0, player.maxAmmo);
           
            Destroy(other.gameObject);
        }
    }

}
