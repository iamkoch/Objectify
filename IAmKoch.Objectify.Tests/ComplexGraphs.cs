using AutoFixture.Xunit2;
using Xbehave;

namespace IAmKoch.ObjectAssign.Tests
{
    public class ComplexGraphs
    {
        [Scenario]
        [AutoData]
        public void AssignTypesInComplexGraph(
            ComplexClass src,
            ComplexClass toAssign)
        {

            "Given a complex source graph"
                .x(() =>
                {

                });
        }
    }
}
