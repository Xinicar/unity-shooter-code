using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZVoice : MonoBehaviour
{
    [SerializeField] AudioClip[] soundClip;          // Звуковой клип для проигрывания.
    public float minDelay = 2f;          // Минимальная задержка перед воспроизведением.
    public float maxDelay = 5f;          // Максимальная задержка перед воспроизведением.
    public float volume = 1f;            // Громкость звука.
    public float pitchMin = 0.9f;        // Минимальная частота звука (перекос в низ).
    public float pitchMax = 1.1f;        // Максимальная частота звука (перекос в верх).

    private AudioSource audioSource;
    private int randomNumber;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        randomNumber= Random.Range(0, soundClip.Length);
        audioSource.clip = soundClip[randomNumber];
        StartCoroutine(PlayRandomSound());
    }

    private IEnumerator PlayRandomSound()
    {
        while (true)
        {
            float delay = Random.Range(minDelay, maxDelay);
            randomNumber = Random.Range(0, soundClip.Length);
            audioSource.clip = soundClip[randomNumber];

            yield return new WaitForSeconds(delay);

            // Применяем случайный перекос в частоте.
            float randomPitch = Random.Range(pitchMin, pitchMax);
            audioSource.pitch = randomPitch;

            audioSource.Play(); // Воспроизводим звук.

            // Ждем, пока звук полностью не закончится.
            yield return new WaitForSeconds(soundClip[randomNumber].length);

            // Останавливаем воспроизведение звука.
            audioSource.Stop();

        }
    }

}
