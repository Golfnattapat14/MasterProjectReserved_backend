using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResDb;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MasterWord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterWord : ControllerBase
    {
        private readonly DatabaseContext _dbContext;

        public MasterWord(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var allWords = await _dbContext.MasterProjectReservedWord
                                           .Where(w => w.IsDeleted == false)
                                           .OrderByDescending(w => w.UpdateDate ?? w.CreateDate)
                                           .Select(w => new
                                           {
                                               w.Id,
                                               w.WordName,
                                               w.CreateDate,
                                               w.CreateBy,
                                               w.UpdateDate,
                                               w.UpdateBy,
                                               w.IsDeleted,
                                               w.IsActive
                                           })
                                           .ToListAsync();
            if (allWords.Any())
            {
                return Ok(allWords);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(string id)
        {
            var word = await _dbContext.MasterProjectReservedWord
                                       .Where(w => w.Id == id)
                                       .Select(w => new
                                       {
                                           w.Id,
                                           w.WordName,
                                           w.CreateDate,
                                           w.CreateBy,
                                           w.UpdateDate,
                                           w.UpdateBy,
                                           w.IsDeleted,
                                           w.IsActive
                                       })
                                       .FirstOrDefaultAsync();
            if (word == null)
            {
                return NotFound();
            }
            return Ok(word);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWord([FromBody] MasterProjectReservedWordReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newWord = new MasterProjectReservedWord
            {
                Id = Guid.NewGuid().ToString(),
                WordName = req.WordName,
                CreateDate = DateTime.UtcNow,
                CreateBy = "System",
                UpdateDate = null, 
                UpdateBy = "System",
                IsDeleted = false,
                IsActive = req.IsActive ?? true,
            };

            _dbContext.MasterProjectReservedWord.Add(newWord);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetId), new { id = newWord.Id }, newWord);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWord(string id, [FromBody] MasterProjectReservedWordReq req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != req.Id)
            {
                return BadRequest("ID mismatch between route and request body.");
            }

            var wordToUpdate = await _dbContext.MasterProjectReservedWord
                                               .FirstOrDefaultAsync(w => w.Id == id && (w.IsDeleted == null || w.IsDeleted == false));
            if (wordToUpdate == null)
            {
                return NotFound();
            }

            wordToUpdate.WordName = req.WordName;
            wordToUpdate.IsActive = req.IsActive ?? true;
            wordToUpdate.UpdateDate = DateTime.UtcNow;
            wordToUpdate.UpdateBy = "System";

            _dbContext.MasterProjectReservedWord.Update(wordToUpdate);
            await _dbContext.SaveChangesAsync();

            return Ok(wordToUpdate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteId(string id)
        {
            var word = await _dbContext.MasterProjectReservedWord
                                       .FirstOrDefaultAsync(w => w.Id == id && (w.IsDeleted == null || w.IsDeleted == false));
            if (word == null)
            {
                return NotFound();
            }

            word.IsDeleted = true;
            word.UpdateDate = DateTime.UtcNow;
            word.UpdateBy = "System";

            _dbContext.MasterProjectReservedWord.Update(word);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
