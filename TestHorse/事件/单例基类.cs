public class 单例基类<T> where T : new()
{
    private static T instance;
    // 多线程安全机制
    private static readonly object locker = new object();
    public static T 实例
    {
        get
        {
            if (instance == null)
            {
                //lock写第一个if里是因为只有该类的实例还没创建时，才需要加锁，这样可以节省性能
                lock (locker)
                {
                    if (instance == null)
                        instance = new T();
                }
            }
            return instance;
        }
    }
}
