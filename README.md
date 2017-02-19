# JsonDatabase

## consider the Following 

```csharp
class File {
public string Name { get; set; }
}
```

## Examples

```csharp
using JsonDatabse;
...

//provide a directory for the database to save .json files to
//if no directory is provided the directory will be ".\Data\"
var db = new Database(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));

//just start inserting anything and it will create a .json file for that specific type in the directory given above
//notice that a type is required
db.InsertOne<File>(new File() { Name = "NewFile" });
//you can also insert a list of Files
db.InsertMany<File>(new List<File>() { new File() { Name = "File1" }, new File() { Name = "File2" } });

//when you call GetCollection<T>(); it will return a List<T> filled with what was inserted of the type given
var FilesCollection = db.GetCollection<File>();

//to delete a record you have to use linq just provide proper filter to delete
//all matching items will be deleted
db.Delete<File>(f => f.Name == "NewFile");

```
