using System;

namespace HikanyanLaboratory
{
    public abstract class PureSingleton<T> : IDisposable where T : class, new()
    {
        private static T _instance = null;

        public static T Instance
        {
            get
            {
                CreateInstance();
                return _instance;
            }
        }

        //インスタンスを生成する
        public static void CreateInstance()
        {
            _instance ??= new T();
        }

        //Singletonを破棄する
        public virtual void Dispose()
        {
            _instance = null;
        }
    }
}