namespace P5.IdentityServer3.Cassandra
{
    public class PropertyValue
    {
        /// <summary>
        /// the name of the property
        /// </summary>
        public string   Name { get; set; }

        /// <summary>
        /// The Value that matches the type
        /// </summary>
        public object Value { get; set; }
    }
}