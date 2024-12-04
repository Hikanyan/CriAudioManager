using System;

namespace HikanyanLaboratory.Audio.Audio_Manager
{
    public abstract class PureSingleton<T> : IDisposable where T : class, new()
    {
        private static T instance = null;

        public static T I => Instance;

        public static T Instance
        {
            get
            {
                createInstance();
                return instance;
            }
        }

        //インスタンスを生成する
        public static void createInstance()
        {
            if (instance == null)
            {
                instance = new T();
            }
        }

        //Singletonが存在するか
        public static bool isExists()
        {
            return instance != null;
        }

        //Singletonを破棄する
        public virtual void Dispose()
        {
            instance = null;
        }
    }
}