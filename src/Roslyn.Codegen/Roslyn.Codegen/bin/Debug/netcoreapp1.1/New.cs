namespace CodeGenerationSample
{
    using System;

    public class Order : BaseEntity<Order>, IHaveIdentity
    {
        private bool canceled;
        public int Quantity
        {
            get;
            set;
        }

        public void MarkAsCanceled()
        {
            canceled = true;
        }
    }
}
namespace CodeGenerationSample
{
    using System;

    public class Order : BaseEntity<Order>, IHaveIdentity
    {
        private bool canceled;
        public int Quantity
        {
            get;
            set;
        }

        public void MarkAsCanceled()
        {
            canceled = true;
        }
    }
}