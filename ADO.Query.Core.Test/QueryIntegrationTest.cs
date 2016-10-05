namespace ADO.Query.Test
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Helper;
    using Mapper;
    using Query;
    using Query.Dto;
    using Core.Test.Data;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryIntegrationTest
    {
        static IConfigurationRoot Configuration;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            LocalDb.CreateLocalDb("querytest", "GenerateDb.sql", true);

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Slapper.AutoMapper.Cache.ClearInstanceCache();
        }

        [TestMethod]
        public void TestIntegrationScalarQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")));
            var id = queryRunner.ExecuteScalar<int>(new QueryUsers());

            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void TestIntegrationDataReaderQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")));
            using (var dr = queryRunner.ExecuteReader(new QueryUsers()))
            {
                Assert.IsNotNull(dr);
                Assert.IsTrue(dr.Read());

                Assert.AreEqual(1, Convert.ToInt32(dr["Id"]));
                Assert.AreEqual("Diana Hendrix", Convert.ToString(dr["Name"]));
            }
        }

        [TestMethod]
        public void TestIntegrationFirstOrDefaultWithResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")), new QueryMapper());
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers()).ToFirstOrDefault();

            Assert.IsNotNull(user);

            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("Diana Hendrix", user.Name);
        }

        [TestMethod]
        public void TestIntegrationFirstOrDefaultWithoutResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")), new QueryMapper());
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers(99)).ToFirstOrDefault();

            Assert.IsNull(user);
        }

        [TestMethod]
        public void TestIntegrationSingleResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")), new QueryMapper());
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers(1)).ToSingle();

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestIntegrationSingleResultFailWithoutResultMapperQuery()
        {
            Assert.ThrowsException<InvalidOperationException>( () =>
            {
                var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")), new QueryMapper());
                queryRunner.Execute<SimpleDto>(new QueryUsers(99)).ToSingle();
            });
        }

        [TestMethod]
        public void TestIntegrationSingleResultFailManyResultMapperQuery()
        {
           Assert.ThrowsException<InvalidOperationException>(() =>
           {
               var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")), new QueryMapper());
               queryRunner.Execute<SimpleDto>(new QueryUsers()).ToSingle();
           });
        }

        [TestMethod]
        public void TestIntegrationOneToManyMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")), new QueryMapper());
            var users = queryRunner.Execute<SimpleDto>(new QueryOneToMany()).ToList();

            Assert.IsNotNull(users);
            // ReSharper disable PossibleMultipleEnumeration
            Assert.AreEqual(3, users.Count());

            var user = users.First();
            // ReSharper restore PossibleMultipleEnumeration

            Assert.IsNotNull(user.Phones);
            Assert.AreEqual(2, user.Phones.Count());
        }

        [TestMethod]
        public void TestIntegrationPerformanceOneToManyMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(new DataAccessSectionHandler(Configuration.GetSection("AdoQuery"), Configuration.GetSection("Data")), new QueryMapper());
            var iterations = 50000;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                var users = queryRunner.Execute<SimpleDto>(new QueryOneToMany()).ToList();

                Assert.IsNotNull(users);
                // ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(3, users.Count());

                var user = users.First();
                // ReSharper restore PossibleMultipleEnumeration

                Assert.IsNotNull(user.Phones);
                Assert.AreEqual(2, user.Phones.Count());
            }

            Trace.WriteLine($"Mapped {iterations} objects in {stopwatch.ElapsedMilliseconds} ms.");
        }
    }
}
