using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : HerenciaEnemy
{
    private Renderer enemyRenderer;
    public GameObject Eyes;
    private MaterialPropertyBlock mpb; //Lo uso para modificar el shader de un solo enemigo.
    private float dissolveAmount = 0f;
    private float dissolveSpeed = 1f;

    private void Start()
    {
        maxHP = 5;
        currentHP = maxHP;
        pushingForce = 10;
        enemyRenderer = GetComponentInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void TakeDamage(int damage, Vector3 attackerPosition)
    {
        currentHP -= damage;
        if (currentHP <= 0)
        {
            Kill();
        }
        else
        {
            Vector3 direction = (transform.position - attackerPosition).normalized;
            rb.AddForce(direction * pushingForce, ForceMode.Impulse);
        }
    }

    public void Kill()
    {
        OnEnemyKilled?.Invoke();
        StopCoroutine(IntHint());
        animator.SetBool("GoblinHit", false);
        StartCoroutine(DieGoblin());
        isAlive = false;
        Debug.Log(isAlive);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Arma")
        {
            Weapons weapon = other.gameObject.GetComponentInParent<Weapons>();
            if (weapon != null)
            {
                StartCoroutine(IntHint());
                TakeDamage(weapon.Damage, weapon.transform.position);
            }
        }
    }

    IEnumerator IntHint()
    {
        animator.SetBool("GoblinHit", true);
        yield return new WaitForSeconds(0.8f);
        animator.SetBool("GoblinHit", false);
    }

    IEnumerator DieGoblin()
    {
        animator.SetTrigger("GoblinDie");
        enemyCollider.enabled = false;
        Destroy(Eyes.gameObject);
        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(1f);
        while (dissolveAmount < 1f)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            SetDissolveAmount(dissolveAmount);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void SetDissolveAmount(float amount)
    {
        mpb.SetFloat("_DissolveAmount", amount);
        enemyRenderer.SetPropertyBlock(mpb);
    }
}
