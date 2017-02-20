using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using Inflector;

namespace JsonDB {
	public class Database {
		private const string JsonExtension = "json";

		private string DatabaseDirectory { get; set; }

		private JsonSerializer Serializer { get; set; } = new JsonSerializer() { Formatting = Formatting.Indented };

		public Database() {
			DatabaseDirectory = Environment.CurrentDirectory;
			DatabaseDirectory = Path.Combine(DatabaseDirectory, "Data");
		}

		public Database(string DatabaseDirectory) {
			this.DatabaseDirectory = DatabaseDirectory;
		}

		public Database(string DatabaseDirectory, JsonSerializer Serializer) {
			this.DatabaseDirectory = DatabaseDirectory;
			this.Serializer = Serializer;
		}

		public List<T> GetCollection<T>() {
			var Collection = new List<T>();

			try {
				using(var sr = new StreamReader(GetPath<T>()))
				using(var jtr = new JsonTextReader(sr)) {
					Collection.AddRange(Serializer.Deserialize<List<T>>(jtr));
				}
			} catch(FileNotFoundException ex) {
				Console.WriteLine("GetCollection<T>():");
				Console.WriteLine(ex.Message);

				File.Create(ex.FileName).Dispose();

				GetCollection<T>();
			} catch(DirectoryNotFoundException ex) {
				Console.WriteLine("GetCollection<T>():");
				Console.WriteLine(ex.Message);
				Directory.CreateDirectory(Path.GetDirectoryName(GetPath<T>()));
				GetCollection<T>();
			} catch(Exception ex) {
				Console.WriteLine("GetCollection<T>():");
				Console.WriteLine(ex.Message);
			}

			return Collection;
		}

		public void InsertOne<T>(T Item) {
			var Collection = GetCollection<T>();

			Collection.Add(Item);

			Serialize(Collection);
		}

		public void InsertMany<T>(IEnumerable<T> Item) {
			var Collection = GetCollection<T>();

			Collection.AddRange(Item);

			Serialize(Collection);
		}

		public int Delete<T>(Predicate<T> Match) {
			var Collection = GetCollection<T>();

			Collection.RemoveAll(Match);

			Serialize(Collection);

			return Collection.Count();
		}

		private void Serialize<T>(IEnumerable<T> List) {
			try {
				using(var streamWriter = new StreamWriter(GetPath<T>())) {
					using(var jsonTextWriter = new JsonTextWriter(streamWriter)) {
						Serializer.Serialize(jsonTextWriter, List);
					}
				}
			} catch(DirectoryNotFoundException) {
				Console.WriteLine("Serialize<T>():");
				Directory.CreateDirectory(Path.GetDirectoryName(GetPath<T>()));
				Serialize(List);
			} catch(Exception ex) {
				Console.WriteLine("Serialize<T>():");
				Console.WriteLine(ex.Message);
			}
		}

		private string GetPath<T>() {
			return Path.ChangeExtension(Path.Combine(DatabaseDirectory, typeof(T).Name.Pluralize().Capitalize()), JsonExtension);
		}
	}
}
