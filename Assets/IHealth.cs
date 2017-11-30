using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IHealth{
	int Health{
		get;
	}

	int MaxHealth {
		get;
	}
	bool IsAlive{
		get;
	}

	void ResetHealth();
}
