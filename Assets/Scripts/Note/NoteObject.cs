using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DrumSmasher;
using System;

namespace DrumSmasher.Note
{
    public class NoteObject : MonoBehaviour
    {
        public bool CanBeHit;
        public bool BigNote;
        public bool Hit;
        public bool ReachedEnd;

        public KeyCode KeyToPress;
        public KeyCode KeyToPress2;

        public GameObject GoodEffect;
        public GameObject MissEffect;
        public GameObject EndLine;

        public Vector3 StartPos;
        public DateTime StartTime;

        public float NoteSpeed;

        public bool DefaultNote;
        public bool ShouldStart;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (ReachedEnd || ShouldStart || DefaultNote)
                return;

            if (transform.position.x < EndLine.transform.position.x)
            {
                ReachedEnd = true;
                Destroy(this);
                return;
            }

            Vector3 fixedLoc = StartPos - new Vector3(NoteSpeed * (float)DateTime.Now.Subtract(StartTime).TotalSeconds * 3f, 0f);
            transform.position = fixedLoc;

            if (CanBeHit)
            {
                bool key1 = Input.GetKeyDown(KeyToPress);
                bool key2 = Input.GetKeyDown(KeyToPress2);

                if (BigNote)
                {
                    if (key1 && key2)
                        OnHit();
                    else if (key1 || key2)
                        OnHit(false);
                }
                else if (key1 || key2)
                    OnHit();
            }
        }

        private void OnHit(bool fullyCorrect = true)
        {
            Logger.Log("Hit");
            CanBeHit = false;
            SpriteRenderer mr = GetComponent<SpriteRenderer>();
            mr.enabled = false;

            Transform overlay = transform.Find("taiko-noteoverlay");
            mr = overlay.GetComponent<SpriteRenderer>();
            mr.enabled = false;

            ReachedEnd = true;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag.Equals("Activator"))
                CanBeHit = true;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag.Equals("Activator"))
            {
                if (!Hit)
                {
                    GameManager.Instance.NoteMissed();
                }
                else
                    GameManager.Instance.NoteHit();

                CanBeHit = false;
            }
        }
    }
}
//class Note : NoteObject
//{
//    public void Update()
//    {
//        if (Input.GetKeyDown(keyToPress))
//        {
//            if (canBeHit)
//            {
//                gameObject.SetActive(false);

//                //GameManager.instance.NoteHit();

//                /*if (Mathf.Abs(transform.position.x) > 1)
//                {
//                    Logger.Log("Good Hit");
//                    GameManager.instance.NormalHit();
//                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
//                }*/
//            }
//        }
//        else if (Input.GetKeyDown(keyToPress2))
//        {
//            if (canBeHit)
//            {
//                gameObject.SetActive(false);

//                //GameManager.instance.NoteHit();

//                /*if (Mathf.Abs(transform.position.x) > 1)
//                {
//                    Logger.Log("Good Hit");
//                    GameManager.instance.NormalHit();
//                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
//                }*/
//            }
//        }
//    }
//}

//class BigNote : NoteObject
//{
//    public void Update()
//    {
//        if (Input.GetKeyDown(keyToPress) & Input.GetKeyDown(keyToPress2))
//        {
//            if (canBeHit)
//            {
//                gameObject.SetActive(false);

//                GameManager.Instance.NoteHit();

//                /*if (Mathf.Abs(transform.position.x) > 1)
//                {
//                    Logger.Log("Good Hit");
//                    GameManager.instance.NormalHit();
//                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
//                } */
//            }
//        }
//        else if (Input.GetKeyDown(keyToPress))
//        {
//            if (canBeHit)
//            {
//                gameObject.SetActive(false);

//                GameManager.Instance.NoteHit();

//                /*if (Mathf.Abs(transform.position.x) > 1)
//                {
//                    Logger.Log("Good Hit");
//                    GameManager.instance.NormalHit();
//                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
//                }*/
//            }
//        }
//        else if (Input.GetKeyDown(keyToPress2))
//        {
//            if (canBeHit)
//            {
//                gameObject.SetActive(false);

//                GameManager.Instance.NoteHit();

//                /*if (Mathf.Abs(transform.position.x) > 0.3)
//                {
//                    Logger.Log("Good Hit");
//                    GameManager.instance.NormalHit();
//                    Instantiate(goodEffect, new Vector3(0, 2.468484f, 0), goodEffect.transform.rotation);
//                } */
//            }
//        }
//    }
//}
