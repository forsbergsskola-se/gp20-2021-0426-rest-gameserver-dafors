namespace GitHubExplorer.Data {
    public interface IGitHubApi {
        IUser GetUser(string userName);
    }
}