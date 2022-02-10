using ASP.NET_API.Data;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_API.Services
{
    public class BillsHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public BillsHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(ProcessBills, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Dispose();
            return Task.CompletedTask;
        }

        private void ProcessBills(object state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                CreateBills(context);
                SetDebtor(context);
            }
        }

        private static void SetDebtor(ApplicationDbContext context)
        {
            context.Database.ExecuteSqlRaw("exec SetUserDebtor");
        }

        private static void CreateBills(ApplicationDbContext context)
        {
            var today = DateTime.Today;
            var dateToCompare = today.AddMonths(-1);
            var billsAlreadyCreatedCurrentMonth = context.CreatedBills.Any(b => b.Year == dateToCompare.Year &&
                                                                                b.Month == dateToCompare.Month);

            if (!billsAlreadyCreatedCurrentMonth)
            {
                var beginDate = new DateTime(dateToCompare.Year, dateToCompare.Month, 1);
                var endDate = beginDate.AddMonths(1);
                context.Database.ExecuteSqlInterpolated($"exec BillCreation {beginDate.ToString("yyyy-MM-dd")} {endDate.ToString("yyyy-MM-dd")}");
            }
        }
    }
}
