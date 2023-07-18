namespace DruidsCornerUnitTests.TestHelpers
{

    public static class TestDatabaseFinder
    {
        public static DirectoryInfo? FindTestDatabase()
        {
            var testDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            // Recurse until base project folder (...)
            while(testDir != null && testDir.Name != nameof(DruidsCornerAPI))
            {
                testDir = testDir.Parent;
            }

            var unitTestFolder = testDir!.GetDirectories("DruidsCornerUnitTests").First()!;
            return unitTestFolder.GetDirectories("TestDatabase").First();
        }
    }


}
