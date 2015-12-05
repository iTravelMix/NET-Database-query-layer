﻿
namespace ADO.Query.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ADO.Query.Mapper;

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
        public void TestDataTableQuery()
        {
            var adoHelper = new AdoMockHelper
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

            var dt = adoHelper.ExecuteDataTable(new QuerySimple());
            
            Assert.IsNotNull(dt);
            Assert.IsNotNull(dt.Rows);

            Assert.AreEqual(1, dt.Rows.Count);
            Assert.AreEqual(1, Convert.ToInt32(dt.Rows[0]["Id"]));
            Assert.AreEqual("Test", Convert.ToString(dt.Rows[0]["Name"]));
        }

        [TestMethod]
        public void TestScalarQuery()
        {
            var adoHelper = new AdoMockHelper
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

            var id = adoHelper.ExecuteScalar<int>(new QuerySimple());
            Assert.AreEqual(1, id);
        }

        [TestMethod]
        public void TestDataReaderQuery()
        {
            var adoHelper = new AdoMockHelper
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

            var dr = adoHelper.ExecuteReader(new QuerySimple());

            Assert.IsNotNull(dr);
            Assert.IsTrue(dr.Read());

            Assert.AreEqual(1, Convert.ToInt32(dr["Id"]));
            Assert.AreEqual("Test", Convert.ToString(dr["Name"]));
        }

        [TestMethod]
        public void TestMapperQuery()
        {
            var adoHelper = new AdoMockHelper(new QueryMapper())
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

            var result = adoHelper.Execute(new QuerySpecification());
           
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
            var adoHelper = new AdoMockHelper(new QueryMapper())
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

            adoHelper.Execute(new QuerySingleSpecification());
        }

        [TestMethod]
        public void TestSingleQuery()
        {
            var adoHelper = new AdoMockHelper(new QueryMapper())
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

            var singleDto = adoHelper.Execute(new QuerySingleSpecification());

            Assert.IsNotNull(singleDto);
            Assert.AreEqual(1, singleDto.Id);
            Assert.AreEqual("Test", singleDto.Name);
        }

        [TestMethod]
        public void TestFirstOrDefaultEmptyQuery()
        {
            var adoHelper = new AdoMockHelper(new QueryMapper())
            {
                ReturnValues = new List<IDictionary<string, object>>()
            };

            var singleDto = adoHelper.Execute(new QueryFirstOrDefaultSpecification());
            Assert.IsNull(singleDto);
        }

        [TestMethod]
        public void TestFirstOrDefaultQuery()
        {
            var adoHelper = new AdoMockHelper(new QueryMapper())
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

            var singleDto = adoHelper.Execute(new QueryFirstOrDefaultSpecification());

            Assert.IsNotNull(singleDto);
            Assert.AreEqual(1, singleDto.Id);
            Assert.AreEqual("Test", singleDto.Name);
        }

        [TestMethod]
        public void TestPageQuery()
        {
            var adoHelper = new AdoMockHelper(new QueryMapper())
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

            var pagedList = adoHelper.Execute(new QueryPageSpecification(page:1, itemsPerPages: 2));

            Assert.IsNotNull(pagedList);
            Assert.AreEqual(2, pagedList.Result.Count());
            Assert.AreEqual(6, pagedList.TotalItems);
            Assert.AreEqual(3, pagedList.TotalPages);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TestPageFailureCountQuery()
        {
            var adoHelper = new AdoMockHelper(new QueryMapper())
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

            adoHelper.Execute(new QueryPageSpecification(page: 1, itemsPerPages: 2));
        }
    }
}