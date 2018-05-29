using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TwinKeyDictionary.NetStandard;

namespace TwinKeyDictionary.NetStandardTests
{
    [TestFixture]
    public class TwinKeyDictionaryTest
    {
        private TwinKeyDictionary<int, string, string> _dictionary;

        [SetUp]
        public void SetUp()
        {
            _dictionary = new TwinKeyDictionary<int, string, string> 
                {
                    {1, "asd", "gosho"}, 
                    {1, "bsd", "pesho"},
                    {2, "asd", "sasho"},
                    {3, "aaa", "penka"}
                };
        }

        [Test]
        public void Add_WithPrimaryKey_ContainsRecordWithDefaultSecondaryKey()
        {
            _dictionary.Add(1, "stamat");

            Assert.That(_dictionary.Contains(new KeyValuePair<(int,string), string>
                ((1, default(string)), "stamat")));
        }

        [Test]
        public void Add_DuplicateKey_ThrowsException()
        {
            Assert.That(()=>_dictionary.Add(1, "asd", "sasho"), Throws.Exception);
        }

        [Test]
        public void TryGetValue_ByPrimaryKey_ReturnsFirstRecordWithSuchKey()
        {
            bool success = _dictionary.TryGetValue(1, out var value);
            
            Assert.That(success);
            Assert.That(value, Is.EqualTo("gosho"));
        }

        [Test]
        public void Indexer_ByPrimaryKey_ReturnsFirstRecordWithSuchKey()
        {
            var value = _dictionary[1];
            
            Assert.That(value, Is.EqualTo("gosho"));
        }

        [Test]
        public void Contains_ExistingPrimaryKeyAndValue_ReturnsTrue()
        {
            bool contains = _dictionary.Contains(new KeyValuePair<int, string>(1, "gosho"));
            
            Assert.That(contains);
        }

        [Test]
        public void Remove_ByPrimaryKey_RemoveFirstRecordWithSuchKey()
        {
            _dictionary.Remove(1);
            
            Assert.That(_dictionary, Does.Not.ContainValue("gosho"));
        }

        [Test]
        public void TryGetValue_ByPrimaryAndSecondaryKey_ReturnsCorrectRecord()
        {
            bool success = _dictionary.TryGetValue((1, "bsd"), out string value);
            
            Assert.That(success);
            Assert.That(value, Is.EqualTo("pesho"));
        }

        [Test]
        public void Indexer_ByPrimaryAndSecondaryKey_ReturnsCorrectRecord()
        {
            var value = _dictionary[1, "bsd"];
            
            Assert.That(value, Is.EqualTo("pesho"));
        }
    }
}