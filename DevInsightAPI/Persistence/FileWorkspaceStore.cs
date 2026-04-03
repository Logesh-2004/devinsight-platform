using System.Text.Json;

namespace DevInsightAPI.Persistence
{
    public class FileWorkspaceStore
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private readonly SemaphoreSlim _gate = new(1, 1);
        private readonly string _dataFilePath;

        public FileWorkspaceStore(IHostEnvironment environment, IConfiguration configuration)
        {
            var relativePath = configuration["Storage:Path"];
            _dataFilePath = Path.IsPathRooted(relativePath)
                ? relativePath
                : Path.Combine(environment.ContentRootPath, relativePath ?? "App_Data\\devinsight-data.json");
        }

        public async Task<WorkspaceDataFile> ReadAsync()
        {
            await _gate.WaitAsync();

            try
            {
                return await ReadCoreAsync();
            }
            finally
            {
                _gate.Release();
            }
        }

        public async Task<TResult> UpdateAsync<TResult>(Func<WorkspaceDataFile, TResult> update)
        {
            await _gate.WaitAsync();

            try
            {
                var data = await ReadCoreAsync();
                var result = update(data);
                await WriteCoreAsync(data);
                return result;
            }
            finally
            {
                _gate.Release();
            }
        }

        private async Task<WorkspaceDataFile> ReadCoreAsync()
        {
            var directory = Path.GetDirectoryName(_dataFilePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_dataFilePath))
            {
                var emptyData = new WorkspaceDataFile();
                await WriteCoreAsync(emptyData);
                return emptyData;
            }

            await using var stream = File.OpenRead(_dataFilePath);
            var data = await JsonSerializer.DeserializeAsync<WorkspaceDataFile>(stream, JsonOptions);
            return data ?? new WorkspaceDataFile();
        }

        private async Task WriteCoreAsync(WorkspaceDataFile data)
        {
            await using var stream = File.Create(_dataFilePath);
            await JsonSerializer.SerializeAsync(stream, data, JsonOptions);
        }
    }
}
