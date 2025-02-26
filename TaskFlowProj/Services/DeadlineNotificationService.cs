using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;
using TaskFlow.Services;
using TaskFlowProj.Hubs;

public class DeadlineNotificationService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<NotificationHub> _hubContext;

    public DeadlineNotificationService(IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Запускаем проверку каждую минуту
        _timer = new Timer(CheckDeadlines, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        return Task.CompletedTask;
    }

    private async void CheckDeadlines(object state)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var taskService = scope.ServiceProvider.GetRequiredService<ITaskService>();
            var tasks = await taskService.GetAllTasksAsync();
            var now = DateTime.Now; // Локальное время сервера (например, GMT+5)
            foreach (var task in tasks)
            {
                // Если deadline хранится в UTC, преобразуем его в локальное время
                DateTime deadlineLocal = task.Deadline.Kind == DateTimeKind.Utc
                    ? task.Deadline.ToLocalTime()
                    : task.Deadline;
                var diff = deadlineLocal - now;
                // Если до дедлайна осталось от 5 минут до 1 минуты (или меньше) и задача ещё не закончилась
                if (diff > TimeSpan.Zero && diff <= TimeSpan.FromMinutes(5))
                {
                    // Вычисляем округлённое число оставшихся минут
                    int minutesRemaining = (int)Math.Ceiling(diff.TotalMinutes);
                    // Отправляем уведомление всем подключенным клиентам с информацией о задаче и оставшемся времени
                    await _hubContext.Clients.All.SendAsync("DeadlineNotification", new { taskTitle = task.Title, minutesRemaining });
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
