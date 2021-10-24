namespace Platform::Data::Doublets::Incrementers
{
    template <typename ...> class UnaryNumberIncrementer;
    template <typename TLink> class UnaryNumberIncrementer<TLink> : public LinksOperatorBase<TLink>, IIncrementer<TLink>
    {
        private: TLink _unaryOne = 0;

        public: UnaryNumberIncrementer(ILinks<TLink> &links, TLink unaryOne) : base(links) { return _unaryOne = unaryOne; }

        public: TLink Increment(TLink unaryNumber)
        {
            auto links = _links;
            if (unaryNumber == _unaryOne)
            {
                return links.GetOrCreate(_unaryOne, _unaryOne);
            }
            auto source = links.GetSource(unaryNumber);
            auto target = links.GetTarget(unaryNumber);
            if (source == target)
            {
                return links.GetOrCreate(unaryNumber, _unaryOne);
            }
            else
            {
                return links.GetOrCreate(source, target + 1);
            }
        }
    };
}
