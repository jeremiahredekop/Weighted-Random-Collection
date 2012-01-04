using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using WeightedRandomCollection;

namespace WRC.Tests
{
    [TestFixture]
    public class RandomTests
    {

        [Test]
        public void WeightsOfZeroValueShouldNotBeIncluded()
        {
            var hi = new[]
                         {
                             new WeightedItem<Guid>(Guid.NewGuid(), 1),
                             new WeightedItem<Guid>(Guid.NewGuid(), 0)
                         };

            var rc = new RandomCollection<Guid>(hi);

            rc.AsItems().Count().Should().Be(1, "An item of 0 weight should never be included in results");


        }

        [Test]
        public void Random_List_Should_Be_Unique_LoopTest()
        {
            for (var i = 0; i < 100; i++)
            {
                Random_List_Should_Be_Unique();
            }
        }

        [Test]
        public void CanHandleSingleItem()
        {
            var q = BuildRandomGuidCollection(1);
            var item = q.PickRandomItem();
            Assert.IsNotNull(item);
        }


        [Test]
        public void CanHandleSingleItem_AsUniqueItems()
        {
            var q = BuildRandomGuidCollection(1);
            var item = q.PickUniqueRandomValues(1);

            item.Should().NotBeNull();
        }


        [Test]
        public void ShouldCreate100Items()
        {
            var c = BuildRandomGuidCollection(100);

            try
            {
                c.AsItems().Count().Should().Be(100);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Test]
        public void Should_Pass_Integrity_Check_After_Building_Colleciton()
        {
            var q = BuildRandomGuidCollection(10000);
            q.CheckIntegrity();
        }

        private static void Random_List_Should_Be_Unique() // fails once in a while
        {
            var q = BuildRandomGuidCollection(1000);

            //var originalKeys = q.AsWeightedItems().Select(o => o.WeightKey).OrderBy(o => o).ToList();

            q.CheckIntegrity();

            // get 25 items
            var output = q.PickUniqueRandomValues(25, true);

            var notallKeysUnique = output.DistinctBy(o => o.WeightKey).Count() != 25;

           // var keys = output.Select(o => o.WeightKey).OrderBy(o => o).ToList();

            // make sure there are 25 unique values
            //Assert.IsTrue(output.CustomDistinctOn(o => o.Item).Count() == 25);
            notallKeysUnique.Should().BeFalse();

        }

        private static RandomCollection<string> BuildRandomGuidCollection(int count)
        {
            var items = new List<WeightedItem<string>>();
            var r = new Random();

            for (var i = 0; i < count; i++)
            {
                var key = r.Next(1, 100);
                items.Add(new WeightedItem<string>(Guid.NewGuid().ToString(), key));
            }

            var q = new RandomCollection<string>(items);
            return q;
        }


        [Test]
        public void Should_Not_Throw_Exception_On_Trying_To_Fetch_Too_Many()
        {
            var q = BuildRandomGuidCollection(100);

            List<WeightedItem<string>> output;
            var success = q.TryPickUniqueRandomValues(101,
                                                        true,
                                                        out output);

            output.Count.Should().Be(100, "100 unique Items were added, and trying to pick 101 should only return 100");
            success.Should().BeFalse("attempt should not have succeeded.");
        }

        [Test]
        public void Should_Return_True_On_Try()
        {
            var q = BuildRandomGuidCollection(100);

            List<WeightedItem<string>> output;

            var success = q.TryPickUniqueRandomValues(85,
                            true,
                            out output);

            output.Count.Should().Be(85, "Exactly 85 items should be returned");
            success.Should().BeTrue("Operation should have succeeded, as enough items were in the collection");
        }

    }
}
