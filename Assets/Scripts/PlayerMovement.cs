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
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement =new Vector3(Input.GetAxis("Horizontal"),0f, Input.GetAxis("Vertical"));
       
        transform.Translate(movement.x*Time.deltaTime* playerSpeed, 0f, movement.z*Time.deltaTime * playerSpeed);
        if (movement.x != 0 || movement.z != 0)
            animator.SetFloat("Blend", movement.magnitude);
        else
            animator.SetFloat("Blend", 0f);
        float rotateX = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0f, rotateX, 0f);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("IsAttack");
        }

    }
    public void ShotFire()
    {
        audioSource.clip = shootClip;
        audioSource.Play();
    }
    public void Walk()
    {
        audioSource.clip = walkClip;
        audioSource.Play();
    }

}
