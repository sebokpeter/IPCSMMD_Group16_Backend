﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ipcsmmd_webshop.Core.DomainService;
using ipcsmmd_webshop.Core.Entity;

namespace ipcsmmd_webshop.Core.ApplicationService.Impl
{
    public class CustomerService : ICustomerService
    {
        private ICustomerRepository _crepo;

        public CustomerService(ICustomerRepository repository)
        {
            _crepo = repository;
        }

        public Customer AddCustomer(Customer customer)
        {
            if (customer.ID != 0)
                throw new InvalidDataException("Cannot add customer with existing ID!");
            if (customer.FirstName == null)
                throw new InvalidDataException("Cannot add customer without first name!");
            if (customer.LastName == null)
                throw new InvalidDataException("Cannot add customer without last name!");
            if (customer.Email == null)
                throw new InvalidDataException("Cannot add customer without email address!");
            if (customer.Address == null)
                throw new InvalidDataException("Cannot add customer without address!");

            return _crepo.Save(customer);
        }

        public Customer GetCustomerByID(int id)
        {
            if (id == 0)
                throw new ArgumentException("Missing customer ID!");
            return _crepo.GetCustomerByID(id);
        }

        public List<Customer> GetAllCustomers()
        {
            return _crepo.GetAll().ToList();
        }

        public Customer UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentException("Missing update data!");
            else if (customer.ID == 0)
                throw new ArgumentException("Missing customer ID!");
            return _crepo.Update(customer);
        }

        public Customer RemoveCustomer(int id)
        {
            if (id == 0)
                throw new ArgumentException("Missing customer ID!");
            return _crepo.Remove(id);
        }
    }
}
