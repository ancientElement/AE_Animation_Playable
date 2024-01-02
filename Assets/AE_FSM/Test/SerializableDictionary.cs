using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AE_Framework
{

    public class SerializableDictionary { }

    [Serializable]
    public class SerializableDictionary<TKey, TValue> :
        SerializableDictionary,
        ISerializationCallbackReceiver,
        IDictionary<TKey, TValue>
    {
        [SerializeField] private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();

        [Serializable]
        private struct SerializableKeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public SerializableKeyValuePair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        private Dictionary<TKey, int> KeyPositions => _keyPositions.Value;
        private Lazy<Dictionary<TKey, int>> _keyPositions;

        public SerializableDictionary()
        {
            _keyPositions = new Lazy<Dictionary<TKey, int>>(MakeKeyPositions);
        }

        private Dictionary<TKey, int> MakeKeyPositions()
        {
            var dictionary = new Dictionary<TKey, int>(list.Count);
            for (var i = 0; i < list.Count; i++)
            {
                dictionary[list[i].Key] = i;
            }
            return dictionary;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            _keyPositions = new Lazy<Dictionary<TKey, int>>(MakeKeyPositions);
        }

        #region IDictionary<TKey, TValue>

        public TValue this[TKey key]
        {
            get => list[KeyPositions[key]].Value;
            set
            {
                var pair = new SerializableKeyValuePair(key, value);
                if (KeyPositions.ContainsKey(key))
                {
                    list[KeyPositions[key]] = pair;
                }
                else
                {
                    KeyPositions[key] = list.Count;
                    list.Add(pair);
                }
            }
        }

        public ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();
        public ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();

        public void Add(TKey key, TValue value)
        {
            if (KeyPositions.ContainsKey(key))
                throw new ArgumentException("An element with the same key already exists in the dictionary.");
            else
            {
                KeyPositions[key] = list.Count;
                list.Add(new SerializableKeyValuePair(key, value));
            }
        }

        public bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);

        public bool Remove(TKey key)
        {
            if (KeyPositions.TryGetValue(key, out var index))
            {
                KeyPositions.Remove(key);

                list.RemoveAt(index);
                for (var i = index; i < list.Count; i++)
                    KeyPositions[list[i].Key] = i;

                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (KeyPositions.TryGetValue(key, out var index))
            {
                value = list[index].Value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        #endregion

        #region ICollection <KeyValuePair<TKey, TValue>>

        public int Count => list.Count;
        public bool IsReadOnly => false;

        public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);

        public void Clear() => list.Clear();
        public bool Contains(KeyValuePair<TKey, TValue> kvp) => KeyPositions.ContainsKey(kvp.Key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var numKeys = list.Count;
            if (array.Length - arrayIndex < numKeys)
                throw new ArgumentException("arrayIndex");
            for (var i = 0; i < numKeys; i++, arrayIndex++)
            {
                var entry = list[i];
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);

        #endregion

        #region IEnumerable <KeyValuePair<TKey, TValue>>

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return list.Select(ToKeyValuePair).GetEnumerator();

            static KeyValuePair<TKey, TValue> ToKeyValuePair(SerializableKeyValuePair skvp)
            {
                return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }

    [CustomPropertyDrawer(typeof(SerializableDictionary), true)]
    public class SerializableDictionaryDrawer : PropertyDrawer
    {
        private SerializedProperty listProperty;

        private SerializedProperty getListProperty(SerializedProperty property) =>
            listProperty ??= property.FindPropertyRelative("list");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, getListProperty(property), label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(getListProperty(property), true);
        }
    }
}