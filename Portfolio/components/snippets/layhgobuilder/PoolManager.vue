<template>
	<v-container class="line-numbers">
<pre><code class="language-js line-numbers" style="white-space: pre-wrap; background-color: transparent;">
private Dictionary< string, IObject> m_templates = new Dictionary< string, IObject>();
private Dictionary< string, Stack< IOBject>> m_pool = new Dictionary< string, Stack< IOBject>>();

public async Task< T> GetAddressableAsset< T>(AssetReference a_reference)
{
    AsyncOperationHandle< T> handle = Addressables.LoadAssetAsync< T>(a_reference);
    await handle.Task;

    if (handle.Status == AsyncOperationStatus.Succeeded)
    {
        return handle.Result;
    }
    return default(T);
}

/// Get the required object from the pool
public async Task< IOBject> GetAsync(ItemObjectSO a_item)
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
        obj = gameObj.GetComponent< IOBject>();
    }
    // if not in the pool and not as template, spawn a new object and add it as template
    else
    {
        GameObject template = await GameManager.Instance.Library.GetAddressableAsset< GameObject>(a_item.prefabReference);
        IObject temp = template.GetComponent< IOBject>();
        m_templates.Add(key, temp);

        GameObject gameObj = Instantiate(m_templates[key].gameObject, m_objectContainer.transform);
        obj = gameObj.GetComponent< IOBject>();
    }
    
    obj.Initialize(a_item);
    return obj;
}

/// Return the object back to the pool.
public void Add(IObject a_obj)
{
    if (a_obj == null)
        return;

    string type = a_obj.ItemSO.name.ToLower();
    // Create a new stack if the type has not returned at the pool before
    if (!m_pool.ContainsKey(type))
        m_pool.Add(type, new Stack< IOBject>());

    a_obj.gameObject.transform.parent = m_poolContainer.transform;
    m_pool[type].Push(a_obj);
}
</code></pre>
</v-container>
</template>

<script>
import Prism from 'prismjs'

export default {
    mounted() {
        Prism.highlightAll();
    }
}
</script>