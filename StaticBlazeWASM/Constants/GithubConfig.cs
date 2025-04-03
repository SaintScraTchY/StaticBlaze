namespace StaticBlazeWASM.Constants;

public static class GithubConfig
{
    public static void SetConfig(string owner, string repo, string branch,string username)
    {
        Owner = owner;
        Repo = repo;
        Branch = branch;
        Username = username;
    }

    public static string Owner { get; private set; } // Replace with your GitHub username
    public static string Repo { get; private set; } // Replace with your GitHub repository name
    public static string Branch { get; private set; } = "main"; // Replace with the target branch
    public static string Username { get; private set; }
}