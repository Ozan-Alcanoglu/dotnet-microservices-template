using MassTransit;
using CSMVCK8S.Shared.Events;
using Microsoft.Extensions.Logging;
namespace CSMVCK8S.Shared.Events
{
    // IConsumer<T> interface'i MassTransit'in ana tüketici yapısıdır.
    public class UserCreatedConsumer : IConsumer<UserCreated>
    {
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger)
        {
            _logger = logger;
        }

        // MassTransit, mesaj geldiğinde bu metodu çağırır.
        public Task Consume(ConsumeContext<UserCreated> context)
        {
            var message = context.Message;
            
            _logger.LogInformation("Product Service, User Created Event'i aldı.");
            _logger.LogInformation(
                "Yeni Kullanıcı Bilgisi: ID={UserId}, Email={Email}", 
                message.UserId, 
                message.Email);

            // Burada Product Service'in bu olaya tepki olarak yapacağı iş mantığı yer alır
            // Örneğin: Yeni kullanıcıya özel bir başlangıç ürünü stoğu ayarlamak.
            
            // İşlem başarılı tamamlandı. MassTransit kuyruktan mesajı siler.
            return Task.CompletedTask;
        }
    }
}