namespace GitHubExplorer {
    public interface IGitHubAPI {
        IUser GetUser(string userName);
    }

    public interface IUser {
        IRepository GetRepository(string repository);
        string Name { get; }
        string Description { get; }
    }

    public interface IRepository {
        string Name { get; }
        string Description { get; }
    }
}