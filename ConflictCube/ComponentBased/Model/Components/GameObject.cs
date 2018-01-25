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
        private GameObject _Parent;
        public GameObject Parent {
            get {
                return _Parent;
            }
            set {
                if (_Parent != null)
                {
                    _Parent.Children.Remove(this);
                }
                else if(RootGameObjectNode.Contains(this))
                {
                    RootGameObjectNode.Remove(this);
                }

                if (value != null)
                {
                    value.Children.Add(this);
                }
                else
                {
                    RootGameObjectNode.Add(this);
                }
                _Parent = value;

            }
        }
        public string Name;
        public GameObjectType Type;
        public bool Enabled {
            private get {
                return _EnabledSelf;
            }
            set {
                _EnabledSelf = value;

                if(value)
                {
                    OnEnable();
                }
                else
                {
                    OnDisable();
                }
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
                while (currentGameObject != null)
                {
                    if (currentGameObject.EnabledSelf == false)
                    {
                        return false;
                    }
                    currentGameObject = currentGameObject.Parent;
                }
                return true;
            }
        }
        
        public GameObject(string name, Transform transform, GameObject parent, bool enabled = true) : this(name, transform, parent, GameObjectType.Default, enabled)
        { }

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


        public void AddComponent(Component component)
        {
            if (component != null)
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
                if (comp is T)
                {
                    return (T)comp;
                }
            }
            return null;
        }

        public List<T> GetComponents<T>() where T : Component
        {
            List<T> comps = new List<T>();
            foreach (Component comp in Components)
            {
                if (comp is T)
                {
                    comps.Add((T)comp);
                }
            }
            return comps;
        }

        public virtual GameObject Clone()
        {
            GameObject newGameObject = (GameObject)MemberwiseClone();
            newGameObject.Children = new List<GameObject>();
            newGameObject.Components = new List<Component>();

            foreach (GameObject child in Children)
            {
                newGameObject.Children.Add(child.Clone());
            }

            foreach (Component comp in Components)
            {
                newGameObject.AddComponent(comp.Clone());
            }

            return newGameObject;
        }

        public void UpdateAll()
        {
            CallAllOnUpdate();

            CallAllOnLateUpdate();
        }

        private void CallAllOnUpdate()
        {
            if (!EnabledInHierachy)
            {
                return;
            }

            OnUpdate();

            foreach (GameObject child in Children)
            {
                child.CallAllOnUpdate();
            }
        }

        private void CallAllOnLateUpdate()
        {
            if (!EnabledInHierachy)
            {
                return;
            }

            OnLateUpdate();

            foreach (GameObject child in Children)
            {
                child.CallAllOnLateUpdate();
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


        //Static


        private static List<GameObject> RootGameObjectNode = new List<GameObject>();

        /// <summary>
        /// Destroies a game object.
        /// Can only be destroied if the GameObject has a parent.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void Destroy(GameObject gameObject)
        {
            if (gameObject.Parent != null)
            {
                gameObject.OnDestroy();
                gameObject.Parent.Children.Remove(gameObject);
            }
            else
            {
                RootGameObjectNode.Remove(gameObject);
            }
        }


        /// <summary>
        /// Find child of type which needs GameObject as a base class.
        /// Search is done as a Breadth-first search.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static GameObject FindGameObjectByType<T>() where T : GameObject
        {
            foreach(GameObject rootGameObjects in RootGameObjectNode)
            {
                if (rootGameObjects is T)
                {
                    return rootGameObjects;
                }

                foreach (GameObject child in rootGameObjects.Children)
                {
                    if (child is T)
                    {
                        return child;
                    }
                }

                foreach (GameObject child in rootGameObjects.Children)
                {
                    GameObject gO = child.FindGameObjectByTypeInChildren<T>();
                    if (gO != null)
                    {
                        return gO;
                    }
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
        public static List<GameObject> FindGameObjectsByType<T>() where T : GameObject
        {
            List<GameObject> allGameObjects = new List<GameObject>();

            foreach(GameObject rootGameObjects in RootGameObjectNode)
            {
                if (rootGameObjects is T)
                {
                    allGameObjects.Add(rootGameObjects);
                }

                foreach (GameObject child in rootGameObjects.Children)
                {
                    if (child is T)
                    {
                        allGameObjects.Add(child);
                    }
                }

                foreach (GameObject child in rootGameObjects.Children)
                {
                    allGameObjects.AddRange(child.FindGameObjectsByTypeInChildren<T>());
                }
            }

            return allGameObjects;
        }
    }
}
