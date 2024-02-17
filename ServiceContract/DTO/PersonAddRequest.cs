﻿using System;
using System.Collections.Generic;
using Entities;
using ServiceContracts.Enums;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{ /// <summary>
  /// Acts as a DTO for inserting a new person
  /// </summary>
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="Person Name Can't be Blank")]
        public string? PersonName { get; set; }
        [Required(ErrorMessage ="Email Can't be Blank")]
        [EmailAddress(ErrorMessage = "Email value " +
            "should be valid email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage ="Please Select Country")]
        public Guid? CountryID { get; set; }
        public string? Address { get; set; }
        public bool ReceiveNewsLetters { get; set; }

        /// <summary>
        /// Converts the current object of PersonAddRequest into a new object of Person type
        /// </summary>
        /// <returns></returns>
        public Person ToPerson()
        {
            return new Person() { PersonName = PersonName, Email = Email, DateOfBirth = DateOfBirth, Gender = Gender.ToString(), Address = Address, CountryID = CountryID, ReceiveNewsLetters = ReceiveNewsLetters };
        }
    }
}
