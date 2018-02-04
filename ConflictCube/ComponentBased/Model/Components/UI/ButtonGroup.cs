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
        private float LastVerticalAxisInput = 0;

        private Material NotActiveMat;
        private Material ActiveMat;
        private bool MaterialsAreInitalized = false;

        private bool WasEnabledLastFrame = false;

        public ButtonGroup(string name, Transform transform, GameObject parent) : base(name, transform, parent)
        {
            if(!MaterialsAreInitalized)
            {
                NotActiveMat = new Material(System.Drawing.Color.Blue);
                ActiveMat = new Material(System.Drawing.Color.Red);
            }
        }

        public override void OnEnable()
        {
            WasEnabledLastFrame = false;
        }


        public override void OnDisable()
        {
            WasEnabledLastFrame = false;
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

            float verticalAxisInput = Input.GetAxis(InputAxis.Player1Vertical, 0);

            if (Input.OnButtonDown(OpenTK.Input.Key.Down) || (LastVerticalAxisInput > -.25f && verticalAxisInput < -.25f))
            {
                ActiveButton++;
            }
            else if(Input.OnButtonDown(OpenTK.Input.Key.Up) || (LastVerticalAxisInput < .25f && verticalAxisInput > .25f))
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

            if(Input.OnButtonIsReleased(OpenTK.Input.Key.Enter) || Input.OnButtonIsReleased(InputKey.PlayerOneUse))
            {
                OnClicks[ActiveButton].Invoke();
            }

            LastActiveButton = ActiveButton;
            LastVerticalAxisInput = verticalAxisInput;
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

        public void AddButton(Transform transform, string text, Action action)
        {
            ColoredBox buttonBox = new ColoredBox("btn" + text, transform, NotActiveMat, this);
            TextField textField = new TextField("tf" + text, new Transform(0, 0, 0.2f * text.Length, 1), text, Font.Instance().NormalFont, buttonBox);
            Buttons.Add(buttonBox);
            Texts.Add(textField);
            

            if(action == null)
            {
                action = new Action(() => { });
            }
            OnClicks.Add(action);

            UpdateSelectedButton();
        }
    }
}
