namespace ADO.Query.Test
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    using Helper;
    using Query;
    using Query.Dto;
    using Core.Test.Data;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Mapper;

    [TestClass]
    public class QueryIntegrationTest
    {
        static DataAccessSectionHandler dataAccessSettings;

        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            LocalDb.CreateLocalDb("querytest", "GenerateDb.sql", true);

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            var adoQuery = configuration.GetSection("AdoQuery");

            dataAccessSettings = new DataAccessSectionHandler(adoQuery["Type"], adoQuery["ConnectionString"]);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Slapper.AutoMapper.Cache.ClearInstanceCache();
        }

        [TestMethod]
        public void TestIntegrationScalarQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(dataAccessSettings);
            var id = queryRunner.ExecuteScalar<int>(new QueryUsers());

            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void TestIntegrationDataReaderQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(dataAccessSettings);
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
            using (var queryMapper = new QueryMapper())
            {
                var queryRunner = QueryRunner.CreateHelper(dataAccessSettings, queryMapper);
                var user = queryRunner.Execute(new QueryUsers()).ToFirstOrDefault();

                Assert.IsNotNull(user);

                Assert.AreEqual(1, user.Id);
                Assert.AreEqual("Diana Hendrix", user.Name);
            }
        }

        [TestMethod]
        public void TestIntegrationFirstOrDefaultWithoutResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(dataAccessSettings);
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers(99)).ToFirstOrDefault();

            Assert.IsNull(user);
        }

        [TestMethod]
        public void TestIntegrationSingleResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(dataAccessSettings);
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers(1)).ToSingle();

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestIntegrationSingleResultFailWithoutResultMapperQuery()
        {
            Assert.ThrowsException<InvalidOperationException>( () =>
            {
                var queryRunner = QueryRunner.CreateHelper(dataAccessSettings);
                queryRunner.Execute<SimpleDto>(new QueryUsers(99)).ToSingle();
            });
        }

        [TestMethod]
        public void TestIntegrationSingleResultFailManyResultMapperQuery()
        {
           Assert.ThrowsException<InvalidOperationException>(() =>
           {
               var queryRunner = QueryRunner.CreateHelper(dataAccessSettings);
               queryRunner.Execute<SimpleDto>(new QueryUsers()).ToSingle();
           });
        }

        [TestMethod]
        public void TestIntegrationOneToManyMapperQuery()
        {
            using (var queryMapper = new QueryMapper())
            {
                var queryRunner = QueryRunner.CreateHelper(dataAccessSettings, queryMapper);
                var users = queryRunner.Execute<SimpleDto>(new QueryOneToMany()).ToList();

                Assert.IsNotNull(users);
                // ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(3, users.Count());

                var user = users.First();
                // ReSharper restore PossibleMultipleEnumeration

                Assert.IsNotNull(user.Phones);
                Assert.AreEqual(2, user.Phones.Count());
            }
        }

        [TestMethod]
        public void TestIntegrationPerformanceOneToManyMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper(dataAccessSettings);
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

        [TestMethod]
        public void TestIntegrationWhithoutCacheResultMapperQuery()
        {
            QueryMapper.RemoveCacheAfterExecuteQuery = true;

            try
            {
                using (var queryMapper = new QueryMapper())
                {
                    var queryRunner = QueryRunner.CreateHelper(dataAccessSettings, queryMapper);

                    var user = queryRunner.Execute(new QueryUsers(1)).ToSingle();
                    var phone = queryRunner.Execute(new QueryPhone(1)).ToSingle();

                    Assert.AreEqual(1, phone.Id);
                    Assert.AreNotEqual(user.Name, phone.Name);
                }
            }
            finally
            {
                QueryMapper.RemoveCacheAfterExecuteQuery = false;
            }
        }

        [TestMethod]
        public void TestIntegrationCacheFailsResultMapperQuery()
        {
            QueryMapper.RemoveCacheAfterExecuteQuery = false;

            using (var queryMapper = new QueryMapper())
            {
                var queryRunner = QueryRunner.CreateHelper(dataAccessSettings, queryMapper);

                var user = queryRunner.Execute(new QueryUsers(1)).ToSingle();
                var phone = queryRunner.Execute(new QueryPhone(1)).ToSingle();

                Assert.AreEqual(1, phone.Id);
                Assert.AreEqual(user.Name, phone.Name);
            }
        }
    }
}
