namespace Cache.DTOs
{
    public class ProcessInfo
    {
        public ProcessInfo(int id, string name, string value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Process Id. It is needed to compare different processes with the same name.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Process name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Process performance value
        /// </summary>
        public string Value { get; set; }
    }
}
