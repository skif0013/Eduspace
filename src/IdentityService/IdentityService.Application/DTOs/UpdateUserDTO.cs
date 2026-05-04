namespace IdentityService.Application.DTOs;

public class UpdateUserDTO  // TODO преписать дтошки на рекорды и разобраться что такое рекорды
{
    public Guid Id { get; set; }  // TODO почитать про то как правильно вытягиваю в асп.нет бекенде айди с токена jwt
    public string UserName { get; set; }
    public string Email { get; set; }
}