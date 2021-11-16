using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Settings.Bindable
{
    /// <inheritdoc/>
    /// <summary>
    /// Bindable Int32 value. Contains extra stuff such as Max/Min values.
    /// </summary>
    public class BindableInt : Bindable<int>
    {
        /// <summary>
        /// The mininimum value that it will be clamped to.
        /// </summary>
        public int MinValue { get; }

        /// <summary>
        /// The maximum value that it will be clamped to
        /// </summary>
        public int MaxValue { get; }

        /// <summary>
        /// The value of this BindedInt
        /// </summary>
        public new int Value
        {
            get => _value;
            set
            {
                var previousVal = _value;
                _value = Mathf.Clamp(value, MinValue, MaxValue);

                if (_value != previousVal)
                    ValueChanged?.Invoke(this, new BindableValueChangedEventArgs<int>(_value, previousVal));
            }
        }

        int _value;

        /// <inheritdoc/>
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultVal"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="action"></param>
        public BindableInt(int defaultVal, int min, int max, EventHandler<BindableValueChangedEventArgs<int>> action = null)
            : base(defaultVal, action)
        {
            MinValue = min;
            MaxValue = max;
            Value = defaultVal;
        }

        /// <inheritdoc/>
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultVal"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="action"></param>
        public BindableInt(string name, int defaultVal, int min, int max, EventHandler<BindableValueChangedEventArgs<int>> action = null)
            : base(name, defaultVal, action)
        {
            MinValue = min;
            MaxValue = max;
            Value = defaultVal;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Prints a stringified value of the Bindable.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value.ToString();
    }
}
