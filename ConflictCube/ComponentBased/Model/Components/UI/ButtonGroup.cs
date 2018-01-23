using ConflictCube.ComponentBased.Components;
using System;
using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Model.Components.UI
{
    public class ButtonGroup : GameObject
    {
        private List<ColoredBox> Buttons = new List<ColoredBox>();
        private List<TextField> Texts = new List<TextField>();
        private List<Action> OnClicks = new List<Action>();
        private int LastActiveButton = 0;
        private int ActiveButton = 0;

        private Material NotActiveMat;
        private Material ActiveMat;
        private bool MaterialsAreInitalized = false;

        private bool WasEnabledLastFrame = false;

        public ButtonGroup(string name, Transform transform) : base(name, transform)
        {
            if(!MaterialsAreInitalized)
            {
                NotActiveMat = new Material(System.Drawing.Color.Blue);
                ActiveMat = new Material(System.Drawing.Color.Red);
            }
        }

        public override void OnUpdate()
        {
            //If OnUpdate is called the object is enabled
            //This guard prevents buttons that enabled on this frame to be pressed (think double click on a just enabled button)
            if(!WasEnabledLastFrame)
            {
                WasEnabledLastFrame = true;
                return;
            }

            if (Input.OnButtonDown(InputKey.PlayerOneMoveDown, 0) || Input.OnButtonDown(OpenTK.Input.Key.Down))
            {
                ActiveButton++;
            }
            else if(Input.OnButtonDown(InputKey.PlayerOneMoveUp, 0) || Input.OnButtonDown(OpenTK.Input.Key.Up))
            {
                ActiveButton--;
            }

            if(ActiveButton < 0)
            {
                ActiveButton = Buttons.Count - 1;
            }
            else
            {
                ActiveButton = ActiveButton % Buttons.Count;
            }

            if(LastActiveButton != ActiveButton)
            {
                UpdateSelectedButton();
            }

            if(Input.OnButtonIsReleased(OpenTK.Input.Key.Enter))
            {
                OnClicks[ActiveButton].Invoke();
            }

            LastActiveButton = ActiveButton;
        }

        private void UpdateSelectedButton()
        {
            SetButtonState(Buttons[LastActiveButton], false);
            SetButtonState(Buttons[ActiveButton], true);
        }

        private void SetButtonState(ColoredBox button, bool state)
        {
            button.RemoveComponent<Material>();
            button.AddComponent(state ? ActiveMat : NotActiveMat);
        }

        public void AddButton(Transform transform, TextField textField, Action action)
        {
            ColoredBox buttonBox = new ColoredBox("btn" + textField.Text, transform, NotActiveMat);
            Buttons.Add(buttonBox);
            Texts.Add(textField);

            if(action == null)
            {
                action = new Action(() => { });
            }
            OnClicks.Add(action);

            AddChild(buttonBox);
            AddChild(textField);

            UpdateSelectedButton();
        }
    }
}
