using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigNoteObject : MonoBehaviour
{
    public bool canBePressed;

    public KeyCode keyToPress;
    public KeyCode keyToPress2;

    public GameObject goodEffect, bigGoodEffect, missEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToPress) & Input.GetKeyDown(keyToPress2))
        {
            if (canBePressed)
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
            if (canBePressed)
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
            if (canBePressed)
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            GameManager.instance.NoteMissed();
            Instantiate(missEffect, new Vector3(0, 2.468484f, 0), missEffect.transform.rotation);
            canBePressed = false;
        }
    }
}
