using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerActions : MonoBehaviour
{
    public Action onRage;
    public PlayerAttributes attributes;
    public PlayerController playerController;
    private float remainingRageDuration = 0f;
    private int enemyKillCount = 0;
    public bool inRageMode;
    private void Start()
    {
        if (playerController == null)
        {
            playerController = PlayerController.Instance;
        }
    }
    private void Awake()
    {
        //Pruebas
        Debug.Log("Valores Restaurados");
        attributes.maxSpeed = 9f;
        attributes.acceleration = 2.5f;
        attributes.currentSpeed = 2f;
        attributes.Life = 200f;
        attributes.Attack = 10;
        attributes.walkSpeed = 5.0f;
        attributes.crouchSpeed = 3.0f;
        attributes.rollForce = 10f;
        attributes.Stamina = 100f;
        attributes.runSpeed = 9f;
    }
    public float GetRemainingRageDuration()
    {
        return (remainingRageDuration / attributes.RageDuration) * 10;
    }

    private void OnEnable()
    {
        HerenciaEnemy.OnEnemyKilled += IncrementEnemyKillCount;
        onRage += ApplyRageEffect;
    }

    private void OnDisable()
    {
        HerenciaEnemy.OnEnemyKilled -= IncrementEnemyKillCount;
        onRage -= ApplyRageEffect;
    }

    public void TriggerRage()
    {
        onRage?.Invoke();
    }

    private void ApplyRageEffect()
    {
        playerController.playerAttributes.Stamina = 100f;
        StartCoroutine(ScaleForDuration(Vector3.one * 1.5f, attributes.RageDuration));
    }

    private IEnumerator ScaleForDuration(Vector3 targetScale, float duration)
    {
        Debug.Log("MODO RAGE");
        Vector3 originalScale = transform.localScale;
        float timer = 0f;
        inRageMode = true;
        while (timer < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            remainingRageDuration = duration - timer;
            yield return null;
        }
        transform.localScale = targetScale;
        attributes.ApplyRageAttributes();

        float downscaleDuration = 1.5f;
        timer = 0f;
        while (timer < downscaleDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / downscaleDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Final del rage");
        transform.localScale = originalScale;
        attributes.ResetAttributes();
        playerController.playerAttributes.Stamina = 100f;
        inRageMode = false;
        enemyKillCount = 0;
    }
    public int GetEnemyKillCount()
    {
        return enemyKillCount;
    }

    public void IncrementEnemyKillCount()
    {
        if (enemyKillCount < 10)
        {
            enemyKillCount++;
        }
    }
}