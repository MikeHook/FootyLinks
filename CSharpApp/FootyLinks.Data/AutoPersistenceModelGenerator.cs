using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping;
using FootyLinks.Core.Domain;
using FluentNHibernate.Conventions.Helpers;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions;
using FootyLinks.Core.DomainModel;

namespace FootyLinks.Data
{
	public class AutoPersistenceModelGenerator
	{
		public AutoPersistenceModel Generate()
		{
			var mappings = new AutoPersistenceModel();

			mappings
				.AddEntityAssembly(typeof(Player).Assembly)
				.UseOverridesFromAssembly(typeof(AutoPersistenceModelGenerator).Assembly)
				.Where(GetAutoMappingFilter);
				//.Setup(GetSetup())
				//.Conventions.Setup(GetConventions);

			MapEnums(mappings);

			return mappings;
		}		

		private static void MapEnums(AutoPersistenceModel mapping)
		{
	
		}

	
		/// <summary>
		/// Configures the given mapping
		/// </summary>
		/// <param name="m"></param>
		public void Run(MappingConfiguration m)
		{
			m.HbmMappings
				.AddFromAssemblyOf<AutoPersistenceModelGenerator>();
			m.FluentMappings
				.AddFromAssemblyOf<AutoPersistenceModelGenerator>()
				.Conventions.Setup(GetConventions);
			m.AutoMappings
				.Add(Generate());

			//m.AutoMappings.ExportTo(@"D:\_Development\JiscCollections\mappings");
			m.FluentMappings.ExportTo(@"C:\_Development\FootyLinks\Database\Mappings");
		}

		
		/// <summary>
		/// Provides a filter for only including types which inherit from the IEntityWithTypedId interface.
		/// </summary>
		private static bool GetAutoMappingFilter(Type t)
		{
			if (t.IsInterface)
				return false;

			var shouldMap = t.GetInterfaces()
				.Any(x => x.IsGenericType
						  && x.GetGenericTypeDefinition() == typeof(IEntityWithTypedId<>));

			return shouldMap && t.Assembly == typeof(Player).Assembly;
		}

		/*
		private static Action<AutoMappingExpressions> GetSetup()
		{
			return c =>
			{
				c.FindIdentity = member => member.Name == "Id";
				c.IsConcreteBaseType = IsBaseTypeConvention;
				c.AbstractClassIsLayerSupertype = type => !IsDomainEntity(type);
				c.IsComponentType = IsComponentTypeConvention;
			};
		}

		private static bool IsDomainEntity(Type type)
		{
			return GetAutoMappingFilter(type)
				   && type.Assembly == typeof(Player).Assembly
				   && type != typeof(SummarisableEntity);
		}*/

		/// <summary>
		/// This project's conventions (which may be changed) are as follows:
		/// * Table names are plural
		/// * The Id of an object is "Id"
		/// * Foreign keys are "ObjectNameFk"
		/// * One-to-Many relationships cascade "All"
		/// 
		/// Feel free to change this to your project's needs!
		/// </summary>
		private static void GetConventions(IConventionFinder finder)
		{
			finder.Add(Table.Is(map => Inflector.Net.Inflector.Pluralize(map.EntityType.Name)));

			//c.FindIdentity = type => type.Name == "Id";
			finder.Add(PrimaryKey.Name.Is(map => "Id"));

			//c.GetForeignKeyName = type => type.Name + "Fk";
			finder.Add(ForeignKey.EndsWith("Fk"));

			//c.GetForeignKeyNameOfParent = type => type.Name + "Fk";

			//c.OneToManyConvention = o => o.Cascade.All();
			finder.Add(ConventionBuilder.Reference.Always(r => r.Cascade.SaveUpdate()));
			finder.Add(ConventionBuilder.HasMany.Always(r => r.Cascade.All()));
			finder.Add(ConventionBuilder.HasManyToMany.Always(r => r.Cascade.All()));

			//finder.Add(new RequiredConvention());
			//finder.Add(new SetConvention());
			//finder.Add(new BlobDocumentConvention());
			//finder.Add(new StringLengthConvention());
			//finder.Add(new ValueTypePropertyConvention());
		}

		/*
		private static bool IsBaseTypeConvention(Type arg)
		{
			bool derivesFromEntity = arg == typeof(Entity)
									 || arg == typeof(SummarisableEntity)
									 || arg == typeof(MessagingEntity);
			bool derivesFromEnumEntity = arg.IsGenericType
										 && (arg.GetGenericTypeDefinition() == typeof(EnumEntity<>));
			bool derivesFromEntityWithTypedId = arg.IsGenericType
												&& (arg.GetGenericTypeDefinition() == typeof(EntityWithTypedId<>));

			return derivesFromEntity || derivesFromEntityWithTypedId || derivesFromEnumEntity;
		}

		private static bool IsComponentTypeConvention(Type arg)
		{
			// A component type is taken to be any Domain class that doesn't inherit from Entity
			return arg.Namespace.StartsWith(typeof(ResourceDescription).Namespace) && !arg.IsSubclassOf(typeof(Entity));
		}*/
	}
}
