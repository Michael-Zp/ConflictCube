namespace Engine.Components
{
    public abstract class GameCallbacks
    {
        public virtual void OnStart()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public virtual void OnUpdate()
        {

        }
        
        /// <summary>
        /// Will be called after all updates are called
        /// </summary>
        public virtual void OnLateUpdate()
        {

        }

        public virtual void OnCollision(Collider other)
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }
    }
}
