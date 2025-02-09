using System;
using System.Collections;
using UnityEngine;

public class Deformation : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] Transform transformToAffect;
    [SerializeField] SquashStretchAxis axisToAffect = SquashStretchAxis.Y;
    [SerializeField, Range(0, 1f)] private float animationDuration = 0.25f;
    [SerializeField] bool canBeOverwritten;
    [SerializeField] bool playOnStart;

    [Flags]
    public enum SquashStretchAxis
    {
        None = 0,
        X = 1,
        Y = 2
    }

    [Space]
    [Header("Animation Settings")]
    [SerializeField] private float initialScale = 1f;
    [SerializeField] private float targettedScale = 1.5f;
    [SerializeField] private bool resetToDefaultScale;

    [SerializeField]
    private AnimationCurve deformationCurve = new AnimationCurve(
        new Keyframe(0f, 0f),
        new Keyframe(0.25f, 1f),
        new Keyframe(1f, 0f)
    );
    [Space]
    [Header("Looping Settings")]
    bool looping;
    private Coroutine _deformationCoroutine;
    private WaitForSeconds _waitForSeconds;
    private Vector2 initialScaleVector;

    private bool affectX => (axisToAffect & SquashStretchAxis.X) != 0;
    private bool affectY => (axisToAffect & SquashStretchAxis.Y) != 0;

    private void Awake()
    {
        if (transformToAffect == null)
        {
            transformToAffect = transform;
        }
        initialScaleVector = transformToAffect.localScale;
    }
    private void Start()
    {
        if (playOnStart)
            CheckAndStartCoroutine();
    }

    [ContextMenu("Play deformation")]

    public void PlayDeformation()
    {
        if (looping && !canBeOverwritten)
            return;

        CheckAndStartCoroutine();
    }

    private void CheckAndStartCoroutine()
    {
        if (axisToAffect == SquashStretchAxis.None)
        {
            Debug.Log("Axis is set to None.", gameObject);
            return;
        }

        if (_deformationCoroutine != null)
        {
            StopCoroutine(_deformationCoroutine);

        }

        _deformationCoroutine = StartCoroutine(DeformationEffect());
    }

    private IEnumerator DeformationEffect()
    {
        do
        {
            float elapsedTime = 0;
            Vector2 originalScale = initialScaleVector;
            Vector2 modifiedScale = originalScale;  

            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float curvePosition = elapsedTime / animationDuration;
                float curveValue = deformationCurve.Evaluate(curvePosition);

                float remappedValue = initialScale + (curveValue * (targettedScale - initialScale));

                float minimumThreshold = 0.0001f;
                if (Mathf.Abs(remappedValue) < minimumThreshold)
                    remappedValue = minimumThreshold;

                if (affectX)
                    modifiedScale.x = originalScale.x * remappedValue;
                else
                    modifiedScale.x = originalScale.x / remappedValue;
                if (affectY)
                    modifiedScale.y = originalScale.y * remappedValue;
                else
                    modifiedScale.y = originalScale.y / remappedValue;

                transformToAffect.localScale = modifiedScale;

                yield return null;
            }

            if (resetToDefaultScale)
                transformToAffect.localScale = originalScale;

            if (looping)
            {
                yield return _waitForSeconds;
            }

        } while (looping);
    }

    public void SetLooping(bool shouldLoop)
    {
        looping = shouldLoop;
    }
}
