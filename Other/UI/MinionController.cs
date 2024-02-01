using System;
using Fizz6.UI;

namespace Fizz6.Roguelike.UI
{
    public class MinionController
    {
        [Serializable]
        public class MinionNameText : TextControllerImplementation<MinionModel>
        {
            public override string Text => Model.name;
        }
    }
}