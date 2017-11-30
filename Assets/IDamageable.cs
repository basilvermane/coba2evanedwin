using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable : IHealth{
	void TakeDamage (int damageAmount);
}
