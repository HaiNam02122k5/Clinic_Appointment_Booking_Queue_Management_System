using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.UnitTests.Common;

namespace Clinic.Domain.UnitTests
{
    public class PersonTests
    {
        [Fact]
        public void TestPersonCreationValid()
        {
            var person = new Person("Hi Low", "1234567890", "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St");
            Assert.Equal("Hi Low", person.FullName);
            Assert.Equal("1234567890", person.PhoneNumber);
            Assert.Equal("main@example.com", person.Email);
            Assert.Equal(new DateOnly(1990, 1, 1), person.DateOfBirth);
            Assert.Equal(Gender.Male, person.Gender);
            Assert.Equal("123 Main St", person.Address);
            Assert.False(person.IsDeleted);
        }

        [Fact]
        public void TestPersonCreattionBlankAddressValid()
        {
            var person = new Person("Hi Low", "1234567890", "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "");
            Assert.Equal("Hi Low", person.FullName);
            Assert.Equal("1234567890", person.PhoneNumber);
            Assert.Equal("main@example.com", person.Email);
            Assert.Equal(new DateOnly(1990, 1, 1), person.DateOfBirth);
            Assert.Equal(Gender.Male, person.Gender);
            Assert.Equal("", person.Address);
            Assert.False(person.IsDeleted);
        }

        [Fact]
        public void TestPersonCreationBlankEmailValid()
        {
            var person = new Person("Hi Low", "1234567890", "", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St");
            Assert.False(person.IsDeleted);

            person = new Person("Hi Low", "1234567890", "          ", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St");
            Assert.False(person.IsDeleted);

            person = new Person("Hi Low", "1234567890", null!, new DateOnly(1990, 1, 1), Gender.Male, "123 Main St");
            Assert.False(person.IsDeleted);
        }

        [Fact]
        public void TestPersonCreationBlankPhoneNumberValid()
        {
            var person = new Person("Hi Low", "", "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St");
            Assert.False(person.IsDeleted);

            person = new Person("Hi Low", "          ", "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St");
            Assert.False(person.IsDeleted);

            person = new Person("Hi Low", null!, "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St");
            Assert.False(person.IsDeleted);
        }

        [Fact]
        public void TestPersonCreationBlankFullNameThrows()
        {
            Assert.Throws<ArgumentException>(() => new Person("", "1234567890", "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St"));
            Assert.Throws<ArgumentException>(() => new Person("         ", "1234567890", "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St"));
            Assert.Throws<ArgumentException>(() => new Person(null!, "1234567890", "main@example.com", new DateOnly(1990, 1, 1), Gender.Male, "123 Main St"));
        }

        [Fact]
        public void TestInvalidPhoneNumberThrows()
        {
            Assert.Throws<ArgumentException>(() => new Person("Hi Low", "abc123", "", new DateOnly(1990, 1, 1), Gender.Male, null!));
        }

        [Fact]
        public void TestFutureDateOfBirthThrows()
        {
            var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
            Assert.Throws<ArgumentException>(() => new Person("Hi Low", "1234567890", "main@example.com", futureDate, Gender.Male, "123 Main St"));
        }

        [Fact]
        public void TestUpdateDetailsValid()
        {
            var person = TestDataFactory.CreatePerson();
            var name = person.FullName;
            person.UpdateDetails("", Gender.Male, "123 Main St");
            Assert.Equal("", person.Email);
            Assert.Equal(Gender.Male, person.Gender);
            Assert.Equal("123 Main St", person.Address);
            Assert.Equal(name, person.FullName); 
        }

        [Fact]
        public void TestDeletePerson()
        {
            var person = TestDataFactory.CreatePerson();
            Assert.False(person.IsDeleted);
            person.Delete();
            Assert.True(person.IsDeleted);
        }
    }
}
