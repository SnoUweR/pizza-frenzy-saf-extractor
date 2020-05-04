namespace PizzaFrenzySAFExtractor
{
    public class SAFHeader
    {
        /// <summary>
        /// Contains ASCII symbols "FFAS" (46, 46, 41, 53).
        /// </summary>
        public char[] ChunkId;

        /// <summary>
        /// Total files lists in the SAF.
        /// In the Pizza Pizzy there is only one.
        /// </summary>
        public int TotalFileLists;

        /// <summary>
        /// File Lists addresses.
        /// </summary>
        public int[] FileListsAddresses;
    }
}