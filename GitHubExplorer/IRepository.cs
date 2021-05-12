using System.Collections.Generic;

namespace GitHubExplorer {
    public interface IRepository {
        string Name { get; }
        string Description { get; }
    }
}