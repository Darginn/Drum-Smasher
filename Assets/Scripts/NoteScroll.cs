using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScroll : MonoBehaviour
{
    public float noteTempo;

    public bool gameStart;
    // Start is called before the first frame update
    void Start()
    {
        noteTempo = noteTempo / 60f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStart)
        {
            /* if (Input.anyKeyDown)
            {
                gameStart = true;
            }  */
        }
        else
        {
            transform.position -= new Vector3((noteTempo * Time.deltaTime) * 3, 0f, 0f);
        }
    }
}
