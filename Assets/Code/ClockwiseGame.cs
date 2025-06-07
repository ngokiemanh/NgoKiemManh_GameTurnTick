using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ClockwiseGame : MonoBehaviour
{
    public List<Transform> dots = new List<Transform>();
    public List<Transform> dotsEmpty = new List<Transform>();

    public Transform arm;
    public Transform pointA;
    public Transform pointB;

    public float rotationSpeed = 180f;
    public SpriteRenderer armSprite;
    public float afterimageDuration = 0.5f;
    public Color afterimageColor = new Color(1f, 1f, 1f, 0.5f);

    public Sprite defaultDotSprite;
    public Sprite highlightDotSprite;
    public Sprite bothDotSprite;

    public Sprite emptyDefaultSprite;
    public Sprite emptyActiveSprite;

    public AudioClip rotateSound;
    public AudioClip dotClickSound;

    private AudioSource rotateAudioSource;
    private bool isRotating = false;
    private Transform pivotPoint;
    private Transform rotatingPoint;
    private Vector3 targetDotPosition;
    private float rotationDirection = 0.1f;

    private Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, float> originalPixelSize = new Dictionary<Transform, float>();

    private bool dotsEmptyToggled = false;

    void Start()
    {
        foreach (Transform dot in dots)
        {
            originalScales[dot] = dot.localScale;
            originalPixelSize[dot] = dot.GetComponent<SpriteRenderer>().bounds.size.x;
        }

        foreach (Transform dotEmpty in dotsEmpty)
        {
            originalScales[dotEmpty] = dotEmpty.localScale;
            originalPixelSize[dotEmpty] = dotEmpty.GetComponent<SpriteRenderer>().bounds.size.x;
        }

        rotateAudioSource = gameObject.AddComponent<AudioSource>();
        rotateAudioSource.clip = rotateSound;
        rotateAudioSource.loop = true;
    }

    void Update()
    {
        UpdateDotSprites();

        if (!isRotating && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            Transform clickedDot = null;
            float tolerance = 0.3f;

            if (hit.collider != null && dots.Contains(hit.collider.transform))
                clickedDot = hit.collider.transform;
            else
            {
                if (!dotsEmptyToggled && hit.collider != null && dotsEmpty.Contains(hit.collider.transform))
                    clickedDot = hit.collider.transform;

                if (clickedDot == null)
                {
                    foreach (Transform dot in dots)
                        if (Vector2.Distance(dot.position, mousePos) < tolerance)
                            clickedDot = dot;

                    if (clickedDot == null)
                    {
                        foreach (Transform dotEmpty in dotsEmpty)
                            if (Vector2.Distance(dotEmpty.position, mousePos) < tolerance)
                                clickedDot = dotEmpty;
                    }
                }
            }

            if (clickedDot == null) return;

            // ✅ Chỉ cho phép nhấn nếu dot này đang có pointA hoặc pointB
            bool isAtPointA = Vector2.Distance(clickedDot.position, pointA.position) < 0.05f;
            bool isAtPointB = Vector2.Distance(clickedDot.position, pointB.position) < 0.05f;

            if (!isAtPointA && !isAtPointB)
                return;

            if (dots.Contains(clickedDot))
            {
                dotsEmptyToggled = !dotsEmptyToggled;
                UpdateDotsEmptySprites();
            }

            if (dotClickSound != null)
                AudioSource.PlayClipAtPoint(dotClickSound, Camera.main.transform.position);

            int clickedIndex = -1;
            bool clickedIsDotEmpty = false;

            if (dots.Contains(clickedDot))
                clickedIndex = dots.IndexOf(clickedDot);
            else if (dotsEmpty.Contains(clickedDot))
            {
                clickedIsDotEmpty = true;
                clickedIndex = dotsEmpty.IndexOf(clickedDot);
            }

            if (clickedIndex == -1) return;

            Transform nearestPoint = Vector2.Distance(clickedDot.position, pointA.position) < Vector2.Distance(clickedDot.position, pointB.position) ? pointA : pointB;
            Transform oppositePoint = (nearestPoint == pointA) ? pointB : pointA;

            if (!clickedIsDotEmpty)
            {
                int nextIndex = (clickedIndex + 1) % dots.Count;
                StartRotation(nearestPoint, oppositePoint, dots[nextIndex]);
            }
            else
            {
                StartRotation(nearestPoint, oppositePoint, clickedDot);
            }
        }

        if (isRotating)
        {
            RotateArm();
        }
    }

    void UpdateDotsEmptySprites()
    {
        foreach (Transform dotEmpty in dotsEmpty)
        {
            SpriteRenderer sr = dotEmpty.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            bool isAtPointA = Vector2.Distance(dotEmpty.position, pointA.position) < 0.05f;
            bool isAtPointB = Vector2.Distance(dotEmpty.position, pointB.position) < 0.05f;

            if (isAtPointA || isAtPointB)
                continue;

            Sprite newSprite = dotsEmptyToggled ? emptyActiveSprite : emptyDefaultSprite;
            if (sr.sprite != newSprite)
            {
                sr.sprite = newSprite;
                ApplyCorrectScale(dotEmpty, newSprite);
            }
        }
    }

    void UpdateDotSprites()
    {
        foreach (Transform dot in dots)
        {
            SpriteRenderer sr = dot.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            bool hasA = Vector2.Distance(dot.position, pointA.position) < 0.05f;
            bool hasB = Vector2.Distance(dot.position, pointB.position) < 0.05f;

            Sprite targetSprite = hasA && hasB ? bothDotSprite : (hasA || hasB ? highlightDotSprite : defaultDotSprite);
            if (sr.sprite != targetSprite)
            {
                sr.sprite = targetSprite;
                ApplyCorrectScale(dot, targetSprite);
            }
        }
    }

    void ApplyCorrectScale(Transform dot, Sprite newSprite)
    {
        float originalSize = originalPixelSize[dot];
        float newSize = newSprite.bounds.size.x;
        float scaleFactor = originalSize / newSize;
        dot.localScale = originalScales[dot] * scaleFactor;
    }

    void StartRotation(Transform pivot, Transform rotating, Transform targetDot)
    {
        if (isRotating)
        {
            Vector3 currentDir = (rotatingPoint.position - pivotPoint.position).normalized;
            Vector3 newDir = (targetDot.position - pivot.position).normalized;

            float angle = Vector3.SignedAngle(currentDir, newDir, Vector3.forward);
            if ((rotationDirection > 0 && angle < -10f) || (rotationDirection < 0 && angle > 10f))
                return;
        }

        pivotPoint = pivot;
        rotatingPoint = rotating;
        targetDotPosition = targetDot.position;

        Vector3 toCurrent = rotating.position - pivot.position;
        Vector3 toTarget = targetDotPosition - pivot.position;
        rotationDirection = Vector3.SignedAngle(toCurrent, toTarget, Vector3.forward) > 0 ? 1f : -1f;
        isRotating = true;

        if (rotateSound != null && !rotateAudioSource.isPlaying)
            rotateAudioSource.Play();
    }

    void RotateArm()
    {
        float angle = rotationSpeed * Time.deltaTime * rotationDirection;
        arm.RotateAround(pivotPoint.position, Vector3.forward, angle);
        CreateAfterImage();

        Transform reachedDot = GetClosestDotOnPath();
        if (reachedDot != null)
        {
            float angleToSnap = Vector2.SignedAngle(rotatingPoint.position - pivotPoint.position, reachedDot.position - pivotPoint.position);
            arm.RotateAround(pivotPoint.position, Vector3.forward, angleToSnap);
            isRotating = false;
            targetDotPosition = reachedDot.position;

            if (rotateAudioSource.isPlaying)
                rotateAudioSource.Stop();
        }
    }

    Transform GetClosestDotOnPath()
    {
        float tolerance = 0.08f;
        Vector3 pivotPos = pivotPoint.position;
        Vector3 currentDir = (rotatingPoint.position - pivotPos).normalized;

        foreach (Transform dot in dots)
        {
            Vector3 dirToDot = (dot.position - pivotPos).normalized;
            float angle = Vector3.SignedAngle(currentDir, dirToDot, Vector3.forward);
            if ((rotationDirection > 0 && angle >= 0 && angle <= 10f) ||
                (rotationDirection < 0 && angle <= 0 && angle >= -10f))
            {
                if (Vector2.Distance(rotatingPoint.position, dot.position) < tolerance)
                    return dot;
            }
        }

        if (!dotsEmptyToggled)
        {
            foreach (Transform dotEmpty in dotsEmpty)
            {
                Vector3 dirToDotEmpty = (dotEmpty.position - pivotPos).normalized;
                float angleEmpty = Vector3.SignedAngle(currentDir, dirToDotEmpty, Vector3.forward);
                if ((rotationDirection > 0 && angleEmpty >= 0 && angleEmpty <= 10f) ||
                    (rotationDirection < 0 && angleEmpty <= 0 && angleEmpty >= -10f))
                {
                    if (Vector2.Distance(rotatingPoint.position, dotEmpty.position) < tolerance)
                        return dotEmpty;
                }
            }
        }

        return null;
    }

    void CreateAfterImage()
    {
        GameObject afterImage = new GameObject("AfterImage");
        afterImage.transform.position = arm.position;
        afterImage.transform.rotation = arm.rotation;
        afterImage.transform.localScale = arm.localScale;

        SpriteRenderer sr = afterImage.AddComponent<SpriteRenderer>();
        sr.sprite = armSprite.sprite;
        sr.sortingOrder = armSprite.sortingOrder - 1;
        sr.color = afterimageColor;

        sr.DOFade(0f, afterimageDuration).OnComplete(() => Destroy(afterImage));
    }
}
