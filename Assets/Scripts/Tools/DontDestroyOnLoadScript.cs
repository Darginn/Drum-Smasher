using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tools
{
    public class DontDestroyOnLoadScript : MonoBehaviour
    {
        public static List<int> TrackedInstances { get; } = new List<int>();

        [SerializeField] private int _id;

        void Awake()
        {
            if (TrackedInstances.Contains(_id))
            {
                Destroy(gameObject);
                return;
            }

            TrackedInstances.Add(_id);
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(this);
        }
    }
}
