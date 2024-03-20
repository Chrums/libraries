using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Fizz6.Core.Editor
{
    public class SearchableDropdown<T> : AdvancedDropdown
    {
        public static Task<T> Show(Rect position, IEnumerable<T> values, string name = null)
        {
            var items = values
                .Select(value => new Item(value, value.ToString()));
            return Show(position, items);
        }
        
        public static Task<T> Show(Rect position, IEnumerable<Item> items, string name = null)
        {
            var taskCompletionSource = new TaskCompletionSource<T>();

            void OnItemSelected(T value) =>
                taskCompletionSource.TrySetResult(value);

            var state = new State();
            var searchableDropdown = new SearchableDropdown<T>(
                state,
                name ?? string.Empty,
                items.ToArray(),
                OnItemSelected
            );
            
            searchableDropdown.Show(position);

            return taskCompletionSource.Task;
        }
        
        public class State : AdvancedDropdownState
        {}
        
        public class Item
        {
            public T Value { get; }
            public string Path { get; }

            public Item(T value, string path)
            {
                Value = value;
                Path = path;
            }
        }
        
        private class DropdownItem : AdvancedDropdownItem
        {
            public Item Item { get; }

            public DropdownItem(string name, Item item) : base(name) =>
                Item = item;
        }

        private static readonly Vector2 DefaultMinimumSize = new(512.0f, 256.0f);

        public Vector2 MinimumSize
        {
            get => minimumSize;
            set => minimumSize = value;
        }

        private readonly string _name;
        private readonly IReadOnlyList<Item> _items;
        private event Action<T> ItemSelectedEvent;
        
        public SearchableDropdown(State state, string name, IReadOnlyList<Item> items, Action<T> onItemSelected) : base(state)
        {
            _name = name;
            _items = items;
            ItemSelectedEvent = onItemSelected;
            minimumSize = DefaultMinimumSize;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(_name);
            
            AdvancedDropdownItem GetOrInsertItem(AdvancedDropdownItem parent, Item item, string token)
            {
                var child = parent.children
                    .FirstOrDefault(advancedDropdownItem => advancedDropdownItem.name == token);
                
                // Add a new item if a child with the given name does not yet exist,
                // or if this is a leaf node with a valid implementation
                if (child != null && item == null) 
                    return child;
                
                child = new DropdownItem(token, item);
                parent.AddChild(child);

                return child;
            }
            
            foreach (var item in _items)
            {
                var tokens = item.Path.Split('/');
                var advancedDropdownItem = root;
                for (var index = 0; index < tokens.Length; index++)
                {
                    var token = tokens[index];
                    var implementation = index == tokens.Length - 1
                        ? item
                        : null;
                    advancedDropdownItem = GetOrInsertItem(advancedDropdownItem, implementation, token);
                }
            }

            return root;
        }
        
        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            var dropdownItem = (DropdownItem)item;
            ItemSelectedEvent?.Invoke(dropdownItem.Item.Value);
        }
    }
}