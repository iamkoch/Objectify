namespace IAmKoch.ObjectAssign.Tests
{
    public class SampleClass
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    public class SampleClassTwo
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
    }

    public class ComplexClass
    {
        public SampleClass Sample { get; set; }
        public SampleClassTwo Complex { get; set; }
    }
}
