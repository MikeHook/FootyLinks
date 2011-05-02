namespace FootyLinks.Core.DomainModel
{
	/// <summary>
	/// Enables developer to set the assigned Id of an object.  This is not part of 
	/// <see cref="Entity" /> since most entities do not have assigned 
	/// Ids and since business rules will certainly vary as to what constitutes a valid,
	/// assigned Id for one object but not for another.
	/// </summary>
	/// <typeparam name="TId"></typeparam>
	public interface IHasAssignedId<TId>
	{
		/// <summary>
		/// Sets the Id of this object
		/// </summary>
		void SetAssignedIdTo(TId assignedId);
	}
}