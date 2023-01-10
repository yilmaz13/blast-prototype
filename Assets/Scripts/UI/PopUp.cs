using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class PopUp : MonoBehaviour
{
    public UnityEvent onOpen;
    public UnityEvent onClose;

    protected virtual void OnEnable()
    {
        onOpen.Invoke();
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        onClose.Invoke();

    }

    protected virtual IEnumerator DestroyPopup()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

}