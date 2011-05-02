using System;

namespace FootyLinks.Core.DomainModel
{
	/// <summary>
	/// Facilitates indicating which property(s) describe the unique signature of an 
	/// entity.  See Entity.GetTypeSpecificSignatureProperties() for when this is leveraged.
	/// </summary>
	/// <remarks>
	/// This is intended for use with <see cref="Entity" />.  It may NOT be used on a <see cref="ValueObject"/>.
	/// </remarks>
	[Serializable]
	public class DomainSignatureAttribute : Attribute { }
}