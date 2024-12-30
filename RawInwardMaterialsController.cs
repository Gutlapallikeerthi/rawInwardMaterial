using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ERP_API.Data;
using ERP_API.Moduls;
using static MudBlazor.Icons;

namespace ERP_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawInwardMaterialsController : ControllerBase
    {
        private readonly Lg202324Context _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RawInwardMaterialsController> _logger;

        public RawInwardMaterialsController(Lg202324Context context, IMapper mapper, ILogger<RawInwardMaterialsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/RawInwardMaterials
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RawInwardMaterialReadOnlyDto>>> GetRawInwardMaterials()
        {
            try
            {
                if (_context.RawInwardMaterials == null)
                {
                    _logger.LogWarning("Attempted to retrieve raw inward materials, but the entity set was null.");
                    return NotFound();
                }

                var materials = await _context.RawInwardMaterials
                    .Include(m => m.RawInwardMaterialSubs)
                     .Include(m => m.Storeadds)
                     .ThenInclude(m => m.Storeaddsubs)
                    .OrderByDescending(m => m.InMatId)
                    .ToListAsync();

                var materialsDto = _mapper.Map<IEnumerable<RawInwardMaterialReadOnlyDto>>(materials);

                _logger.LogInformation("Retrieved all raw inward materials with their subs.");
                return Ok(materialsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving raw inward materials.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }

        // GET: api/RawInwardMaterials/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RawInwardMaterialReadOnlyDto>> GetRawInwardMaterial(int id)
        {
            try
            {
                var material = await _context.RawInwardMaterials
                    .Include(m => m.RawInwardMaterialSubs)
                    .Include(m => m.Storeadds)
                    .ThenInclude(m => m.Storeaddsubs)// Include child entities
                    .FirstOrDefaultAsync(m => m.InMatId == id);

                if (material == null)
                {
                    _logger.LogWarning($"Raw inward material with ID {id} was not found.");
                    return NotFound();
                }

                var materialDto = _mapper.Map<RawInwardMaterialReadOnlyDto>(material);

                _logger.LogInformation($"Retrieved raw inward material with ID {id} and its subs.");
                return Ok(materialDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving raw inward material with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }   
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRawInwardMaterial(int id, RawInwardMaterialCreateOnlyDto materialDto)
        {
            if (id != materialDto.InMatId)
            {
                return BadRequest();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Clear EF Core's ChangeTracker to avoid conflicts
                _context.ChangeTracker.Clear();

                // Load the existing material from the database
                var existingMaterial = await _context.RawInwardMaterials
                    .Include(m => m.RawInwardMaterialSubs)
                    .Include(m => m.Storeadds)
                        .ThenInclude(s => s.Storeaddsubs)
                    .FirstOrDefaultAsync(m => m.InMatId == id);

                if (existingMaterial == null)
                {
                    return NotFound();
                }

                // Update main entity properties
                _mapper.Map(materialDto, existingMaterial);
                _context.Entry(existingMaterial).State = EntityState.Modified;

                // Update RawInwardMaterialSubs
                if (materialDto.RawInwardMaterialSubs != null)
                {
                    var keepSubIds = materialDto.RawInwardMaterialSubs
                        .Where(s => s.InMatSubId.HasValue)
                        .Select(s => s.InMatSubId.Value)
                        .ToList();

                    // Remove subs that are no longer needed
                    _context.RawInwardMaterialSubs.RemoveRange(existingMaterial.RawInwardMaterialSubs
                        .Where(s => !keepSubIds.Contains(s.InMatSubId)));

                    // Add or update subs
                    foreach (var subDto in materialDto.RawInwardMaterialSubs)
                    {
                        if (subDto.InMatSubId.HasValue)
                        {
                            // Update existing subs
                            var existingSub = existingMaterial.RawInwardMaterialSubs
                                .FirstOrDefault(s => s.InMatSubId == subDto.InMatSubId.Value);

                            if (existingSub != null)
                            {
                                _mapper.Map(subDto, existingSub);
                                _context.Entry(existingSub).State = EntityState.Modified;
                            }
                        }
                        else
                        {
                            // Add new subs
                            var newSub = _mapper.Map<RawInwardMaterialSub>(subDto);
                            newSub.InMatId = id;
                            existingMaterial.RawInwardMaterialSubs.Add(newSub);
                        }
                    }
                }

                // Update Storeadds
                if (materialDto.Storeadds != null)
                {
                    var keepStoreAddIds = materialDto.Storeadds
                        .Where(s => s.StoreAddId.HasValue)
                        .Select(s => s.StoreAddId.Value)
                        .ToList();

                    // Remove storeadds and their subs that are no longer needed
                    var storeAddsToDelete = existingMaterial.Storeadds
                        .Where(s => !keepStoreAddIds.Contains(s.StoreAddId.Value))
                        .ToList();

                    foreach (var storeAddToDelete in storeAddsToDelete)
                    {
                        _context.Storeaddsubs.RemoveRange(storeAddToDelete.Storeaddsubs);
                        _context.Storeadds.Remove(storeAddToDelete);
                    }

                    // Add or update storeadds
                    foreach (var storeAddDto in materialDto.Storeadds)
                    {
                        if (storeAddDto.StoreAddId.HasValue)
                        {
                            // Update existing storeadds
                            var existingStoreAdd = existingMaterial.Storeadds
                                .FirstOrDefault(s => s.StoreAddId == storeAddDto.StoreAddId.Value);

                            if (existingStoreAdd != null)
                            {
                                _mapper.Map(storeAddDto, existingStoreAdd);
                                _context.Entry(existingStoreAdd).State = EntityState.Modified;

                                if (storeAddDto.Storeaddsubs != null)
                                {
                                    var keepSubIds = storeAddDto.Storeaddsubs
                                        .Where(s => s.storeAddSubId.HasValue)
                                        .Select(s => s.storeAddSubId.Value)
                                        .ToList();

                                    // Remove subs that are no longer needed
                                    _context.Storeaddsubs.RemoveRange(existingStoreAdd.Storeaddsubs
                                        .Where(s => !keepSubIds.Contains(s.storeAddSubId.Value)));

                                    // Add or update subs
                                    foreach (var subDto in storeAddDto.Storeaddsubs)
                                    {
                                        if (subDto.storeAddSubId.HasValue)
                                        {
                                            // Update existing subs
                                            var existingSub = existingStoreAdd.Storeaddsubs
                                                .FirstOrDefault(s => s.storeAddSubId == subDto.storeAddSubId.Value);

                                            if (existingSub != null)
                                            {
                                                _mapper.Map(subDto, existingSub);
                                                _context.Entry(existingSub).State = EntityState.Modified;
                                            }
                                        }
                                        else
                                        {
                                            // Add new subs
                                            var newSub = _mapper.Map<Storeaddsub>(subDto);
                                            newSub.StoreAddId = existingStoreAdd.StoreAddId;
                                            existingStoreAdd.Storeaddsubs.Add(newSub);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Add new storeadds
                            var newStoreAdd = _mapper.Map<Storeadd>(storeAddDto);
                            newStoreAdd.InMatId = id;
                            existingMaterial.Storeadds.Add(newStoreAdd);

                            if (storeAddDto.Storeaddsubs != null)
                            {
                                foreach (var subDto in storeAddDto.Storeaddsubs)
                                {
                                    var newSub = _mapper.Map<Storeaddsub>(subDto);
                                    newSub.StoreAddId = newStoreAdd.StoreAddId;
                                    newStoreAdd.Storeaddsubs.Add(newSub);
                                }
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"Error updating raw inward material with ID {id}");
                return StatusCode(500, "An error occurred while updating the record.");
            }
        }

        // POST: api/RawInwardMaterials
        [HttpPost]
        public async Task<ActionResult<RawInwardMaterialReadOnlyDto>> PostRawInwardMaterial(RawInwardMaterialCreateOnlyDto materialDto)
        {
            if (_context.RawInwardMaterials == null)
            {
                _logger.LogWarning("Attempted to create a raw inward material but the entity set was null.");
                return Problem("Entity set 'Lg202324Context.RawInwardMaterials' is null.");
            }

            try
            {
                // Map the incoming DTO to the entity
                var material = _mapper.Map<RawInwardMaterial>(materialDto);

                // Add the material to the context and save changes
                _context.RawInwardMaterials.Add(material);
                await _context.SaveChangesAsync();

                // Retrieve the created material along with its child entities
                var createdMaterial = await _context.RawInwardMaterials
                    .Include(m => m.RawInwardMaterialSubs)
                     .Include(m => m.Storeadds)
                     .ThenInclude(m => m.Storeaddsubs)// Including related child entities
                    .FirstOrDefaultAsync(m => m.InMatId == material.InMatId);

                // If the material is not found after saving, log a warning and return not found
                if (createdMaterial == null)
                {
                    _logger.LogWarning("Failed to retrieve the created raw inward material.");
                    return NotFound("The newly created raw inward material could not be found.");
                }

                // Map the created material entity back to a read-only DTO for returning in the response
                var createdMaterialDto = _mapper.Map<RawInwardMaterialReadOnlyDto>(createdMaterial);

                _logger.LogInformation($"Created a new raw inward material with ID {material.InMatId}.");

                // Return the created material along with a URI to access it via GET
                return CreatedAtAction(nameof(GetRawInwardMaterial), new { id = material.InMatId }, createdMaterialDto);
            }
            catch (Exception ex)
            {
                // Log and handle any unexpected errors
                _logger.LogError(ex, "An error occurred while creating a raw inward material.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }
       
        // DELETE: api/RawInwardMaterials/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRawInwardMaterial(int id)
        {
            try
            {
                var material = await _context.RawInwardMaterials
                    .Include(m => m.RawInwardMaterialSubs)
                     .Include(m => m.Storeadds)
                     .ThenInclude(m => m.Storeaddsubs)// Include child entities
                    .FirstOrDefaultAsync(m => m.InMatId == id);

                if (material == null)
                {
                    _logger.LogWarning($"Raw inward material with ID {id} was not found.");
                    return NotFound();
                }
                // Remove CustomerLocationDepartments
                foreach (var storeAddSub in material.Storeadds)
                {
                    _context.Storeaddsubs.RemoveRange(storeAddSub.Storeaddsubs);
                }
                _context.Storeadds.RemoveRange(material.Storeadds);

                _context.RawInwardMaterials.Remove(material);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted raw inward material with ID {id}.");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting raw inward material with ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error.");
            }
        }


        private bool RawInwardMaterialExists(int id)
        {
            return (_context.RawInwardMaterials?.Any(e => e.InMatId == id)).GetValueOrDefault();
        }

    }
}