namespace ject
{
    public class TestDiGetter : DiGetter
    {
        private TestDiService _testDiService;
        
        public TestDiGetter(TestDiService testDiService)
        {
            _testDiService = testDiService;

            testDiService = new TestDiService();
        }
    }
}