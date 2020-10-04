using Business.PayOne.Helpers;

namespace Business.PayOne.Model
{
    public abstract class ModelBase
    {
        protected void Decode(NvCodec decoder)
        {
            var t = this.GetType();
            foreach (var prop in t.GetProperties())
            {
                var name = prop.Name.ToLower();
                var value = decoder[name];
                if (value != null)
                    prop.SetValue(this, value, null);
            }
        }
    }
}