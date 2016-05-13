using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	public int identification = 0;
	public Vector3 nextPosition = Vector3.zero;
	public Vector3 velocity = Vector3.left;

	public void Initialize(int id, int x, int y, Vector3 vel)
	{
		identification = id;
		nextPosition = new Vector3(x, y, 0);
		transform.position = nextPosition;
		velocity = vel;
	}

	public void Move(float a, float b)
	{
		float t = a + (b * Mathf.Sqrt (2));
		nextPosition = nextPosition + (velocity * t);
		iTween.MoveTo(this.gameObject, iTween.Hash(
			"position", nextPosition,
			"time", t,
			"easetype", "linear"
		));
		EndFix(a, b);
	}

	private IEnumerator EndFix(float a, float b){
		float time = a + (b * Mathf.Sqrt (2));
		yield return new WaitForSeconds(time);
		transform.position = nextPosition;
	}
}
