using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;

namespace FootyLinks.Data
{
	public class NHibernateHelper
	{
		private static ISessionFactory _sessionFactory;

		private static ISessionFactory SessionFactory
		{
			get
			{
				if (_sessionFactory == null)
				{
					var cfg = new Configuration().Configure();
					cfg.Properties.Add("proxyfactory.factory_class",
									   "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");

					_sessionFactory = Fluently.Configure(cfg)
						.Mappings(new AutoPersistenceModelGenerator().Run)
						.BuildSessionFactory();
				}
				return _sessionFactory;
			}
		}		

		public static ISession OpenSession()
		{
			return SessionFactory.OpenSession();
		}
	}
}
