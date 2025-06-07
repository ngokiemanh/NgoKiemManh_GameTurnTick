using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Transform buttonTransform;
    private Tween hoverTween;
    private Tween clickTween;
    private Tween pulseTween;

    private Vector3 defaultScale;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonTransform = transform;

        defaultScale = buttonTransform.localScale;

        button.onClick.AddListener(OnClickEffect);

        StartPulse();
    }

    // Click effect: scale nhỏ rồi trở lại
    private void OnClickEffect()
    {
        clickTween?.Kill();
        buttonTransform.DOKill();

        buttonTransform.localScale = defaultScale;

        clickTween = buttonTransform
            .DOScale(defaultScale * 0.9f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                buttonTransform.DOScale(defaultScale, 0.2f).SetEase(Ease.OutBack);
            });
    }

    // Hover vào: scale lớn hơn, tạm dừng pulse
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverTween?.Kill();
        pulseTween?.Pause(); // Tạm dừng pulse để hover override

        hoverTween = buttonTransform
            .DOScale(defaultScale * 1.1f, 0.2f)
            .SetEase(Ease.OutSine);
    }

    // Hover ra: scale trở lại mặc định (defaultScale), resume pulse
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTween?.Kill();
        hoverTween = buttonTransform
            .DOScale(defaultScale, 0.2f)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                pulseTween?.Play(); // Tiếp tục pulse sau khi hover xong
            });
    }

    private void StartPulse()
    {
        pulseTween?.Kill();
        pulseTween = buttonTransform
            .DOScale(defaultScale * 1.05f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        hoverTween?.Kill();
        clickTween?.Kill();
        pulseTween?.Kill();
    }
}
