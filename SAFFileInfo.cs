namespace PizzaFrenzySAFExtractor
{
    public class SAFFileInfo
    {
        /// <summary>
        /// Address (bytes count from beginning of the file) with file data.
        /// </summary>
        public int DataAddress;
        
        /// <summary>
        /// Data size in bytes.
        /// </summary>
        public int DataSize;
        
        /// <summary>
        /// 16 bytes of unknown information.
        /// </summary>
        public byte[] UnknownInformation;
        
        /// <summary>
        /// Length of the file path string, including zero symbol.
        /// </summary>
        public short PathLength;
        
        /// <summary>
        /// File path string, excluding zero symbol.
        /// </summary>
        public string Path;
    }
}