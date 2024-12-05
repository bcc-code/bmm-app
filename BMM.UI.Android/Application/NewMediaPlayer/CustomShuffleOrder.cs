using System.Collections.Generic;
using System.Linq;
using BMM.Core.NewMediaPlayer;
using Com.Google.Android.Exoplayer2.Source;
using Java.Util;
using Random = System.Random;

namespace BMM.UI.Droid.Application.NewMediaPlayer.Service
{
    /// <summary>
    /// Custom implementation of IShuffleOrder that allows to play all tracks of a list when shuffling.
    /// Unfortunately when enabling shuffle for an existing queue it's not sure all tracks are played.
    /// </summary>
    public class CustomShuffleOrder : Java.Lang.Object, IShuffleOrder
    {
        private readonly int[] _shuffled;
        private readonly int[] _indexInShuffled;
        private readonly Random _random;
        private const int IndexUnset = -1;

        private readonly int _temporaryStartIndex;
        private bool _useTemporaryStartIndex;

        public CustomShuffleOrder() : this(0, new Random())
        { }

        public CustomShuffleOrder(int startIndex, int length) : this(length, new Random())
        {
            _temporaryStartIndex = startIndex;
            _useTemporaryStartIndex = true;
        }

        private CustomShuffleOrder(int length, Random random) : this(0, false, CreateShuffledList(length, random), random)
        { }

        private CustomShuffleOrder(int temporaryStartIndex, bool useTemporaryStartIndex, int length, Random random) :
            this(temporaryStartIndex, useTemporaryStartIndex, CreateShuffledList(length, random), random)
        { }

        private CustomShuffleOrder(int temporaryStartIndex, bool useTemporaryStartIndex, int[] shuffled, Random random)
        {
            _temporaryStartIndex = temporaryStartIndex;
            _useTemporaryStartIndex = useTemporaryStartIndex;

            _shuffled = shuffled;
            _random = random;
            _indexInShuffled = new int[shuffled.Length];
            for (int i = 0; i < shuffled.Length; i++)
            {
                _indexInShuffled[shuffled[i]] = i;
            }
        }

        public int GetNextIndex(int index)
        {
            var shuffledIndex = _indexInShuffled[index];
            return ++shuffledIndex < _shuffled.Length ? _shuffled[shuffledIndex] : IndexUnset;
        }

        public int GetPreviousIndex(int index)
        {
            var shuffledIndex = _indexInShuffled[index];
            return --shuffledIndex >= 0 ? _shuffled[shuffledIndex] : IndexUnset;
        }

        public int FirstIndex => _shuffled.Length > 0 ? _shuffled[0] : IndexUnset;

        public int LastIndex => _shuffled.Length > 0 ? _shuffled[_shuffled.Length - 1] : IndexUnset;

        public int Length => _shuffled.Length;

        public IShuffleOrder CloneAndInsert(int insertionIndex, int insertionCount)
        {
            if (_useTemporaryStartIndex && insertionIndex == 0 && _temporaryStartIndex < insertionCount)
            {
                _useTemporaryStartIndex = false;
                var list = CreateShuffledListStartingWith(_temporaryStartIndex, insertionCount, _random);
                return new CustomShuffleOrder(_temporaryStartIndex, _useTemporaryStartIndex, list.ToArray(), new Random(_random.Next(int.MaxValue)));
            }

            int[] insertionPoints = new int[insertionCount];
            int[] insertionValues = new int[insertionCount];
            for (int i = 0; i < insertionCount; i++) {
                insertionPoints[i] = _random.Next(_shuffled.Length + 1);
                int swapIndex = _random.Next(i + 1);
                insertionValues[i] = insertionValues[swapIndex];
                insertionValues[swapIndex] = i + insertionIndex;
            }
            Arrays.Sort(insertionPoints);
            int[] newShuffled = new int[_shuffled.Length + insertionCount];
            int indexInOldShuffled = 0;
            int indexInInsertionList = 0;
            for (int i = 0; i < _shuffled.Length + insertionCount; i++) {
                if (indexInInsertionList < insertionCount
                    && indexInOldShuffled == insertionPoints[indexInInsertionList]) {
                    newShuffled[i] = insertionValues[indexInInsertionList++];
                } else {
                    newShuffled[i] = _shuffled[indexInOldShuffled++];
                    if (newShuffled[i] >= insertionIndex) {
                        newShuffled[i] += insertionCount;
                    }
                }
            }

            return new CustomShuffleOrder(_temporaryStartIndex, _useTemporaryStartIndex, newShuffled, new Random(_random.Next(int.MaxValue)));
        }

        public IShuffleOrder CloneAndRemove(int indexFrom, int indexToExclusive)
        {
            int[] newShuffled = new int[_shuffled.Length - (indexToExclusive - indexFrom)];
            int j = 0;
            
            for (int i = 0; i < _shuffled.Length; i++)
            {
                if (i >= indexFrom && i < indexToExclusive)
                    continue;
                
                newShuffled[j++] = _shuffled[i];
            }

            return new CustomShuffleOrder(_temporaryStartIndex, _useTemporaryStartIndex, newShuffled, new Random(_random.Next(int.MaxValue)));
        }

        public IShuffleOrder CloneAndClear()
        {
            return new CustomShuffleOrder(_temporaryStartIndex, _useTemporaryStartIndex, 0, new Random(_random.Next(int.MaxValue)));
        }

        private static int[] CreateShuffledListStartingWith(int startIndex, int length, Random random)
        {
            var list = IndexList(1, length);
            
            if (startIndex > 0 && startIndex <= list.Count)
                list[startIndex - 1] = 0;
            
            ShuffleableQueue.ShuffleList(list, random);
            return list.Prepend(startIndex).ToArray();
        }

        private static int[] CreateShuffledList(int length, Random random)
        {
            var list = IndexList(0, length);
            ShuffleableQueue.ShuffleList(list, random);
            return list.ToArray();
        }

        private static IList<int> IndexList(int from, int to)
        {
            var list = new List<int>();
            for (int i = from; i < to; i++)
            {
                list.Add(i);
            }

            return list;
        }
    }
}