namespace QuizCore.Domain.Entities;

public class MediaFile
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    
    public int? QuestionId { get; set; }
    public Question? Question { get; set; }
}
