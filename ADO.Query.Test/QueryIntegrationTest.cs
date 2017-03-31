namespace ADO.Query.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using ADO.Query.Helper;
    using ADO.Query.Mapper;
    using ADO.Query.Test.AdoMocks;
    using ADO.Query.Test.Query;
    using ADO.Query.Test.Query.Dto;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueryIntegrationTest
    {
        [ClassInitialize]
        public static void SetUp(TestContext context)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(@"..\..\"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            Slapper.AutoMapper.Cache.ClearInstanceCache();
        }

         [TestMethod]
        public void TestIntegrationScalarQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper");
            var id = queryRunner.ExecuteScalar<int>(new QueryUsers());

            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void TestIntegrationDataReaderQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper");
            using (var dr = queryRunner.ExecuteReader(new QueryUsers()))
            {
                Assert.IsNotNull(dr);
                Assert.IsTrue(dr.Read());

                Assert.AreEqual(1, Convert.ToInt32(dr["Id"]));
                Assert.AreEqual("User 1", Convert.ToString(dr["Name"]));
            }
        }

        [TestMethod]
        public void TestIntegrationFirstOrDefaultWithResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper", new QueryMapper());
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers()).ToFirstOrDefault();

            Assert.IsNotNull(user);

            Assert.AreEqual(1, user.Id);
            Assert.AreEqual("User 1", user.Name);
        }

        [TestMethod]
        public void TestIntegrationFirstOrDefaultWithoutResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper", new QueryMapper());
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers(99)).ToFirstOrDefault();

            Assert.IsNull(user);
        }

        [TestMethod]
        public void TestIntegrationSingleResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper", new QueryMapper());
            var user = queryRunner.Execute<SimpleDto>(new QueryUsers(1)).ToSingle();

            Assert.IsNotNull(user);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIntegrationSingleResultFailWithoutResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper", new QueryMapper());
            queryRunner.Execute<SimpleDto>(new QueryUsers(99)).ToSingle();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIntegrationSingleResultFailManyResultMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper", new QueryMapper());
            queryRunner.Execute<SimpleDto>(new QueryUsers()).ToSingle();
        }

        [TestMethod]
        public void TestIntegrationOneToManyMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper", new QueryMapper());
            var users = queryRunner.Execute<SimpleDto>(new QueryOneToMany()).ToList();

            Assert.IsNotNull(users);
            // ReSharper disable PossibleMultipleEnumeration
            Assert.AreEqual(2, users.Count());

            var user = users.First();
            // ReSharper restore PossibleMultipleEnumeration

            Assert.IsNotNull(user.Phones);
            Assert.AreEqual(2, user.Phones.Count());
        }

        [TestMethod]
        public void TestIntegrationPerformanceOneToManyMapperQuery()
        {
            var queryRunner = QueryRunner.CreateHelper("SqlAdoHelper", new QueryMapper());
            var iterations = 50000;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < iterations; i++)
            {
                var users = queryRunner.Execute<SimpleDto>(new QueryOneToMany()).ToList();

                Assert.IsNotNull(users);
                // ReSharper disable PossibleMultipleEnumeration
                Assert.AreEqual(2, users.Count());

                var user = users.First();
                // ReSharper restore PossibleMultipleEnumeration

                Assert.IsNotNull(user.Phones);
                Assert.AreEqual(2, user.Phones.Count());
            }

            Trace.WriteLine(string.Format("Mapped {0} objects in {1} ms.", iterations, stopwatch.ElapsedMilliseconds));
        }
    }
}
