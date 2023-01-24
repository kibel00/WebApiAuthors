using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Tests.UnitTests
{
    [TestClass]
    public class FirstCapitalLetterTests
    {
        [TestMethod]
        public void FirstCapitalLetter_ReturnError()
        {
            //Preparation
            var firstCapitalLetter = new FirstCapitalLetter();
            var value = "felipe";

            var validationContext = new ValidationContext(new { Nombre = value });


            //Execution
            var result = firstCapitalLetter.GetValidationResult(value, validationContext);

            //Verification


            Assert.AreEqual("The first letter must be Capital", result.ErrorMessage);
        }


        [TestMethod]
        public void NullValue_NoReturnError()
        {
            //Preparation
            var firstCapitalLetter = new FirstCapitalLetter();
            string value = null;

            var validationContext = new ValidationContext(new { Nombre = value });


            //Execution
            var result = firstCapitalLetter.GetValidationResult(value, validationContext);

            //Verification


            Assert.IsNull(result);
        }

        [TestMethod]
        public void ValueWithFirstCapitalLetter_NoReturnError()
        {
            //Preparation
            var firstCapitalLetter = new FirstCapitalLetter();
            string value = "Felipe";

            var validationContext = new ValidationContext(new { Nombre = value });


            //Execution
            var result = firstCapitalLetter.GetValidationResult(value, validationContext);

            //Verification


            Assert.IsNull(result);
        }
    }
}