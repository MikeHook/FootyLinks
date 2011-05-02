using System.Collections.Generic;
using System.Reflection;

namespace FootyLinks.Core.DomainModel
{
	/// <summary>
	/// This serves as a base interface for <see cref="EntityWithTypedId{T}"/> and 
	/// <see cref="Entity"/>. Also provides a simple means to develop your own base entity.
	/// </summary>
	public interface IEntityWithTypedId<TId>
	{
		/// <summary>
		/// Gets the unique identifier for this entity
		/// </summary>
		TId Id { get; }
		/// <summary>
		/// Gets a value which determines whether this entity is transient (ie has not been saved)
		/// </summary>
		/// <returns></returns>
		bool IsTransient();
		/// <summary>
		/// Gets the properties from this instance which have not been 
		/// </summary>
		/// <returns></returns>
		IEnumerable<PropertyInfo> GetSignatureProperties();
	}
}
