using System.Collections;
using System.Collections.Generic;

namespace Titan.Publisher.InTextAds
{
    public class MappedInTextAdvertCollection : IEnumerable<MappedInTextAdvert>
    {
        private List<MappedInTextAdvert> list { get; set; }

        public MappedInTextAdvertCollection()
        {
            list = new List<MappedInTextAdvert>();
        }

        public MappedInTextAdvertCollection Add(MappedInTextAdvert ad)
        {
            list.Add(ad);
            return this;
        }

        public IEnumerator<MappedInTextAdvert> GetEnumerator()
        {
            return list.GetEnumerator();

        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}