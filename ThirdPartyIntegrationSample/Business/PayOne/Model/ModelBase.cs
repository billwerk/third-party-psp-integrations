using Business.PayOne.Helpers;

namespace Business.PayOne.Model
{
    public abstract class ModelBase
    {
        public void Encode(NvCodec collection)
        {
            var t = this.GetType();

            foreach (var prop in t.GetProperties())
            {
                var pt = prop.PropertyType;

                if (pt.BaseType == typeof(ModelBase))
                {
                    var obj = prop.GetValue(this, null) as ModelBase;
                    obj?.Encode(collection);
                }
                else
                {
                    var name = prop.Name.ToLower();
                    var value = (string) prop.GetValue(this, null);
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    collection[name] = value;
                }
            }
        }

        protected virtual void Decode(NvCodec decoder)
        {
            var t = this.GetType();
            foreach (var prop in t.GetProperties())
            {
                var name = prop.Name.ToLower();
                var value = decoder[name];
                if (value != null) prop.SetValue(this, value, null);
            }
        }
    }
}