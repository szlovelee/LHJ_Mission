using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    private Dictionary<string, DataController> controllers = new Dictionary<string, DataController>();

    private void Awake()
    {
        instance = this;
    }

    public T GetDataController<T>(string name) where T : DataController, new()
    {
        if (controllers.ContainsKey(name))
        {
            return controllers[name] as T;
        }
        else
        {
            T controller = new T();
            controllers[name] = controller as DataController;

            return controller;
        }
    }
}
