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
        public bool Enabled {
            private get {
                return _EnabledSelf;
            }
            set {
                _EnabledSelf = value;
            }
        }
        private bool _EnabledSelf;
        public bool EnabledSelf {
            get {
                return _EnabledSelf;
            }
        }
        public bool EnabledInHierachy {
            get {
                GameObject currentGameObject = this;
                while(currentGameObject != null)
                {
                    if(currentGameObject.EnabledSelf == false)
                    {
                        return false;
                    }
                    currentGameObject = currentGameObject.Parent;
                }
                return true;
            }
        }

        public GameObject(string name, Transform transform) : this(name, transform, null)
        {}

        public GameObject(string name, Transform transform, GameObject parent) : this(name, transform, parent, GameObjectType.Default)
        {}

        public GameObject(string name, Transform transform, GameObject parent, GameObjectType type, bool enabled = true)
        {
            Name = name;
            Transform = transform;
            Transform.SetOwner(this);
            Parent = parent;
            Type = type;
            Enabled = enabled;

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
                    comp.OnRemove();
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
            if(!EnabledInHierachy)
            {
                return;
            }

            OnUpdate();

            foreach(GameObject child in Children)
            {
                child.UpdateAll();
            }
        }


        /// <summary>
        /// Find child of type which needs GameObject as a base class.
        /// Search is done as a Breadth-first search.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public GameObject FindGameObjectByTypeInChildren<T>() where T : GameObject
        {
            if (this is T)
            {
                return this;
            }

            foreach (GameObject child in Children)
            {
                if (child is T)
                {
                    return child;
                }
            }

            foreach (GameObject child in Children)
            {
                GameObject gO = child.FindGameObjectByTypeInChildren<T>();
                if (gO != null)
                {
                    return gO;
                }
            }

            return null;
        }

        /// <summary>
        /// Find child of type which needs GameObject as a base class.
        /// Search is done as a Breadth-first search.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<GameObject> FindGameObjectsByTypeInChildren<T>() where T : GameObject
        {
            List<GameObject> allGameObjects = new List<GameObject>();

            if (this is T)
            {
                allGameObjects.Add(this);
            }

            foreach (GameObject child in Children)
            {
                if (child is T)
                {
                    allGameObjects.Add(child);
                }
            }

            foreach (GameObject child in Children)
            {
                allGameObjects.AddRange(child.FindGameObjectsByTypeInChildren<T>());
            }

            return allGameObjects;
        }


    }
}
