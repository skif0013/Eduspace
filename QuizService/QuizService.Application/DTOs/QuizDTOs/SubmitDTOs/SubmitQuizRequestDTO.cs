namespace QuizService.Application.DTOs.QuizDTOs.SubmitDTOs;

public class SubmitQuizRequestDTO
{
    public Guid QuizId { get; set; }
    
    public List<QuestionAnswerDTO> Answers { get; set; }
}