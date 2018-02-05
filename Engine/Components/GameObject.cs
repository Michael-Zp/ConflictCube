using Engine.Time;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Engine.Components
{
    public class GameObject : GameCallbacks
    {
        public bool CallOnStart = true;
        public bool CallOnDestroy = true;
        /// <summary>
        /// Will also include the OnUpdate() call in all the components of the game object.
        /// </summary>
        public bool CallOnUpdate = true;
        public bool CallOnLateUpdate = true;
        public bool CallOnEnable = true;
        public bool CallOnDisable = true;

        /// <summary>
        /// If the CallOnUpdate flag is set to false and this flag is set to true, then no OnUpdate
        /// will be called on the children of this GameObject.
        /// </summary>
        public bool IncludeChildrenInCallOnUpdateFlag = false;
        
        /// <summary>
        /// If the CallOnLateUpdate flag is set to false and this flag is set to true, then no OnLateUpdate
        /// will be called on the children of this GameObject.
        /// </summary>
        public bool IncludeChildrenInCallOnLateUpdateFlag = false;

        private bool DestroyAtEndOfFrame = false;

#pragma warning disable 0649

        [Import(typeof(ITime))]
        private ITime Time;
        
#pragma warning restore 0649

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
                else if (RootGameObjectNode.Contains(this))
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
        public string Type;
        public bool Enabled {
            private get {
                return _EnabledSelf;
            }
            set {
                if (DestroyAtEndOfFrame)
                {
                    return;
                }

                _EnabledSelf = value;

                if (value)
                {
                    if(CallOnEnable)
                        OnEnable();
                }
                else
                {
                    if(CallOnDisable)
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

        public static string DefaultGameObjectType = "def";

        public GameObject(string name, Transform transform, GameObject parent, bool enabled = true) : this(name, transform, parent, DefaultGameObjectType, enabled)
        { }

        public GameObject(string name, Transform transform, GameObject parent, string type, bool enabled = true)
        {
            GameEngine.Container.ComposeParts(this);
            
            Name = name;
            Transform = transform;
            Transform.SetOwner(this);
            Parent = parent;
            Type = type;
            Enabled = enabled;

            if(CallOnStart)
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
            if(CallOnUpdate || !IncludeChildrenInCallOnUpdateFlag)
                CallAllOnUpdate();

            if (CallOnLateUpdate || !IncludeChildrenInCallOnLateUpdateFlag)
                CallAllOnLateUpdate();
        }

        private void CallAllOnUpdate()
        {
            if (!EnabledInHierachy)
            {
                return;
            }

            if(CallOnUpdate)
            {
                OnUpdate();

                foreach(Component comp in Components)
                {
                    if(comp.Enabled)
                    {
                        comp.OnUpdate();
                    }
                }
            }

            for(int i = 0; i < Children.Count; i++)
            {
                Children[i].CallAllOnUpdate();
            }
        }

        private void CallAllOnLateUpdate()
        {
            if (!EnabledInHierachy)
            {
                return;
            }

            if(CallOnLateUpdate)
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

        private static GameObject _staticInstance = null;
        private static GameObject staticInstance {
            get {
                if(_staticInstance == null)
                {
                    _staticInstance = new GameObject("staticInstance", new Transform(), null);
                }
                return _staticInstance;
            }

            set {
                _staticInstance = value;
            }
        }

        //Static
        private static List<GameObject> RootGameObjectNode = new List<GameObject>();
        private static List<Tuple<float, GameObject>> GameObjectsToBeDestroied = new List<Tuple<float, GameObject>>();
        
        /// <summary>
        /// Destroies a game object.
        /// Can only be destroied if the GameObject has a parent.
        /// </summary>
        /// <param name="gameObject"></param>
        public static void Destroy(GameObject gameObject, float afterTime = 0)
        {
            if (gameObject.CallOnDestroy)
                gameObject.OnDestroy();
            gameObject.DestroyAtEndOfFrame = true;
            gameObject.Enabled = false;
            GameObjectsToBeDestroied.Add(Tuple.Create(staticInstance.Time.CurrentTime + afterTime, gameObject));
        }


        /// <summary>
        /// Find child of type which needs GameObject as a base class.
        /// Search is done as a Breadth-first search.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static GameObject FindGameObjectByType<T>() where T : GameObject
        {
            foreach (GameObject rootGameObjects in RootGameObjectNode)
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

            foreach (GameObject rootGameObjects in RootGameObjectNode)
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

        public static void DestroyGameObjects()
        {
            foreach(Tuple<float, GameObject> obj in GameObjectsToBeDestroied)
            {
                if(staticInstance.Time.CurrentTime >= obj.Item1)
                {
                    foreach(Component comp in obj.Item2.Components)
                    {
                        comp.OnRemove();
                    }

                    if (obj.Item2.Parent != null)
                    {
                        obj.Item2.Parent.Children.Remove(obj.Item2);
                    }
                    else
                    {
                        RootGameObjectNode.Remove(obj.Item2);
                    }
                }
            }
        }
    }
}
