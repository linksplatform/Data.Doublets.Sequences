namespace Platform::Data::Doublets::Sequences::Tests
{
    TEST_CLASS(ILinksExtensionsTests)
    {
        public: TEST_METHOD(FormatTest)
        {
            using (auto scope = TempLinksTestScope())
            {
                auto links = scope.Links;
                auto link = links.Create();
                auto linkString = links.Format(link);
                Assert::AreEqual("(1: 1 1)", linkString);
            }
        }
    };
}
