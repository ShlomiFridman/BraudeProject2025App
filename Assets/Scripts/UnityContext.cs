using System.Threading;
using UnityEngine;

public class UnityContext : MonoBehaviour
{
    
    public static UnityContext Instance { get;  private set; }
    

    public SynchronizationContext Context { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        
    }

    void Start()
    {
        this.Context = SynchronizationContext.Current;
    }
}
