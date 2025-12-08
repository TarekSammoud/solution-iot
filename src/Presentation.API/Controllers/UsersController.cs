using Application.DTOs;
using Application.DTOs.User;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService; 

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Récupère touts les utilisateurs.
        /// </summary>
        /// <returns>Liste de toutes les utilisateurs.</returns>
        /// <response code="200">Retourne la liste des utilisateurs.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll([FromQuery] string? searchString = null)
        {

            var filteredUsers = await _userService.SearchQuery(searchString);

            return Ok(filteredUsers);
            

          /*  var users = await _userService.GetAllAsync();
            return Ok(users);*/
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Crée une nouvelle utilisateur.
        /// </summary>
        /// <param name="dto">Les données de la utilisateur à créer.</param>
        /// <returns>La utilisateur créée avec Id et DateCreation générés.</returns>
        /// <response code="201">La utilisateur a été créée avec succès.</response>
        /// <response code="400">Les données fournies sont invalides.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Met à jour une utilisateur existante.
        /// </summary>
        /// <param name="id">L'identifiant de la utilisateur à mettre à jour.</param>
        /// <param name="dto">Les nouvelles données de la utilisateur.</param>
        /// <returns>La utilisateur mise à jour.</returns>
        /// <response code="200">La utilisateur a été mise à jour avec succès.</response>
        /// <response code="400">Les données fournies sont invalides ou l'ID ne correspond pas.</response>
        /// <response code="404">La utilisateur n'existe pas.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans le body");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _userService.UpdateAsync(dto);

            if (updated == null)
            {
                return NotFound();
            }

            return Ok(updated);
        }

        /// <summary>
        /// Supprime une utilisateur.
        /// </summary>
        /// <param name="id">L'identifiant de la utilisateur à supprimer.</param>
        /// <returns>Aucun contenu si la suppression réussit.</returns>
        /// <response code="204">La utilisateur a été supprimée avec succès.</response>
        /// <response code="404">La utilisateur n'existe pas.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
