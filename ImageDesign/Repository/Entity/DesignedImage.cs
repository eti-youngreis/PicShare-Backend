namespace Repository.Entity
{
    public class DesignedImage
    {
        public int Id { get; set; }
      //  public virtual User User { get; set; }
        public virtual Image Image { get; set; }
        public virtual DesignTemplate Template { get; set; }
    }
}
