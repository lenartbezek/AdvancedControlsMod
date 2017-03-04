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
        ///     Block handler object.
        /// </summary>
        protected Block Block;

        /// <summary>
        ///     Blocks BlockBehaviour game object.
        /// </summary>
        protected BlockBehaviour Bb => Block.BlockBehaviour;

        internal PropertyWrapper(Block block)
        {
            Block = block;
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

        internal SliderWrapper(Block b) : base(b)
        {
        }

        public override float this[string key]
        {
            get { return Bb.Sliders.Find(m => m.Key == key).Value; }
            set { Bb.Sliders.Find(m => m.Key == key).Value = value; }
        }

        public override string Current => Bb.Sliders[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < Bb.Sliders.Count;
        }

        public override void Reset()
        {
            _enumeratorPosition = 0;
        }
    }

    public class ToggleWrapper : PropertyWrapper<bool>
    {
        private int _enumeratorPosition;

        internal ToggleWrapper(Block b) : base(b)
        {
        }

        public override bool this[string key]
        {
            get { return Bb.Toggles.Find(m => m.Key == key).IsActive; }
            set { Bb.Toggles.Find(m => m.Key == key).IsActive = value; }
        }

        public override string Current => Bb.Toggles[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < Bb.Sliders.Count;
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

        public KeyWrapper(Block b) : base(b)
        {
        }

        public override IList<KeyCode> this[string key]
        {
            get { return Bb.Keys.Find(m => m.Key == key).KeyCode; }
            set { KeyCodeFieldInfo.SetValue(Bb.Keys.Find(m => m.Key == key), value); }
        }

        public override string Current => Bb.Keys[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < Bb.Sliders.Count;
        }

        public override void Reset()
        {
            _enumeratorPosition = 0;
        }
    }

    public class ColorWrapper : PropertyWrapper<Color>
    {
        private int _enumeratorPosition;

        public ColorWrapper(Block b) : base(b)
        {
        }

        public override Color this[string key]
        {
            get { return ((MColourSlider)Bb.MapperTypes.Find(m => m is MColourSlider && m.Key == key)).Value; }
            set { ((MColourSlider) Bb.MapperTypes.Find(m => m is MColourSlider && m.Key == key)).Value = value; }
        }

        public override string Current => Bb.MapperTypes.FindAll(m => m is MColourSlider)[_enumeratorPosition].Key;

        public override void Dispose()
        {
        }

        public override bool MoveNext()
        {
            _enumeratorPosition++;
            return _enumeratorPosition < Bb.Sliders.Count;
        }

        public override void Reset()
        {
            _enumeratorPosition = 0;
        }
    }
}