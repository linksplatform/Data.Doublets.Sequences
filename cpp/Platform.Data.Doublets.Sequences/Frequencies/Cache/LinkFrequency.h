namespace Platform::Data::Doublets::Sequences::Frequencies::Cache
{
    template <typename ...> class LinkFrequency;
    template <typename TLink> class LinkFrequency<TLink>
    {
        public: inline TLink Frequency;
        public: inline TLink Link;

        public: LinkFrequency(TLink frequency, TLink link)
        {
            Frequency = frequency;
            Link = link;
        }

        public: LinkFrequency() { }

        public: void IncrementFrequency() { Frequency = Arithmetic<TLink>.Increment(Frequency); }

        public: void DecrementFrequency() { Frequency = Arithmetic<TLink>.Decrement(Frequency); }

        public: operator std::string() const { return std::string("F: ").append(Platform::Converters::To<std::string>(Frequency)).append(", L: ").append(Platform::Converters::To<std::string>(Link)).append(""); }

        public: friend std::ostream & operator <<(std::ostream &out, const LinkFrequency<TLink> &obj) { return out << (std::string)obj; }
    };
}
