using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Cube : MonoBehaviour
{
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public CubeConfig.CubeData GetCubeData()
    {
        return new CubeConfig.CubeData
        {
            Identifier = name,
            Image = GetComponentInChildren<Image>().sprite
        };
    }

    public void AnimateAddition()
    {
        transform.localScale = Vector3.zero;

        float randomAngle = Random.Range(-15f, 15f);

        transform.DOScale(Vector3.one, 0.5f)
            .SetEase(Ease.OutBack);

        transform.DORotate(new Vector3(0, 0, randomAngle), 0.25f)
            .SetEase(Ease.InOutSine)
            .SetLoops(2, LoopType.Yoyo);
    }

    public void DestroyCube()
    {
        transform.DOScale(Vector3.zero, 0.5f)
       .SetEase(Ease.InBack)
       .OnComplete(() =>
       {
           Debug.Log($"Объект {gameObject.name} уничтожен");
           Destroy(gameObject);
       });
    }

    public void ResetScale() => transform.localScale = Vector3.zero;
    public void MoveTo(Vector3 targetPosition, float duration)
    {
        transform.DOMove(targetPosition, duration).SetEase(Ease.InOutQuad);
    }

    public void AnimateCollapse(float cubeWidth, System.Action onComplete)
    {
        float randomRotation = Random.Range(-15f, 15f);
        float randomHorizontalShift = Random.Range(-cubeWidth * 0.2f, cubeWidth * 0.2f);

        Vector3 initialPosition = new Vector3(
            transform.position.x + randomHorizontalShift,
            transform.position.y - cubeWidth * 1.2f,
            transform.position.z
        );

        Vector3 finalPosition = new Vector3(
            transform.position.x,
            transform.position.y - cubeWidth,
            transform.position.z
        );

        transform.DOMove(initialPosition, 0.3f)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                transform.DORotate(new Vector3(0, 0, randomRotation), 0.3f)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        onComplete?.Invoke();
                    });
            });
    }
    public void AnimateToTrashCan(RectTransform trashCan, float duration)
    {
        transform.SetParent(trashCan);

        SetPosition(trashCan.position);

        Vector3 targetPosition = trashCan.position;

        float holeHeight = trashCan.rect.height;
        targetPosition.y -= holeHeight/2;

        float randomRotationZ = Random.Range(-360f, 360f); 


        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.InOutQuad)
            .OnUpdate(() =>
            {
                transform.DORotate(new Vector3(0, 0, randomRotationZ), 0.5f).SetEase(Ease.OutQuad);
            })
            .OnComplete(() =>
            {
                DestroyCube();
            });
    }
}
