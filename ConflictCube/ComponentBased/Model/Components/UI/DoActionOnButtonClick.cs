using ConflictCube.ComponentBased.Components;
using System;
using OpenTK.Input;

namespace ConflictCube.ComponentBased.Model.Components.UI
{
    public enum KeyPressType
    {
        Down,
        Release
    }

    public class DoActionOnButtonClick : GameObject
    {
        private Key Key;
        private Action ActionOnKeyPress;
        private KeyPressType KeyPressType;

        public DoActionOnButtonClick(string name, Transform transform, Key key, Action actionOnKeyPress, KeyPressType keyPressType) : base(name, transform)
        {
            Key = key;
            ActionOnKeyPress = actionOnKeyPress;
            KeyPressType = keyPressType;
        }

        public override void OnUpdate()
        {
            bool pressed = false;
            switch(KeyPressType)
            {
                case KeyPressType.Down:
                    if (Input.OnButtonDown(Key))
                    {
                        pressed = true;
                    }
                    break;

                case KeyPressType.Release:
                    if (Input.OnButtonIsReleased(Key))
                    {
                        pressed = true;
                    }
                    break;
            }

            if(pressed)
            {
                ActionOnKeyPress.Invoke();
            }
        }
    }
}
