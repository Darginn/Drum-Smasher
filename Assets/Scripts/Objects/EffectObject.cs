using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrumSmasher.Objects
{
    public class EffectObject : MonoBehaviour
    {
        public float Lifetime = 0.5f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Destroy(gameObject, Lifetime);
        }
    }
}