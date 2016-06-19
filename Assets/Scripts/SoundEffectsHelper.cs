using UnityEngine;
using System.Collections;

/// <summary>
/// Creating instance of sounds from code with no effort
/// </summary>
public class SoundEffectsHelper : MonoBehaviour
{

  public static SoundEffectsHelper Instance;

  public AudioClip shotReceivedSound;
  public AudioClip playerShotSound;
  public AudioClip boostSound;

  void Awake()
  {
    // Register the singleton
    if (Instance != null)
    {
      Debug.LogError("Instancias múltiples.");
    }
    Instance = this;
  }

  public void MakeShotReceivedSound()
  {
    MakeSound(shotReceivedSound);
  }

  public void MakePlayerShotSound()
  {
    MakeSound(playerShotSound);
  }

  public void MakeBoostSound()
  {
   //MakeSound(boostSound);
  }

  private void MakeSound(AudioClip originalClip)
  {
    // As it is not 3D audio clip, position doesn't matter.
    AudioSource.PlayClipAtPoint(originalClip, transform.position);
  }
}
