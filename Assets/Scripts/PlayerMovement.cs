using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float playerSpeed;
    public float rotationSpeed;
    Animator animator;
    AudioSource audioSource;
    public AudioClip shootClip;
    public AudioClip walkClip;
    public GameObject bulletPrefab;
    public Transform bulletPosition;
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement =new Vector3(Input.GetAxis("Horizontal"),0f, Input.GetAxis("Vertical")); // Taking inputs
       
        transform.Translate(movement.x*Time.deltaTime* playerSpeed, 0f, movement.z*Time.deltaTime * playerSpeed); // PLayer moving
        if (movement.x != 0 || movement.z != 0)
            animator.SetFloat("Blend", movement.magnitude);          // animation based on movement
        else
            animator.SetFloat("Blend", 0f);
        float rotateX = Input.GetAxis("Mouse X") * rotationSpeed;   // player rotation left and right
        transform.Rotate(0f, rotateX, 0f); 
        if(Input.GetKeyDown(KeyCode.Space))     // PLayer Attacking
        {
            animator.SetTrigger("IsAttack");
            GameObject temp=Instantiate(bulletPrefab, bulletPosition.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);

        }

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

}
