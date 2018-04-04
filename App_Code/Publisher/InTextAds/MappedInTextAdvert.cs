
namespace Titan.Publisher.InTextAds
{
    public class MappedInTextAdvert
    {
        public string Id { get; private set; }
        public string Description { get; private set; }
        public string Tag { get; private set; }
        public string Title { get; private set; }

        public MappedInTextAdvert(int id, string title, string description, string tag)
        {
            this.Id = Encryption.Encrypt(id.ToString());
            this.Title = title;
            this.Description = description;
            this.Tag = tag;
        }
    }
}