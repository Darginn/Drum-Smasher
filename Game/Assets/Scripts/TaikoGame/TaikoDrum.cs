using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.TaikoGame
{
    public class TaikoDrum : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _renderer;
        [SerializeField] Sprite _triggerSprite;
        [SerializeField] Sprite _defaultSprite;

        [SerializeField] float _timeSinceLastHit;
        [SerializeField] float _autoplayHitDuration = 0.5f;
        [SerializeField] AudioSource _hitSound;
        [SerializeField] bool _wasHitByAuto;

        public void Trigger()
        {
            if (ActiveTaikoSettings.IsAutoplayActive)
                _wasHitByAuto = true;

            _timeSinceLastHit = 0f;
            _hitSound.Play();
            Highlight();
        }

        void Highlight()
        {
            _renderer.sprite = _triggerSprite;
        }

        void DeHighlight()
        {
            _renderer.sprite = _defaultSprite;
        }

        void Update()
        {
            if (ActiveTaikoSettings.IsAutoplayActive && _wasHitByAuto)
            {
                _timeSinceLastHit += Time.deltaTime;
                
                if (_timeSinceLastHit >= _autoplayHitDuration)
                {
                    _wasHitByAuto = false;
                    _timeSinceLastHit = 0;
                    DeHighlight();
                }
            }
        }
    }
}
