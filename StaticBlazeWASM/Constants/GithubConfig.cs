namespace StaticBlazeWASM.Constants;

public static class GithubConfig
{
    public static void SetConfig(string owner, string repo, string branch)
    {
        Owner = owner;
        Repo = repo;
        Branch = branch;
    }

    public static string Owner { get; private set; } // Replace with your GitHub username
    public static string Repo { get; private set; } // Replace with your GitHub repository name
    public static string Branch { get; private set; } // Replace with the target branch
}