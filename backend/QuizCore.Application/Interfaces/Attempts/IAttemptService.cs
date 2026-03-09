using QuizCore.Application.DTOs.Attempts;

namespace QuizCore.Application.Interfaces.Attempts;

public interface IAttemptService
{
    Task<StartAttemptResponse> StartExamAsync(int examId, int userId);
    Task AutoSaveAsync(int attemptId, int userId, AutoSaveRequest request);
    Task<AttemptProgressResponse> GetProgressAsync(int attemptId, int userId);
    Task<AttemptResultResponse> SubmitAsync(int attemptId, int userId);
    Task<AttemptResultResponse> GetResultAsync(int attemptId, int userId);
}
