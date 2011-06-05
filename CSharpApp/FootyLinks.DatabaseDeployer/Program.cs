using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Bytecode;
using FluentNHibernate.Cfg.Db;
using FootyLinks.Core.Domain;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Tool.hbm2ddl;
using FootyLinks.Data.Mappings;
using NHibernate.ByteCode.Castle;
using FootyLinks.Data;

namespace FootyLinks.DatabaseDeployer
{
	class Program
	{
		//public const string dbFilePath = @"C:\_Development\FootyLinks\Database\footylinks.db";
		public const string mappingPath = @"C:\_Development\FootyLinks\Database\Mappings\";
		public const string dbFilePath = "footylinks.db";

		static void Main(string[] args)
		{
			SampleDataTest();			
		}

		private static void SampleDataTest()
		{
			var sessionFactory = CreateSessionFactory();

			//SaveSampleData(sessionFactory);
			OutputData(sessionFactory);
		}

		private static void SaveSampleData(ISessionFactory sessionFactory)
		{
			using (var session = sessionFactory.OpenSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					// create a couple of Clubs with some players
					var liverpool = new Club(1, "Liverpool");
					var chelsea = new Club(2, "Chelsea");
					var realMadrid = new Club(3, "Real Madrid");

					var stevieG = new Player(1, "Steven Gerrard", liverpool);
					var fernandoTorres = new Player(2, "Fernando Torres", chelsea);
					var joeCole = new Player(3, "Joe Cole", liverpool);
					//I know this is not right, just testing the mappings!
					var xabiAlonso = new Player(4, "Xabi Alonso", realMadrid);
					var jamieCarragher = new Player(5, "Jamie Carragher", liverpool);
					var dirkKuyt = new Player(6, "Dirk Kuyt", liverpool);
					var fatFrank = new Player(7, "Frank Lampard", chelsea);

					AddFormerPlayersToClub(liverpool, fernandoTorres, xabiAlonso);
					AddFormerPlayersToClub(chelsea, joeCole);

					// save both stores, this saves everything else via cascading
					session.SaveOrUpdate(liverpool);
					session.SaveOrUpdate(chelsea);
					session.SaveOrUpdate(realMadrid);
					transaction.Commit();
				}

				session.Clear();
			}
		}

		private static void OutputData(ISessionFactory sessionFactory)
		{
			Console.WriteLine("Sample Data:");

			using (var session = sessionFactory.OpenSession())
			{
				using (session.BeginTransaction())
				{
					//var clubs = session.QueryOver<Club>().Where(p => p.Name == "Liverpool").List();
					var clubs = session.QueryOver<Club>().List<Club>();
					foreach (var club in clubs)
					{
						WriteClubPretty(club);
					}
				}
				Console.ReadKey();
			}
		}

		private static void WriteClubPretty(Club club)
		{
			Console.WriteLine(club.Name);
			Console.WriteLine(" Current players:");

			foreach (var player in club.CurrentPlayers)
			{
				Console.WriteLine("  " + player.Name);
			}
			Console.WriteLine(" Former Players:");

			foreach (var player in club.FormerPlayers)
			{
				Console.WriteLine("  " + player.Name);
			}
			Console.WriteLine();
		}

		public static void AddFormerPlayersToClub(Club club, params Player[] players)
		{
			foreach (var player in players)
			{
				club.AddFormerPlayer(player);
			}
		}

		private static ISessionFactory CreateSessionFactory()
		{
			var cfg = new Configuration().Configure();
			cfg.Properties.Add("proxyfactory.factory_class",
							   "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");

			return Fluently.Configure(cfg)
			  //.Database(
				//SQLiteConfiguration.Standard
				//	.InMemory()					
				 // .UsingFile(dbFilePath)
				//  .ProxyFactoryFactory(typeof(ProxyFactoryFactory))
			  //)			  

			  .Mappings(new AutoPersistenceModelGenerator().Run)
			  .ExposeConfiguration(BuildSchema)
			  .BuildSessionFactory();
		}		

		private static void BuildSchema(Configuration config)
		{
			// delete the existing db on each run
			if (File.Exists(dbFilePath))
				File.Delete(dbFilePath);

			//Note - Don't run this if the db doesn't exist yet
			new SchemaExport(config).Drop(false, false);

			// this NHibernate tool takes a configuration (with mapping info in)
			// and exports a database schema from it
			new SchemaExport(config).Create(false, true);
		}
	}
}
