using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoahController : MonoBehaviour
{
    public IEnumerator NoahEffect(string EffectName)
    {

        // effect[0] = 행동, [1] = 정도, [2] = 시간, [3] = 방향
        string[] effect = EffectName.Split('`');
        if (effect[0] == "NormalMove")
        {
            yield return StartCoroutine(NormalMove(float.Parse(effect[1]), float.Parse(effect[2]), float.Parse(effect[3])));
        }
        else if (effect[0] == "Shake")
        {
            yield return StartCoroutine(Shake(float.Parse(effect[1]), float.Parse(effect[2])));
        }
        else if (effect[0] == "Jump")
        {
            yield return StartCoroutine(Jump(float.Parse(effect[1]), float.Parse(effect[2])));
        }
    }


    // 좌 우 이동
    public IEnumerator NormalMove(float moveDistance, float moveDuration, float diraction)
    {
        Vector2 startPosition = transform.position;
        Vector2 endPosition = startPosition + new Vector2(moveDistance * diraction, 0);

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    
    // 한 번 흔들림
    public IEnumerator Shake(float shakeAmount, float shakeDuration)
    {
        Vector2 originalPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < shakeDuration)
        {
            float x = originalPosition.x + Mathf.Sin(elapsedTime * Mathf.PI * 2 / shakeDuration) * shakeAmount;
            transform.position = new Vector2(x, originalPosition.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }


   
    // 점프
    public IEnumerator Jump(float jumpHeight, float jumpDuration)
    {
        Vector2 startPosition = transform.position;
        Vector2 peakPosition = startPosition + new Vector2(0, jumpHeight);

        // 점프 상승
        float elapsedTime = 0;
        while (elapsedTime < jumpDuration / 2)
        {
            float t = elapsedTime / (jumpDuration / 2);
            transform.position = Vector2.Lerp(startPosition, peakPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 점프 하강
        elapsedTime = 0;
        while (elapsedTime < jumpDuration / 2)
        {
            float t = elapsedTime / (jumpDuration / 2);
            transform.position = Vector2.Lerp(peakPosition, startPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
    }
}
