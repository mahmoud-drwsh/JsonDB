using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace JsonDB {
	public class JsonList<T> : List<T> {
		private Database Database { get; set; }

		public JsonList() {

		}

		public JsonList(Database database) {
			Database = database;
		}

		public void Save() {
			Database.SaveCollectionToDisk(this);
		}

		public bool Exists(Func<T, bool> filter) {
			if(this.Where(filter).Count() > 0) {
				return true;
			}
			return false;
		}

		public bool AddIfNotExists(T item, Func<T, bool> filter) {
			if(!Exists(filter)) {
				Add(item);
				return true;
			}
			return false;
		}

		public bool AddRangeIfNotExists(IEnumerable<T> item, Func<T, bool> filter) {
			if(!Exists(filter)) {
				AddRange(item);
				return true;
			}
			return false;
		}

		public bool AddIfNotExistsAndSave(T item, Func<T, bool> filter) {
			if(!Exists(filter)) {
				Add(item);
				Save();
				return true;
			}
			return false;
		}

		public bool AddRangeIfNotExistsAndSave(IEnumerable<T> item, Func<T, bool> filter) {
			if(!Exists(filter)) {
				AddRange(item);
				Save();
				return true;
			}
			return false;
		}
	}
}