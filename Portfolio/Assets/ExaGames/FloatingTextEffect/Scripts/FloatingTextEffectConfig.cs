namespace ExaGames.Common.FloatingTextEffect {
	/// <summary>
	/// Floating text effect type.
	/// </summary>
	public enum Types {
		GENERIC,
		PLUS,
	}

	/// <summary>
	/// Floating text effect templates.
	/// </summary>
	class Templates {
		#region Singleton declaration
		/// <summary>
		/// Singleton instance of FloatingTextEffectTemplate.
		/// </summary>
		private static readonly Templates instance = new Templates();
		
		static Templates(){}
		private Templates(){}
		
		public static Templates Instance {
			get { return instance; }
		}
		#endregion

		public const string GENERIC = "{0}";
		public const string PLUS = "+{0}";
	}
}