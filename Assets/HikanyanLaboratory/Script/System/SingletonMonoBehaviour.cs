using UnityEngine;

namespace HikanyanLaboratory.System
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        protected virtual bool UseDontDestroyOnLoad { get; } = false;
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = (T)FindObjectOfType(typeof(T));

                if (_instance == null)
                {
                    SetupInstance();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            // 重複回避のためのチェック
            RemoveDuplicates();
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        // シングルトン初期化
        private static void SetupInstance()
        {
            GameObject gameObj = new GameObject();
            gameObj.name = typeof(T).Name;

            _instance = gameObj.AddComponent<T>();
            if ((_instance as SingletonMonoBehaviour<T>)!.UseDontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObj);
            }
        }

        private void RemoveDuplicates()
        {
            if (_instance == null)
            {
                _instance = this as T;
                if (UseDontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}