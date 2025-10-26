using KwikNesta.Contracts.Models;
using KwikNesta.Mediatrix.Core.Abstractions;
using KwikNesta.Property.Svc.Application.Commands;
using KwikNesta.Property.Svc.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KwikNesta.Property.Svc.API.Controllers.V1
{
    [Route("api/v{version:apiversion}/properties")]
    [ApiVersion("1.0")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly IKwikMediator _mediator;

        public PropertiesController(IKwikMediator mediator)
        {
            _mediator = mediator;
        }

       /// <summary>
       /// Creates a property listing
       /// </summary>
       /// <param name="command"></param>
       /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "LandLord")]
        [ProducesResponseType(typeof(ApiResult<AddPropertyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] AddPropertyCommand command) 
            => Ok(await _mediator.SendAsync(command));

        /// <summary>
        /// Upload video/images for the property
        /// </summary>
        /// <param name="id"></param>
        /// <param name="video"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "LandLord")]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadMedia([FromRoute] Guid id, 
                                                     [FromForm] IFormFile video, 
                                                     [FromForm] IFormFileCollection images)
            => Ok(await _mediator.SendAsync(new UploadPropertyMediaCommand
            {
                PropertyId = id,
                Video = video,
                Images = images
            }));
    }
}