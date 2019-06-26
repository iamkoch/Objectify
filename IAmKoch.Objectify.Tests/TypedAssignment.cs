using Shouldly;
using Xbehave;
using Xunit;

namespace IAmKoch.ObjectAssign.Tests
{
    public class TypedAssignment
    {
        [Scenario]
        public void LikeForLike(
            SampleClass src,
            SampleClass target)
        {
            "Given an instance of a class with two properties"
                .x(() => src = new SampleClass
                {
                    IntProperty = 6,
                    StringProperty = "something"
                });

            "When I assign it to a new object"
                .x(() => { target = Objectify.Assign<SampleClass>(src, src); });

            "Then that object should have those properties"
                .x(() =>
                {
                    Assert.Equal(src.IntProperty, target.IntProperty);
                    Assert.Equal(src.StringProperty, target.StringProperty);
                    Assert.False(object.ReferenceEquals(src, target));
                });
        }


        [Scenario]
        public void ActLikeAMapper(
            SampleClass src,
            SampleClassTwo similarTarget)
        {
            "Given an instance of a class with two properties"
                .x(() => src = new SampleClass
                {
                    IntProperty = 6,
                    StringProperty = "something"
                });

            "When I assign it to a new object with an extra property"
                .x(() => { similarTarget = Objectify.Assign<SampleClassTwo>(src); });

            "Then that object should have those properties"
                .x(() =>
                {
                    Assert.Equal(similarTarget.IntProperty, src.IntProperty);
                    Assert.Equal(similarTarget.StringProperty, src.StringProperty);
                });

            "And they shouldn't be the same reference"
                .x(() => Assert.False(ReferenceEquals(src, similarTarget)));
        }

        [Scenario]
        public void ActLikeAMapperWithAdditionalOverwritingProperties(
            SampleClass src,
            SampleClassTwo similarTarget)
        {
            "Given an instance of a class with two properties"
                .x(() => src = new SampleClass
                {
                    IntProperty = 6,
                    StringProperty = "something"
                });

            "When I assign it to a new object with an extra property"
                .x(() => { similarTarget = Objectify.Assign<SampleClassTwo>(src, new { intproperty = 7 }, new { stringproperty = "seven" }); });

            "Then that object should have those properties"
                .x(() =>
                {
                    Assert.Equal(7, similarTarget.IntProperty);
                    Assert.Equal("seven", similarTarget.StringProperty);
                });
        }

        [Scenario]
        public void OverwriteProperty(
            SampleClass src,
            SampleClass target)
        {
            "Given an instance of a class with two properties"
                .x(() => src = new SampleClass
                {
                    IntProperty = 6,
                    StringProperty = "something"
                });

            "When I assign it to a new object with an extra property"
                .x(() => { target = Objectify.Assign<SampleClass>(src, new { StringProperty = "something else" }); });

            "Then that object should have those properties"
                .x(() =>
                {
                    Assert.Equal(src.IntProperty, target.IntProperty);
                    Assert.Equal("something else", target.StringProperty);
                });
        }
    }
}
