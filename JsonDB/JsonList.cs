using System.Collections.Generic;

namespace JsonDB {
	public class JsonList<T> : List<T> {

		private Database DB { get; set; }

		public JsonList(Database dB) {
			DB = dB;
		}

		public void Save() {
			DB.SaveToDisk(this);
		}
	}
}