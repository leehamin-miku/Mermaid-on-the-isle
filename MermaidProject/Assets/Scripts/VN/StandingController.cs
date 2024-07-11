using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StandingController : MonoBehaviour
{
    public IEnumerator Effect(string EffectName)
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
        else if (effect[0] == "SmoothChange")
        {

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
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
            {
                yield return null;
                break;
            }
            transform.position = Vector2.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
    }


    // 한 번 흔들림
    public IEnumerator Shake(float shakeAmount, float shakeDuration)
    {
        Vector2 startPosition = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < shakeDuration)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
            {
                yield return null;
                break;
            }
            float x = startPosition.x + Mathf.Sin(elapsedTime * Mathf.PI * 2 / shakeDuration) * shakeAmount;
            transform.position = new Vector2(x, startPosition.y);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
    }



    // 점프
    public IEnumerator Jump(float jumpHeight, float jumpDuration)
    {
        Vector2 startPosition = transform.position;
        Vector2 peakPosition = startPosition + new Vector2(0, jumpHeight);
        bool isSkip = false;


        // 점프 상승
        float elapsedTime = 0;
        while (elapsedTime < jumpDuration / 2)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
            {
                isSkip = true;
                yield return null;
                break;
            }

            float t = elapsedTime / (jumpDuration / 2);
            transform.position = Vector2.Lerp(startPosition, peakPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 점프 하강
        elapsedTime = 0;
        if (!isSkip)
        {
            while (elapsedTime < jumpDuration / 2)
            {
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space"))
                {
                    yield return null;
                    break;
                }
                float t = elapsedTime / (jumpDuration / 2);
                transform.position = Vector2.Lerp(peakPosition, startPosition, Mathf.Sin(t * Mathf.PI * 0.5f));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }


        transform.position = startPosition;
    }
    public IEnumerator SmoothSpriteChange(string after_img)
    {
        Sprite newSprite = Resources.Load<Sprite>("Image/VN/" + after_img);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = newSprite;
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1,1,1,0);
        transform.GetChild(0).position = transform.position;
        transform.GetChild(0).localScale = new Vector3(1, 1, 1);

        while (transform.GetComponent<SpriteRenderer>().color.a > 0)
        {
            transform.GetComponent<SpriteRenderer>().color = new Color(1,1,1, transform.GetComponent<SpriteRenderer>().color.a - Time.deltaTime / .5f);
            transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, transform.GetChild(0).GetComponent<SpriteRenderer>().color.a + Time.deltaTime / .5f);
            yield return null;
        }
        transform.GetComponent<SpriteRenderer>().sprite = newSprite;
        transform.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = null;

        yield return null;

    }
}

