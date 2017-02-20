using System;
using System.Collections.Generic;

namespace JsonDB {
	public class JsonList<T> : List<T> {
		private Database Database { get; set; }

		public JsonList(Database database) {
			Database = database;
		}

		public void Save() {
			Database.SaveToDisk(this);
		}
	}
}