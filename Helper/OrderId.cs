using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCFOS.Helper
{
    public class OrderId
    {
        private string _referenceType;
		private int _componentId;

		public int Id { get; set; }
		public string OrderId { get; set; }

		public OrderId() { }

		public OrderId(string referenceType, int componentId)
		{
			_referenceType = referenceType;
			_componentId = componentId;

			Id = 0;
            OrderId = null;
		}

		public OrderId(int id)
		{
			Id = id;
            OrderId = null;
		}

        public OrderId(int id, string orderId)
		{
			Id = id;
            OrderId = orderId;
		}

		public int OrderIdNextId()
		{
			Id += 1;
			return Id;
		}

		public string NextOrderId()
		{
			if (string.IsNullOrEmpty(OrderId))
				return NewOrderId();

			string[] s = OrderId.Split('-');

			string refType = s[0].Substring(0,1);
			string compId = s[0].Substring(1, 1);

			int year = int.Parse(s[1].Substring(0, 2));
			int sequence = int.Parse(s[1].Substring(2));
			int currentYear = int.Parse(DateTime.Now.ToString("yy"));

			if (currentYear == year)
				sequence++;
			else
			{
				year = currentYear;
				sequence = 1;
			}

			ReferenceNo = string.Format("{0}{1}-{2}{3}", refType, compId, year.ToString("00"), sequence.ToString("00000"));
			return ReferenceNo;
		}

		public string NewOrderId()
		{
            return "00001";
		}
    }
}
