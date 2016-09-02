using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.
		Collider[] overlapSphere=Physics.OverlapSphere(transform.position,m_ExplosionRadius,m_TankMask);
		for (int i = 0; i < overlapSphere.Length; i++) {
			Rigidbody targetRigibody = overlapSphere [i].GetComponent<Rigidbody> ();

			if (!targetRigibody)
				continue;
			targetRigibody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

			TankHealth targetHeath = targetRigibody.GetComponent<TankHealth> ();

			if (!targetHeath)
				continue;
			float damage = CalculateDamage (targetRigibody.position);
			targetHeath.TakeDamage (damage);
		}
		m_ExplosionParticles.transform.parent = null;
		m_ExplosionParticles.Play ();
		m_ExplosionAudio.Play ();
		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
		Destroy (gameObject);

    }


    private float CalculateDamage(Vector3 targetPosition)
    {

		Vector3 explosionTarget = targetPosition - transform.position;
		float explosionDistance = explosionTarget.magnitude;

		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

		float damage = relativeDistance * m_MaxDamage;

		damage = Mathf.Max (0.0f, damage);
		return damage;
    }
}