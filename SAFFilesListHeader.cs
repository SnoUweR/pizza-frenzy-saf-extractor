namespace PizzaFrenzySAFExtractor
{
    public class SAFFilesListHeader
    {
        /// <summary>
        /// Files List Index.
        /// </summary>
        public int Index;

        /// <summary>
        /// 16 bytes of unknown information.
        /// </summary>
        public byte[] UnknownInformation;

        /// <summary>
        /// Total files in the list.
        /// </summary>
        public int FilesCount;
    }
}