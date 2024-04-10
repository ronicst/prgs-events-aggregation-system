namespace MBDataTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            using (var db = new MBDataTestContext())
            {
                var aaa = db.FileDownloads.Count();
                foreach (var x in db.FileDownloads)
                {
                    Console.WriteLine(x.ToString());
                }
                Console.WriteLine("Found {0} file download events before test",
                                  aaa);
            }
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}