using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Numerics;
using Platform.Collections;
using Platform.Collections.Lists;
using Platform.Collections.Stacks;
using Platform.Threading.Synchronization;
using Platform.Data.Doublets.Sequences.Walkers;
using Platform.Delegates;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Sequences
{
  /// <summary>
  /// Представляет хранилище связей переменной длины.
  /// </summary>
     /// <remarks>
     /// Обязательно реализовать атомарность каждого публичного метода.
     ///
     /// TODO:
     ///
     /// !!! Повышение вероятности повторного использования групп (подпоследовательностей),
     /// через естественную группировку по unicode типам, все whitespace вместе, все символы вместе, все числа вместе и т.п.
     /// + использовать ровно сбалансированный вариант, чтобы уменьшать вложенность (глубину графа)
     ///
     /// x*y - найти все связи между, в последовательностях любой формы, если не стоит ограничитель на то, что является последовательностью, а что нет,
     /// то находятся любые структуры связей, которые содержат эти элементы именно в таком порядке.
     ///
     /// Рост последовательности слева и справа.
     /// Поиск со звёздочкой.
     /// URL, PURL - реестр используемых во вне ссылок на ресурсы,
     /// так же проблема может быть решена при реализации дистанционных триггеров.
     /// Нужны ли уникальные указатели вообще?
     /// Что если обращение к информации будет происходить через содержимое всегда?
     ///
     /// Писать тесты.
     ///
     ///
     /// Можно убрать зависимость от конкретной реализации Links,
     /// на зависимость от абстрактного элемента, который может быть представлен несколькими способами.
     ///
     /// Можно ли как-то сделать один общий интерфейс
     ///
     ///
     /// Блокчейн и/или гит для распределённой записи транзакций.
     ///
     /// </remarks>
    public partial class Sequences<TLinkAddress> : ILinks<TLinkAddress> where TLinkAddress : struct, IUnsignedNumber<TLinkAddress>, IComparisonOperators<TLinkAddress, TLinkAddress, bool> // IList<string>, IList<TLinkAddress[]> (после завершения реализации Sequences)
     {
      /// <summary>Возвращает значение TLinkAddress, обозначающее любое количество связей.</summary>
        public static readonly TLinkAddress ZeroOrMany = TLinkAddress.CreateSaturating(ulong.MaxValue);


      /// <summary>
      /// <para>
      /// Gets the options value.
      /// </para>
      /// <para></para>
      /// </summary>
        public SequencesOptions<TLinkAddress> Options { get; }
      /// <summary>
      /// <para>
      /// Gets the links value.
      /// </para>
      /// <para></para>
      /// </summary>
        public SynchronizedLinks<TLinkAddress> Links { get; }
        private readonly ISynchronization _sync;

      /// <summary>
      /// <para>
      /// Gets the constants value.
      /// </para>
      /// <para></para>
      /// </summary>
        public LinksConstants<TLinkAddress> Constants { get; }


      /// <summary>
      /// <para>
      /// Initializes a new <see cref="Sequences{TLinkAddress}"/> instance.
      /// </para>
      /// <para></para>
      /// </summary>
      /// <param name="links">
      /// <para>A links.</para>
      /// <para></para>
      /// </param>
      /// <param name="options">
      /// <para>A options.</para>
      /// <para></para>
      /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sequences(SynchronizedLinks<TLinkAddress> links, SequencesOptions<TLinkAddress> options)
        {
            Links = links;
            _sync = links.SyncRoot;
            Options = options;
            Options.ValidateOptions();
            Options.InitOptions(Links);
            Constants = links.Constants;
        }


      /// <summary>
      /// <para>
      /// Initializes a new <see cref="Sequences{TLinkAddress}"/> instance.
      /// </para>
      /// <para></para>
      /// </summary>
      /// <param name="links">
      /// <para>A links.</para>
      /// <para></para>
      /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Sequences(SynchronizedLinks<TLinkAddress> links) : this(links, new SequencesOptions<TLinkAddress>()) { }

         /// <summary>
         /// <para>
         /// Determines whether this instance is sequence.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="sequence">
         /// <para>The sequence.</para>
         /// <para></para>
         /// </param>
         /// <returns>
         /// <para>The bool</para>
         /// <para></para>
         /// </returns>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public bool IsSequence(TLinkAddress sequence)
         {
             return _sync.DoRead(() =>
             {
                 if (Options.UseSequenceMarker)
                 {
                     return Options.MarkedSequenceMatcher.IsMatched(sequence);
                 }
                 return !Links.Unsync.IsPartialPoint(sequence);
             });
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress GetSequenceByElements(TLinkAddress sequence)
         {
             if (Options.UseSequenceMarker)
             {
                 return Links.SearchOrDefault(Options.SequenceMarkerLink, sequence);
             }
             return sequence;
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress GetSequenceElements(TLinkAddress sequence)
         {
             if (Options.UseSequenceMarker)
             {
                 var linkContents = new Link<TLinkAddress>(Links.GetLink(sequence));
                 if (linkContents.Source == Options.SequenceMarkerLink)
                 {
                     return linkContents.Target;
                 }
                 if (linkContents.Target == Options.SequenceMarkerLink)
                 {
                     return linkContents.Source;
                 }
             }
             return sequence;
         }

         #region Count

         /// <summary>
         /// <para>
         /// Counts the restriction.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="restriction">
         /// <para>The restriction.</para>
         /// <para></para>
         /// </param>
         /// <exception cref="NotImplementedException">
         /// <para></para>
         /// <para></para>
         /// </exception>
         /// <returns>
         /// <para>The link index</para>
         /// <para></para>
         /// </returns>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public TLinkAddress Count(IList<TLinkAddress>? restriction)
         {
             if (restriction.IsNullOrEmpty())
             {
                 return Links.Count(Constants.Any, Options.SequenceMarkerLink, Constants.Any);
             }
             if (restriction.Count == 1) // Первая связь это адрес
             {
                 var sequenceIndex = restriction[0];
                 if (sequenceIndex == Constants.Null)
                 {
                     return TLinkAddress.Zero;
                 }
                 if (sequenceIndex == Constants.Any)
                 {
                     return Count(null);
                 }
                 if (Options.UseSequenceMarker)
                 {
                     return Links.Count(Constants.Any, Options.SequenceMarkerLink, sequenceIndex);
                 }
                 return Links.Exists(sequenceIndex) ? TLinkAddress.One : TLinkAddress.Zero;
             }
             throw new NotImplementedException();
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress CountUsages(params TLinkAddress[] restriction)
         {
             if (restriction.Length == 0)
             {
                 return TLinkAddress.Zero;
             }
             if (restriction.Length == 1) // Первая связь это адрес
             {
                 if (restriction[0] == Constants.Null)
                 {
                     return TLinkAddress.Zero;
                 }
                 var any = Constants.Any;
                 if (Options.UseSequenceMarker)
                 {
                     var elementsLink = GetSequenceElements(restriction[0]);
                     var sequenceLink = GetSequenceByElements(elementsLink);
                     if (sequenceLink != Constants.Null)
                     {
                         return Links.Count(any, sequenceLink) + Links.Count(any, elementsLink) - TLinkAddress.One;
                     }
                     return Links.Count(any, elementsLink);
                 }
                 return Links.Count(any, restriction[0]);
             }
             throw new NotImplementedException();
         }

         #endregion

         #region Create

         /// <summary>
         /// <para>
         /// Creates the restriction.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="restriction">
         /// <para>The restriction.</para>
         /// <para></para>
         /// </param>
         /// <returns>
         /// <para>The link index</para>
         /// <para></para>
         /// </returns>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public TLinkAddress Create(IList<TLinkAddress>? restriction, WriteHandler<TLinkAddress>? handler = null)
         {
             return _sync.DoWrite(() =>
             {
                 if (restriction.IsNullOrEmpty())
                 {
                     return Constants.Null;
                 }
                 Links.EnsureInnerReferenceExists(restriction, nameof(restriction));
                 return CreateCore(restriction);
             });
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress CreateCore(IList<TLinkAddress>? restriction)
         {
             TLinkAddress[] sequence = restriction.SkipFirst();
             if (Options.UseIndex)
             {
                 Options.Index.Add(sequence);
             }
             var sequenceRoot = default(TLinkAddress);
             if (Options.EnforceSingleSequenceVersionOnWriteBasedOnExisting)
             {
                 var matches = Each(restriction);
                 if (matches.Count > 0)
                 {
                     sequenceRoot = matches[0];
                 }
             }
             else if (Options.EnforceSingleSequenceVersionOnWriteBasedOnNew)
             {
                 return CompactCore(sequence);
             }
             if (sequenceRoot == default)
             {
                 sequenceRoot = Options.LinksToSequenceConverter.Convert(sequence);
             }
             if (Options.UseSequenceMarker)
             {
                 return Links.Unsync.GetOrCreate(Options.SequenceMarkerLink, sequenceRoot);
             }
             return sequenceRoot; // Возвращаем корень последовательности (т.е. сами элементы)
         }

         #endregion

         #region Each

         /// <summary>
         /// <para>
         /// Eaches the sequence.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="sequence">
         /// <para>The sequence.</para>
         /// <para></para>
         /// </param>
         /// <returns>
         /// <para>The results.</para>
         /// <para></para>
         /// </returns>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public List<TLinkAddress> Each(IList<TLinkAddress> sequence)
         {
             var results = new List<TLinkAddress>();
             var filler = new ListFiller<TLinkAddress, TLinkAddress>(results, Constants.Continue);
             Each(sequence, filler.AddFirstAndReturnConstant);
             return results;
         }

         /// <summary>
         /// <para>
         /// Eaches the handler.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="handler">
         /// <para>The handler.</para>
         /// <para></para>
         /// </param>
         /// <param name="restriction">
         /// <para>The restriction.</para>
         /// <para></para>
         /// </param>
         /// <exception cref="NotImplementedException">
         /// <para></para>
         /// <para></para>
         /// </exception>
         /// <returns>
         /// <para>The link index</para>
         /// <para></para>
         /// </returns>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public TLinkAddress Each(IList<TLinkAddress>? restriction, ReadHandler<TLinkAddress>? handler)
         {
             return _sync.DoRead(() =>
             {
                 if (restriction.IsNullOrEmpty())
                 {
                     return Constants.Continue;
                 }
                 Links.EnsureInnerReferenceExists(restriction, nameof(restriction));
                 if (restriction.Count == 1)
                 {
                     var link = restriction[0];
                     var any = Constants.Any;
                     if (link == any)
                     {
                         if (Options.UseSequenceMarker)
                         {
                             return Links.Unsync.Each(new Link<TLinkAddress>(any, Options.SequenceMarkerLink, any), handler);
                         }
                         else
                         {
                             return Links.Unsync.Each(handler, new Link<TLinkAddress>(any, any, any));
                         }
                     }
                     if (Options.UseSequenceMarker)
                     {
                         var sequenceLinkValues = Links.Unsync.GetLink(link);
                         if (sequenceLinkValues[Constants.SourcePart] == Options.SequenceMarkerLink)
                         {
                             link = sequenceLinkValues[Constants.TargetPart];
                         }
                     }
                     var sequence = Options.Walker.Walk(link).ToArray().ShiftRight();
                     sequence[0] = link;
                     return handler(sequence);
                 }
                 else if (restriction.Count == 2)
                 {
                     throw new NotImplementedException();
                 }
                 else if (restriction.Count == 3)
                 {
                     return Links.Unsync.Each(restriction, handler);
                 }
                 else
                 {
                     var sequence = restriction.SkipFirst();
                     if (Options.UseIndex && !Options.Index.MightContain(sequence))
                     {
                         return Constants.Break;
                     }
                     return EachCore(sequence, handler);
                 }
             });
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress EachCore(IList<TLinkAddress> values, ReadHandler<TLinkAddress> handler)
         {
             var matcher = new Matcher(this, values, new HashSet<TLinkAddress>(), handler);
             // TODO: Find out why matcher.HandleFullMatched executed twice for the same sequence Id.
             Func<IList<TLinkAddress>, TLinkAddress> innerHandler = Options.UseSequenceMarker ? (Func<IList<TLinkAddress>, TLinkAddress>)matcher.HandleFullMatchedSequence : matcher.HandleFullMatched;
             //if (sequence.Length >= 2)
             if (StepRight(innerHandler, values[0], values[1]) != Constants.Continue)
             {
                 return Constants.Break;
             }
             var last = values.Count - 2;
             for (var i = 1; i < last; i++)
             {
                 if (PartialStepRight(innerHandler, values[i], values[i + 1]) != Constants.Continue)
                 {
                     return Constants.Break;
                 }
             }
             if (values.Count >= 3)
             {
                 if (StepLeft(innerHandler, values[values.Count - 2], values[values.Count - 1]) != Constants.Continue)
                 {
                     return Constants.Break;
                 }
             }
             return Constants.Continue;
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress PartialStepRight(Func<IList<TLinkAddress>, TLinkAddress> handler, TLinkAddress left, TLinkAddress right)
         {
             return Links.Unsync.Each(doublet =>
             {
                 var doubletIndex = doublet[Constants.IndexPart];
                 if (StepRight(handler, doubletIndex, right) != Constants.Continue)
                 {
                     return Constants.Break;
                 }
                 if (left != doubletIndex)
                 {
                     return PartialStepRight(handler, doubletIndex, right);
                 }
                 return Constants.Continue;
             }, new Link<TLinkAddress>(Constants.Any, Constants.Any, left));
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress StepRight(Func<IList<TLinkAddress>, TLinkAddress> handler, TLinkAddress left, TLinkAddress right) => Links.Unsync.Each(rightStep => TryStepRightUp(handler, right, rightStep[Constants.IndexPart]), new Link<TLinkAddress>(Constants.Any, left, Constants.Any));
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress TryStepRightUp(Func<IList<TLinkAddress>, TLinkAddress> handler, TLinkAddress right, TLinkAddress stepFrom)
         {
             var upStep = stepFrom;
             var firstSource = Links.Unsync.GetTarget(upStep);
             while (firstSource != right && firstSource != upStep)
             {
                 upStep = firstSource;
                 firstSource = Links.Unsync.GetSource(upStep);
             }
             if (firstSource == right)
             {
                 return handler(new LinkAddress<TLinkAddress>(stepFrom));
             }
             return Constants.Continue;
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress StepLeft(Func<IList<TLinkAddress>, TLinkAddress> handler, TLinkAddress left, TLinkAddress right) => Links.Unsync.Each(leftStep => TryStepLeftUp(handler, left, leftStep[Constants.IndexPart]), new Link<TLinkAddress>(Constants.Any, Constants.Any, right));
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress TryStepLeftUp(Func<IList<TLinkAddress>, TLinkAddress> handler, TLinkAddress left, TLinkAddress stepFrom)
         {
             var upStep = stepFrom;
             var firstTarget = Links.Unsync.GetSource(upStep);
             while (firstTarget != left && firstTarget != upStep)
             {
                 upStep = firstTarget;
                 firstTarget = Links.Unsync.GetTarget(upStep);
             }
             if (firstTarget == left)
             {
                 return handler(new LinkAddress<TLinkAddress>(stepFrom));
             }
             return Constants.Continue;
         }

         #endregion

         #region Update

         /// <summary>
         /// <para>
         /// Updates the restriction.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="restriction">
         /// <para>The restriction.</para>
         /// <para></para>
         /// </param>
         /// <param name="substitution">
         /// <para>The substitution.</para>
         /// <para></para>
         /// </param>
         /// <returns>
         /// <para>The link index</para>
         /// <para></para>
         /// </returns>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public TLinkAddress Update(IList<TLinkAddress>? restriction, IList<TLinkAddress>? substitution, WriteHandler<TLinkAddress> handler)
         {
             var sequence = restriction.SkipFirst();
             var newSequence = substitution.SkipFirst();
             if (sequence.IsNullOrEmpty() && newSequence.IsNullOrEmpty())
             {
                 return Constants.Null;
             }
             if (sequence.IsNullOrEmpty())
             {
                 return Create(substitution);
             }
             if (newSequence.IsNullOrEmpty())
             {
                 Delete(restriction);
                 return Constants.Null;
             }
             return _sync.DoWrite(() =>
             {
                 Links.EnsureLinkExists(sequence);
                 Links.EnsureLinkExists(newSequence);
                 return UpdateCore(sequence, newSequence, handler);
             });
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private TLinkAddress UpdateCore(IList<TLinkAddress> sequence, IList<TLinkAddress> newSequence, WriteHandler<TLinkAddress>? handler = null)
         {
             TLinkAddress bestVariant;
             if (Options.EnforceSingleSequenceVersionOnWriteBasedOnNew && !sequence.EqualTo(newSequence))
             {
                 bestVariant = CompactCore(newSequence);
             }
             else
             {
                 bestVariant = CreateCore(newSequence);
             }
             // TODO: Check all options only ones before loop execution
             // Возможно нужно две версии Each, возвращающий фактические последовательности и с маркером,
             // или возможно даже возвращать и тот и тот вариант. С другой стороны все варианты можно получить имея только фактические последовательности.
             foreach (var variant in Each(sequence))
             {
                 if (variant != bestVariant)
                 {
                     UpdateOneCore(variant, bestVariant, handler);
                 }
             }
             return bestVariant;
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private void UpdateOneCore(TLinkAddress sequence, TLinkAddress newSequence, WriteHandler<TLinkAddress>? handler)
         {
             if (Options.UseGarbageCollection)
             {
                 var sequenceElements = GetSequenceElements(sequence);
                 var sequenceElementsContents = new Link<TLinkAddress>(Links.GetLink(sequenceElements));
                 var sequenceLink = GetSequenceByElements(sequenceElements);
                 var newSequenceElements = GetSequenceElements(newSequence);
                 var newSequenceLink = GetSequenceByElements(newSequenceElements);
                 if (Options.UseCascadeUpdate || CountUsages(sequence).Equals(TLinkAddress.Zero))
                 {
                     if (sequenceLink != Constants.Null)
                     {
                         Links.Unsync.MergeAndDelete(sequenceLink, newSequenceLink);
                     }
                     Links.Unsync.MergeAndDelete(sequenceElements, newSequenceElements);
                 }
                 ClearGarbage(sequenceElementsContents.Source);
                 ClearGarbage(sequenceElementsContents.Target);
             }
             else
             {
                 if (Options.UseSequenceMarker)
                 {
                     var sequenceElements = GetSequenceElements(sequence);
                     var sequenceLink = GetSequenceByElements(sequenceElements);
                     var newSequenceElements = GetSequenceElements(newSequence);
                     var newSequenceLink = GetSequenceByElements(newSequenceElements);
                     if (Options.UseCascadeUpdate || CountUsages(sequence).Equals(TLinkAddress.Zero))
                     {
                         if (sequenceLink != Constants.Null)
                         {
                             Links.Unsync.MergeAndDelete(sequenceLink, newSequenceLink);
                         }
                         Links.Unsync.MergeAndDelete(sequenceElements, newSequenceElements);
                     }
                 }
                 else
                 {
                     if (Options.UseCascadeUpdate || CountUsages(sequence).Equals(TLinkAddress.Zero))
                     {
                         Links.Unsync.MergeAndDelete(sequence, newSequence);
                     }
                 }
             }
         }

         #endregion

         #region Delete

         /// <summary>
         /// <para>
         /// Deletes the restriction.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="restriction">
         /// <para>The restriction.</para>
         /// <para></para>
         /// </param>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public TLinkAddress Delete(IList<TLinkAddress>? restriction, WriteHandler<TLinkAddress>? handler = null)
         {
             return _sync.DoWrite(() =>
             {
                 var sequence = restriction.SkipFirst();
                 // TODO: Check all options only ones before loop execution
                 foreach (var linkToDelete in Each(sequence))
                 {
                     DeleteOneCore(linkToDelete);
                 }
                 return Constants.Null;
             });
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         private void DeleteOneCore(TLinkAddress link)
         {
             if (Options.UseGarbageCollection)
             {
                 var sequenceElements = GetSequenceElements(link);
                 var sequenceElementsContents = new Link<TLinkAddress>(Links.GetLink(sequenceElements));
                 var sequenceLink = GetSequenceByElements(sequenceElements);
                 if (Options.UseCascadeDelete || CountUsages(link).Equals(TLinkAddress.Zero))
                 {
                     if (sequenceLink != Constants.Null)
                     {
                         Links.Unsync.Delete(sequenceLink);
                     }
                     Links.Unsync.Delete(link);
                 }
                 ClearGarbage(sequenceElementsContents.Source);
                 ClearGarbage(sequenceElementsContents.Target);
             }
             else
             {
                 if (Options.UseSequenceMarker)
                 {
                     var sequenceElements = GetSequenceElements(link);
                     var sequenceLink = GetSequenceByElements(sequenceElements);
                     if (Options.UseCascadeDelete || CountUsages(link).Equals(TLinkAddress.Zero))
                     {
                         if (sequenceLink != Constants.Null)
                         {
                             Links.Unsync.Delete(sequenceLink);
                         }
                         Links.Unsync.Delete(link);
                     }
                 }
                 else
                 {
                     if (Options.UseCascadeDelete || CountUsages(link).Equals(TLinkAddress.Zero))
                     {
                         Links.Unsync.Delete(link);
                     }
                 }
             }
         }

         #endregion

         #region Compactification

         /// <summary>
         /// <para>
         /// Compacts the all.
         /// </para>
         /// <para></para>
         /// </summary>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public void CompactAll()
         {
             _sync.DoWrite(() =>
             {
                 var sequences = Each((LinkAddress<TLinkAddress>)Constants.Any);
                 for (int i = 0; i < sequences.Count; i++)
                 {
                     var sequence = this.ToList(sequences[i]);
                     Compact(sequence.ShiftRight());
                 }
             });
         }

         /// <remarks>
         /// bestVariant можно выбирать по максимальному числу использований,
         /// но балансированный позволяет гарантировать уникальность (если есть возможность,
         /// гарантировать его использование в других местах).
         ///
         /// Получается этот метод должен игнорировать Options.EnforceSingleSequenceVersionOnWrite
         /// </remarks>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public TLinkAddress Compact(IList<TLinkAddress> sequence)
         {
             return _sync.DoWrite(() =>
             {
                 if (sequence.IsNullOrEmpty())
                 {
                     return Constants.Null;
                 }
                 Links.EnsureInnerReferenceExists(sequence, nameof(sequence));
                 return CompactCore(sequence);
             });
         }
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TLinkAddress CompactCore(IList<TLinkAddress> sequence) => UpdateCore(sequence, sequence, null);

         #endregion


         #region Walkers

         /// <summary>
         /// <para>
         /// Determines whether this instance each part.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <param name="handler">
         /// <para>The handler.</para>
         /// <para></para>
         /// </param>
         /// <param name="sequence">
         /// <para>The sequence.</para>
         /// <para></para>
         /// </param>
         /// <returns>
         /// <para>The bool</para>
         /// <para></para>
         /// </returns>
         [MethodImpl(MethodImplOptions.AggressiveInlining)]
         public bool EachPart(Func<TLinkAddress, bool> handler, TLinkAddress sequence)
         {
             return _sync.DoRead(() =>
             {
                 var links = Links.Unsync;
                 foreach (var part in Options.Walker.Walk(sequence))
                 {
                     if (!handler(part))
                     {
                         return false;
                     }
                 }
                 return true;
             });
         }

         /// <summary>
         /// <para>
         /// Represents the matcher.
         /// </para>
         /// <para></para>
         /// </summary>
         /// <seealso cref="RightSequenceWalker{TLinkAddress}"/>
         public class Matcher : RightSequenceWalker<TLinkAddress>
         {
             private readonly Sequences<TLinkAddress> _sequences;
             private readonly IList<TLinkAddress> _patternSequence;
             private readonly HashSet<TLinkAddress> _linksInSequence;
             private readonly HashSet<TLinkAddress> _results;
             private readonly ReadHandler<TLinkAddress> _stopableHandler;
             private readonly HashSet<TLinkAddress> _readAsElements;
             private int _filterPosition;

             /// <summary>
             /// <para>
             /// Initializes a new <see cref="Matcher"/> instance.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="sequences">
             /// <para>A sequences.</para>
             /// <para></para>
             /// </param>
             /// <param name="patternSequence">
             /// <para>A pattern sequence.</para>
             /// <para></para>
             /// </param>
             /// <param name="results">
             /// <para>A results.</para>
             /// <para></para>
             /// </param>
             /// <param name="stopableHandler">
             /// <para>A stopable handler.</para>
             /// <para></para>
             /// </param>
             /// <param name="readAsElements">
             /// <para>A read as elements.</para>
             /// <para></para>
             /// </param>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public Matcher(Sequences<TLinkAddress> sequences, IList<TLinkAddress> patternSequence, HashSet<TLinkAddress> results, ReadHandler<TLinkAddress> stopableHandler, HashSet<TLinkAddress>? readAsElements = null)
                 : base(sequences.Links.Unsync, new DefaultStack<TLinkAddress>())
             {
                 _sequences = sequences;
                 _patternSequence = patternSequence;
                 _linksInSequence = new HashSet<TLinkAddress>(patternSequence.Where(x => x != _links.Constants.Any && x != ZeroOrMany));
                 _results = results;
                 _stopableHandler = stopableHandler;
                 _readAsElements = readAsElements;
             }

             /// <summary>
             /// <para>
             /// Determines whether this instance is element.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="link">
             /// <para>The link.</para>
             /// <para></para>
             /// </param>
             /// <returns>
             /// <para>The bool</para>
             /// <para></para>
             /// </returns>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             protected override bool IsElement(TLinkAddress link) => base.IsElement(link) || (_readAsElements != null && _readAsElements.Contains(link)) || _linksInSequence.Contains(link);

             /// <summary>
             /// <para>
             /// Determines whether this instance full match.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="sequenceToMatch">
             /// <para>The sequence to match.</para>
             /// <para></para>
             /// </param>
             /// <returns>
             /// <para>The bool</para>
             /// <para></para>
             /// </returns>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public bool FullMatch(TLinkAddress sequenceToMatch)
             {
                 _filterPosition = 0;
                 foreach (var part in Walk(sequenceToMatch))
                 {
                     if (!FullMatchCore(part))
                     {
                         break;
                     }
                 }
                 return _filterPosition == _patternSequence.Count;
             }
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             private bool FullMatchCore(TLinkAddress element)
             {
                 if (_filterPosition == _patternSequence.Count)
                 {
                     _filterPosition = -2; // Длиннее чем нужно
                     return false;
                 }
                 if (_patternSequence[_filterPosition] != _links.Constants.Any
                  && element != _patternSequence[_filterPosition])
                 {
                     _filterPosition = -1;
                     return false; // Начинается/Продолжается иначе
                 }
                 _filterPosition++;
                 return true;
             }

             /// <summary>
             /// <para>
             /// Adds the full matched to results using the specified restriction.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="restriction">
             /// <para>The restriction.</para>
             /// <para></para>
             /// </param>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public void AddFullMatchedToResults(IList<TLinkAddress>? restriction)
             {
                 var sequenceToMatch = restriction[_links.Constants.IndexPart];
                 if (FullMatch(sequenceToMatch))
                 {
                     _results.Add(sequenceToMatch);
                 }
             }

             /// <summary>
             /// <para>
             /// Handles the full matched using the specified restriction.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="restriction">
             /// <para>The restriction.</para>
             /// <para></para>
             /// </param>
             /// <returns>
             /// <para>The link index</para>
             /// <para></para>
             /// </returns>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public TLinkAddress HandleFullMatched(IList<TLinkAddress>? restriction)
             {
                 var sequenceToMatch = restriction[_links.Constants.IndexPart];
                 if (FullMatch(sequenceToMatch) && _results.Add(sequenceToMatch))
                 {
                     return _stopableHandler(new LinkAddress<TLinkAddress>(sequenceToMatch));
                 }
                 return _links.Constants.Continue;
             }

             /// <summary>
             /// <para>
             /// Handles the full matched sequence using the specified restriction.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="restriction">
             /// <para>The restriction.</para>
             /// <para></para>
             /// </param>
             /// <returns>
             /// <para>The link index</para>
             /// <para></para>
             /// </returns>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public TLinkAddress HandleFullMatchedSequence(IList<TLinkAddress>? restriction)
             {
                 var sequenceToMatch = restriction[_links.Constants.IndexPart];
                 var sequence = _sequences.GetSequenceByElements(sequenceToMatch);
                 if (sequence != _links.Constants.Null && FullMatch(sequenceToMatch) && _results.Add(sequenceToMatch))
                 {
                     return _stopableHandler(new LinkAddress<TLinkAddress>(sequence));
                 }
                 return _links.Constants.Continue;
             }

             /// <remarks>
             /// TODO: Add support for LinksConstants.Any
             /// </remarks>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public bool PartialMatch(TLinkAddress sequenceToMatch)
             {
                 _filterPosition = -1;
                 foreach (var part in Walk(sequenceToMatch))
                 {
                     if (!PartialMatchCore(part))
                     {
                         break;
                     }
                 }
                 return _filterPosition == _patternSequence.Count - 1;
             }
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             private bool PartialMatchCore(TLinkAddress element)
             {
                 if (_filterPosition == (_patternSequence.Count - 1))
                 {
                     return false; // Нашлось
                 }
                 if (_filterPosition >= 0)
                 {
                     if (element == _patternSequence[_filterPosition + 1])
                     {
                         _filterPosition++;
                     }
                     else
                     {
                         _filterPosition = -1;
                     }
                 }
                 if (_filterPosition < 0)
                 {
                     if (element == _patternSequence[0])
                     {
                         _filterPosition = 0;
                     }
                 }
                 return true; // Ищем дальше
             }

             /// <summary>
             /// <para>
             /// Adds the partial matched to results using the specified sequence to match.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="sequenceToMatch">
             /// <para>The sequence to match.</para>
             /// <para></para>
             /// </param>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public void AddPartialMatchedToResults(TLinkAddress sequenceToMatch)
             {
                 if (PartialMatch(sequenceToMatch))
                 {
                     _results.Add(sequenceToMatch);
                 }
             }

             /// <summary>
             /// <para>
             /// Handles the partial matched using the specified restriction.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="restriction">
             /// <para>The restriction.</para>
             /// <para></para>
             /// </param>
             /// <returns>
             /// <para>The link index</para>
             /// <para></para>
             /// </returns>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public TLinkAddress HandlePartialMatched(IList<TLinkAddress>? restriction)
             {
                 var sequenceToMatch = restriction[_links.Constants.IndexPart];
                 if (PartialMatch(sequenceToMatch))
                 {
                     return _stopableHandler(new LinkAddress<TLinkAddress>(sequenceToMatch));
                 }
                 return _links.Constants.Continue;
             }

             /// <summary>
             /// <para>
             /// Adds the all partial matched to results using the specified sequences to match.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="sequencesToMatch">
             /// <para>The sequences to match.</para>
             /// <para></para>
             /// </param>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public void AddAllPartialMatchedToResults(IEnumerable<TLinkAddress> sequencesToMatch)
             {
                 foreach (var sequenceToMatch in sequencesToMatch)
                 {
                     if (PartialMatch(sequenceToMatch))
                     {
                         _results.Add(sequenceToMatch);
                     }
                 }
             }

             /// <summary>
             /// <para>
             /// Adds the all partial matched to results and read as elements using the specified sequences to match.
             /// </para>
             /// <para></para>
             /// </summary>
             /// <param name="sequencesToMatch">
             /// <para>The sequences to match.</para>
             /// <para></para>
             /// </param>
             [MethodImpl(MethodImplOptions.AggressiveInlining)]
             public void AddAllPartialMatchedToResultsAndReadAsElements(IEnumerable<TLinkAddress> sequencesToMatch)
             {
                 foreach (var sequenceToMatch in sequencesToMatch)
                 {
                     if (PartialMatch(sequenceToMatch))
                     {
                         _readAsElements.Add(sequenceToMatch);
                         _results.Add(sequenceToMatch);
                     }
                 }
             }
         }

        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearGarbage(TLinkAddress element)
        {
            // Implementation for garbage collection of individual elements
            // This method should be implemented based on the specific garbage collection strategy
        }
    }
}
