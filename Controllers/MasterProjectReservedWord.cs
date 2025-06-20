using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResDb;
using ResDb.Controllers;
using System.Xml.Linq;
using useDb;

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
            var GetAll = await _dbContext.MasterProjectReservedWord
                                            .Where(w => w.IsDeleted == null || w.IsDeleted == false)
                                            .ToListAsync();
            if (GetAll.Any())
            {
                return Ok(GetAll);
            }
            else
            {
                return NoContent();
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(string id)
        {
            var word = await _dbContext.MasterProjectReservedWord.FirstOrDefaultAsync(w => w.Id == id);
            return Ok(word);
        }
        [HttpPost]
        public async Task<IActionResult> DatabaseContext([FromBody] MasterProjectReservedWordReq req)
        {
            var Iteam = new MasterProjectReservedWord 
            {
                Id = Guid.NewGuid().ToString(),
                WordName = req.WordName,
                CreateDate = DateTime.Now,
                CreateBy = "System", 
                UpdateDate = null,
                UpdateBy = null,
                IsDeleted = false,
                IsActive = true

            };
            _dbContext.MasterProjectReservedWord.Add(Iteam);
            await _dbContext.SaveChangesAsync(); 
            return CreatedAtAction(nameof(DatabaseContext), new { id = req.Id }, req);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutWord(string id, [FromBody] MasterProjectReservedWordReq req)
        {
            MasterProjectReservedWord? w = await _dbContext.MasterProjectReservedWord.FindAsync(id);
            if (w == null)
            {
                return NotFound();
            }
            w.WordName = req.WordName;
            w.UpdateDate = DateTime.Now;
            w.UpdateBy = "System";
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")] 
        public async Task<IActionResult> DeleteId(string id) 
        {
            var word = await _dbContext.MasterProjectReservedWord.FirstOrDefaultAsync(w => w.Id == id);
            if (word == null)
            {
                return NotFound(); 
            }
            word.IsDeleted = true;
            _dbContext.MasterProjectReservedWord.Update(word);
            await _dbContext.SaveChangesAsync();

            return NoContent(); 
        }
    }
}