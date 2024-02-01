using System;
using Fizz6.Autofill;
using TMPro;
using UnityEngine;

namespace Fizz6.UI
{
    [Serializable]
    public abstract class TextControllerImplementation<TModel> : ControllerImplementation<TModel>
        where TModel : Model
    {
        [SerializeField, Autofill] 
        private TextMeshProUGUI textMeshProUGUI;
        
        public abstract string Text { get; }
        
        public override void OnModelBroadcast()
        {
            textMeshProUGUI.text = Text;
        }
    }
}