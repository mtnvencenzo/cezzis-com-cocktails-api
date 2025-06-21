namespace Cocktails.Api.Domain.Common;

using Cocktails.Api.Domain.Aggregates.AccountAggregate;
using MediatR;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public abstract class Entity : IEventEmitter<IEvent>
{
    [JsonIgnore]
    [NotMapped]
    private readonly List<IEvent> domainEvents = [];

    [JsonIgnore]
    [NotMapped]
    private int? requestedHashCode;

    [JsonInclude]
    public virtual string Id { get; protected set; }

    [JsonInclude]
    public virtual DateTimeOffset CreatedOn { get; protected set; }

    [JsonInclude]
    public virtual DateTimeOffset UpdatedOn { get; protected set; }

    [JsonIgnore]
    [NotMapped]
    public IReadOnlyCollection<IEvent> DomainEvents => this.domainEvents?.AsReadOnly();

    public void AddDomainEvent(IEvent eventItem) => this.domainEvents.Add(eventItem);

    public void RemoveDomainEvent(IEvent eventItem) => this.domainEvents.Remove(eventItem);

    public void ClearDomainEvents() => this.domainEvents.Clear();

    public bool IsTransient() => EqualityComparer<string>.Default.Equals(this.Id, default);

    public override bool Equals(object obj)
    {
        if (obj is null or not Entity)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (this.GetType() != obj.GetType())
        {
            return false;
        }

        var item = (Entity)obj;

        if (item.IsTransient() || this.IsTransient())
        {
            return false;
        }
        else
        {
            return EqualityComparer<string>.Default.Equals(this.Id, item.Id);
        }
    }

    public override int GetHashCode()
    {
        if (!this.IsTransient())
        {
            if (!this.requestedHashCode.HasValue)
            {
                // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                this.requestedHashCode = this.Id.GetHashCode() ^ 31;
            }

            return this.requestedHashCode.Value;
        }
        else
        {
            return base.GetHashCode();
        }
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (Equals(left, null))
        {
            return Equals(right, null);
        }
        else
        {
            return left.Equals(right);
        }
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }
}
