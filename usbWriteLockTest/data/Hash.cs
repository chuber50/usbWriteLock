using System.ComponentModel;

namespace usbWriteLockTest.data
{
    public class Hash
    {
        public Hash(int index, string hashCode)
        {
            this.index = index;
            this.hashCode = hashCode;
        }

        [DisplayName("Index")]
        public int index { get; set; }

        [DisplayName("Hash Code")]
        public string hashCode { get; set; }

    }
}
