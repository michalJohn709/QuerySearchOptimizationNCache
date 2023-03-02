using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleData
{
    /// <summary>
    /// Model class for Customers
    /// </summary>
    [Serializable]
    public class Customer
    {
        public Customer()
        { }

        /// <summary>
        /// Unique Id of the customer
        /// </summary>
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// Contact name of the customer
        /// </summary>
        public virtual string ContactName
        {
            set;
            get;
        }

        /// <summary>
        /// Company the customer works for
        /// </summary>
        public virtual string CompanyName
        {
            set;
            get;
        }

        /// <summary>
        /// Contact number of the customer
        /// </summary>
        public virtual string ContactNo
        {
            set;
            get;
        }

        /// <summary>
        /// Residential address of the customer
        /// </summary>
        public virtual string Address
        {
            set;
            get;
        }

        /// <summary>
        /// Residence city of the customer
        /// </summary>
        public virtual string City
        {
            set;
            get;
        }

        /// <summary>
        /// Nationality of the customer
        /// </summary>
        public virtual string Country
        {
            set;
            get;
        }

        /// <summary>
        /// Postal code of the customer
        /// </summary>
        public virtual string PostalCode
        {
            set;
            get;
        }

        /// <summary>
        /// Fax number of the customer
        /// </summary>
        public virtual string Fax
        {
            set;
            get;
        }
    }
}
