using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Octokit;

namespace DSPatcher.Patching
{
    public class GithubController
    {
        public bool IsDisposed { get; private set; }
        public long RepositoryId { get; private set; }

        private GitHubClient _client;


        private string _owner;
        private string _repository;
        private string _projectName;
        private string _patcherVersion;

        public GithubController(string owner, string repository,
                                string projectName, string patcherVersion)
        {
            _owner = owner;
            _repository = repository;
            _projectName = projectName;
            _patcherVersion = patcherVersion;
            _client = new GitHubClient(new ProductHeaderValue(_projectName, _patcherVersion));
            RepositoryId = GetId().Result;
        }

        public async Task<byte[]> GetContent(string path)
        {
            return await _client.Repository.Content.GetRawContent(_owner, _repository, path);
        }

        public async Task<string> GetContentString(string path)
        {
            return Encoding.UTF8.GetString(await GetContent(path));
        }

        public async Task<List<RepositoryContent>> GetAllContent()
        {
            var content = await _client.Repository.Content.GetAllContents(RepositoryId);
            return content.ToList();
        }

        public async Task<List<RepositoryContent>> GetAllContent(string path)
        {
            var content = await _client.Repository.Content.GetAllContents(RepositoryId, path);
            return content.ToList();
        }

        public async Task<List<RepositoryContent>> GetAllContent(string path = null, bool recursive = true, ContentType? type = null)
        {
            List<RepositoryContent> content = await (string.IsNullOrEmpty(path) ? GetAllContent() : GetAllContent(path));

            for (int i = 0; i < content.Count; i++)
            {
                if (recursive &&
                    content[i].Type.Value == ContentType.Dir)
                    content.AddRange(await GetAllContent(content[i].Path));

                //Filter type
                if (type.HasValue)
                {
                    //Wrong type
                    if (content[i].Type.Value != type.Value)
                    {
                        content.RemoveAt(i);
                        i--;
                        continue;
                    }
                }
            }

            if (type.HasValue && type.Value != ContentType.Dir)
                content.RemoveAll(c => c.Type.Value == ContentType.Dir);

            return content;
        }

        private async Task<long> GetId()
        {
            Repository rep = await _client.Repository.Get(_owner, _repository);
            return rep.Id;
        }
    }
}
