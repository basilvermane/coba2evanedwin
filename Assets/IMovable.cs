using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface IMovable{
	Vector3 Position{
		get;
	}
	Vector3 Destination {
		get;
		set;
	}
	Vector3 Direction {
		get;
	}
	float Speed {
		get;
	}
	Quaternion TargetRotation {
		get;
	}
}

