using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.GameInput
{
    public class KeyController : MonoBehaviour
    {
        public bool IsKeyDown => _keyDown;
        public float HoldingSince => _holdingSince;

        public KeyCode? Key;

        [SerializeField] protected bool _keyDown;
        [SerializeField] private float _holdingSince;

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            if (!Key.HasValue)
                return;

            if (Input.GetKey(Key.Value))
            {
                if (_keyDown)
                {
                    _holdingSince += Time.deltaTime;
                    OnKeyHold(_holdingSince);
                }
                else
                {
                    _keyDown = true;
                    OnKeyDown();
                }
            }
            else
            {
                if (_keyDown)
                {
                    _keyDown = false;
                    _holdingSince = 0f;
                    OnKeyUp();
                }
            }
        }

        public virtual void OnKeyDown()
        {

        }

        public virtual void OnKeyUp()
        {

        }

        public virtual void OnKeyHold(float holdingSince)
        {

        }
    }
}
