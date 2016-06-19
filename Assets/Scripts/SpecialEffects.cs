using UnityEngine;
using System.Collections;

public class SpecialEffects : MonoBehaviour {
public static SpecialEffects Instance;

  public ParticleSystem smokeEffect;
  public ParticleSystem fireEffect;
  public ParticleSystem dmgEffect;

  void Awake()
  {
    if (Instance != null)
    {
      Debug.LogError("Multiple instances of SpecialEffectsHelper!");
    }

    Instance = this;
  }

  public void Explosion(Vector3 position)
  {
    instantiate(smokeEffect, position);

    instantiate(fireEffect, position);
  }

  public void Damage(Vector3 position)
  {
    instantiate(dmgEffect, position);
  }

  private ParticleSystem instantiate(ParticleSystem prefab, Vector3 position)
  {
    ParticleSystem newParticleSystem = Instantiate(
      prefab,
      position,
      Quaternion.identity
    ) as ParticleSystem;

    Destroy(
      newParticleSystem.gameObject,
      newParticleSystem.startLifetime
    );

    return newParticleSystem;
  }
}
