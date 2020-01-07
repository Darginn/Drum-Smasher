using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrumSmasher
{
    public class NoteScroll : MonoBehaviour
    {
        public float NoteTempo;
        public bool GameStart;

        // Start is called before the first frame update
        void Start()
        {
            NoteTempo = NoteTempo / 60f;
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameStart)
            {
                /* if (Input.anyKeyDown)
                {
                    gameStart = true;
                }  */
            }
            else
            {
                transform.position -= new Vector3((NoteTempo * Time.deltaTime) * 3, 0f, 0f);
            }
        }
    }
}
