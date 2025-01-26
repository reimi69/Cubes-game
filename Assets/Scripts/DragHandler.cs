using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;    
    private CanvasGroup canvasGroup;      
    private Vector2 startPosition;       

    private Canvas canvas;                

    private readonly Subject<Vector2> onDropSubject = new Subject<Vector2>();
    private readonly Subject<GameObject> onBeginDragSubject = new Subject<GameObject>();
    private readonly Subject<Vector3> onDragSubject = new Subject<Vector3>();

    public IObservable<Vector2> OnDrop => onDropSubject;
    public IObservable<GameObject> BeginDrag => onBeginDragSubject;
    public IObservable<Vector3> Drag => onDragSubject;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.anchoredPosition; 
        canvasGroup.blocksRaycasts = false;
        onBeginDragSubject.OnNext(gameObject);
        gameObject.transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        Vector2 pointerDelta = eventData.delta / canvas.scaleFactor;
        rectTransform.anchoredPosition += pointerDelta;
        onDragSubject.OnNext(rectTransform.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; 
        Vector3 globalPosition = rectTransform.position;
        rectTransform.anchoredPosition = startPosition;
        onDropSubject.OnNext(globalPosition);
    }
    private void OnDestroy()
    {
        onDropSubject.Dispose();
        onBeginDragSubject.Dispose();
        onDragSubject.Dispose();
    }
}
