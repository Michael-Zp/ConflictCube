using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Components
{
    public class GameObject : GameCallbacks
    {
        public Transform Transform { get; private set; }
        public List<GameObject> Children;
        public List<Component> Components;
        public GameObject Parent;
        public string Name;
        public GameObjectType Type;

        public GameObject(string name, Transform transform)
        {
            Name = name;
            Parent = null;

            if(transform != null)
            {
                Transform = transform;
            }
            else
            {
                Transform = new Transform();
            }

            OnStart();
        }

        public GameObject(string name, Transform transform, GameObject parent)
        {
            Name = name;
            Parent = parent;

            OnStart();
        }

        public GameObject(string name, Transform transform, GameObject parent, GameObjectType type)
        {
            Name = name;
            Transform = transform;
            Parent = parent;
            Type = type;

            OnStart();
        }

        public static void Destroy(GameObject gameObject)
        {
            gameObject.OnDestroy();
            gameObject = null;
        }

        public void AddComponent(Component component)
        {
            Components.Add(component);
        }

        /// <summary>
        /// Removes first component of type T in the GameObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RemoveComponent<T>()
        {
            foreach (Component comp in Components)
            {
                if (comp is T)
                {
                    Components.Remove(comp);
                    break;
                }
            }
        }

        public T GetComponent<T>() where T : Component
        {
            foreach(Component comp in Components)
            {
                if(comp is T)
                {
                    return (T)comp;
                }
            }
            return null;
        }

        public void AddChild(GameObject child)
        {
            Children.Add(child);
            child.Parent = this;
        }
        
        public virtual GameObject Clone()
        {
            GameObject newGameObject = (GameObject)MemberwiseClone();

            foreach(GameObject child in Children)
            {
                newGameObject.Children.Add(child.Clone());
            }

            foreach(Component comp in Components)
            {
                newGameObject.AddComponent(comp.Clone());
            }

            return newGameObject;
        }

        public void UpdateAll()
        {
            OnUpdate();

            foreach(GameObject child in Children)
            {
                child.OnUpdate();
            }
        }
    }
}
