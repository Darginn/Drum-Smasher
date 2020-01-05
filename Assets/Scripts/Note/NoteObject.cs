using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBeHit;

    public KeyCode keyToPress;
    public KeyCode keyToPress2;

    public GameObject goodEffect, missEffect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBeHit = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            GameManager.instance.NoteMissed();
            Instantiate(missEffect, new Vector3(0, 2.468484f, 0), missEffect.transform.rotation);
            canBeHit = false;
        }
    }
}
class Note : NoteObject
{
    public void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            if (canBeHit)
            {
                gameObject.SetActive(false);

                //GameManager.instance.NoteHit();

                /*if (Mathf.Abs(transform.position.x) > 1)
                {
                    Debug.Log("Good Hit");
                    GameManager.instance.NormalHit();
                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
                }*/
            }
        }
        else if (Input.GetKeyDown(keyToPress2))
        {
            if (canBeHit)
            {
                gameObject.SetActive(false);

                //GameManager.instance.NoteHit();

                /*if (Mathf.Abs(transform.position.x) > 1)
                {
                    Debug.Log("Good Hit");
                    GameManager.instance.NormalHit();
                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
                }*/
            }
        }
    }
}

class BigNote : NoteObject
{
    public void Update()
    {
        if (Input.GetKeyDown(keyToPress) & Input.GetKeyDown(keyToPress2))
        {
            if (canBeHit)
            {
                gameObject.SetActive(false);

                GameManager.instance.NoteHit();

                /*if (Mathf.Abs(transform.position.x) > 1)
                {
                    Debug.Log("Good Hit");
                    GameManager.instance.NormalHit();
                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
                } */
            }
        }
        else if (Input.GetKeyDown(keyToPress))
        {
            if (canBeHit)
            {
                gameObject.SetActive(false);

                GameManager.instance.NoteHit();

                /*if (Mathf.Abs(transform.position.x) > 1)
                {
                    Debug.Log("Good Hit");
                    GameManager.instance.NormalHit();
                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
                }*/
            }
        }
        else if (Input.GetKeyDown(keyToPress2))
        {
            if (canBeHit)
            {
                gameObject.SetActive(false);

                GameManager.instance.NoteHit();

                /*if (Mathf.Abs(transform.position.x) > 0.3)
                {
                    Debug.Log("Good Hit");
                    GameManager.instance.NormalHit();
                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
                } */
            }
        }
    }
}
