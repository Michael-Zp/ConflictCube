using Engine.Components;
using System.Collections;
using System.Collections.Generic;

namespace Engine.ModelView
{
    public class ViewModelElement : IViewModelElement
    {
        public Transform Transform {
            get {
                return GameObject.Transform;
            }
        }

        public Material Material {
            get {
                return GameObject.GetComponent<Material>();
            }
        }

        public IEnumerator<IViewModelElement> GetEnumerator()
        {
            int position = 0;
            foreach (var item in GameObject.Children)
            {
                position++;
                yield return new ViewModelElement(GameObject.Children[position]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            int position = 0;
            foreach (var item in GameObject.Children)
            {
                position++;
                yield return new ViewModelElement(GameObject.Children[position]);
            }
        }


        private GameObject GameObject;

        public ViewModelElement(GameObject obj)
        {
            GameObject = obj;
        }
    }
}
