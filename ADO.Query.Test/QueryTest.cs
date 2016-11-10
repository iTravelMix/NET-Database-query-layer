
namespace ADO.Query.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ADO.Query.Mapper;
    using ADO.Query.Test.Query.Dto;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using global::ADO.Query.Test.AdoMocks;
    using global::ADO.Query.Test.Query;

    [TestClass]
    public class QueryTest
    {
        [TestCleanup]
        public void Cleanup()
        {
            Slapper.AutoMapper.Cache.ClearInstanceCache();
        }

        [TestMethod]
        public void TestScalarQuery()
        {
            var queryRunner = new MockQueryRunner
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        {"Name", "Test"}                   
                    }                
                }
            };

            var id = queryRunner.ExecuteScalar<int>(new QuerySimple());
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void TestDataReaderQuery()
        {
            var queryRunner = new MockQueryRunner
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        {"Name", "Test"}                   
                    }                
                }
            };

            var dr = queryRunner.ExecuteReader(new QuerySimple());

            Assert.IsNotNull(dr);
            Assert.IsTrue(dr.Read());

            Assert.AreEqual(1, Convert.ToInt32(dr["Id"]));
            Assert.AreEqual("Test", Convert.ToString(dr["Name"]));
        }

        [TestMethod]
        public void TestMapperQuery()
        {
            var queryRunner = new MockQueryRunner(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        {"Name", "Test"}                   
                    },
                    new Dictionary<string, object>
                    {
                        {"Id", 2},
                        {"Name", "Test 2"}                   
                    }
                }
            };

            var result = queryRunner.Execute<SimpleDto>(new QuerySimple()).ToList();
           
            Assert.IsNotNull(result);
            // ReSharper disable PossibleMultipleEnumeration
            Assert.AreEqual(2, result.Count());

            var simpleDto = result.First();
            // ReSharper restore PossibleMultipleEnumeration

            Assert.AreEqual(1, simpleDto.Id);
            Assert.AreEqual("Test", simpleDto.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestSingleMapperFailureWithMorethanOneElementQuery()
        {
            var queryRunner = new MockQueryRunner(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        {"Name", "Test"}                   
                    },
                    new Dictionary<string, object>
                    {
                        {"Id", 2},
                        {"Name", "Test 2"}                   
                    }
                }
            };

            queryRunner.Execute<SimpleDto>(new QuerySimple()).ToSingle();
        }

        [TestMethod]
        public void TestSingleQuery()
        {
            var queryRunner = new MockQueryRunner(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        {"Name", "Test"}                   
                    }
                }
            };

            var singleDto = queryRunner.Execute<SimpleDto>(new QuerySimple()).ToSingle();

            Assert.IsNotNull(singleDto);
            Assert.AreEqual(1, singleDto.Id);
            Assert.AreEqual("Test", singleDto.Name);
        }

        [TestMethod]
        public void TestFirstOrDefaultEmptyQuery()
        {
            var queryRunner = new MockQueryRunner(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>()
            };

            var singleDto = queryRunner.Execute<SimpleDto>(new QuerySimple()).ToFirstOrDefault();
            Assert.IsNull(singleDto);
        }

        [TestMethod]
        public void TestFirstOrDefaultQuery()
        {
            var queryRunner = new MockQueryRunner(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", 1},
                        {"Name", "Test"}                   
                    },
                    new Dictionary<string, object>
                    {
                        {"Id", 2},
                        {"Name", "Test 2"}                   
                    }
                }
            };

            var singleDto = queryRunner.Execute<SimpleDto>(new QuerySimple()).ToFirstOrDefault();

            Assert.IsNotNull(singleDto);
            Assert.AreEqual(1, singleDto.Id);
            Assert.AreEqual("Test", singleDto.Name);
        }

        [TestMethod]
        public void TestPageQuery()
        {
            var queryRunner = new MockQueryRunner(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", (long)6},
                        {"Name", "Test"}                   
                    },
                    new Dictionary<string, object>
                    {
                        {"Id", 2},
                        {"Name", "Test 2"}                   
                    }
                }
            };

            var pagedList = queryRunner.Execute<SimpleDto>(new QueryPageSpecification(page: 1, itemsPerPages: 2));

            Assert.IsNotNull(pagedList);
            Assert.AreEqual(2, pagedList.Result.Count());
            Assert.AreEqual(6, pagedList.TotalItems);
            Assert.AreEqual(3, pagedList.TotalPages);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestPageFailureCountQuery()
        {
            var queryRunner = new MockQueryRunner(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        {"Id", "Execute scalar to return count, is not numeric"},
                        {"Name", "Test"}                   
                    },
                    new Dictionary<string, object>
                    {
                        {"Id", "Other"},
                        {"Name", "Test 2"}                   
                    }
                }
            };

            queryRunner.Execute<SimpleDto>(new QueryPageSpecification(page: 1, itemsPerPages: 2));
        }
    }
}
