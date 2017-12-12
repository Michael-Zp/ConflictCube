using System.Collections.Generic;

namespace ConflictCube.ComponentBased.Components
{
    public class GameObject : GameCallbacks
    {
        private Transform _Transform;
        public Transform Transform {
            get {
                return _Transform;
            }
            private set {
                _Transform = value;
                _Transform.Owner = this;
            }
        }
        public List<GameObject> Children = new List<GameObject>();
        public List<Component> Components = new List<Component>();
        public GameObject Parent;
        public string Name;
        public GameObjectType Type;

        public GameObject(string name, Transform transform) : this(name, transform, null)
        {}

        public GameObject(string name, Transform transform, GameObject parent) : this(name, transform, parent, GameObjectType.Default)
        {}

        public GameObject(string name, Transform transform, GameObject parent, GameObjectType type)
        {
            Name = name;
            Transform = transform;
            Transform.SetOwner(this);
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
            if(component != null)
            {
                Components.Add(component);
                component.SetOwner(this);
            }
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
            foreach (Component comp in Components)
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
