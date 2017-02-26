using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Lench.AdvancedControls
{
    /// <summary>
    ///     PropertyWrapper wraps properties of a BlockBehaviour of type T to a object,
    ///     where they are exposed through the indexing operator behind a string key.
    /// </summary>
    /// <typeparam name="T">Type of the properties.</typeparam>
    public abstract class PropertyWrapper<T> : IEnumerator<string>, IEnumerable<string>
    {
        /// <summary>
        ///     BlockBehaviour object.
        /// </summary>
        protected BlockBehaviour B;

        internal PropertyWrapper(BlockBehaviour bb)
        {
            B = bb;
        }

        /// <summary>
        ///     Access a property of type T by key.
        /// </summary>
        /// <param name="key">Key of the property.</param>
        public abstract T this[string key] { get; set; }

#pragma warning disable 1591
        public abstract void Dispose();
        public abstract bool MoveNext();
        public abstract void Reset();
        object IEnumerator.Current => Current;

        public abstract string Current { get; }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<string> GetEnumerator() => this;
    }

    public class SliderWrapper : PropertyWrapper<float>
    {
        private int _enumeratorPosition;

        internal SliderWrapper(BlockBehaviour bb) : base(bb)
        {
        }

        public override float this[string key]
        {
            get { return B.Sliders.Find(m => m.Key == key).Value; }
            set { B.Sliders.Find(m => m.Key == key).Value = value; }
        }

        public override string Current => B.Sliders[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < B.Sliders.Count;
        }

        public override void Reset()
        {
            _enumeratorPosition = 0;
        }
    }

    public class ToggleWrapper : PropertyWrapper<bool>
    {
        private int _enumeratorPosition;

        internal ToggleWrapper(BlockBehaviour bb) : base(bb)
        {
        }

        public override bool this[string key]
        {
            get { return B.Toggles.Find(m => m.Key == key).IsActive; }
            set { B.Toggles.Find(m => m.Key == key).IsActive = value; }
        }

        public override string Current => B.Toggles[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < B.Sliders.Count;
        }

        public override void Reset()
        {
            _enumeratorPosition = 0;
        }
    }

    public class KeyWrapper : PropertyWrapper<IList<KeyCode>>
    {
        private static readonly FieldInfo KeyCodeFieldInfo = typeof(MKey).GetField("_keyCodes");
        private int _enumeratorPosition;

        public KeyWrapper(BlockBehaviour bb) : base(bb)
        {
        }

        public override IList<KeyCode> this[string key]
        {
            get { return B.Keys.Find(m => m.Key == key).KeyCode; }
            set { KeyCodeFieldInfo.SetValue(B.Keys.Find(m => m.Key == key), value); }
        }

        public override string Current => B.Keys[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < B.Sliders.Count;
        }

        public override void Reset()
        {
            _enumeratorPosition = 0;
        }
    }

    public class ColorWrapper : PropertyWrapper<Color>
    {
        private int _enumeratorPosition;

        public ColorWrapper(BlockBehaviour bb) : base(bb)
        {
        }

        public override Color this[string key]
        {
            get { return ((MColourSlider)B.MapperTypes.Find(m => m is MColourSlider && m.Key == key)).Value; }
            set { ((MColourSlider) B.MapperTypes.Find(m => m is MColourSlider && m.Key == key)).Value = value; }
        }

        public override string Current => B.MapperTypes.FindAll(m => m is MColourSlider)[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < B.Sliders.Count;
        }

        public override void Reset()
        {
            _enumeratorPosition = 0;
        }
    }
}