namespace JsonDatabase
{
    public class Program
    {
        public static void main(){
            var db = new JsonDatabase.Database(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));

            db.Insert("Hello");
        }
    }
}