using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using Inflector;

namespace JsonDB {
	public class Database : IDisposable {

		#region Properties

		private const string JsonExtension = "json";

		public string DatabaseDirectory { get; set; }

		private JsonSerializer Serializer { get; set; }

		#endregion

		#region Constructors

		public Database() {
			DatabaseDirectory = Path.Combine(Environment.CurrentDirectory, "JsonDB");
			Serializer = new JsonSerializer();
		}

		public Database(string databaseDirectory, JsonSerializer serializer) {
			DatabaseDirectory = databaseDirectory;
			Serializer = serializer;
		}

		public Database(string databaseDirectory) {
			DatabaseDirectory = databaseDirectory;
			Serializer = new JsonSerializer();
		}

		#endregion

		public JsonList<T> GetCollection<T>() {
			var Collection = new JsonList<T>(this);
			var path = GetPath<T>();

			EnsureCollectionFileExists(path);

			using(var streamReader = new StreamReader(path))
			using(var jsonTextReader = new JsonTextReader(streamReader)) {
				var items = Serializer.Deserialize<JsonList<T>>(jsonTextReader);

				if(items != null) {
					Collection.AddRange(items);
				}
			}

			return Collection;
		}

		internal void SaveCollectionToDisk<T>(IEnumerable<T> List) {
			var path = GetPath<T>();
			EnsureCollectionDirectoryExists(path);

			using(var streamWriter = new StreamWriter(path))
			using(var jsonTextWriter = new JsonTextWriter(streamWriter)) {
				Serializer.Serialize(jsonTextWriter, List);
			}
		}

		#region Helpers

		private string GetPath<T>() {
			return Path.ChangeExtension(Path.Combine(DatabaseDirectory, typeof(T).Name.Pluralize().Capitalize()), JsonExtension);
		}

		private static void EnsureCollectionDirectoryExists(string path) {
			var directory = Path.GetDirectoryName(path);

			if(!string.IsNullOrEmpty(directory)) {
				Directory.CreateDirectory(directory);
			}
		}

		private static void EnsureCollectionFileExists(string path) {
			EnsureCollectionDirectoryExists(path);

			File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite).Dispose();
		}

		void IDisposable.Dispose() {

		}

		#endregion
	}
}
