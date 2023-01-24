using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using WebApiAuthors.Controllers.V1;
using WebApiAuthors.Tests.Mocks;

namespace WebApiAuthors.Tests.UnitTests
{
    [TestClass]
    public class RootControllesTests
    {
        [TestMethod]
        public async Task IfUsersIsAdmin_Get4Links()
        {
            //Preparation
            var authorizationServices = new AuthorizationServicesMock();
            authorizationServices.Result = AuthorizationResult.Success();
            var rootController = new RootController(authorizationServices);
            rootController.Url = new URLHelperMock();

            //Execution

            var result = await rootController.Get();

            //Verification
            Assert.AreEqual(4, result.Value.Count());
        }


        [TestMethod]
        public async Task IfUsersIsNotAdmin_Get2Links()
        {
            //Preparation
            var authorizationServices = new AuthorizationServicesMock();
            authorizationServices.Result = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationServices);
            rootController.Url = new URLHelperMock();

            //Execution

            var result = await rootController.Get();

            //Verification
            Assert.AreEqual(2, result.Value.Count());
        }



        [TestMethod]
        public async Task IfUsersIsNotAdmin_Get2LinksUSingMoq()
        {
            //Preparation
            var mockAuthorizationServices = new Mock<IAuthorizationService>();
            mockAuthorizationServices.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));



            mockAuthorizationServices.Setup(x => x.AuthorizeAsync(
               It.IsAny<ClaimsPrincipal>(),
               It.IsAny<object>(),
               It.IsAny<string>()
               )).Returns(Task.FromResult(AuthorizationResult.Failed()));


            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(),
                It.IsAny<object>())).Returns(string.Empty);
            var rootController = new RootController(mockAuthorizationServices.Object);
            rootController.Url = mockUrlHelper.Object;

            //Execution

            var result = await rootController.Get();

            //Verification
            Assert.AreEqual(2, result.Value.Count());
        }
    }
}
