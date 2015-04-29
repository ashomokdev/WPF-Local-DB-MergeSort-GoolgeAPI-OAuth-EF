using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using WpfMergeSort;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Merge()
        {
            List<object[]> left = new List<object[]>();
            left.Add(new object[] {1, "Iuliia", "Developer"});
            left.Add(new object[] { 2, "Anna", "Developer" });
            left.Add(new object[] {4, "Micha", "Developer" });
            left.Add(new object[] { 6, "Anna", "Developer" });
            left.Add(new object[] { 15, "Anna", "Developer" });
            left.Add(new object[] { 25, "Anna", "Developer" });
         
            List<object[]> right = new List<object[]>();
            right.Add(new object[] { 0, "Micha", "Developer" });
            right.Add(new object[] { 3, "Iuliia", "Developer" });
            right.Add(new object[] { 5, "Anna", "Developer" });

            Merge(left, right);
            Merge(right, left);
            Merge(left, left);
            Merge(right, right);
        }

        private static void Merge(List<object[]> left, List<object[]> right)
        {

            List<object[]> result = MergeSort.Merge(left, right);
            List<object[]> expected = left.Concat(right).ToList();
            expected.Sort((val1, val2) => ((IComparable)val1.GetValue(MergeSort.ColumnIndex)).CompareTo(val2.GetValue(MergeSort.ColumnIndex)));
            Assert.AreEqual(result.Count, expected.Count);

            for (int k = 0; k < result.Count; k++)
            {
                for (int i = 0; i < result[k].Length; i++)
                {
                    Assert.AreEqual((result[k])[i], (expected[k])[i]);
                }
            }
        }
    }
}
