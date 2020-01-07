using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DrumSmasher.GameInput
{
    public class ButtonController : MonoBehaviour
    {
        public Sprite DefaultImage;
        public Sprite PressedImage;
        public KeyCode KeyToPress;

        private SpriteRenderer _spriteRenderer;

        // Start is called before the first frame update
        void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyToPress))
            {
                _spriteRenderer.sprite = PressedImage;
            }

            if (Input.GetKeyUp(KeyToPress))
            {
                _spriteRenderer.sprite = DefaultImage;
            }
        }
    }
}