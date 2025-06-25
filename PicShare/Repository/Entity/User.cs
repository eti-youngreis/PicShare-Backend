namespace Repository.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
