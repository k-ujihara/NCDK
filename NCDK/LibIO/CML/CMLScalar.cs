namespace NCDK.LibIO.CML
{
    public partial class CMLScalar
    {
        public CMLScalar(int value)
            : this()
        {
            SetValue(value);
        }

        public CMLScalar(double value)
            : this()
        {
            SetValue(value);
        }

        public CMLScalar(bool value)
            : this()
        {
            SetValue(value);
        }

        public void SetValue(bool scalar)
        {
            Value = scalar.ToString().ToLowerInvariant();
            DataType = "xsd:bool";
        }

        public void SetValue(double scalar)
        {
            Value = scalar.ToString();
            DataType = "xsd:double";
        }

        /// <summary>
        /// sets value to int.. updates dataType.
        /// </summary>
        /// <param name="scalar"></param>
        public void SetValue(int scalar)
        {
            Add(scalar.ToString());
            DataType = "xsd:integer";
        }
    }


}
