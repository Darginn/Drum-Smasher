using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameInput
{
    public class TaikoDrumHotKey : KeyController
    {
        public bool IsAutoplayEnabled;
        public float TimeSinceLastHit => _timeSinceLastHit;

        public new KeyCode Key;

        [SerializeField] SpriteRenderer _renderer;
        [SerializeField] Sprite _triggerSprite;
        [SerializeField] Sprite _defaultSprite;

        [SerializeField] float _timeSinceLastHit;
        [SerializeField] float _autoplayHitDuration;
        [SerializeField] AudioSource _hitSound;

        protected override void Update()
        {
            if (!base.Key.HasValue || this.Key != base.Key.Value)
                base.Key = this.Key;

            if (!IsAutoplayEnabled)
                base.Update();
            else
            {
                if (IsKeyDown)
                {
                    _timeSinceLastHit += Time.deltaTime;

                    if (_timeSinceLastHit >= _autoplayHitDuration)
                        OnKeyUp();
                }
            }
        }

        public override void OnKeyUp()
        {
            _renderer.sprite = _defaultSprite;
            _timeSinceLastHit = 0f;

            if (IsAutoplayEnabled)
                _keyDown = false;
        }

        public override void OnKeyDown()
        {
            if (_hitSound != null && _hitSound.clip != null)
                _hitSound.Play();
            _renderer.sprite = _triggerSprite;
            _timeSinceLastHit = 0f;

            if (IsAutoplayEnabled)
                _keyDown = true;
        }
    }
}
