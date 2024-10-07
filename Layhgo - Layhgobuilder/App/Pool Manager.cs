/// <summary>
/// Pooling is an optimization to reuse objects when they are removed from the scene.
/// This is to limit the amount of spawning new objects.
/// </summary>

private Dictionary<string, IObject> m_templates = new Dictionary<string, IObject>();
private Dictionary<string, Stack<IObject>> m_pool = new Dictionary<string, Stack<IObject>>();

public async Task<T> GetAddressableAsset<T>(AssetReference a_reference)
{
    AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(a_reference);
    await handle.Task;

    if (handle.Status == AsyncOperationStatus.Succeeded)
    {
        return handle.Result;
    }
    return default(T);
}

/// <summary>
/// Get the required object from the pool
/// </summary>
/// <param name="a_item"></param>
/// <returns></returns>
public async Task<IObject> GetAsync(ItemObjectSO a_item)
{
    IObject obj = default;
    string key = a_item.name.ToLower();
    // First check if the object is available in the pool
    if (m_pool.ContainsKey(key))
    {
        obj = m_pool[key].Pop();
    }
    // if it is not in the pool, see if there is a template object available
    else if (m_templates.ContainsKey(key))
    {
        GameObject gameObj = Instantiate(m_templates[key].gameObject, m_objectContainer.transform);
        obj = gameObj.GetComponent<IObject>();
    }
    // if not in the pool and not as template, spawn a new object and add it as template
    else
    {
        GameObject template = await GameManager.Instance.Library.GetAddressableAsset<GameObject>(a_item.prefabReference);
        IObject temp = template.GetComponent<IObject>();
        m_templates.Add(key, temp);

        GameObject gameObj = Instantiate(m_templates[key].gameObject, m_objectContainer.transform);
        obj = gameObj.GetComponent<IObject>();
    }
    
    obj.Initialize(a_item);
    return obj;
}

/// <summary>
/// Return the object back to the pool.
/// </summary>
/// <param name="a_obj"></param>
public void Add(IObject a_obj)
{
    if (a_obj == null)
        return;

    string type = a_obj.ItemSO.name.ToLower();
    // Create a new stack if the type has not returned at the pool before
    if (!m_pool.ContainsKey(type))
        m_pool.Add(type, new Stack<IObject>());

    a_obj.gameObject.transform.parent = m_poolContainer.transform;
    m_pool[type].Push(a_obj);
}