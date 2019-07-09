using System;
using System.Collections.Generic;
using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.API.Controllers
{
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        // Create a field to store the mapper object
        private readonly IMapper _mapper;
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository, IMapper mapper)
        {
            _libraryRepository = libraryRepository;
            _mapper = mapper;
        }

        [HttpGet("api/authors")]
        public IActionResult GetAuthors(){
            var authorFromRepository = _libraryRepository.GetAuthors();
            var authors = _mapper.Map<IEnumerable<AuthorDto>>(authorFromRepository);
            return Ok(authors);
        }

        [HttpGet("api/authors/{id}", Name = "GetAuthor")]
        public IActionResult GetAuthors(Guid Id){
            var authorFromRepo = _libraryRepository.GetAuthor(Id);
            if (authorFromRepo == null)
            {
                return NotFound();
            }
            var author = _mapper.Map<AuthorDto>(authorFromRepo);
            return Ok(author);
        }

        [HttpPost("api/authors")]
        public IActionResult CreateAuthor([FromBody] AuthorForCreationDto author)
        {
            if (author == null)
            {
                return BadRequest();
            }
            var authorEntity = _mapper.Map<Author>(author);
            _libraryRepository.AddAuthor(authorEntity);
            if (!_libraryRepository.Save())
            {
                return StatusCode(500, "A problem occurred while createing the author.");
            }
            var authorToReturn = _mapper.Map<AuthorDto>(authorEntity);
            return CreatedAtRoute("GetAuthor", new { id = authorToReturn.Id}, authorToReturn);
        }

        [HttpPost("api/authors/{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
            return NotFound();
        }
    }
}