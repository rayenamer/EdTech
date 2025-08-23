using System;

namespace API.Dtos;

public class UserDataWithDocumentsDto
{
    public UserDataDto? UserData { get; set; }
    public DocumentStatusDto DocumentStatus { get; set; } = new DocumentStatusDto();
}