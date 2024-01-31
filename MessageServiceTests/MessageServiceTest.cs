using System.Linq.Expressions;
using AutoMapper;
using MessageApi.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using WebApiLibrary;
using WebApiLibrary.DataStore.Entities;

namespace MessageServiceTests
{
    public static class ContextMockExtensions
    {
        public static Mock<AppDbContext> MockDbSetContent<TEntity>(this Mock<AppDbContext> contextMock, List<TEntity> content, Expression<Func<AppDbContext, DbSet<TEntity>>> propertyReference)
            where TEntity : class
        {
            var mockSet = new Mock<DbSet<TEntity>>();
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Provider).Returns(content.AsQueryable().Provider);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.Expression).Returns(content.AsQueryable().Expression);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.ElementType).Returns(content.AsQueryable().ElementType);
            mockSet.As<IQueryable<TEntity>>().Setup(m => m.GetEnumerator()).Returns(content.GetEnumerator());

            contextMock.Setup(x => x.Set<TEntity>())
                .Returns(mockSet.Object);

            contextMock.Setup(propertyReference)
                .Returns(mockSet.Object);

            return contextMock;
        }
    }

    public class MessageServiceTest
    {
        [Test]
        public void GetNewMessages_ReturnsMessagesAndMarksAsRead()
        {
            var recipientEmail = "test@example.com";
            var userEntity = new UserEntity { Email = recipientEmail };
            var messageEntities = new List<MessageEntity>
            {
                new() { Id = Guid.NewGuid(), Recipient = userEntity, IsRead = false, Text = "Hello" },
                new() { Id = Guid.NewGuid(), Recipient = userEntity, IsRead = false, Text = "Hi" }
            };

            var contextMock = new Mock<AppDbContext>();
            contextMock.MockDbSetContent(messageEntities, x => x.Messages);

            var mapperMock = new Mock<IMapper>();
            var messageService = new MessageService(() => contextMock.Object, mapperMock.Object);

            var result = messageService.GetNewMessages("test@example.com");


            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Messages);
            Assert.AreEqual(2, result.Messages.Count);


            foreach (var messageEntity in messageEntities)
            {
                Assert.IsTrue(messageEntity.IsRead);
            }
        }
    }
}
