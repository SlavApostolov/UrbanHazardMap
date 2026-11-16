namespace UrbanHazardMap.Services
{
    public interface IPhotoStorageService
    {
        Task<string> SaveAsync(Stream content,string originalFileName);

        Task<Stream?> OpenReadAsync(string photoPath);
    }
}