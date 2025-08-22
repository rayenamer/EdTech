using System;
using API.interfaces;
using API.Dtos;
using API.entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UniProgramController : BaseApiController
{
    private readonly IUniProgramRepository _repository;
    private readonly IMapper _mapper;

    public UniProgramController(IUniProgramRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("add-program")]
    public async Task<IActionResult> AddProgram(UniProgramDto UniProgramDto)
    {
        // Map DTO to entity
        var uniProgram = _mapper.Map<UniProgram>(UniProgramDto);

        var createdProgram = await _repository.AddAsync(uniProgram);
        return Ok(createdProgram);
    }

    
    
    [Authorize]
    [HttpGet("get-programs")]
    public async Task<ActionResult<IEnumerable<UniProgram>>> GetPrograms()
    {
        var programs = await _repository.GetAllAsync();
        return Ok(programs);
    }
    
    
    [Authorize]
    [HttpGet("get-program/{id}")]
    public async Task<ActionResult<UniProgram>> GetProgram(int id)
    {
        var program = await _repository.GetByIdAsync(id);
        var programDto = _mapper.Map<UniProgramDto>(program);
        if (program == null) return NotFound("Program not found");

        return Ok(programDto);
    }

    
    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("delete-program/{id}")]
    public async Task<IActionResult> DeleteProgram(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        if (!deleted) return NotFound("Program not found");

        return Ok();
    }
    
    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("delete-programs")]
    public async Task<IActionResult> DeleteAllPrograms()
    {
        var programs = await _repository.GetAllAsync();
        foreach (var program in programs)
        {
            await _repository.DeleteAsync(program.Id);
        }
        return Ok("All programs deleted");
    }
}
