namespace QuizCore.Application.DTOs.Attempts;

public class AutoSaveRequest
{
    public int QuestionId { get; set; }
    public List<int> SelectedAnswerIds { get; set; } = [];
}

public class SubmitAttemptRequest
{
    public List<AutoSaveRequest> Answers { get; set; } = [];
}

public class AttemptProgressResponse
{
    public int AttemptId { get; set; }
    public Dictionary<int, List<int>> SavedAnswers { get; set; } = []; // questionId -> answerIds
    public int SecondsRemaining { get; set; }
}
