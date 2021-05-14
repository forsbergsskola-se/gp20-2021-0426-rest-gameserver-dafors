using System.Collections.Generic;

namespace GitHubExplorer {
    public interface IUser {
        IRepository GetRepository(string repository);
        string Name { get; }
        string Description { get; }
        public IEnumerable<IRepository> Repositories();
        public IEnumerable<Organization> Organizations();
        public void PrintAdditionalData();
    }
}