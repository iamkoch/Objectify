using Xbehave;
using Xunit;

namespace IAmKoch.ObjectAssign.Tests
{
    public class DynamicAssignment
    {
        [Scenario]
        public void LikeForLike(
            SampleClass src,
            dynamic target)
        {
            "Given an instance of a class with two properties"
                .x(() => src = new SampleClass
                {
                    IntProperty = 6,
                    StringProperty = "something"
                });

            "When I assign it to a new object"
                .x(() => { target = Objectify.Assign(new { }, src); });

            "Then that object should have those properties"
                .x(() =>
                {
                    Assert.Equal(target.IntProperty, src.IntProperty);
                    Assert.Equal(target.StringProperty, src.StringProperty);
                });
        }

        [Scenario]
        public void AdditionalProperty(
            SampleClass src,
            dynamic target)
        {
            "Given an instance of a class with two properties"
                .x(() =>
                {
                    src = new SampleClass
                    {
                        IntProperty = 6,
                        StringProperty = "something"
                    };
                });

            "When I assign it to a new object with an extra property"
                .x(() => { target = Objectify.Assign(new { AnotherProperty = "something else" }, src); });

            "Then that object should have those properties"
                .x(() =>
                {
                    Assert.Equal(target.IntProperty, src.IntProperty);
                    Assert.Equal(target.StringProperty, src.StringProperty);
                    Assert.Equal(target.AnotherProperty, "something else");
                });
        }


        [Scenario]
        public void RightToLeftPrecedence(
            SampleClass one,
            SampleClass two,
            dynamic target)
        {
            "Given an instance of a class with two properties set to variations of one"
                .x(() => one = new SampleClass
                {
                    IntProperty = 1,
                    StringProperty = "one"
                });

            "And another instance with two properties set to variations of two"
                .x(() =>
                {
                    two = new SampleClass
                    {
                        IntProperty = 2,
                        StringProperty = "two"
                    };
                });

            "When I apply assign them giving one precedence"
                .x(() => { target = Objectify.Assign(new { }, two, one); });

            "Then the output should contain the properties of the right most parameter"
                .x(() =>
                {
                    Assert.Equal(target.IntProperty, one.IntProperty);
                    Assert.Equal(target.StringProperty, one.StringProperty);
                });
        }


    }
}
