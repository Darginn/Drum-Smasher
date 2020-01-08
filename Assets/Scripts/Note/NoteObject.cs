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
        public bool AutoPlay;

        public bool DefaultNote;

        public event EventHandler OnNoteMiss;
        public event EventHandler<bool> OnNoteHit;

        private SpriteRenderer _spriteThis;
        private SpriteRenderer _spriteOverlay;

        // Start is called before the first frame update
        void Start()
        {
            _spriteThis = GetComponent<SpriteRenderer>();
            _spriteOverlay = transform.Find("taiko-noteoverlay").GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (ReachedEnd || DefaultNote)
                return;
            
            if (AutoPlay && transform.position.x < 0.8f && transform.position.x > -0.8f)
            {
                OnHit();
                return;
            }

            if (CanBeHit)
            {
                bool key1 = Input.GetKeyDown(KeyToPress);
                bool key2 = Input.GetKeyDown(KeyToPress2);

                if (BigNote)
                {
                    if (key1 && key2)
                    {
                        OnHit();
                        return;
                    }
                    else if (key1 || key2)
                    {
                        OnHit(false);
                        return;
                    }
                }
                else if (key1 || key2)
                {
                    OnHit();
                    return;
                }
            }


            if (transform.position.x < EndLine.transform.position.x)
            {
                ReachedEnd = true;
                Destroy(gameObject);
                return;
            }
            
            transform.position -= new Vector3(NoteSpeed * (float)DateTime.Now.Subtract(StartTime).TotalSeconds * 3f, 0f);
        }

        private void OnHit(bool fullyCorrect = true)
        {
            bool goodHit = fullyCorrect;

            if (!BigNote && (transform.position.x >= 1.1f || transform.position.x <= -1.1f))
                goodHit = false;

            Hit = true;
            CanBeHit = false;

            _spriteThis.enabled = false;
            _spriteOverlay.enabled = false;

            ReachedEnd = true;

            OnNoteHit?.Invoke(this, goodHit);
            Destroy(gameObject);
        }

        private void OnMiss()
        {
            OnNoteMiss?.Invoke(this, null);
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
                    OnMiss();

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
