using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Assets.Scripts.GameInput
{
    public class TaikoDrumHotKey : KeyController
    {
        public bool IsAutoplayEnabled;
        public float TimeSinceLastHit => _timeSinceLastHit;

        public new KeyCode Key;

        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private Sprite _triggerSprite;
        [SerializeField] private Sprite _defaultSprite;

        [SerializeField] private float _timeSinceLastHit;
        [SerializeField] private float _autoplayHitDuration;
        [SerializeField] private AudioSource _hitSound;

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
