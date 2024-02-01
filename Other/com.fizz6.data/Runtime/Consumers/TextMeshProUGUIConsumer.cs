using System;
using Fizz6.Autofill;
using TMPro;
using UnityEngine;

namespace Fizz6.Data
{
    [Serializable]
    public class TextMeshProUGUIConsumer : Consumer<string>
    {
        [SerializeField, Autofill]
        private TextMeshProUGUI textMeshProUGUI;

        protected override void Invoke(string in0) =>
            textMeshProUGUI.text = in0;
    }
}
