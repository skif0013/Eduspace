using QuizService.Application.Contracts;
using QuizService.Application.Contracts.QuestionsContract;
using QuizService.Application.DTOs.QuestionsDTOs.CreateRequestDTOs;
using QuizService.Application.DTOs.QuestionsDTOs.ResponseDTO;
using QuizService.Application.Repositories;
using QuizService.Domain.Models;


namespace QuizService.Application.Services;

public class QuestionService : IQuestionService
{
    
    private readonly IUnitOfWork _unitOfWork;
    private readonly IQuestionRepository _questionRepository;
    private readonly IQuestionMapper _questionMapper;
    
    public QuestionService( IUnitOfWork unitOfWork, IQuestionRepository questionRepository, IQuestionMapper questionMapper)
    {
        _questionMapper = questionMapper;
        _unitOfWork = unitOfWork;
        _questionRepository = questionRepository;
    }


    public async Task<QuestionResponseDTO> CreateQuestionWithTitleAsync(CreateQuestionRequestDTO dto)
    {
        var question = new Question(dto.QuizId, dto.Text, dto.Order, dto.MaxScore, dto.QuestionType);


        if (dto.Options != null && dto.Options.Any())
        {
            foreach (var opt in dto.Options)
            {
                question.AddAnswerOption(opt.Text, opt.IsCorrect, opt.Score, opt.Order);
            }
        }
        
        await _questionRepository.AddQuestion(question);
        await _unitOfWork.SaveChangesAsync();
        
        return _questionMapper.ToResponse(question);
    }
    
    public Task<QuestionResponseDTO> GetQuestionByIdAsync(Guid id)
    {
        var find = _questionRepository.FindByIdAsync(id);
        
        if (find == null)
        {
            throw new Exception("Question not found");
        }
        
        return Task.FromResult(_questionMapper.ToResponse(find.Result));
    }

    public async Task<IReadOnlyCollection<QuestionResponseDTO>> GetAllQuestionsAsync()
    {
        var questions = await _questionRepository.GetAllQuestionsAsync();

        return questions
            .Select(_questionMapper.ToResponse)
            .ToList();
    }

    public async Task<bool> DeleteQuestionFromQuizAsync(Guid id)
    {
        var find = await _questionRepository.FindByIdAsync(id);
        
        if (find == null)
        {
            throw new Exception("Question not found");
        }
        
        await _questionRepository.RemoveAsync(find);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }
    
    
    public async Task<QuestionResponseDTO> UpdateQuestionToQuizAsync(CreateQuestionRequestDTO requestDTO, Guid questionId)
    {
        var find = await _questionRepository.FindByIdAsync(questionId);
        
        if (find == null)
        {
            throw new Exception("Question not found");
        }
        
        find.Text = requestDTO.Text;
        find.Order = requestDTO.Order;
        find.MaxScore = requestDTO.MaxScore;
        find.QuestionType = requestDTO.QuestionType;
        find.IsActive = true;
        find.QuizId = requestDTO.QuizId;
        
        await _unitOfWork.SaveChangesAsync();
        
        return _questionMapper.ToResponse(find);
    }
    
    public async Task<QuestionResponseDTO> CompletedQuestionAsync(Guid questionId)
    {
        var find = await _questionRepository.FindByIdAsync(questionId);
        
        if (find == null)
        {
            throw new Exception("Question not found");
        }
        
        find.IsActive = false;
        
        await _unitOfWork.SaveChangesAsync();
        
        return _questionMapper.ToResponse(find);
    }
}