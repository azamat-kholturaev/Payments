namespace Payments.Domain.Common
{
    public abstract class Entity(Guid? id = null)
    {
        public Guid Id { get; protected set; } = id ?? Guid.CreateVersion7();

        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id != Guid.Empty && Id == other.Id;
        }

        public override int GetHashCode()
            => Id.GetHashCode();
    }
}
