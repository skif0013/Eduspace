using QuizService.Application.DTOs;
using QuizService.Application.DTOs.QuizDTOs.ResponeDTO;
using QuizService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizService.Application.Contracts
{
    public  interface IQuizMapper
    {
        Quiz MapToDomain(CreatingQuizRequestDTO request, Guid userId);

        void MapToDomain(QuizUpdateRequestDTO request, Quiz quiz);

        public QuizResponseDTO MapToResponseDTO(Quiz quiz);
    }
}
