using muzafarova_backend.Models;

namespace muzafarova_backend
{
    public static class SharedData
    {
        public static HashSet<string> Summaries { get; } = new HashSet<string>
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public static List<user> users { get; } = new List<user>
        {
            new user(){ Login = "user", Password = "user" },
            new user(){ Login = "admin", Password = "admin" },
        };
    }
}
