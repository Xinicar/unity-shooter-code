using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZVoice : MonoBehaviour
{
    [SerializeField] AudioClip[] soundClip;          // �������� ���� ��� ������������.
    public float minDelay = 2f;          // ����������� �������� ����� ����������������.
    public float maxDelay = 5f;          // ������������ �������� ����� ����������������.
    public float volume = 1f;            // ��������� �����.
    public float pitchMin = 0.9f;        // ����������� ������� ����� (������� � ���).
    public float pitchMax = 1.1f;        // ������������ ������� ����� (������� � ����).

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

            // ��������� ��������� ������� � �������.
            float randomPitch = Random.Range(pitchMin, pitchMax);
            audioSource.pitch = randomPitch;

            audioSource.Play(); // ������������� ����.

            // ����, ���� ���� ��������� �� ����������.
            yield return new WaitForSeconds(soundClip[randomNumber].length);

            // ������������� ��������������� �����.
            audioSource.Stop();

        }
    }

}
