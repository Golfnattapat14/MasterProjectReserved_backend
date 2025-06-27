using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResDb;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Mime;
using System.Threading;
using MasterWord.Services;

namespace MasterWord.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterWord : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly DropboxService _dropboxService;

        public MasterWord(DatabaseContext dbContext, DropboxService dropboxService)
        {
            _dbContext = dbContext;
            _dropboxService = dropboxService;
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
            List<MasterProjectReservedWordRespond> _allWords = new List<MasterProjectReservedWordRespond>();
            int Sequence = 0;
            foreach (var item in allWords)
            {
                MasterProjectReservedWordRespond _item = new MasterProjectReservedWordRespond
                {
                    Id = item.Id,
                    WordName = item.WordName,
                    CreateDate = item.CreateDate,
                    CreateBy = item.CreateBy,
                    UpdateDate = item.UpdateDate,
                    UpdateBy = item.UpdateBy,
                    IsDeleted = item.IsDeleted,
                    IsActive = item.IsActive,
                    Sequence = Sequence + 1
                };

                _allWords.Add(_item);
                Sequence = Sequence + 1;
            }
            
            if (_allWords.Any())
            {
                return Ok(_allWords);
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

            bool isNameUsed = await _dbContext.MasterProjectReservedWord
                .AnyAsync(w => w.WordName == req.WordName && (w.IsDeleted == null || w.IsDeleted == false));

            if (isNameUsed)
            {
                return Conflict(new { message = "ชื่อนี้มีอยู่ในระบบอยู่แล้ว : กรุณาใช้ชื่ออื่น" });
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
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { error = string.Join("; ", errors) });
            }
            if (id != req.Id)
            {
                return BadRequest(new { error = "ID mismatch between route and request body." });
            }

            if (string.IsNullOrWhiteSpace(req.WordName))
            {
                return BadRequest(new { error = "Word Name cannot be empty." });
            }

            bool isNameUsed = await _dbContext.MasterProjectReservedWord
                .AnyAsync(w => w.WordName == req.WordName && w.Id != id && (w.IsDeleted == null || w.IsDeleted == false));

            if (isNameUsed)
            {
                return Conflict(new { error = "ชื่อนี้มีอยู่ในระบบอยู่แล้ว : กรุณาใช้ชื่ออื่น" });
            }

            var wordToUpdate = await _dbContext.MasterProjectReservedWord
                .FirstOrDefaultAsync(w => w.Id == id && (w.IsDeleted == null || w.IsDeleted == false));

            if (wordToUpdate == null)
            {
                return NotFound(new { error = "Word not found." });
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

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest(new { error = "No file uploaded." });

            var result = await _dropboxService.UploadFileAsync(request.File, cancellationToken);

            if (!result.Success)
                return StatusCode(500, new { error = result.ErrorMessage });

            return Ok(new FileUploadResponse
            {
                FileName = result.FileName,
                FileUrl = result.FileUrl
            });
        }
    }
}
