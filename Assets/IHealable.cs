using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHealable : IHealth{
	void Heal(int healAmount);
}
