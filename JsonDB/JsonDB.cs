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
			DatabaseDirectory = Path.Combine(Environment.CurrentDirectory, "Database");
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

			try {
				using(var streamWriter = new StreamReader(GetPath<T>()))
				using(var jsonTextReader = new JsonTextReader(streamWriter)) {
					Collection.AddRange(Serializer.Deserialize<List<T>>(jsonTextReader));
				}
			} catch(FileNotFoundException ex) {
				File.Create(ex.FileName).Dispose();
				GetCollection<T>();
			} catch(DirectoryNotFoundException) {
				Directory.CreateDirectory(Path.GetDirectoryName(GetPath<T>()));
				GetCollection<T>();
			} catch(Exception) {

			}

			return Collection;
		}

		internal void SaveToDisk<T>(IEnumerable<T> List) {
			try {
				using(var streamWriter = new StreamWriter(GetPath<T>()))
				using(var jsonTextWriter = new JsonTextWriter(streamWriter)) {
					Serializer.Serialize(jsonTextWriter, List);
				}
			} catch(DirectoryNotFoundException) {
				Directory.CreateDirectory(Path.GetDirectoryName(GetPath<T>()));
				SaveToDisk(List);
			} catch(Exception) {

			}
		}

		#region Helpers

		private string GetPath<T>() {
			return Path.ChangeExtension(Path.Combine(DatabaseDirectory, typeof(T).Name.Pluralize().Capitalize()), JsonExtension);
		}

		void IDisposable.Dispose() {

		}

		#endregion
	}
}
