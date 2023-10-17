using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace ExaGames.Common.FloatingTextEffect {
	/// <summary>
	/// Floating text effect.
	/// </summary>
	/// <author>Eduardo Casillas Marca - ExaGames</author>
	public sealed class FloatingTextEffect : MonoBehaviour {
		/// <summary>
		/// Default lifetime for floating text effects.
		/// </summary>
		public const float DEFAULT_LIFETIME = 2f;
		/// <summary>
		/// FloatingTextEffect resource path.
		/// </summary>
		private const string PREFAB_PATH = "FloatingTextEffect";

		#region Poperties
		/// <summary>
		/// Text to be drawn.
		/// </summary>
		public string Text = string.Empty;
		/// <summary>
		/// Speed with which the text will float
		/// </summary>
		public float FloatingSpeed = 50f;
		/// <summary>
		/// Text component to draw the text
		/// </summary>
		private Text textComponent;
		#endregion

		#region Unity Behaviour Methods
		/// <summary>
		/// Initializes the text component.
		/// </summary>
		private void Awake(){
			textComponent = GetComponentInChildren<Text>();
		}

		/// <summary>
		/// Sets the text and moves the object.
		/// </summary>
		private void Update() {
			textComponent.text = Text;
			transform.rotation = Camera.main.transform.rotation;
			transform.Translate(new Vector3(0,FloatingSpeed * Time.deltaTime,0));
		}
		#endregion

		/// <summary>
		/// Creates and configures a floating text effect.
		/// </summary>
		/// <param name="gameObjectName">Name of the game object with the floating text effect.</param>
		/// <param name="position">Initial position of the floating text.</param>
		/// <param name="value">Value to be shown in floating text.</param>
		/// <param name="type">Type of the floating text.</param>
		/// <param name="lifeTime">Life time in seconds.</param>
		/// <param name="parent">Parent transform.</param>
		/// <returns>Effect game object</returns>
		public static GameObject Create(
			string gameObjectName, 
			Vector3 position,
			object value, 
			Types type = Types.GENERIC, 
			float lifeTime = 0f,
			Transform parent = null){
			GameObject floatingTextGameObject = (GameObject)Instantiate (Resources.Load (PREFAB_PATH),position, Quaternion.identity) as GameObject;
			floatingTextGameObject.name = gameObjectName;
			floatingTextGameObject.GetComponent<FloatingTextEffect>().Configure (type, value, lifeTime);
			if(parent!=null){
				floatingTextGameObject.transform.SetParent (parent);
			}
			return floatingTextGameObject;
		}

		/// <summary>
		/// Configures the floating text effect.
		/// </summary>
		/// <param name="type">Effect type</param>
		/// <param name="value">Value to be shown</param>
		/// <param name="lifeTime">Life time of the effect GameObject. If zero, Destroy is not called.</param>
		public void Configure(Types type, object value, float lifeTime = 0f) {
			try {
				string textTemplate = string.Empty;

				textTemplate = (string)Templates.Instance.GetType ().GetField (type.ToString ()).GetValue (Templates.Instance);

				Text = string.Format (textTemplate, value.ToString ());
			} catch (System.NullReferenceException){
				Debug.LogErrorFormat (
					"Could not create the required floating text effect. {0} is not defined in FloatingTextTemplate class",
					type.ToString ());
			}
			
			if(lifeTime>0f){
				Destroy (this.gameObject, lifeTime);
			}
		}
	}
}