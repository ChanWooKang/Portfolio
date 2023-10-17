using UnityEngine;
using System.Collections;

public class DemoController : MonoBehaviour {

	void Start(){
		StartCoroutine (createRandomTexts ());
	}

	private IEnumerator createRandomTexts(){
		while(Application.isPlaying){
			Vector3 randomPosition = Random.insideUnitSphere * 100;
			randomPosition.Scale (new Vector3(1,0,1));

			ExaGames.Common.FloatingTextEffect.FloatingTextEffect.Create (
				gameObjectName: "FloatingText", // Name of the game object with the floating text effect
				position: randomPosition, 		// Initial position of the floating text
				value: "ExaGames", 				// Value to be shown in floating text.
				lifeTime: 5f);					// Lifetime in seconds

			yield return new WaitForSeconds(0.2f);
		}
	}
}
