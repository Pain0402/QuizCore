using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace QuizCore.API.Hubs;

[Authorize]
public class ExamHub : Hub
{
    public async Task JoinExamSession(string examId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"exam_session_{examId}");
        // We could also notify the teacher's dashboard that a student went online
        await Clients.Group($"exam_session_{examId}_teachers").SendAsync("StudentStatusUpdate", Context.UserIdentifier, "Online");
    }

    public async Task LeaveExamSession(string examId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"exam_session_{examId}");
        await Clients.Group($"exam_session_{examId}_teachers").SendAsync("StudentStatusUpdate", Context.UserIdentifier, "Offline");
    }

    // Teachers check in to a specific exam to monitor it
    public async Task JoinAsMonitor(string examId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"exam_session_{examId}_teachers");
    }
}
