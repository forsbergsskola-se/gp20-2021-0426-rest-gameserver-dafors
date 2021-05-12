using System.Collections;
using System.Collections.Generic;

namespace GitHubExplorer {
    public interface IUser {
        IRepository GetRepository(string repository);
        string Name { get; }
        string Description { get; }
        IEnumerable<IRepository> Repositories();

    }
}