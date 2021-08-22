using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences.HeightProviders
{
    /// <summary>
    /// <para>
    /// Defines the sequence height provider.
    /// </para>
    /// <para></para>
    /// </summary>
    /// <seealso cref="IProvider{TLink, TLink}"/>
    public interface ISequenceHeightProvider<TLink> : IProvider<TLink, TLink>
    {
    }
}
