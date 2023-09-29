using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WeirdBrothers.CharacterController;
using WeirdBrothers.ThirdPersonController;

public class Player
{
    private int health = 100;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }
}

public class HealthControl : MonoBehaviour
{
    [SerializeField] GameObject[] _hideGOs;

    [SerializeField] TMP_Text _textHP;
    Player player = new();

    bool playerDead = false;

    [SerializeField] GameObject _deathScreen;
    void Start()
    {
        _textHP.text = player.Health.ToString();
    }

    public void DamagePlayer(int damage)
    {
        if (!playerDead)
        {
            FindObjectOfType<AnimateSlider>().AnimateHit();
            player.Health = player.Health - damage;  
            _textHP.text = player.Health.ToString();
            
            if (player.Health <= 25)
            {
                _textHP.color = Color.red;
            }
        
            if (player.Health <= 0)
            {
                player.Health = 0;
                _textHP.text = player.Health.ToString();
                PlayerDeath();
            }
        }
    }

    private void PlayerDeath()
    {
        playerDead = true;
        GetComponent<WBThirdPersonController>().enabled = false;
        GetComponent<WBInputHandler>().enabled = false;
        GetComponent<WBCharacterController>().enabled = false;

        GetComponent<Animator>().SetTrigger("Dead");
        foreach (var go in _hideGOs)
        {
            go.SetActive(false);
        }

        _deathScreen.SetActive(true);

        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        StartCoroutine(TimeStop());
    }

    private IEnumerator TimeStop()
    {
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0;
    }
}