using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class Sounds : MonoBehaviour {
  public AudioClip coinSound;
  public AudioClip invalidSound;
  public AudioClip selectedSound;
  public AudioClip lossSound;

  private AudioSource source;

  private void Awake() {
    source = GetComponent<AudioSource>();
  }

  public void PlayCoinSound() {
    if (coinSound) {
      source.clip = coinSound;
      source.pitch = 1.0f + Random.Range(-0.1f, 0.1f);
      source.volume = 1.0f + Random.Range(-0.1f, 0.1f);
      source.Play();
    }
  }

  public void PlayInvalidSound() {
    if (invalidSound) {
      source.clip = invalidSound;
      source.pitch = 1.0f + Random.Range(-0.1f, 0.1f);
      source.volume = 1.0f + Random.Range(0.0f, 0.2f);
      source.Play();
    }
  }

  public void PlaySelectedSound() {
    if (selectedSound) {
      source.clip = selectedSound;
      source.pitch = 1.0f + Random.Range(-0.1f, 0.1f);
      source.volume = 1.0f + Random.Range(0.0f, 0.1f);
      source.Play();
    }
  }

  public void PlayLossSound() {
    if (lossSound) {
      source.clip = lossSound;
      source.pitch = 1.0f;
      source.volume = 1.0f;
      source.Play();
    }
  }
}
